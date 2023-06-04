using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using Spine;
using static UnityEngine.GraphicsBuffer;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    private enum FunctionButton
    { COLLECT, AUTO_MERGE, AUTO_COLLECT, GIVE_UP, CHALLANGE_BOSS, FEVER }

    private enum FuncText
    { RESERVE, SUPPLY, BOSS_TIME, LEADER_COUNT, AUTO_MERGE, AUTO_COLLECT, FEVER }

    private enum FuncImage
    { COLLECT, AUTO_COLLECT, AUTO_MERGE, BOSS_TIME, LEADER_COUNT }

    private enum UnLockButton
    { Card,Training, Relic, Raid, Shop, Ad,AutoMerge,AutoCollect,BellOfCall,Event,Finger,Sorting }

    [Header("액션 버튼")]
    [SerializeField] private Button[] buttons = null;

    [Header("텍스트 UI")]
    [SerializeField] private TextMeshProUGUI[] funcText = null;

    [Header("메뉴 버튼")]
    [SerializeField] private Button[] menuButtons = null;

    [Header("UIManager 상속받는 UI")]
    [SerializeField] private UIManager[] uiMenus = null;

    [Header("FillImage UI")]
    [SerializeField] private Image[] fillImages = null;

    [Header("재화 UI")]
    [SerializeField] private GoodsUI[] goodsUIs;

    [Header("튜토리얼 UnLock 버튼&메뉴")]
    public GameObject[] unLockGameObject;

    public FloatingCurrency goldFloating;
    public FloatingCurrency diamondFloating;
    public FloatingCurrency stoneFloating;

    [SerializeField] public NewHero newHero;

    //포기 버튼
    [SerializeField] private GameObject giveUpGameObject;

    //레이드 타이머
    [SerializeField] private GameObject bossTimerGameObject;

    //리더 카운트
    [SerializeField] private GameObject leaderCountGameObject;

    [SerializeField] private GameObject bossPanelGameObject;

    //보스 도전 버튼
    [SerializeField] public GameObject challangeBossGameObject;

    [SerializeField] public GameObject loadingImage;
    [SerializeField] private NotifyStage mNotifyStage = null;

    private bool isAutoMerge = false;
    private bool isAutoCollect = false;
    private float chargeTime;
    private float autoMergeTime;
    private float autoCollectTime;
    public AnimationCurve curve;

    public DungeonUI dungeonUI;
    public AlarmPopup alarmPopup;
    public FailChallange failChallange;
    public SimpleRewardPopup simplePopup;
    private AbilityManager abilityManager;
    private GameDataManager gameDataManager;


    public ParticleSystem bellOfCallParticle;

    Character mergeHero;
    Character upgradeHero;
    int mergeLevel;



    private void Awake()
    {
        instance = this;
        menuButtons[(int)MenuType.TRAINING].onClick.AddListener(() => OpenMenu(MenuType.TRAINING));
        menuButtons[(int)MenuType.MERCENARY].onClick.AddListener(() => OpenMenu(MenuType.MERCENARY));
        menuButtons[(int)MenuType.RELIC].onClick.AddListener(() => OpenMenu(MenuType.RELIC));
        menuButtons[(int)MenuType.BOSSRAID].onClick.AddListener(() => OpenMenu(MenuType.BOSSRAID));
        menuButtons[(int)MenuType.SHOP].onClick.AddListener(() => OpenMenu(MenuType.SHOP));
        menuButtons[(int)MenuType.ADVERTISE].onClick.AddListener(() => OpenMenu(MenuType.ADVERTISE));
        menuButtons[(int)MenuType.OPTION].onClick.AddListener(() => OpenMenu(MenuType.OPTION));
        menuButtons[(int)MenuType.EVENT].onClick.AddListener(() => OpenMenu(MenuType.EVENT));

        buttons[0].onClick.AddListener(ClickCollectMenu);
        buttons[1].onClick.AddListener(ButtonAutoMerge);
        buttons[2].onClick.AddListener(ButtonAutoCollect);
        buttons[3].onClick.AddListener(GiveUpBattle);
        buttons[4].onClick.AddListener(ChallangeBoss);
        buttons[5].onClick.AddListener(TheCallOfBell);
        
    }

    private void Start()
    {
        abilityManager = GameController.instance.abilityManager;
        gameDataManager = GameDataManager.instance;
        EliteChallangeButton(false);
    }

    public IEnumerator Init()
    {
        chargeTime = abilityManager.TotalAbilityData(AbilityType.CollectSpeed);
        autoMergeTime = abilityManager.TotalAbilityData(AbilityType.AutoMergeTime);
        autoCollectTime = abilityManager.TotalAbilityData(AbilityType.AutoCollectTime);
        funcText[(int)FuncText.RESERVE].text = string.Format("{0}/{1}", GameController.instance.MergeManager.CurrentBattleMercenary, GameController.instance.MergeManager.GetMaxBattleMercenary);

        var gold = gameDataManager.GetCurrency(RewardTYPE.Gold);
        UpdateGoldText(NumberToSymbol.ChangeNumber(gold),"",Color.white);

        var diamond = gameDataManager.GetCurrency(RewardTYPE.Diamond);
        UpdateDiamondText(diamond.ToString(), "", Color.white);

        var symbol = NumberToSymbol.ChangeNumber(gameDataManager.GetCurrency(RewardTYPE.SpiritStone));

        UpdateSpiritStoneText(symbol, "", Color.white);

        dungeonUI.Init();

        UpdateToturialUI(gameDataManager.GetSaveData.tutorialStep);

        UpdateReserveUI();
        UpdateSupplyUI();
        UpdateTheBellOfCall();
        StartCoroutine(ChargeCollect());
        StartCoroutine(AutoMergeDuration());
        StartCoroutine(AutoCollectDuration());
        yield return null;

    }

    //메세지 알람 팝업
    public void SendPopupMessage(AlarmTYPE msg)
    {
        alarmPopup.ShowAlarmMessage(msg);
    }

    //확정 보상 팝업
    public void ShowSimpleRewardPopUp(string msg, Sprite sprite, Action callBack = null)
    {
        simplePopup.SetPopup(msg, sprite, callBack);
    }

    //보스 도전 버튼 활성화/비활성화
    public void EliteChallangeButton(bool active)
    {
        challangeBossGameObject.SetActive(active);
    }

    //보스전투 UI 활성화/비활성화
    public void SetChallangeBattleUI(bool active)
    {
        giveUpGameObject.SetActive(active);
        bossTimerGameObject.SetActive(active);
        bossPanelGameObject.SetActive(active);
        leaderCountGameObject.SetActive(!active);
    }

    //UI 세팅
    public void SetUI(EnemyTYPE type, int stage)
    {

        GameController.instance.IsGameStop = false;
        mNotifyStage.Notify(stage, type);

        switch (type)
        {
            case EnemyTYPE.Common:

                break;

            case EnemyTYPE.Elite:
                SetGiveUpButton(GiveUpBattle);
                StartCoroutine(CorBossTimer(EnemyTYPE.Elite));
                break;

            case EnemyTYPE.Unique:
                StartCoroutine(CorBossTimer(EnemyTYPE.Unique));
                break;

            default:
                break;
        }
    }

    //오픈 메뉴
    public void OpenMenu(MenuType type)
    {
        if (GameController.instance.BattleManager.IsBoss)
        {
            SendPopupMessage(AlarmTYPE.CAN_NOT_OPEN_CHALLENGE);
            return;
        }

        uiMenus[(int)type].OpenUI();


        if(TutorialManager.instance.CheckTutorialStep(TUTORIAL_STEP.UPGRADE_TRAINING)) return;
        if(TutorialManager.instance.CheckTutorialStep(TUTORIAL_STEP.UPGRADE_HERO)) return;
        if(TutorialManager.instance.CheckTutorialStep(TUTORIAL_STEP.CHALLENGE_RAID_BOSS)) return;
        if(TutorialManager.instance.CheckTutorialStep(TUTORIAL_STEP.FREE_BUFF)) return;
        if (TutorialManager.instance.CheckTutorialStep(TUTORIAL_STEP.FREE_RELIC)) return;

    }

    public void CloseAllMenu()
    {
        for (int i = 0; i < uiMenus.Length; i++)
        {
            uiMenus[i].CloseUI();
        }
    }

    //보스 포기 버튼 액션 세팅
    public void SetGiveUpButton(UnityEngine.Events.UnityAction action)
    {
        buttons[(int)FunctionButton.GIVE_UP].onClick.RemoveAllListeners();
        buttons[(int)FunctionButton.GIVE_UP].onClick.AddListener(action);
    }

    //엘리트보스 도전 버튼 클릭
    public void ChallangeBoss()
    {
        if (GameController.instance.BattleManager.IsBoss) return;
        GameController.instance.FadeInOut(1f, CallBackChallengeBoss);
        GameController.instance.BattleManager.IsBoss = true;
    }

    //도전 버튼 클릭 이후 이벤트
    private void CallBackChallengeBoss()
    {
        GameController.instance.BattleManager.SpawnEnemy(EnemyTYPE.Elite);
    }

    //엘리트 도전 포기 버튼 클릭
    private void GiveUpBattle()
    {
        GameController.instance.BattleManager.IsBoss = false;
        GameController.instance.FadeInOut(1f, CallBackGiveUpBattle);
    }

    //엘리트 도전 포기 버튼 클릭 이후 이벤트
    private void CallBackGiveUpBattle()
    {
        if (GameController.instance.BattleManager.enemyBase != null)
        {
            GameController.instance.BattleManager.enemyBase.ForceCommonEliteKill();
        }
        
        EliteChallangeButton(true);//보스 도전 버튼 활성화
        GameController.instance.BattleManager.SpawnEnemy(EnemyTYPE.Common);//일반 몬스터 생성
    }

    //신규 용병 획득 알림 창
    public void OpenNewHeroPopUp(MercenaryType type)
    {
        GameDataManager.instance.GetSaveData.unLockLevel = (int)type;
        //용병레벨에 따른 보상 테이블 작성
        newHero.SetRewardData(new DropTable(
            RewardTYPE.Gold,
            gameDataManager.SheetJsonLoader.GetPlayerData(type).REWARD
            ),
            type);
    }

    //모집 가능한 용병이 있는지 체크
    public bool IsCanCollectMercenary()
    {
        //대기중인 용병이 있고 현재 고용중인 용병이 최대 고용 가능한 용병 수 보다 작다면
        if (GameController.instance.MergeManager.CurrentWaitMercenary > 0 && !GameController.instance.MergeManager.IsBattleHeroFull())
        {
            return true;
        }
        return false;
    }

    //대기 용병이 꽉 찾는지 확인
    public bool IsFullWaitingHero()
    {
        if (GameController.instance.MergeManager.CurrentWaitMercenary >= GameController.instance.MergeManager.GetMaxWaitMercenary)
        {
            return true;
        }

        return false;
    }

    //용명 충전
    private IEnumerator ChargeCollect()
    {
        float time = 0;

        fillImages[(int)FuncImage.COLLECT].fillAmount = 0;

        while (true)
        {
            if (!GameController.instance.MergeManager.IsWaitFull())
            {
                time += Time.deltaTime;
                fillImages[(int)FuncImage.COLLECT].fillAmount = time / chargeTime;

                if (time > chargeTime)
                {
                    time = 0;
                    GameController.instance.MergeManager.CurrentWaitMercenary++;
                    fillImages[(int)FuncImage.COLLECT].DOKill(true);
                    fillImages[(int)FuncImage.COLLECT].transform.parent.DOScale(1.5f, 0.2f).SetEase(curve);
                }
            }
            yield return null;
        }
    }
    //보스타이머
    private IEnumerator CorBossTimer(EnemyTYPE monsterType)
    {
        float currentTime = Define.BOSS_COUNT_TIME;

        while (true)
        {
            //보스를 처치했을때 처리
            if (!GameController.instance.BattleManager.IsBoss)
            {
                yield break;
            }

            currentTime -= Time.deltaTime;

            funcText[(int)FuncText.BOSS_TIME].text = string.Format("{0:F1}", currentTime);

            fillImages[(int)FuncImage.BOSS_TIME].fillAmount = currentTime / Define.BOSS_COUNT_TIME;

            if (currentTime <= 0)
            {
                currentTime = 0;
                GameController.instance.BattleManager.enemyBase.isDie = true;
                GameController.instance.BattleManager.IsBoss = false;
                if (monsterType.Equals(EnemyTYPE.Elite)) GiveUpBattle();
                if (monsterType.Equals(EnemyTYPE.Unique)) failChallange.ShowFailChallangeUI(dungeonUI.GiveUp);

                yield break;
            }

            yield return null;
        }
    }

    #region UPDATE UI

    public void UpdateProgressLeaderCountUI(int stage, int step)
    {
        var data = gameDataManager.SheetJsonLoader.GetEnemyData(EnemyTYPE.Common, stage).LEADER_STAGE;
        funcText[(int)FuncText.LEADER_COUNT].text = string.Format("{0} / {1}", step, data);
        fillImages[(int)FuncImage.LEADER_COUNT].fillAmount = (float)step / (float)data;
    }

    public void UpdateTimeUI()
    {
        //Debug.Log("모집속도(훈련:" + GetTrainingDataByLevel(TRAINING_TYPE.S_COLLECT_SPEED) + "초+유물:" + FindAbilityToDictionary(AbilityType.CollectSpeed) + "초) 합계<"+ GetTrainingDataByLevel(TRAINING_TYPE.S_COLLECT_SPEED) + FindAbilityToDictionary(AbilityType.CollectSpeed)+"초>");
        chargeTime = abilityManager.TotalAbilityData(AbilityType.CollectSpeed);
        autoMergeTime = abilityManager.TotalAbilityData(AbilityType.AutoMergeTime);
        autoCollectTime = abilityManager.TotalAbilityData(AbilityType.AutoCollectTime);
    }

    public void UpdateGoldText(string total, string add,Color color)
    {
        goodsUIs[0].UpdateGoodsValue(total);
        goldFloating.SetCurrencyText(add, color);
    }

    public void UpdateDiamondText(string total, string add, Color color)
    {
        goodsUIs[1].UpdateGoodsValue(total);
        diamondFloating.SetCurrencyText(add, color);
    }

    public void UpdateSpiritStoneText(string total, string add, Color color)
    {
        goodsUIs[2].UpdateGoodsValue(total);
        stoneFloating.SetCurrencyText(add, color);
    }

    public void UpdateReserveUI()
    {
        funcText[(int)FuncText.RESERVE].text = string.Format("{0}/{1}", GameController.instance.MergeManager.CurrentBattleMercenary, GameController.instance.MergeManager.GetMaxBattleMercenary);
        funcText[(int)FuncText.RESERVE].DOKill(true);
        funcText[(int)FuncText.RESERVE].DOScale(1.2f, 0.2f).SetEase(curve);
    }

    public void UpdateSupplyUI()
    {
        funcText[(int)FuncText.SUPPLY].text = string.Format("{0} / {1}", GameController.instance.MergeManager.CurrentWaitMercenary, GameController.instance.MergeManager.GetMaxWaitMercenary);
        funcText[(int)FuncText.SUPPLY].DOKill(true);
        funcText[(int)FuncText.SUPPLY].DOScale(1.2f, 0.2f).SetEase(curve);
    }

    public void UpdateTheBellOfCall()
    {
        funcText[(int)FuncText.FEVER].text = string.Format("{0}", GameDataManager.instance.GetSaveData.theBellOfCallCount);
    }

    #endregion UPDATE UI

    #region UI Controll

    public void ShowLoadingImage(bool active)
    {
        loadingImage.SetActive(active);
    }

    public void TheCallOfBell()
    {
        //부름의 종이 충분한지 체크

        if (GameDataManager.instance.GetSaveData.theBellOfCallCount <= 0)
        {
            SendPopupMessage(AlarmTYPE.NOT_ENOUGH_BELLOFCALL);
            return;
        }
        //현재 모집가능한 용병 수가 꽉찾는지 체크
        if (IsFullWaitingHero())
        {
            SendPopupMessage(AlarmTYPE.FULL_HERO);
            return;
        }



        //부름의 종 카운팅
        GameDataManager.instance.GetSaveData.theBellOfCallCount--;

        //모집가능한 용병수를 끝까지 늘리기
        GameController.instance.MergeManager.CurrentWaitMercenary = GameController.instance.MergeManager.GetMaxWaitMercenary;

        bellOfCallParticle.Stop();
        bellOfCallParticle.Play();


        GameController.instance.AudioManager.PlayEffect(ESoundEffect.Bell);


        UpdateTheBellOfCall();
    }

    //용병 고용하기 버튼 / 용병 생성할떄 시작 용병 레벨 업그레이드 체크
    public void ClickCollectMenu()
    {
        //모집이 가능한 용병이 있다면
        if (IsCanCollectMercenary())
        {

            int startLevel = (int)GameController.instance.abilityManager.TotalAbilityData(AbilityType.HeroStartLevel);
            //Debug.Log("시작레벨 : " + startLevel);
            GameController.instance.CollectHero(startLevel);
        }

        TutorialManager.instance.CheckTutorialStep(TUTORIAL_STEP.MERGE);//튜토리얼
    }

    //자동 머지 버튼
    public void ButtonAutoMerge()
    {
        isAutoMerge = !isAutoMerge;

        if (isAutoMerge)
        {
            fillImages[(int)FuncImage.AUTO_MERGE].color = Color.white;
        }
        else
        {
            fillImages[(int)FuncImage.AUTO_MERGE].color = Color.black;
        }
    }

    //자동 출전 버튼
    private void ButtonAutoCollect()
    {
        isAutoCollect = !isAutoCollect;

        if (isAutoCollect)
        {
            fillImages[(int)FuncImage.AUTO_COLLECT].color = Color.white;
        }
        else
        {
            fillImages[(int)FuncImage.AUTO_COLLECT].color = Color.black;
        }
    }

    private IEnumerator AutoCollectDuration()
    {
        yield return new WaitForSeconds(1f);

        while (true)
        {
            if (isAutoCollect)
            {
                float currentTime = 0;
                float duration = autoCollectTime;

                while (currentTime < duration)
                {
                    currentTime += Time.deltaTime;

                    fillImages[(int)FuncImage.AUTO_COLLECT].fillAmount = currentTime / duration;
                    funcText[(int)FuncText.AUTO_COLLECT].text = $"{duration - currentTime:F0}s";

                    yield return null;
                }

                ClickCollectMenu();
            }
            else
            {
                funcText[(int)FuncText.AUTO_COLLECT].text = "";
            }

            yield return new WaitForSeconds(0.1f);
        }

    }

    private IEnumerator AutoMergeDuration()
    {
        yield return new WaitForSeconds(1f);

        while (true)
        {
            if (isAutoMerge)
            {
                float currentTime = 0;
                float duration = autoMergeTime;

                while (currentTime < duration)
                {
                    currentTime += Time.deltaTime;

                    fillImages[(int)FuncImage.AUTO_MERGE].fillAmount = currentTime / duration;
                    funcText[(int)FuncText.AUTO_MERGE].text = $"{duration - currentTime:F0}s";

                    yield return null;
                }

                GameController.instance.mergeController.Merging();
            }
            else
            {
                funcText[(int)FuncText.AUTO_MERGE].text = "";
            }

            yield return new WaitForSeconds(0.1f);
        }
    }





    public void UpdateToturialUI(TUTORIAL_STEP step)
    {
        //버튼에서 언락 블랙패널
        unLockGameObject[(int)UnLockButton.Card].SetActive(true);
        unLockGameObject[(int)UnLockButton.Training].SetActive(true);
        unLockGameObject[(int)UnLockButton.Relic].SetActive(true);
        unLockGameObject[(int)UnLockButton.Raid].SetActive(true);
        unLockGameObject[(int)UnLockButton.Shop].SetActive(true);
        //게임 오브젝트 활성화/비활성화
        unLockGameObject[(int)UnLockButton.Ad].SetActive(false);
        unLockGameObject[(int)UnLockButton.AutoMerge].SetActive(false);
        unLockGameObject[(int)UnLockButton.AutoCollect].SetActive(false);
        unLockGameObject[(int)UnLockButton.BellOfCall].SetActive(false);
        unLockGameObject[(int)UnLockButton.Event].SetActive(false);
        unLockGameObject[(int)UnLockButton.Finger].SetActive(false);
        unLockGameObject[(int)UnLockButton.Sorting].SetActive(false);

        switch (step)
        {
            case TUTORIAL_STEP.COLLECT:
                unLockGameObject[(int)UnLockButton.Card].SetActive(false);
                break;
            case TUTORIAL_STEP.MERGE:
                unLockGameObject[(int)UnLockButton.Card].SetActive(false);
                break;
            case TUTORIAL_STEP.OPEN_HERO_MENU:
                unLockGameObject[(int)UnLockButton.Card].SetActive(false);
                break;
            case TUTORIAL_STEP.UPGRADE_HERO:
                unLockGameObject[(int)UnLockButton.Card].SetActive(false);
                break;
            case TUTORIAL_STEP.OPEN_TRAINING:
                unLockGameObject[(int)UnLockButton.Training].SetActive(false);
                unLockGameObject[(int)UnLockButton.Card].SetActive(false);
                break;
            case TUTORIAL_STEP.UPGRADE_TRAINING:
                unLockGameObject[(int)UnLockButton.Card].SetActive(false);
                unLockGameObject[(int)UnLockButton.Training].SetActive(false);
                break;
            case TUTORIAL_STEP.OPEN_RAID_MENU:
                unLockGameObject[(int)UnLockButton.Card].SetActive(false);
                unLockGameObject[(int)UnLockButton.Training].SetActive(false);
                unLockGameObject[(int)UnLockButton.Raid].SetActive(false);
                break;
            case TUTORIAL_STEP.CHALLENGE_RAID_BOSS:
                unLockGameObject[(int)UnLockButton.Card].SetActive(false);
                unLockGameObject[(int)UnLockButton.Training].SetActive(false);
                unLockGameObject[(int)UnLockButton.Raid].SetActive(false);
                break;
            case TUTORIAL_STEP.GUIDE_AUTO_SKILL:
                unLockGameObject[(int)UnLockButton.Card].SetActive(false);
                unLockGameObject[(int)UnLockButton.Training].SetActive(false);
                unLockGameObject[(int)UnLockButton.Raid].SetActive(false);
                unLockGameObject[(int)UnLockButton.AutoMerge].SetActive(true);
                unLockGameObject[(int)UnLockButton.AutoCollect].SetActive(true);
                break;
            case TUTORIAL_STEP.OPEN_AD_MENU:
                unLockGameObject[(int)UnLockButton.Card].SetActive(false);
                unLockGameObject[(int)UnLockButton.Training].SetActive(false);
                unLockGameObject[(int)UnLockButton.Raid].SetActive(false);
                unLockGameObject[(int)UnLockButton.AutoMerge].SetActive(true);
                unLockGameObject[(int)UnLockButton.AutoCollect].SetActive(true);
                unLockGameObject[(int)UnLockButton.Ad].SetActive(true);
                break;
            case TUTORIAL_STEP.FREE_BUFF:
                unLockGameObject[(int)UnLockButton.Card].SetActive(false);
                unLockGameObject[(int)UnLockButton.Training].SetActive(false);
                unLockGameObject[(int)UnLockButton.Raid].SetActive(false);
                unLockGameObject[(int)UnLockButton.AutoMerge].SetActive(true);
                unLockGameObject[(int)UnLockButton.AutoCollect].SetActive(true);
                unLockGameObject[(int)UnLockButton.Ad].SetActive(true);
                break;
            case TUTORIAL_STEP.OPEN_SHOP:
                unLockGameObject[(int)UnLockButton.Card].SetActive(false);
                unLockGameObject[(int)UnLockButton.Training].SetActive(false);
                unLockGameObject[(int)UnLockButton.Raid].SetActive(false);
                unLockGameObject[(int)UnLockButton.Shop].SetActive(false);
                unLockGameObject[(int)UnLockButton.AutoMerge].SetActive(true);
                unLockGameObject[(int)UnLockButton.AutoCollect].SetActive(true);
                unLockGameObject[(int)UnLockButton.Ad].SetActive(true);
                break;
            case TUTORIAL_STEP.FREE_RELIC:
                unLockGameObject[(int)UnLockButton.Card].SetActive(false);
                unLockGameObject[(int)UnLockButton.Training].SetActive(false);
                unLockGameObject[(int)UnLockButton.Raid].SetActive(false);
                unLockGameObject[(int)UnLockButton.Shop].SetActive(false);
                unLockGameObject[(int)UnLockButton.AutoMerge].SetActive(true);
                unLockGameObject[(int)UnLockButton.AutoCollect].SetActive(true);
                unLockGameObject[(int)UnLockButton.Ad].SetActive(true);
                break;
            case TUTORIAL_STEP.OPEN_RELIC_MENU:
                unLockGameObject[(int)UnLockButton.Card].SetActive(false);
                unLockGameObject[(int)UnLockButton.Training].SetActive(false);
                unLockGameObject[(int)UnLockButton.Raid].SetActive(false);
                unLockGameObject[(int)UnLockButton.Shop].SetActive(false);
                unLockGameObject[(int)UnLockButton.Relic].SetActive(false);
                unLockGameObject[(int)UnLockButton.AutoMerge].SetActive(true);
                unLockGameObject[(int)UnLockButton.AutoCollect].SetActive(true);
                unLockGameObject[(int)UnLockButton.Ad].SetActive(true);
                break;
            case TUTORIAL_STEP.GIFT_CURRENCY:
                unLockGameObject[(int)UnLockButton.Card].SetActive(false);
                unLockGameObject[(int)UnLockButton.Training].SetActive(false);
                unLockGameObject[(int)UnLockButton.Raid].SetActive(false);
                unLockGameObject[(int)UnLockButton.Shop].SetActive(false);
                unLockGameObject[(int)UnLockButton.Relic].SetActive(false);
                unLockGameObject[(int)UnLockButton.AutoMerge].SetActive(true);
                unLockGameObject[(int)UnLockButton.AutoCollect].SetActive(true);
                unLockGameObject[(int)UnLockButton.Ad].SetActive(true);
                unLockGameObject[(int)UnLockButton.BellOfCall].SetActive(true);
                unLockGameObject[(int)UnLockButton.Event].SetActive(true);
                unLockGameObject[(int)UnLockButton.Finger].SetActive(true);
                unLockGameObject[(int)UnLockButton.Sorting].SetActive(true);
                break;
            case TUTORIAL_STEP.END:
                unLockGameObject[(int)UnLockButton.Card].SetActive(false);
                unLockGameObject[(int)UnLockButton.Training].SetActive(false);
                unLockGameObject[(int)UnLockButton.Raid].SetActive(false);
                unLockGameObject[(int)UnLockButton.Shop].SetActive(false);
                unLockGameObject[(int)UnLockButton.Relic].SetActive(false);
                unLockGameObject[(int)UnLockButton.AutoMerge].SetActive(true);
                unLockGameObject[(int)UnLockButton.AutoCollect].SetActive(true);
                unLockGameObject[(int)UnLockButton.Ad].SetActive(true);
                unLockGameObject[(int)UnLockButton.BellOfCall].SetActive(true);
                unLockGameObject[(int)UnLockButton.Event].SetActive(true);
                unLockGameObject[(int)UnLockButton.Finger].SetActive(true);
                unLockGameObject[(int)UnLockButton.Sorting].SetActive(true);
                break;

            default:
                break;
        }


    }

    #endregion UI Controll
}