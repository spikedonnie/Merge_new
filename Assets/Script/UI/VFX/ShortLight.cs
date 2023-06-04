using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ShortLight : MonoBehaviour
{
    Image lightImage;
    private void Awake()
    {
        lightImage = GetComponent<Image>();
    }

    public void ShowLight()
    {
        lightImage.color = Color.white;
        lightImage.DOFade(0, 0.2f).SetEase(Ease.InQuint);
    }
}
