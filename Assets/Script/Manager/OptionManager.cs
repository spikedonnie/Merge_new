using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class OptionManager : UIManager
{
    [SerializeField] Button[] optionButtons;


    private bool effectOn = false;
    private bool bgmOn = false;

    public TextMeshProUGUI effectText;
    public TextMeshProUGUI bgmText;

    OptionData optionData;

    public Sprite bgmOffSprite;
    public Sprite bgmOnSprite;
    public Sprite effectOffSprite;
    public Sprite effectOnSprite;
    public Image bgmImage;
    public Image effectImage;



    enum OptionTYPE
    {
        CloudLoad,
        CloudSave,
        Review,
        Bgm,
        Sfx,
        Max
    }


    void Start()
    {
        base.Init();

        for (OptionTYPE i = OptionTYPE.CloudLoad; i < OptionTYPE.Max; i++)
        {
            OptionTYPE temp = i;
            optionButtons[(int)temp].onClick.AddListener(() => ButtonClick(temp));
        }



        optionData = GameDataManager.instance.GetSaveData.optionData;

        if (optionData.isEffectOn)
        {
            effectOn = true;
            effectText.text = string.Format("OFF");
            AudioManager.Instance.EffectVolume(1f);
            effectImage.sprite = effectOnSprite;
        }
        else
        {
            effectOn = false;
            effectText.text = string.Format("ON");
            AudioManager.Instance.EffectVolume(0f);
            effectImage.sprite = effectOffSprite;
        }

        if (optionData.isBgmOn)
        {
            bgmOn = true;
            bgmText.text = string.Format("OFF");
            AudioManager.Instance.BgmVolume(1f);
            bgmImage.sprite = bgmOnSprite;
        }
        else
        {
            bgmOn = false;
            bgmText.text = string.Format("ON");
            AudioManager.Instance.BgmVolume(0f);
            bgmImage.sprite = bgmOffSprite;
        }

    }
    void ButtonClick(OptionTYPE index)
    {
        switch (index)
        {
            case OptionTYPE.CloudLoad: 
                StartCoroutine(LoadCloud()); 
                break;
            case OptionTYPE.CloudSave:
                StartCoroutine(SaveCloud());
                break;
            case OptionTYPE.Review:
                Review();

                break;
            case OptionTYPE.Bgm:
                BgmControl();

                break;
            case OptionTYPE.Sfx:
                SfxControl();

                break;
            case OptionTYPE.Max:

                break;
            default:
                break;
        }
    }

    void Restart()
    {
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            const int kIntent_FLAG_ACTIVITY_CLEAR_TASK = 0x00008000;
            const int kIntent_FLAG_ACTIVITY_NEW_TASK = 0x10000000;

            var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var pm = currentActivity.Call<AndroidJavaObject>("getPackageManager");
            var intent = pm.Call<AndroidJavaObject>("getLaunchIntentForPackage", Application.identifier);

            intent.Call<AndroidJavaObject>("setFlags", kIntent_FLAG_ACTIVITY_NEW_TASK | kIntent_FLAG_ACTIVITY_CLEAR_TASK);
            currentActivity.Call("startActivity", intent);
            currentActivity.Call("finish");
            var process = new AndroidJavaClass("android.os.Process");
            int pid = process.CallStatic<int>("myPid");
            process.CallStatic("killProcess", pid);
        }
    }
    IEnumerator SaveCloud()
    {
        UIController.instance.ShowLoadingImage(true);
        GameDataManager.instance.SaveData();
        yield return new WaitForSeconds(0.2f);
        CloudManager.Instance.CloudSave();
        yield return new WaitUntil(() => !CloudManager.Instance.isProcessing);
        UIController.instance.ShowLoadingImage(false);
        Debug.Log("구글 클라우드 저장 완료");
    }
    IEnumerator LoadCloud()
    {
        UIController.instance.ShowLoadingImage(true);
        CloudManager.Instance.CloudLoad();
        yield return new WaitUntil(() => !CloudManager.Instance.isProcessing);
        GameDataManager.instance.SaveData();
        yield return new WaitForSeconds(0.5f);
        UIController.instance.ShowLoadingImage(false);
        Debug.Log("구글 클라우드 불러오기 완료");
        Restart();
    }
    void SetLanguege()
    {
        Debug.Log("SetLanguege");

    }

    void Review()
    {
        Application.OpenURL("market://details?id=com.donnienest.MergeMercenary");
    }
    void BgmControl()
    {
        if (bgmOn)
        {
            bgmText.text = string.Format("ON");
            bgmImage.sprite = bgmOffSprite;
            AudioManager.Instance.BgmVolume(0f);
            bgmOn = false;
            GameDataManager.instance.GetSaveData.optionData.isBgmOn = false;
        }
        else
        {
            bgmText.text = string.Format("OFF");
            bgmImage.sprite = bgmOnSprite;
            AudioManager.Instance.BgmVolume(1f);
            bgmOn = true;
            GameDataManager.instance.GetSaveData.optionData.isBgmOn = true;

        }
    }

    void SfxControl()
    {
        if (effectOn)
        {
            effectImage.sprite = effectOffSprite;
            effectText.text = string.Format("ON");
            AudioManager.Instance.EffectVolume(0f);
            effectOn = false;
            GameDataManager.instance.GetSaveData.optionData.isEffectOn = false;

        }
        else
        {
            effectImage.sprite = effectOnSprite;
            effectText.text = string.Format("OFF");
            AudioManager.Instance.EffectVolume(1f);
            effectOn = true;
            GameDataManager.instance.GetSaveData.optionData.isEffectOn = true;

        }
    }

    void SendEmail()
    {
        Debug.Log("SendEmail");

    }





}

