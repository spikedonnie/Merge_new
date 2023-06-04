using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class SimpleRewardPopup : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI closeText;
    public Image icon;

    private Action okAction = null;
    public EventTrigger eventTrigger;
    bool isClose = false;
    private void Awake()
    {
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    private void Start()
    {
        CloseUI();
    }

    public void SetPopup(string content, Sprite sprite, Action yes = null)
    {
        isClose = false;
        descriptionText.text = content;
        icon.sprite = sprite;
        okAction = yes;
        OpenUI();
        StopCoroutine("WaitingSecond");
        StartCoroutine("WaitingSecond");
    }

    private IEnumerator WaitingSecond()
    {
        eventTrigger.enabled = false;
        closeText.enabled = false;
        yield return new WaitForSeconds(1f);
        eventTrigger.enabled = true;
        closeText.enabled = true;
        yield return new WaitForSeconds(5f);
        OkButton();
    }
    

    public void OkButton()
    {
        if (isClose) return;
        isClose = true;
        if (okAction != null)
        {
            okAction();
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