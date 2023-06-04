using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class SummonSlot : MonoBehaviour
{

    [SerializeField] Image rectImage;
    [SerializeField] Image iconImage;
    [SerializeField] Image whiteImage;
    [SerializeField] TextMeshProUGUI infoTxt;

    [SerializeField] AudioSource audioSource;

    public void SendSummonInfo(SummonData data)
    {
        if(data != null)
        {
            rectImage.sprite = data.box;
            iconImage.sprite = data.icon;
            //infoTxt.text = data.infoText;
            infoTxt.text = I2.Loc.LocalizationManager.GetTranslation(data.infoText);
            audioSource.Play();
        }

    }

    private void OnEnable()
    {
        whiteImage.DOColor(Color.clear, 0.2f);
    }
    private void OnDisable()
    {
        whiteImage.color = Color.white;
    }


}
