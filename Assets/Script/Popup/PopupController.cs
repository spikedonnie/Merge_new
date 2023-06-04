using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RewardInfoData
{
    public float amount;
    public RewardTYPE type;

    public RewardInfoData(RewardTYPE type, float amount)
    {
        this.type = type;
        this.amount = amount;
    }
}

public enum EEfectGameObjectTYPE
{
    None,
    BossReward,
    Max
}


public class PopupRewardInfoData
{
    private List<RewardInfoData> rewardInfoList;
    private string popupTitleName;

    private EEfectGameObjectTYPE effectType;

    private Action callBack;

    public void SetTitle(string title)
    {
        this.popupTitleName = title;
    }
    public void SetRewardInfoList(List<RewardInfoData> list)
    {
        this.rewardInfoList = list;
    }
    public void SetEffectType(EEfectGameObjectTYPE type)
    {
        this.effectType = type;
    }
    public void SetCallBack(Action cb)
    {
        this.callBack = cb;
    }

    //GET
    public string GetTitle()
    {
        return popupTitleName;
    }
    public List<RewardInfoData> GetRewardInfoDataList()
    {
        return rewardInfoList;
    }
    public EEfectGameObjectTYPE GetEffectType()
    {
        return effectType;
    }
    public Action GetCallBack()
    {
        return callBack;
    }
    

}

public class PopupController : MonoBehaviour
{
    public static PopupController instance;

    PopupRewardInfoData popupRewardInfoData = null;
    List<Action> callbacks = new List<Action>();

    public GameObject bossRewardEffect;
    


    private void Awake()
    {
        instance = this;
    }


    public void SetupPopupInfo(PopupRewardInfoData data)
    {
        popupRewardInfoData = data;

        callbacks.Clear();

        var cb = popupRewardInfoData.GetCallBack();

        var efx = GetEffectGameObject(popupRewardInfoData.GetEffectType());

        if(efx != null)
        {
            efx.SetActive(true);
        }

        if (cb != null)
        {
            callbacks.Add(cb);
        }

        callbacks.Add(Reward);

        PopupBuilder popupBuilder = new PopupBuilder(this.transform);
        popupBuilder.SetTitle(popupRewardInfoData.GetTitle());
        popupBuilder.SetButton(I2.Loc.LocalizationManager.GetTranslation("탭하여닫기 버튼") , callbacks);
        var rewards = popupRewardInfoData.GetRewardInfoDataList();

        for (int i = 0; i < rewards.Count; i++)
        {
            popupBuilder.SetRewardInfo(rewards[i].type, rewards[i].amount);

        }

        popupBuilder.Build();

    }

    GameObject GetEffectGameObject(EEfectGameObjectTYPE type)
    {

        switch (type)
        {
            case EEfectGameObjectTYPE.BossReward:
                return bossRewardEffect;
            default:
                return null;
        }

    }

    void Reward()
    {
        AudioManager.Instance.PlayEffect(ESoundEffect.GetReward);
        var rewards = popupRewardInfoData.GetRewardInfoDataList();

        for (int i = 0; i < rewards.Count; i++)
        {
            switch (rewards[i].type)
            {
                case RewardTYPE.Diamond:
                    GameDataManager.instance.AddDiamondData(rewards[i].amount);
                    UIController.instance.UpdateDiamondText(string.Format("{0:F0}", GameDataManager.instance.GetCurrency(RewardTYPE.Diamond)), string.Format("{0:F0}", rewards[i].amount), Color.cyan);

                    break;
                case RewardTYPE.Key:
                    GameDataManager.instance.GetSaveData.dungeonKey += (int)rewards[i].amount;

                    break;
                case RewardTYPE.Contract:
                    GameDataManager.instance.GetSaveData.contract += (int)rewards[i].amount;

                    break;
                case RewardTYPE.SpiritStone:
                    GameDataManager.instance.AddSpiritStone(rewards[i].amount);

                    // GameDataManager.instance.SaveDataCurrency(RewardTYPE.SpiritStone, rewards[i].amount);
                    // UIController.instance.UpdateSpiritStoneText(string.Format("{0:F0}", GameDataManager.instance.GetCurrency(RewardTYPE.SpiritStone)), string.Format("{0:F0}", rewards[i].amount), Color.white);

                    break;
                case RewardTYPE.Bell:

                    GameDataManager.instance.SetTheBellOfCall((int)rewards[i].amount);
                    UIController.instance.UpdateTheBellOfCall();
                    break;
                case RewardTYPE.Gold:
                    GameDataManager.instance.AddGoldData(rewards[i].amount);
                    // var data = NumberToSymbol.ChangeNumber(GameDataManager.instance.GetCurrency(RewardTYPE.Gold));
                    // UIController.instance.UpdateGoldText(data, NumberToSymbol.ChangeNumber(rewards[i].amount), Color.yellow);

                    break;
                default:
                    break;
            }
        }

        var efx = GetEffectGameObject(popupRewardInfoData.GetEffectType());

        if(efx != null)
        {
            efx.SetActive(false);
        }

        GameDataManager.instance.SaveData();
    }




}
