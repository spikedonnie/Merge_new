using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TwoButtonPopup : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    public TextMeshProUGUI descriptionText;

    public Button onYesButton;
    public Button onNoButton;

    private Action clickYes = null;
    private Action clickNo = null;

    private void Awake()
    {
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        onYesButton.onClick.AddListener(YesButton);
        onNoButton.onClick.AddListener(NoButton);
    }

    private void Start()
    {
        CloseUI();
    }

    public void SetUp(string content, Action yes, Action no)
    {
        descriptionText.text = content;
        clickYes = yes;
        clickNo = no;
        OpenUI();
    }

    private void YesButton()
    {
        if (clickYes != null)
        {
            clickYes();
        }
        CloseUI();
    }

    private void NoButton()
    {
        if (clickNo != null)
        {
            clickNo();
        }

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