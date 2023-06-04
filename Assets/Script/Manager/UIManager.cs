using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    [SerializeField] protected Button closeButton;
    public GameObject viewObj;

    private void Awake()
    {
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        if (closeButton != null) closeButton.onClick.AddListener(CloseUI);
    }

    public virtual void Init()
    {
        CloseUI();
    }

    public virtual void OpenUI()
    {
        viewObj.SetActive(true);
        canvasGroup.blocksRaycasts = true;
        canvasGroup.DOKill();
        canvasGroup.DOFade(1f, 0.4f);
    }

    public virtual void CloseUI()
    {
        viewObj.SetActive(false);
        canvasGroup.blocksRaycasts = false;
        canvasGroup.DOKill();
        canvasGroup.DOFade(0f, 0.4f);
    }
}