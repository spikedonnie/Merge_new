using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class UI_Ad : UIManager
{
    private BuffType rewardAdType = BuffType.DAMAGE;
    private BuffTag bTag;
    private ADManager adManager;

    [SerializeField] private TextMeshProUGUI[] infoTexts;
    [SerializeField] private TextMeshProUGUI[] dayTexts;
    [SerializeField] private TextMeshProUGUI[] timeTexts;
    [SerializeField] private Button[] buttons;

    [SerializeField] private bool isOpen = false;



    public void TutorialButton()
    {

        rewardAdType = BuffType.DAMAGE;
        bTag = BuffTag.Damage;
        ApplyBuffByAdReward(true);

    }

    public void SetUI(ADManager ad)
    {
        adManager = ad;

        base.Init();

        for (int i = 0; i < infoTexts.Length; i++)
        {
            infoTexts[i].text = GetAdText((BuffType)i);
        }

        buttons[0].onClick.AddListener(BtnBuffDamage);
        buttons[1].onClick.AddListener(BtnBuffGold);
        buttons[2].onClick.AddListener(BtnBuffSpiritStone);

        UpdateUI();
    }

    public void BtnBuffDamage()
    {
        if (adManager.CheckIsRunningBuff(BuffTag.Damage) || !adManager.CheckEnoughAdCount(BuffType.DAMAGE)) return;

        rewardAdType = BuffType.DAMAGE;
        bTag = BuffTag.Damage;



#if UNITY_ANDROID && !UNITY_EDITOR
        Admob.Instance.AdmobShow(ApplyBuffByAdReward);
#endif

#if UNITY_EDITOR
        ApplyBuffByAdReward(true);
#endif
    }

    public void BtnBuffGold()
    {
        if (adManager.CheckIsRunningBuff(BuffTag.Gold) || !adManager.CheckEnoughAdCount(BuffType.GOLD)) return;

        rewardAdType = BuffType.GOLD;
        bTag = BuffTag.Gold;
#if UNITY_ANDROID && !UNITY_EDITOR
        Admob.Instance.AdmobShow(ApplyBuffByAdReward);
#endif

#if UNITY_EDITOR
        ApplyBuffByAdReward(true);
#endif
    }

    public void BtnBuffSpiritStone()
    {
        if (adManager.CheckIsRunningBuff(BuffTag.Stone) || !adManager.CheckEnoughAdCount(BuffType.STONE)) return;

        rewardAdType = BuffType.STONE;
        bTag = BuffTag.Stone;
#if UNITY_ANDROID && !UNITY_EDITOR
        Admob.Instance.AdmobShow(ApplyBuffByAdReward);
#endif

#if UNITY_EDITOR
        ApplyBuffByAdReward(true);
#endif
    }

    public override void OpenUI()
    {
        base.OpenUI();

        isOpen = true;

        UpdateUI();
    }

    public override void CloseUI()
    {
        base.CloseUI();

        isOpen = false;
    }

    private void Update()
    {
        if (isOpen)
        {
            timeTexts[0].text = string.Format("{0}", CheckCanUseBuffing(BuffTag.Damage));
            timeTexts[1].text = string.Format("{0}", CheckCanUseBuffing(BuffTag.Gold));
            timeTexts[2].text = string.Format("{0}", CheckCanUseBuffing(BuffTag.Stone));
        }
    }

    private string CheckCanUseBuffing(BuffTag type)
    {
        float time = adManager.GetBuffTime(type);

        if (time == 0)
        {
            return I2.Loc.LocalizationManager.GetTranslation("음식먹기");
        }
        else
        {
            return string.Format("{0:D2}:{1:D2}", (int)(time / 60), (int)(time % 60));
        }
    }

    private void ApplyBuffByAdReward(bool flag)
    {
        if (flag)
        {
            //콜백을 받아 이곳에서 버프 보상 처리
            switch (rewardAdType)
            {
                case BuffType.DAMAGE:
                    UIController.instance.SendPopupMessage(AlarmTYPE.COMPLETE_AD_DAMAGE);
                    adManager.buffDataDic[BuffType.DAMAGE].buffCount -= 1;
                    break;

                case BuffType.GOLD:
                    UIController.instance.SendPopupMessage(AlarmTYPE.COMPLETE_AD_GOLD);
                    adManager.buffDataDic[BuffType.GOLD].buffCount -= 1;

                    break;

                case BuffType.STONE:
                    UIController.instance.SendPopupMessage(AlarmTYPE.COMPLETE_AD_STONE);
                    adManager.buffDataDic[BuffType.STONE].buffCount -= 1;

                    break;

                case BuffType.MAX:
                    break;

                default:
                    break;
            }

            adManager.StartTimer(Define.DEFAULT_BUFF_TIME, bTag);

            UpdateUI();
            GameController.instance.AudioManager.PlayEffect(ESoundEffect.SuccessAdView);
            GameDataManager.instance.SaveData();

            CloseUI();
        }
        
    }

    private string GetAdText(BuffType type)
    {
        switch (type)
        {
            case BuffType.DAMAGE:
                return string.Format("<color=green>{0}</color>%\n1 Hour", 200);

            case BuffType.GOLD:
                return string.Format("<color=green>{0}</color>%\n1 Hour", 200);

            case BuffType.STONE:
                return string.Format("<color=green>{0}</color>%\n1 Hour", 150);
        }
        return null;
    }

    private void UpdateUI()
    {
        dayTexts[0].text = string.Format("({0}/{1})", adManager.buffDataDic[BuffType.DAMAGE].buffCount, Define.MAX_FOOD_COUNT);
        dayTexts[1].text = string.Format("({0}/{1})", adManager.buffDataDic[BuffType.GOLD].buffCount, Define.MAX_FOOD_COUNT);
        dayTexts[2].text = string.Format("({0}/{1})", adManager.buffDataDic[BuffType.STONE].buffCount, Define.MAX_FOOD_COUNT);


    }
}