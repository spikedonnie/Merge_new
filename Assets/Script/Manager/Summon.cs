using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Summon : MonoBehaviour
{
    public static Summon instance;

    [SerializeField] private Button smallRelicBtn;
    [SerializeField] private Button commonRelicBtn;
    [SerializeField] private Button relicAdBtn;
    [SerializeField] private Button heroAdBtn;
    [SerializeField] private Button contractBtn;
    [SerializeField] private SummonUI summonUI;

    [SerializeField] private TextMeshProUGUI maxSummonRelicText;
    [SerializeField] private TextMeshProUGUI maxSummonMercenaryText;
    [SerializeField] private TextMeshProUGUI contractText;

    AdvertiseSaveData adSaveData;

    private void Awake()
    {
        instance = this;
        contractBtn.onClick.AddListener(UseContract);
        smallRelicBtn.onClick.AddListener(PurchaseSmallSummonRelic);
        commonRelicBtn.onClick.AddListener(PurchaseCommonSummonRelic);
        relicAdBtn.onClick.AddListener(ShowAdRelic);
        heroAdBtn.onClick.AddListener(ShowAdHero);
    }

    //튜토리얼
    public void FreeRelic()
    {
        GameController.instance.AudioManager.PlayEffect(ESoundEffect.ChestOpen);
        summonUI.SummonRelic(1);
    }

    public void Init()
    {
        summonUI.Init(this);
        adSaveData = GameDataManager.instance.GetSaveData.advertiseSaveData;
        UpdateUI();

    }
    public void UpdateUI()
    {
        maxSummonRelicText.text = string.Format("{0}/{1}", adSaveData.adRelicCount, Define.MAX_SUMMON_RELIC);
        maxSummonMercenaryText.text = string.Format("{0}/{1}", adSaveData.adMercenaryCount, Define.MAX_SUMMON_MERCENARY);
        contractText.text = string.Format("{0}", GameDataManager.instance.GetSaveData.contract);
    }

    //유물이 모두 Max레벨인지 확인
    public bool CheckRelicAllMaxLevel()
    {
        for (RelicType i = 0; i < RelicType.MAX; i++)
        {
            if (!GameController.instance.RelicManager.IsCheckMaxLevel(i))
            {
                return false;
            }
        }
        UIController.instance.SendPopupMessage(AlarmTYPE.RELIC_ALL_MAX_LEVEL);
        return true;
    }
    //유물1개 구매
    public void PurchaseSmallSummonRelic()
    {
        if (CheckRelicAllMaxLevel())
        {
            return;
        }
        
        if (GameDataManager.instance.GetCurrency(RewardTYPE.Diamond) >= Define.DEFAULT_SUMMON_PRICE)
        {
            GameController.instance.AudioManager.PlayEffect(ESoundEffect.ChestOpen);
            GameDataManager.instance.SubtractDiamondData(Define.DEFAULT_SUMMON_PRICE);
            summonUI.SummonRelic(1);
        }
        else
        {
            UIController.instance.SendPopupMessage(AlarmTYPE.NOT_ENOUGH_DIAMOND);
        }
    }

    //유물 10개 구매
    public void PurchaseCommonSummonRelic()
    {
        if (CheckRelicAllMaxLevel())
        {
            return;
        }
        if (GameDataManager.instance.GetCurrency(RewardTYPE.Diamond) >= Define.SMALL_SUMMON_PRICE)
        {
            GameController.instance.AudioManager.PlayEffect(ESoundEffect.ChestOpen);
            GameDataManager.instance.SubtractDiamondData(Define.SMALL_SUMMON_PRICE);
            summonUI.SummonRelic(10);
        }
        else
        {
            UIController.instance.SendPopupMessage(AlarmTYPE.NOT_ENOUGH_DIAMOND);
        }
    }

    //유물 30개  구매
    public void PurchaseLargeSummonRelic()
    {
        if (CheckRelicAllMaxLevel())
        {
            return;
        }

        if (GameDataManager.instance.GetCurrency(RewardTYPE.Diamond) >= Define.LARGE_SUMMON_PRICE)
        {
            GameController.instance.AudioManager.PlayEffect(ESoundEffect.ChestOpen);
            GameDataManager.instance.SubtractDiamondData(Define.LARGE_SUMMON_PRICE);
            summonUI.SummonRelic(30);
        }
        else
        {
            UIController.instance.SendPopupMessage(AlarmTYPE.NOT_ENOUGH_DIAMOND);
        }
    }

    private bool CheckEnoughAdCount(float data)
    {
        if (data <= 0)
        {
            UIController.instance.SendPopupMessage(AlarmTYPE.NOT_ENOUGH_ADCOUNT);

            return false;
        }
        else
        {
            return true;
        }
    }
    //용병 계약서 사용
    void UseContract()
    {
        var cont = GameDataManager.instance.GetSaveData.contract;

        if(cont <= 0)
        {
            UIController.instance.SendPopupMessage(AlarmTYPE.NOT_ENOUGH_CONTRACT);
            return;
        }

        if (GameController.instance.MergeManager.IsBattleHeroFull())
        {
            UIController.instance.SendPopupMessage(AlarmTYPE.FULL_BATTLE);
            return;
        }
        GameDataManager.instance.SetContract(-1);

        CallBackAdHero(true);

    }


    //영웅뽑기 광고
    private void ShowAdHero()
    {
        if (!CheckEnoughAdCount(adSaveData.adMercenaryCount)) return;

        if(GameController.instance.MergeManager.IsBattleHeroFull())
        {
            UIController.instance.SendPopupMessage(AlarmTYPE.FULL_BATTLE);
            return;
        }
        adSaveData.adMercenaryCount--;

#if UNITY_ANDROID && !UNITY_EDITOR
        Admob.Instance.AdmobShow(CallBackAdHero);
#endif

#if UNITY_EDITOR
        CallBackAdHero(true);
#endif
    }
    
    private void CallBackAdHero(bool flag)
    {
        if (flag)
        {
            AudioManager.Instance.PlayEffect(ESoundEffect.Contract);

            //현재 고용중인 최고 등급의 영웅 레벨 가져오기
            int highest = GameController.instance.mergeController.GetHighestHeroLevel();
            //최고 낮은 등급 영웅 레벨 가져오기

            int lowLevel = (int)GameController.instance.abilityManager.TotalAbilityData(AbilityType.HeroStartLevel);

            int val = ((highest + 1) - lowLevel) / 4;
            //Debug.Log($"기본레벨은 {lowLevel} 이고 {lowLevel + val}부터 {temp} 사이의 값 뽑기");
            int rnd = Random.Range(lowLevel + val, highest);

            var heroType = (MercenaryType)rnd;

            //UIController.instance.newHero.SetRewardData(heroType);
            Player player = GameDataManager.instance.SheetJsonLoader.GetPlayerData(heroType);

            var heroName = player.Name_KR;
            UIController.instance.ShowSimpleRewardPopUp(heroName, Utils.GetDocumentHeroSprite(heroType.ToString()).uiSprite, null);
            //GameController.instance.ShopManager.CloseUI();
            GameController.instance.CollectHero(rnd);
            UpdateUI();
            GameDataManager.instance.SaveData();

        }

    }

    //유물뽑기 광고
    public void ShowAdRelic()
    {
        if (!CheckEnoughAdCount(adSaveData.adRelicCount)) return;

        if (CheckRelicAllMaxLevel())
        {
            return;
        }


#if UNITY_ANDROID && !UNITY_EDITOR
        Admob.Instance.AdmobShow(CallBackAdRelic);
#endif

#if UNITY_EDITOR
        CallBackAdRelic(true);
#endif
    }

    private void CallBackAdRelic(bool flag)
    {
        if (flag)
        {
            GameController.instance.AudioManager.PlayEffect(ESoundEffect.ChestOpen);
            summonUI.SummonRelic(3);
            adSaveData.adRelicCount--;
            UpdateUI();
            GameDataManager.instance.SaveData();
        }

    }


}

public class SummonData
{
    public int itemIndex;
    public Sprite icon;
    public Sprite box;
    public string infoText;
}