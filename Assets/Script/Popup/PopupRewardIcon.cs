using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupRewardIcon : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI amount;

    public void SetUI(RewardInfoData reward)
    {
        icon.sprite = Utils.GetUiSprite(reward.type.ToString()).uiSprite;
        amount.text = string.Format("{0:F0}", NumberToSymbol.ChangeNumber(reward.amount));
    }
}
