using DG.Tweening;
using TMPro;
using UnityEngine;

public class AlarmPopup : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private TextMeshProUGUI messageTxt;

    Sequence seq;

    private void Start()
    {
        DOTween.Init();
        seq = DOTween.Sequence().SetAutoKill(false).Pause();
        seq.Append(transform.DOScale(1.2f, 0.2f));
        seq.Join(canvasGroup.DOFade(0f, 1f).SetEase(Ease.InExpo));
        seq.Append(transform.DOScale(1f, 0.1f));
    }

    public void ShowAlarmMessage(AlarmTYPE alarmType)
    {
        canvasGroup.alpha = 1;
        seq.Rewind();
        seq.Restart();
        //messageTxt.text = string.Format("{0}", GameDataManager.instance.SheetJsonLoader.GetAlarmData(alarmType).MESSAGE);
        messageTxt.text = string.Format("{0}", I2.Loc.LocalizationManager.GetTranslation(alarmType.ToString()));



    }
}