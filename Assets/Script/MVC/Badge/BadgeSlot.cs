using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BadgeSlot : MonoBehaviour
{
    public Image highLightColor;
    public Image badgeIcon;
    public Button selectButton;

    BadgeView badgeView;
    public BadgeModel badgeModel;

    void Start()
    {
        selectButton.onClick.AddListener(OnClickSelectButton);
    }
    public void SetBadgeSlot(BadgeModel model,BadgeView view)
    {
        badgeView = view;
        badgeModel = model;
        badgeIcon.sprite = badgeModel.badgeIconSprite;
        ChangeColor(badgeModel.color);
    }

    void ChangeColor(string hex)
    {
         Color color;
        if (ColorUtility.TryParseHtmlString(hex, out color))
        {
            highLightColor.color = color;
        }
    }

    void OnClickSelectButton()
    {
        badgeView.SelectedBadge(this);
    }
}
