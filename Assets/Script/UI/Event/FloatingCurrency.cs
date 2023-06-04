using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class FloatingCurrency : MonoBehaviour
{
    public TextMeshProUGUI currency;
    [SerializeField] private CanvasGroup canvasGroup;

    public void SetCurrencyText(string value, Color color)
    {
        canvasGroup.alpha = 1;
        canvasGroup.DOKill();
        canvasGroup.DOFade(0f, 1f).SetEase(Ease.InExpo);
        currency.text = value;
        currency.color = color;

    }



}
