using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using static TutorialManager;
using System;
using System.Linq;

[System.Serializable]
public class TutorialModel
{
    public string description;
    public TUTORIAL_STEP tutorialStep;
    public int unLockStageLevel;
    public CanvasGroup canvasGroup;
    public Button btn;
    public TextMeshProUGUI text;
}

public enum TUTORIAL_STEP
{
    NONE,
    COLLECT,//모집하기
    MERGE,//합성하기
    OPEN_HERO_MENU,//영웅 메뉴 열기
    UPGRADE_HERO,//영웅 업그레이드 하기
    OPEN_TRAINING,//훈련메뉴 열기
    UPGRADE_TRAINING,//훈련 업그레이드 하기
    OPEN_RAID_MENU,//레이드 메뉴 열기
    CHALLENGE_RAID_BOSS,//보스에 도전하기
    GUIDE_AUTO_SKILL,//자동 출전, 합성 오픈
    OPEN_AD_MENU,//광고 메뉴 열기
    FREE_BUFF,//광고 버프 1회 무료
    OPEN_SHOP,//상점 열기
    FREE_RELIC,//유물 뽑기 1회 무료
    OPEN_RELIC_MENU,//유물 메뉴 열기
    GIFT_CURRENCY,//튜토리얼 끝 보상
    END
}

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    [SerializeField] private TutorialSlot[] slots;

    private TUTORIAL_STEP CurrentStep { get; set; }
    private TUTORIAL_STEP NextStep { get; set; }

    public delegate void NextEvent();

    [SerializeField] private Dictionary<TUTORIAL_STEP, NextEvent> eventDictionary = new Dictionary<TUTORIAL_STEP, NextEvent>();

    Tutorial tutorialData;

    private void Awake()
    {
        instance = this;
    }

    public void SetTutorialDictionary()
    {
        //17개
        eventDictionary.Add(TUTORIAL_STEP.COLLECT, Collect);
        eventDictionary.Add(TUTORIAL_STEP.MERGE, Merge);
        eventDictionary.Add(TUTORIAL_STEP.OPEN_HERO_MENU, OpenHeroMenu);
        eventDictionary.Add(TUTORIAL_STEP.UPGRADE_HERO, UpgradeHero);
        eventDictionary.Add(TUTORIAL_STEP.OPEN_TRAINING, OpenTraining);
        eventDictionary.Add(TUTORIAL_STEP.UPGRADE_TRAINING, UpgradeTraining);
        eventDictionary.Add(TUTORIAL_STEP.OPEN_RAID_MENU, OpenRaidMenu);
        eventDictionary.Add(TUTORIAL_STEP.CHALLENGE_RAID_BOSS, ChallengeRaidBoss); 
        eventDictionary.Add(TUTORIAL_STEP.GUIDE_AUTO_SKILL, GuideAutoSkill);
        eventDictionary.Add(TUTORIAL_STEP.OPEN_AD_MENU, OpenAdMenu);
        eventDictionary.Add(TUTORIAL_STEP.FREE_BUFF, FreeBuff);
        eventDictionary.Add(TUTORIAL_STEP.OPEN_SHOP, OpenShop);
        eventDictionary.Add(TUTORIAL_STEP.FREE_RELIC, FreeRelic);
        eventDictionary.Add(TUTORIAL_STEP.OPEN_RELIC_MENU, OpenRelicMenu);
        eventDictionary.Add(TUTORIAL_STEP.GIFT_CURRENCY, GiftCurrency);

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetSlot(OnNextEvent, (TUTORIAL_STEP)i+1);
        }

        CurrentStep = GameDataManager.instance.GetSaveData.tutorialStep;

    }

    public bool CheckTutorialStep(TUTORIAL_STEP step)
    {
        var stage = GameController.instance.BattleManager.GetStage();

        if (stage >= GameDataManager.instance.SheetJsonLoader.GetTutorialData(step).Stage)
        {
            //Debug.Log($"<color=red> CurrentStep : {CurrentStep} </color> / step : {step}");


            if (CurrentStep < step && CurrentStep == step-1)
            {
                CurrentStep = step;

                UIController.instance.UpdateToturialUI(CurrentStep);


                if (CurrentStep == TUTORIAL_STEP.OPEN_TRAINING || CurrentStep == TUTORIAL_STEP.MERGE || CurrentStep == TUTORIAL_STEP.OPEN_HERO_MENU || CurrentStep == TUTORIAL_STEP.GUIDE_AUTO_SKILL
                    || CurrentStep == TUTORIAL_STEP.OPEN_RAID_MENU || CurrentStep == TUTORIAL_STEP.OPEN_AD_MENU || CurrentStep == TUTORIAL_STEP.OPEN_SHOP
                    || CurrentStep == TUTORIAL_STEP.GIFT_CURRENCY)
                {
                    UIController.instance.CloseAllMenu();
                }

                slots[(int)CurrentStep - 1].StartTutorial();


                if (CurrentStep == TUTORIAL_STEP.OPEN_HERO_MENU)
                {
                    slots[(int)TUTORIAL_STEP.MERGE - 1].EndTutorial();

                }


                return true;
            }

        }

        return false;
    }

    //튜토리얼 시작 (화면 가이드)
    //public void OnStartTutorial(TUTORIAL_STEP step)
    //{
    //    Debug.Log($"<color=red> 현재 진행중인 튜토리얼 : {CurrentStep} </color> / 다음 튜토리얼 : {step}");
    //    var stage = GameController.instance.BattleManager.GetStage();

    //    if (stage >= GameDataManager.instance.GetTutorialData(step).Stage)
    //    {
    //        Debug.Log($"<color=red> CurrentStep : {CurrentStep} </color> / step : {step}");


    //        if (CurrentStep < step)
    //        {
    //            CurrentStep = step;

    //            UIController.instance.UpdateToturialUI(CurrentStep);


    //            if (CurrentStep == TUTORIAL_STEP.KILL_LEADER || CurrentStep == TUTORIAL_STEP.OPEN_TRAINING || CurrentStep == TUTORIAL_STEP.MERGE || CurrentStep == TUTORIAL_STEP.OPEN_HERO_MENU || CurrentStep == TUTORIAL_STEP.GUIDE_AUTO_SKILL
    //                || CurrentStep == TUTORIAL_STEP.OPEN_RAID_MENU || CurrentStep == TUTORIAL_STEP.OPEN_AD_MENU || CurrentStep == TUTORIAL_STEP.OPEN_SHOP || CurrentStep == TUTORIAL_STEP.OPEN_RELIC_MENU
    //                || CurrentStep == TUTORIAL_STEP.GIFT_CURRENCY)
    //            {
    //                GameController.instance.AllCloseUI();
    //            }

    //            slots[(int)CurrentStep-1].StartTutorial();
    //        }

    //    }
    //    else
    //    {
    //        Debug.Log($"<color=red> 현재 스테이지 : {stage} </color> / 목표 스테이지 : {GameDataManager.instance.GetTutorialData(step).Stage}");

    //    }

    //}

    //튜토리얼 화면 버튼을 눌렀을때 실행되는 메소드

    private void OnNextEvent()
    {
        if (eventDictionary.ContainsKey(CurrentStep))
        {
            if (eventDictionary[CurrentStep] != null)
            {
                slots[(int)CurrentStep - 1].EndTutorial();

                GameDataManager.instance.GetSaveData.tutorialStep = CurrentStep;

                eventDictionary[CurrentStep].Invoke();
            }
        }
    }
    #region CallBackEvent

    private void Collect()
    {
        UIController.instance.ClickCollectMenu();
    }

    private void Merge()
    {
        slots[(int)TUTORIAL_STEP.MERGE-1].EndTutorial();
    }
    private void OpenHeroMenu()
    {
        UIController.instance.OpenMenu(MenuType.MERCENARY);
    }

    private void UpgradeHero()
    {
        GameController.instance.CardManager.UpGradeHero();
    }
    private void KillLeader()
    {
        UIController.instance.ChallangeBoss();
    }
    private void OpenTraining()
    {
        UIController.instance.OpenMenu(MenuType.TRAINING);
    }

    private void UpgradeTraining()
    {
        GameController.instance.trainingManager.TutorialUpgrade();
    }

    private void GuideAutoSkill()
    {
        UIController.instance.ButtonAutoMerge();
    }

    private void OpenRaidMenu()
    {
        UIController.instance.OpenMenu(MenuType.BOSSRAID);
    }

    private void ChallengeRaidBoss()
    {
        UIController.instance.dungeonUI.Challenge();
    }

    private void OpenAdMenu()
    {
        UIController.instance.OpenMenu(MenuType.ADVERTISE);
    }

    private void FreeBuff()
    {
        GameController.instance.AdManager.uiAd.TutorialButton();
    }

    private void OpenShop()
    {
        UIController.instance.OpenMenu(MenuType.SHOP);
    }

    private void FreeRelic()
    {
        Summon.instance.FreeRelic();
    }

    private void OpenRelicMenu()
    {
        UIController.instance.OpenMenu(MenuType.RELIC);
    }

    private void GiftCurrency()
    {
        UIController.instance.ShowSimpleRewardPopUp("5000", Utils.GetUiSprite("SpiritStone").uiSprite, () => GameDataManager.instance.AddSpiritStone(5000));
    }

    #endregion CallBackEvent
}