using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

[Serializable]
public class Timer
{
    public bool isActive;

    public BuffTag bufftag;

    public float duration;

    public CanvasGroup canvasGroup;

    public GameObject effect;

    public Timer(float rest, BuffTag tag, GameObject efx, CanvasGroup group)
    {
        duration = rest;
        bufftag = tag;
        effect = efx;
        effect.SetActive(true);
        canvasGroup = group;
        canvasGroup.alpha = 1f;
        isActive = true;


    }

    public void EndTimer()
    {
        duration = 0;
        isActive = false;
        canvasGroup.alpha = 0.2f;
        effect.SetActive(false);
        GameController.instance.abilityManager.RemoveBuff(bufftag);
    }
}

public class ADManager : MonoBehaviour
{
    [SerializeField] private List<Timer> timerList = new List<Timer>();

    public UI_Ad uiAd;

    [SerializeField] private TextMeshProUGUI[] buffText = null;
    [SerializeField] private CanvasGroup[] buffCanvasGroups;
    [SerializeField] private GameObject[] effect;

    public Dictionary<BuffType,BuffData> buffDataDic = new Dictionary<BuffType,BuffData>();
    
    public IEnumerator Init()
    {
        yield return null;

        buffDataDic[BuffType.DAMAGE] = GameDataManager.instance.GetSaveData.damageBuffData;
        buffDataDic[BuffType.GOLD] = GameDataManager.instance.GetSaveData.goldBuffData;
        buffDataDic[BuffType.STONE] = GameDataManager.instance.GetSaveData.stoneBuffData;

        if(buffDataDic[BuffType.DAMAGE].isActive) StartTimer(buffDataDic[BuffType.DAMAGE].buffDuration, BuffTag.Damage);
        if(buffDataDic[BuffType.GOLD].isActive) StartTimer(buffDataDic[BuffType.GOLD].buffDuration, BuffTag.Gold);
        if(buffDataDic[BuffType.STONE].isActive) StartTimer(buffDataDic[BuffType.STONE].buffDuration, BuffTag.Stone);

        uiAd.SetUI(this);

        StartCoroutine(InitBuffData());

    }

    public void SaveBuffData()
    {

        for(int i = 0; i < timerList.Count; i++)
        {
            var currentTimer = timerList[i];

            switch(currentTimer.bufftag)
            {
                case BuffTag.Damage:

                    buffDataDic[BuffType.DAMAGE].buffDuration = currentTimer.duration;
                    break;

                case BuffTag.Gold:

                    buffDataDic[BuffType.GOLD].buffDuration = currentTimer.duration;
                    break;

                case BuffTag.Stone:

                    buffDataDic[BuffType.STONE].buffDuration = currentTimer.duration;
                    break;

                case BuffTag.All:
                    break;

                default:
                    break;
            }
        }

        GameDataManager.instance.GetSaveData.damageBuffData = buffDataDic[BuffType.DAMAGE];
        GameDataManager.instance.GetSaveData.goldBuffData = buffDataDic[BuffType.GOLD];
        GameDataManager.instance.GetSaveData.stoneBuffData = buffDataDic[BuffType.STONE];
    }



    IEnumerator InitBuffData()
    {
        while(true)
        {
            if(!GameController.instance.IsGameStop)
            {
                for (int i = timerList.Count - 1; i >= 0; i--)
                {
                    var currentTimer = timerList[i];

                    if (timerList[i].duration > 0)
                    {

                        currentTimer.duration -= Time.deltaTime;

                        var index = BuffTagToIndex(currentTimer.bufftag);
                        //buffText[i].text = string.Format("{0:0#}:{1:0#}", (int)(currentTimer.duration / 60), (int)(currentTimer.duration % 60));
                        buffText[index].text = $"{Mathf.FloorToInt(currentTimer.duration / 60f):00}:{Mathf.FloorToInt(currentTimer.duration % 60f):00}";

                    }
                    else if(currentTimer.duration  <= 0)
                    {
                        var index = BuffTagToIndex(currentTimer.bufftag);
                        buffText[index].text = "";
                        currentTimer.EndTimer();
                        timerList.RemoveAt(i);
                        yield return new WaitForSeconds(0.05f);
                    }

                }
            }

            yield return null;

        }

    }



    public bool CheckIsRunningBuff(BuffTag tag)
    {
        foreach (var timer in timerList)
        {
            if (timer.bufftag == tag && timer.isActive)
            {
                UIController.instance.SendPopupMessage(AlarmTYPE.ACTIVE_BUFF);
                return true;
            }
        }
        
        return false;
    }

    public bool CheckEnoughAdCount(BuffType tag)
    {

        foreach(var dic in buffDataDic)
        {
            if(dic.Key == tag)
            {
                if(dic.Value.buffCount > 0)
                {
                    return true;
                }
                else
                {
                    UIController.instance.SendPopupMessage(AlarmTYPE.NOT_ENOUGH_ADCOUNT);
                    return false;
                }
            }
        }

        return false;
    }

    public void StartTimer(float time, BuffTag tag)
    {
        var index = BuffTagToIndex(tag);

        switch(tag)
        {
            case BuffTag.Damage:

                buffDataDic[BuffType.DAMAGE].isActive = true;
                timerList.Add(new Timer(time, tag, effect[index], buffCanvasGroups[index]));

                break;

            case BuffTag.Gold:

                buffDataDic[BuffType.GOLD].isActive = true;
                timerList.Add(new Timer(time, tag, effect[index], buffCanvasGroups[index]));
                break;

            case BuffTag.Stone:

                buffDataDic[BuffType.STONE].isActive = true;
                timerList.Add(new Timer(time, tag, effect[index], buffCanvasGroups[index]));
                break;

            case BuffTag.All:
                break;

            default:
                break;
        }

        GameController.instance.abilityManager.AddBuff(tag);
        //Debug.Log(tag + "<color=red>버프 시작</color>");
    }
    public int BuffTagToIndex(BuffTag tag)
    {
        switch (tag)
        {
            case BuffTag.None:
                break;

            case BuffTag.Damage:

                return 0;

            case BuffTag.Gold:

                return 1;

            case BuffTag.Stone:

                return 2;

            case BuffTag.All:
                break;

            default:
                break;
        }

        return 0;
    }

    public void EndBuffTimer(BuffTag tag)
    {
        switch(tag)
        {
            case BuffTag.Damage:

                buffDataDic[BuffType.DAMAGE].isActive = false;
                break;

            case BuffTag.Gold:

                buffDataDic[BuffType.GOLD].isActive = false;
                break;

            case BuffTag.Stone:

                buffDataDic[BuffType.STONE].isActive = false;
                break;

            case BuffTag.All:
                break;

            default:
                break;
        }
        GameController.instance.abilityManager.RemoveBuff(tag);

        //Debug.Log(tag + "<color=red>버프 종료</color>");
    }

    public float GetBuffTime(BuffTag tag)
    {
        foreach (var timer in timerList)
        {
            if (timer.bufftag == tag)
            {
                return timer.duration;
            }
        }
        return 0;
    }


}