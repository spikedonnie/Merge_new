using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using LitJson;
using System.Reflection;
using UnityEngine.Rendering;
using I2.Loc;
using Sirenix.OdinInspector;

public class GameController : MonoBehaviour
{
    public EnemyManager enemyManager;
    public InApp inApp;
    public ADManager AdManager;
    public RelicManager RelicManager;
    public CardManager CardManager;
    public ShopManager ShopManager;
    public TrainingManager trainingManager;
    public AbilityManager abilityManager;
    public DamageManager DamageManager { get; set; }
    public MergeManager MergeManager { get; set; }
    public BattleManager BattleManager { get; set; }
    public InputManager InputManager { get; set; }
    public AudioManager AudioManager { get; set; }
    public PopupCanvasUI PopUpCanvasUI;
    public OptionManager optionManager;
    public MergeController mergeController;

    public BadgeController badgeController;
    public EventUI eventUI;
    public static GameController instance;

    public Image fadeInOut;

    public Background backGround;

    private bool isPaused = false;//���� Ȱ��ȭ ���¸� �����ϴ� ����

    [SerializeField] private bool isGameStop = false;//���� ���� üũ

    float offLineGold;
    float offLineStone;
    WaitForSeconds waitSecond;


    public bool IsGameStop
    {
        get
        {
            return isGameStop;
        }
        set
        {
            isGameStop = value;
        }
    }

    #region TEST CODE
    bool isAutoTest = false;
    public void AutoPlayGame()
    {
        if (!isAutoTest)
        {
            StartCoroutine("Repeat");
        }
        else 
        {
            StopCoroutine("Repeat");
        }

        isAutoTest = !isAutoTest;


    }
    IEnumerator Repeat()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            mergeController.Merging();
            UIController.instance.ClickCollectMenu();
        }
    }

    #endregion

    private void Awake()
    {
        instance = this;
        DamageManager = GetComponentInChildren<DamageManager>();
        MergeManager = GetComponentInChildren<MergeManager>();
        BattleManager = GetComponent<BattleManager>();
        AudioManager = GetComponentInChildren<AudioManager>();
        gameObject.GetComponentInChildren<InputManager>();
        fadeInOut.color = new Color(0, 0, 0, 1);
        Utils.InitResources();
    }

    private void Start()
    {
        StartCoroutine(InitManagers());
    }

    private IEnumerator InitManagers()
    {
        yield return new WaitForSeconds(0.1f);
        
        // if (Application.systemLanguage.Equals(SystemLanguage.Korean))
        // {
        //     LocalizationManager.CurrentLanguage = "Kr";
        // }
        // else
        // {
        //     LocalizationManager.CurrentLanguage = "En";
        // }

        yield return StartCoroutine(abilityManager.SetAbilityManager());
        yield return StartCoroutine(mergeController.Init());

        yield return StartCoroutine(inApp.Init());
        yield return StartCoroutine(RelicManager.Initialize());
        yield return StartCoroutine(CardManager.Initialize());
        yield return StartCoroutine(AudioManager.SetAudioManager());
        yield return StartCoroutine(trainingManager.SetTrainingManager());
        yield return StartCoroutine(MergeManager.SetMergeManager());
        yield return StartCoroutine(badgeController.Init());
        yield return StartCoroutine(UIController.instance.Init());
        DamageManager.SetPlayerDamageModelList();
        yield return StartCoroutine(BattleManager.Init());
        TutorialManager.instance.SetTutorialDictionary();
        yield return StartCoroutine(AdManager.Init());
        yield return StartCoroutine(ShopManager.Initialize());
        optionManager.Init();
        eventUI.Init();
        CheckNewUser();

        FadeOut(2f, null);


    }







    //ó�������ϴ� �����ΰ� üũ�ϱ�
    void CheckNewUser()
    {
        if (GameDataManager.instance.GetSaveData.isFirstConnect)
        {
            GameDataManager.instance.GetSaveData.isFirstConnect = false;
            TutorialManager.instance.CheckTutorialStep(TUTORIAL_STEP.COLLECT);//Ʃ�丮��
            CollectHero(0);
        }
        else
        {

            CheckTimeReset();

            //CheckAfterOneDay();//�� ���ӿ� ���� ���� �ʱ�ȭ ������ ��ģ �� ����ī��Ʈ ����
            OfflineGold();
        }
    }

    void CheckTimeReset()
    {
        CheckAdResetTime dTime = new CheckAdResetTime();
        dTime.CheckOverDayTime(OnChargeRewardCount);
    }

    

    void OnChargeRewardCount(string resetTime)
    {
        SaveData saveData = GameDataManager.instance.GetSaveData;

        saveData.advertiseSaveData.adRelicCount = Define.MAX_SUMMON_RELIC;
        saveData.advertiseSaveData.adMercenaryCount = Define.MAX_SUMMON_MERCENARY;
        saveData.advertiseSaveData.adFreeCallOfBell = Define.MAX_FREE_BELL;
        saveData.advertiseSaveData.adFreeDiamondCount = Define.MAX_FREE_DIAMOND;

        saveData.damageBuffData.buffCount= Define.MAX_FOOD_COUNT;
        saveData.goldBuffData.buffCount = Define.MAX_FOOD_COUNT;
        saveData.stoneBuffData.buffCount = Define.MAX_FOOD_COUNT;
        saveData.adResetTime = resetTime;
        saveData.godFingerLevel = 0;
        saveData.buyKeyCount = Define.MAX_BUY_DUNGEONKEY;

        GameDataManager.instance.SaveData();
    }

    //�뺴 �����ϱ�
    public void CollectHero(int index)
    {
        MergeManager.CurrentWaitMercenary--;

        var lv = CalcCollectLevelJump(index);

        mergeController.GetNewHeroByObjectPool.CreateCharacter(lv);
    }


    int CalcCollectLevelJump(int level)
    {
        var bJump = GameController.instance.abilityManager.TotalAbilityData(AbilityType.SpawnJump);

        //현재 고용중인 최고 등급의 영웅 레벨 가져오기
        int highest = GameController.instance.mergeController.GetHighestHeroLevel();

        if(level >= highest)
        {
            return level;
        }

        if (bJump > 0)
        {
            var jump = UnityEngine.Random.Range(0, 100);

            if (jump < bJump)
            {
                return level + 1;
            }
        }

        return level;

    }
    

    //���? ����
    public void SetBackground(int backIndex)
    {
        backGround.SetBackGround(backIndex);
    }

    public void EndGamePopUp()
    {
        GameDataManager.instance.SaveData();
        PopUpCanvasUI.ShowTwoButtonPopupp(GameDataManager.instance.SheetJsonLoader.GetConvertTextData(TextType.EndGame).Kr, SaveAndEndGame, null);
    }

    void SaveAndEndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #region ���̵� �� �ƿ�

    public void FadeInOut(float fadeOutTime, Action nextEvent = null)
    {
        IsGameStop = true;
        StartCoroutine(CoFadeInOut(fadeOutTime, nextEvent));
    }

    public void FadeIn(float fadeOutTime, Action nextEvent = null)
    {
        IsGameStop = true;
        StartCoroutine(CoFadeIn(fadeOutTime, nextEvent));
    }

    public void FadeOut(float fadeOutTime, Action nextEvent = null)
    {
        IsGameStop = true;
        StartCoroutine(CoFadeOut(fadeOutTime, nextEvent));
    }

    private IEnumerator CoFadeInOut(float fadeOutTime, System.Action nextEvent = null)
    {
        bool isBlack = false;
        fadeInOut.raycastTarget = true;
        Color tempColor = fadeInOut.color;
        //���� ����������
        while (tempColor.a < 1f && !isBlack)
        {
            tempColor.a += Time.deltaTime / fadeOutTime;
            fadeInOut.color = tempColor;

            if (tempColor.a >= 1f)
            {
                isBlack = true;
                tempColor.a = 1f;
            }

            yield return null;
        }

        if (nextEvent != null) nextEvent();

        while (tempColor.a > 0f && isBlack)
        {
            tempColor.a -= Time.deltaTime / fadeOutTime;
            fadeInOut.color = tempColor;

            if (tempColor.a <= 0f)
            {
                isBlack = false;
                tempColor.a = 0f;
            }

            yield return null;
        }

        fadeInOut.color = tempColor;

        fadeInOut.raycastTarget = false; ;
        
        IsGameStop = false;
    }

    private IEnumerator CoFadeIn(float fadeOutTime, System.Action nextEvent = null)
    {
        fadeInOut.raycastTarget = true;
        Color tempColor = fadeInOut.color;

        while (tempColor.a < 1f)
        {
            tempColor.a += Time.deltaTime / fadeOutTime;
            fadeInOut.color = tempColor;

            if (tempColor.a >= 1f) tempColor.a = 1f;

            yield return null;
        }

        fadeInOut.color = tempColor;

        fadeInOut.raycastTarget = false; ;

        if (nextEvent != null) nextEvent();
        yield return waitSecond;
        IsGameStop = false;

    }

    private IEnumerator CoFadeOut(float fadeOutTime, System.Action nextEvent = null)
    {
        Color tempColor = fadeInOut.color;
        fadeInOut.raycastTarget = true;
        while (tempColor.a > 0f)
        {
            tempColor.a -= Time.deltaTime / fadeOutTime;
            fadeInOut.color = tempColor;

            if (tempColor.a <= 0f) tempColor.a = 0f;

            yield return null;
        }

        fadeInOut.color = tempColor;

        fadeInOut.raycastTarget = false; ;

        if (nextEvent != null) nextEvent();
        yield return waitSecond;
        IsGameStop = false;
    }

    #endregion ���̵� �� �ƿ�

    //������ �ð� ����Ͽ�? ���? ����
    void OfflineGold()
    {
        var loadGold = GameDataManager.instance.SheetJsonLoader.GetEnemyData(EnemyTYPE.Elite, GameDataManager.instance.GetSaveData.battleSaveData.currentStage);
        var loadStone = GameDataManager.instance.SheetJsonLoader.GetEnemyData(EnemyTYPE.Unique, GameDataManager.instance.GetSaveData.battleSaveData.uniqueMonsterIndex);
        
        offLineGold = loadGold?.GOLD ?? 7;
        offLineStone = loadStone?.STONE ?? 7;


        string oldTime = GameDataManager.instance.GetSaveData.offlineGoldTime;

        DateTime sysTime = DateTime.Now;

        DateTime oldDateTime = DateTime.Parse(oldTime);

        TimeSpan ts = sysTime - oldDateTime;

        double diffDay = ts.TotalHours;

        //�ִ� 8�ð�
        if (diffDay >= 12)
        {
            diffDay = 12;
        }
        //�ּ� 0.1�ð�
        if (diffDay < 1)
        {
            diffDay = 0.01f;
        }

        float passTime = (float)diffDay;

        offLineGold = passTime * offLineGold;
        

        offLineStone = (passTime * 0.1f) * offLineStone;

        offLineGold = offLineGold + (offLineGold * abilityManager.TotalAbilityData(AbilityType.OfflineBonus));

        offLineStone = offLineStone + (offLineStone * abilityManager.TotalAbilityData(AbilityType.OfflineBonus));
        
        //보상 리스트를 만들고
        List<RewardInfoData> rewards = new List<RewardInfoData>();
        rewards.Add(new RewardInfoData(RewardTYPE.Gold, offLineGold));
        rewards.Add(new RewardInfoData(RewardTYPE.SpiritStone, offLineStone));
        //보상 타이틀명
        var title = LocalizationManager.GetTranslation("오프수익");
        //팝업 정보를 만들어서 전달
        PopupRewardInfoData infoData = new PopupRewardInfoData();
        infoData.SetTitle(title);
        infoData.SetRewardInfoList(rewards);
        PopupController.instance.SetupPopupInfo(infoData);


        //PopupController.instance.SetPopup(rewards, GameDataManager.instance.SheetJsonLoader.GetConvertTextData(TextType.TitleOffLine).Kr);
        //PopupController.instance.SetPopup(rewards, LocalizationManager.GetTranslation("오프수익"));

        GameDataManager.instance.GetSaveData.offlineGoldTime = System.DateTime.Now.ToString();
    }

    public void NetWork(Action action)
    {
        StartCoroutine(CheckInternetConnection(isConnected =>
        {
            if (isConnected)
            {
                if (action != null)
                {
                    action();
                }
                Debug.Log("Internet Available!");
            }
            else
            {
                UIController.instance.SendPopupMessage(AlarmTYPE.FAIL_NETWORK);
                Debug.Log("Internet Not Available");
            }
        }));
    }

    //���ͳ� �������? üũ
    private IEnumerator CheckInternetConnection(Action<bool> action)
    {
        UnityWebRequest request = new UnityWebRequest("http://google.com");

        yield return request.SendWebRequest();

        if (request.error != null)
        {
            action(false);
        }
        else
        {
            action(true);
        }
    }
    #region Application System

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            isPaused = true;
            Time.timeScale = 0;
            isGameStop = true;
        }
        else
        {
            if (isPaused)
            {
                isPaused = false;
                Time.timeScale = 1;
                isGameStop = false;
            }
        }
    }

    private void OnApplicationQuit()
    {
        IsGameStop = true;
        GameDataManager.instance.GetSaveData.offlineGoldTime = System.DateTime.Now.ToString();
        GameDataManager.instance.SaveData();
    }
    #endregion Application System

    //     /* 해상도 설정하는 함수 */
    // void SetResolution()
    // {
    //     int setWidth = 1920; // 사용자 설정 너비
    //     int setHeight = 1080; // 사용자 설정 높이

    //     int deviceWidth = Screen.width; // 기기 너비 저장
    //     int deviceHeight = Screen.height; // 기기 높이 저장

    //     Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true); // SetResolution 함수 제대로 사용하기

    //     if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) // 기기의 해상도 비가 더 큰 경우
    //     {
    //         float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight); // 새로운 너비
    //         Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // 새로운 Rect 적용
    //     }
    //     else // 게임의 해상도 비가 더 큰 경우
    //     {
    //         float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight); // 새로운 높이
    //         Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // 새로운 Rect 적용
    //     }
    // }
}