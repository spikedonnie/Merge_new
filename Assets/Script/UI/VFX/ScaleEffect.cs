using DG.Tweening;

using UnityEngine;

public class ScaleEffect : MonoBehaviour
{
    private RectTransform trans;

    private void Awake()
    {
        trans = GetComponent<RectTransform>();
    }

    private void Start()
    {
        trans.DOScale(1.2f, 1f).SetLoops(-1,LoopType.Yoyo).SetEase(Ease.Linear);
    }
}
