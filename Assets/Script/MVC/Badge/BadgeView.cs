using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BadgeView : MonoBehaviour
{

    public Image badgeIcon;
    public TextMeshProUGUI badgeName;
    public TextMeshProUGUI abilityInfoText;
    public TextMeshProUGUI price;
    public CanvasGroup badgePanelCanvasGroup;
    public Button purchaseButton;
    public Button badgeMenuButton;

    public List<BadgeSlot> badgeSlots;

    BadgeSlot selectedBadgeSlot;

    void Start()
    {
        badgeMenuButton.onClick.AddListener(() => OpenBadgePanel(true));
        purchaseButton.onClick.AddListener(PurchaseBadge);
        OpenBadgePanel(false);
    }

    public void InitView(List<BadgeModel> badgeModelList)
    {
        for (int i = 0; i < badgeModelList.Count; i++)
        {
            badgeSlots[i].SetBadgeSlot(badgeModelList[i], this);
        }

        var badgemodel = badgeModelList[0];

        SelectedBadge(badgeSlots[0]);
    }

    public void SelectedBadge(BadgeSlot badgeSlot)
    {
        selectedBadgeSlot = badgeSlot;
        badgeIcon.sprite = selectedBadgeSlot.badgeIcon.sprite;
        badgeName.text = I2.Loc.LocalizationManager.GetTranslation(selectedBadgeSlot.badgeModel.badgeName);
        abilityInfoText.text = string.Format("{0} <color=orange>{1}%</color>",I2.Loc.LocalizationManager.GetTranslation(selectedBadgeSlot.badgeModel.typeName), selectedBadgeSlot.badgeModel.power);
        UpdatePurchaseUI();

    }


    public void OpenBadgePanel(bool bOpen)
    {

        if (bOpen)
        {
            badgePanelCanvasGroup.alpha = 1;
            badgePanelCanvasGroup.blocksRaycasts = true;

        }
        else
        {
            badgePanelCanvasGroup.alpha = 0;
            badgePanelCanvasGroup.blocksRaycasts = false;
        }
    }


    public void PurchaseBadge()
    {
        if(CheckEnoughGem(selectedBadgeSlot.badgeModel) && !CheckHaveBadge(selectedBadgeSlot.badgeModel))
        {
            GameDataManager.instance.SetCurrency(RewardTYPE.Diamond, -selectedBadgeSlot.badgeModel.price);
            GameDataManager.instance.SetBadgeData((BADGE_TYPE)System.Enum.Parse(typeof(BADGE_TYPE), selectedBadgeSlot.badgeModel.badgeType), true);    
            UpdatePurchaseUI();
        }

    }

    bool CheckEnoughGem(BadgeModel model)
    {
        var gem = GameDataManager.instance.GetCurrency(RewardTYPE.Diamond);

        if (gem >= model.price)
        {
            return true;
        }
        else
        {
            UIController.instance.SendPopupMessage(AlarmTYPE.NOT_ENOUGH_DIAMOND);
            return false;
        }
    }

    bool CheckHaveBadge(BadgeModel model)
    {
        var have = GameDataManager.instance.GetBadgePurchaseData((BADGE_TYPE)System.Enum.Parse(typeof(BADGE_TYPE), model.badgeType));

        if (have)
        {
            UIController.instance.SendPopupMessage(AlarmTYPE.PURCHASED_BADGE);
            return true;
        }
        else
        {
            return false;
        }
    }

    void UpdatePurchaseUI()
    {
        var bPurchase = GameDataManager.instance.GetBadgePurchaseData((BADGE_TYPE)System.Enum.Parse(typeof(BADGE_TYPE), selectedBadgeSlot.badgeModel.badgeType));
        
        if(bPurchase)
        {
            price.text = I2.Loc.LocalizationManager.GetTranslation("purchased");
        }
        else
        {
            price.text = selectedBadgeSlot.badgeModel.price.ToString();
        }
    }



}
