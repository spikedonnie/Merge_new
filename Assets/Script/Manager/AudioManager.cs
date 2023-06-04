using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public enum ESoundEffect
{
    Merge,
    CriticalMerge,
    KnightAttack,
    Magic,
    ChestOpen,
    Summon,
    BattleWin,
    CoinJingle,
    NewHero,
    Negative,
    SuccessAdView,
    Click,
    Bell,
    Coins,
    GetCurrency,
    GetReward,
    Contract,
    RewardIcon

}
public enum PoolAudio { MELEE,RANGE,MAGIC, CARD, RELIC,UPGRADE, CREATEHERO, COINS }

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class Sound
    {
        public string name;
        private AudioSource source;
        private AudioClip clip;

        public Sound(AudioSource _source, AudioClip c, string n)
        {
            name = n;
            clip = c;
            source = _source;
            source.playOnAwake = false;
            source.clip = clip;
        }

        public void Play(float vol)
        {
            source.volume = vol;
            source.Play();
        }
    }
    public Transform m_pooledObjectsParent;
    public AudioClip[] poolsAudio;
    private List<GameObject> m_pooledObjects = new List<GameObject>();

    public AudioSource mainBgmSource;

    //[SerializeField]
    //private float bgmVolume = 0.7f;

    [SerializeField]
    private float effectVolume = 1f;

    private AudioClip titleSound;
    private AudioClip mainSound;
    private AudioClip evolutionSound;

    private string[] playSounaName;

    [Header("사운드 목록")]
    [SerializeField]
    private Sound[] soundList;

    private static AudioManager instance = null;

    public static AudioManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    public IEnumerator SetAudioManager()
    {
        MakeSound();

        //if (!GameDataManager.Instance.GetDataInfo().isBgmOn)
        //    bgmVolume = 0;
        //if (!GameDataManager.Instance.GetDataInfo().isEffectOn)
        //    effectVolume = 0;

        PlayMainBgm(true, 1);

        yield return null;
    }

    //사운드 미리생성
    private void MakeSound()
    {
        //titleSound = Resources.Load<AudioClip>("Audio/BGM/TitleBGM");
        //mainSound = Resources.Load<AudioClip>("Audio/BGM/ManiBGM");
        //evolutionSound = Resources.Load<AudioClip>("Audio/BGM/EvolutionBGM");

        AudioClip[] clips = Resources.LoadAll<AudioClip>("Audio");
        soundList = new Sound[clips.Length];

        for (int i = 0; i < soundList.Length; i++)
        {
            GameObject _go = new GameObject(clips[i].name);
            _go.transform.SetParent(this.transform);
            soundList[i] = new Sound(_go.AddComponent<AudioSource>(), clips[i], clips[i].name);
        }
    }

    //사운드 재생
    public void PlayEffect(ESoundEffect type)
    {
        for (int i = 0; i < soundList.Length; i++)
        {
            if (soundList[i].name.Equals(type.ToString()))
            {
                soundList[i].Play(effectVolume);
                return;
            }
        }
    }



    public void PlayMainBgm(bool play, float pitch)
    {
        if (play)
        {
            mainBgmSource.Play();
            mainBgmSource.pitch = pitch;
        }
        else
        {
            mainBgmSource.DOFade(0, 2f);
        }

    }

    public void BgmVolume(float vol)
    {
        mainBgmSource.volume = vol;
    }
    public void EffectVolume(float vol)
    {
        effectVolume = vol;

    }

    public void PlaySound(PoolAudio pool)
    {

        GameObject audioObject = GetAudioObject();
        AudioSource audioSource = audioObject.GetComponent<AudioSource>();
        audioObject.name = poolsAudio[(int)pool].name;
        audioSource.clip = poolsAudio[(int)pool];
        audioSource.volume = effectVolume;
        audioSource.Play();
        StartCoroutine(AutoDisableCoroutine(audioObject, audioSource));

    }
    private IEnumerator AutoDisableCoroutine(GameObject targetObject, AudioSource source)
    {
        while (source.isPlaying)
            yield return new  WaitForFixedUpdate();

        targetObject.SetActive(false);
    }

    private GameObject GetAudioObject()
    {
        foreach (var pooledObject in m_pooledObjects)
        {
            if (pooledObject.activeInHierarchy == false)
            {
                pooledObject.SetActive(true);
                return pooledObject;
            }
        }

        GameObject audioObject = new GameObject();
        audioObject.transform.parent = m_pooledObjectsParent;
        audioObject.AddComponent<AudioSource>().playOnAwake = false;
        m_pooledObjects.Add(audioObject);

        return audioObject;
    }

}