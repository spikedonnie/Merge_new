using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using GoogleMobileAds.Api;

public class CustomPopup : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI titleText = null;
    [SerializeField] PopupButton popupButton = null;
    [SerializeField] Transform parent;
    [SerializeField] GameObject rewardInfoPrefab;

    Queue<GameObject> rewardQueue = new Queue<GameObject>();

    public void Init()
    {
        StartCoroutine("Show");
    }

    IEnumerator Show()
    {
        popupButton.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        while(rewardQueue.Count > 0)
        {
            rewardQueue.Dequeue().SetActive(true);
            AudioManager.Instance.PlayEffect(ESoundEffect.RewardIcon);
            yield return new WaitForSeconds(0.5f);
        }

        popupButton.gameObject.SetActive(true);
    }

    public void SetTitle(string _title)
    {
        this.titleText.text = _title;
    }

    public void SetButtons(PopupButtonInfo info)
    {
        popupButton.Init(info.text, info.callback, this.gameObject);
    }
    public void SetRewardInfo(List<RewardInfoData> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            GameObject rif = Instantiate(this.rewardInfoPrefab);
            rif.transform.SetParent(this.parent, false);
            PopupRewardIcon popupRewardIcon = rif.GetComponent<PopupRewardIcon>();
            popupRewardIcon.SetUI(list[i]);
            rif.SetActive(false);
            rewardQueue.Enqueue(rif);
        }

    }



}
