using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FailChallange : MonoBehaviour
{

    System.Action back;

    public GameObject contents;

    public void ShowFailChallangeUI(System.Action callBack)
    {
        GameController.instance.IsGameStop = true;
        GameController.instance.AudioManager.PlayEffect(ESoundEffect.Negative);
        back = callBack;
        contents.SetActive(true);
        transform.DOKill(true);
        transform.DOScale(1f, 0.4f).SetEase(Ease.InOutExpo).SetUpdate(true).OnComplete(() => transform.localScale = Vector3.one);
    }


    public void ExitUI()
    {
        if(back != null) back.Invoke();
        contents.SetActive(false);

    }






}
