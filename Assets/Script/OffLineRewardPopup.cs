using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Web;

public class OffLineRewardPopup : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private Action okAction = null;

    public TextMeshProUGUI goldText;
    public TextMeshProUGUI stoneText;


    private void Awake()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        CloseUI();
    }

    public void SetOffLinePopup(string gold, string stone,  Action yes = null)
    {
        goldText.text = gold;
        stoneText.text = stone;

        okAction = yes;
        OpenUI();
    }




    public void OkButton()
    {
        if (okAction != null)
        {
            okAction.Invoke();
        }
        
        AudioManager.Instance.PlayEffect(ESoundEffect.Click);
        CloseUI();
    }

    public virtual void OpenUI()
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.DOKill();
        canvasGroup.DOFade(1f, 0.4f);
    }

    public void CloseUI()
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.DOKill();
        canvasGroup.DOFade(0f, 0.4f);
    }


















}
