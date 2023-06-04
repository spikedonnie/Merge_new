using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LoofColorEffect : MonoBehaviour
{
    Outline targetOutLine;
    public Color startColor;
    public Color endColor;

    private void Awake()
    {
        targetOutLine = GetComponent<Outline>();
        startColor = targetOutLine.effectColor;
    }

    private void OnEnable()
    {
        targetOutLine.effectColor = startColor;
        targetOutLine.DOColor(endColor,1f).SetLoops(-1,LoopType.Yoyo);
    }

    private void OnDisable()
    {
        DOTween.Kill(targetOutLine);
    }




}
