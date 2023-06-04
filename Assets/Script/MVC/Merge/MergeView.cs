using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MergeView : MonoBehaviour
{
    GodFingerModel godFingerModel;
    public float SortingCoolTime{get;set;}
    public float CurrentCooldownTime {get;set;}   
    //GOD FINGER VIEW
    [Header("[갓 핑거 지속시간]")]
    public TextMeshProUGUI godFingerDurationText = null;
    [Header("[갓 핑거 레벨]")]
    public TextMeshProUGUI godFingerLevelText = null;
    [Header("[갓 핑거 비용]")]
    public TextMeshProUGUI godFingerCostText = null;
    [Header("[갓 핑거 활성화 버튼]")]
    public Button godFingerUseButton = null;
    [Header("[갓 핑거 슬라이더 값")]
    public Slider godFingerSliderValue = null;
    [Header("[갓 핑거 패널]")]
    public GameObject godFingerPanel = null;
    [Header("[갓 핑거 메뉴 버튼]")]
    public Button godFingerMenuButton = null;
    [Header("[갓 핑거 시간 텍스트]")]
    public TextMeshProUGUI godFingerTimeText = null;
    [Header("[갓 핑거 추가 지속시간]")]
    public TextMeshProUGUI godFingerAddDurationText = null;
    [Header("[정렬 버튼]")]
    public Button sortingButton = null;
    [Header("[정렬FillAmount]")]
    public Image sortingFillAmount;
    [Header("[정렬 쿨타임 Text]")]
    public TextMeshProUGUI sortingCoolTimeText = null;
    public void Init(GodFingerModel model,float sort)
    {
        godFingerModel = model;
        godFingerModel.onChange += ChangedGodFingerModel;
        CurrentCooldownTime = sort;
        SortingCoolTime = Define.DEFAULT_SORTING_COOLTIME;

        if(CurrentCooldownTime < SortingCoolTime)
        {
            StartCoroutine(SortingMercenary());
        }
    }

    public void ChangedGodFingerModel(GodFingerModel model)
    {
        godFingerModel = model;
    }

    public void GodFingerButtonClick(bool active)
    {
        godFingerMenuButton.interactable = active;
        godFingerTimeText.gameObject.SetActive(!active);
    }
    public void SetSortingButtonEvent(System.Action action)
    {
        sortingButton.onClick.AddListener(() => action());
    }

    public void SetGoldFingerUseButtonEvent(System.Action action)
    {
        godFingerUseButton.onClick.AddListener(() => action());
    }
    public void SetGoldFingerPopUpOpenButtonEvent(GodFingerModel model)
    {
        godFingerMenuButton.onClick.AddListener(() => UpdateGodFingerPanel(model,true));
    }
    void UpdateGodFingerPanel(GodFingerModel model,bool active)
    {
        OpenGodFingerPenel(active);
        UpdateGodFingerTextUI(model);
    }
    public void OpenGodFingerPenel(bool active)
    {
        godFingerPanel.SetActive(active);
    }
    void UpdateGodFingerTextUI(GodFingerModel model)
    {
        var addTime = GameController.instance.abilityManager.TotalAbilityData(AbilityType.GoldFingerDuration);
        godFingerDurationText.text = string.Format("{0}s",model.duration);
        godFingerAddDurationText.text = string.Format("(+{0})",addTime);
        
        godFingerLevelText.text = string.Format("{0}/{1}",model.count,model.maxCount);
        godFingerCostText.text = string.Format("{0}",model.cost);
        godFingerSliderValue.value = model.count;
    }
    public IEnumerator SortingMercenary()
    {
        sortingButton.interactable = false;
        sortingFillAmount.fillAmount = 0f;

        while (CurrentCooldownTime < SortingCoolTime)
        {
            CurrentCooldownTime += Time.deltaTime;
            sortingCoolTimeText.text = string.Format("{0:0.0}", SortingCoolTime - CurrentCooldownTime);
            sortingFillAmount.fillAmount = CurrentCooldownTime / SortingCoolTime;
            yield return null;
        }

        CurrentCooldownTime = 0;
        sortingCoolTimeText.text = "";
        sortingButton.interactable = true;
        sortingFillAmount.fillAmount = 1f;
    }

}
