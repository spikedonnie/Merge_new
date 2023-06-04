using DG.Tweening;
using UnityEngine;

public class LoopEffect : MonoBehaviour
{
    private Vector3 angle = new Vector3(0, 0, 360);
    private RectTransform trans;

    private void Awake()
    {
        trans = GetComponent<RectTransform>();
    }

    private void Start()
    {
        trans.DORotate(angle, 10f, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);
    }
}