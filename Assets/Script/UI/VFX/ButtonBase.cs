using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonBase : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool isDown = false;
    private Transform target;
    private Button parentButton;

    private void Awake()
    {
        target = this.transform;
        parentButton = GetComponent<Button>();
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (!parentButton.interactable) return;
        isDown = true;
        target.transform.DOKill(true);
        target.transform.DOScale(0.95f, 0.1f).SetEase(Ease.OutCubic).SetUpdate(true);
    }

    public void OnPointerUp(PointerEventData data)
    {
        if (!isDown || !parentButton.interactable) return;
        isDown = false;

        target.transform.DOKill(true);

        target.transform.DOScale(1f, 0.4f).SetEase(Ease.OutElastic).SetUpdate(true);

        AudioManager.Instance.PlayEffect(ESoundEffect.Click);
    }
}