using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
public class ShopManager : UIManager
{
    [SerializeField] private ShopSlot[] boxSlot;
    [SerializeField] private Summon summon;
    [SerializeField] private ContentSizeFitter fitter;

    
    [SerializeField] private Button adByDiamond;
    [SerializeField] private Button adByBell;
    [SerializeField] private Button gemByBell;
    [SerializeField] private Button gemByKey;

    public TextMeshProUGUI adGemCountText;
    public TextMeshProUGUI adBellCountText;
    public TextMeshProUGUI buyKeyMaxCountText;
    public TextMeshProUGUI priceKeyText;
    public TextMeshProUGUI buyBellAmountText;
    public TextMeshProUGUI adBellAmountText;

    public RectTransform rect;

    public GameObject adPack;
    public GameObject startPack;
    public GameObject goodPack;
    public GameObject fastPack;


    private float resetPosition;


    private void Start()
    {
        resetPosition = rect.localPosition.x;
        adByDiamond.onClick.AddListener(ShowAdFreeDiamond);
        adByBell.onClick.AddListener(ShowAdFreeTheBellOfCall);
        gemByBell.onClick.AddListener(PurchaseTheBellOfCall);
        gemByKey.onClick.AddListener(PurchaseDungeonKey);
    }

    public IEnumerator Initialize()
    {
        base.Init();

        for (int i = 0; i < boxSlot.Length; i++)
        {
            ShopProductType type = (ShopProductType)i;
            ProductInfo info = new ProductInfo();
            var sheetData = GameDataManager.instance.SheetJsonLoader.GetProductData(type);
            info.type = type;
            info.name = sheetData.NAME;
            info.price = sheetData.PRICE;
            info.infoText1 = sheetData.INFO_TEXT_1;
            info.infoText2 = sheetData.INFO_TEXT_2;
            info.infoText3 = sheetData.INFO_TEXT_3;
            info.infoIcon1 = sheetData.INFO_ICON_1;
            info.infoIcon2 = sheetData.INFO_ICON_2;
            info.infoIcon3 = sheetData.INFO_ICON_3;

            boxSlot[i].Init(this, info);
        }

        summon.Init();
        RefreshHasPackage();
        yield return null;
    }

    public void RefreshHasPackage()
    {
        var packData = GameDataManager.instance.GetSaveData.productPackage;

        if (packData.buyAdPassPackage) adPack.SetActive(true);
        if (packData.buyStartPackage) startPack.SetActive(true);
        if (packData.buyGoodPackage) goodPack.SetActive(true);
        if (packData.buyFastPackage) fastPack.SetActive(true);

    }

    public override void CloseUI()
    {
        base.CloseUI();

        var tutoType = GameDataManager.instance.GetSaveData.tutorialStep;

        TutorialManager.instance.CheckTutorialStep(TUTORIAL_STEP.OPEN_RELIC_MENU);//튜토리얼

    }

    //부름의 종 구매
    public void PurchaseTheBellOfCall()
    {
        if (!GameDataManager.instance.CheckIsEnoughCurrency(RewardTYPE.Diamond, Define.PRODUCT_BELL_PRICE))
        {
            return;
        }
        AudioManager.Instance.PlayEffect(ESoundEffect.Contract);

        GameDataManager.instance.SubtractDiamondData(Define.PRODUCT_BELL_PRICE);

        UIController.instance.ShowSimpleRewardPopUp(Define.PRODUCT_BELL_REWARD_COUNT.ToString(), Utils.GetUiSprite("Bell").uiSprite, () => GameDataManager.instance.SetTheBellOfCall(Define.PRODUCT_BELL_REWARD_COUNT));
    }
    //던전 키 구매
    public void PurchaseDungeonKey()
    {
        
        if (!GameDataManager.instance.CheckIsEnoughCurrency(RewardTYPE.Diamond, Define.PRODUCT_KEY_PRICE))
        {
            return;
        }

        if (GameDataManager.instance.GetSaveData.buyKeyCount <= 0)
        {
            
            return;
        }

        AudioManager.Instance.PlayEffect(ESoundEffect.Contract);

        GameDataManager.instance.SubtractDiamondData(Define.PRODUCT_KEY_PRICE);
        GameDataManager.instance.GetSaveData.buyKeyCount--;
        UIController.instance.ShowSimpleRewardPopUp("1", Utils.GetUiSprite("Key").uiSprite, () => GameDataManager.instance.SetDungeonKey(1));
        UpdateTextUI();

    }
 

    //광고 무료 보석
    public void ShowAdFreeDiamond()
    {

        if (GameDataManager.instance.GetSaveData.advertiseSaveData.adFreeDiamondCount <= 0) 
        {
            UIController.instance.SendPopupMessage(AlarmTYPE.NOT_ENOUGH_ADCOUNT);
            return;
        }


#if UNITY_ANDROID && !UNITY_EDITOR
        Admob.Instance.AdmobShow(CallBackFreeDiamond);
#endif

#if UNITY_EDITOR
        CallBackFreeDiamond(true);
#endif

    }

    private void CallBackFreeDiamond(bool flag)
    {
        if (flag)
        {
            AudioManager.Instance.PlayEffect(ESoundEffect.Contract);
            UIController.instance.ShowSimpleRewardPopUp(Define.FREE_DIAMOND_REWARD_COUNT.ToString(), Utils.GetUiSprite("Diamond").uiSprite, () => GameDataManager.instance.AddDiamondData(Define.FREE_DIAMOND_REWARD_COUNT));
            GameDataManager.instance.GetSaveData.advertiseSaveData.adFreeDiamondCount--;
            UpdateTextUI();
            GameDataManager.instance.SaveData();
        }
    }

    //광고 무료 부름의 종
    public void ShowAdFreeTheBellOfCall()
    {
        if (GameDataManager.instance.GetSaveData.advertiseSaveData.adFreeCallOfBell <= 0)
        {
            UIController.instance.SendPopupMessage(AlarmTYPE.NOT_ENOUGH_ADCOUNT);
            return;
        }
#if UNITY_ANDROID && !UNITY_EDITOR
        Admob.Instance.AdmobShow(CallBackFreeBell);
#endif

#if UNITY_EDITOR
        CallBackFreeBell(true);
#endif
    }

    private void CallBackFreeBell(bool flag)
    {
        if (flag)
        {
            AudioManager.Instance.PlayEffect(ESoundEffect.Contract);
            UIController.instance.ShowSimpleRewardPopUp(Define.PRODUCT_BELL_REWARD_COUNT.ToString(), Utils.GetUiSprite("Bell").uiSprite, () => GameDataManager.instance.SetTheBellOfCall(Define.PRODUCT_BELL_REWARD_COUNT));
            GameDataManager.instance.GetSaveData.advertiseSaveData.adFreeCallOfBell--;
            UpdateTextUI();
            GameDataManager.instance.SaveData();
        }
    }

    void UpdateTextUI()
    {
        adGemCountText.text = string.Format("{0}/{1}", GameDataManager.instance.GetSaveData.advertiseSaveData.adFreeDiamondCount, Define.MAX_FREE_DIAMOND);
        adBellCountText.text = string.Format("{0}/{1}", GameDataManager.instance.GetSaveData.advertiseSaveData.adFreeCallOfBell, Define.MAX_FREE_BELL);
        buyKeyMaxCountText.text = string.Format("{0}/{1}", GameDataManager.instance.GetSaveData.buyKeyCount, Define.MAX_BUY_DUNGEONKEY);
        priceKeyText.text  = string.Format("{0}", Define.PRODUCT_KEY_PRICE);
        buyBellAmountText.text = string.Format("{0}", Define.PRODUCT_BELL_REWARD_COUNT);
        adBellAmountText.text = string.Format("{0}", Define.PRODUCT_BELL_REWARD_COUNT);

        for (int i = 0; i < boxSlot.Length; i++)
        {
            boxSlot[i].RefreshUI();
        }

    }

    public override void OpenUI()
    {
        base.OpenUI();
        rect.localPosition = new Vector3(resetPosition, 0, 0);

        //LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)fitter.transform);
        fitter.SetLayoutHorizontal();
        fitter.SetLayoutVertical();
        UpdateTextUI();
        summon.UpdateUI();
    }
}

public class ProductInfo
{
    public ShopProductType type;
    public string name;
    public string price;
    public string infoText1;
    public string infoText2;
    public string infoText3;
    public string infoIcon1;
    public string infoIcon2;
    public string infoIcon3;
}