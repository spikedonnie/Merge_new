using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class EventSlot : MonoBehaviour
{
    public TextMeshProUGUI rewardText_1;
    public TextMeshProUGUI rewardText_2;
    public TextMeshProUGUI rewardText_3;

    public Image image_1;
    public Image image_2;
    public Image image_3;

    public Button rewardBtn;
    public GameObject hide;

    bool active = false;

    EventNewUser eventUserData;

    EventUI eventUI;

    private void Start()
    {
        rewardBtn.onClick.AddListener(Reward);
    }

    public void Init(EventNewUser e, bool active, EventUI eUI)
    {
        eventUI = eUI;
        eventUserData = e;
        rewardText_1.text = eventUserData.amount_1.ToString();
        rewardText_2.text = eventUserData.amount_2.ToString();
        rewardText_3.text = eventUserData.amount_3.ToString();

        image_1.sprite = Utils.GetUiSprite(eventUserData.rewardType_1).uiSprite;
        image_2.sprite = Utils.GetUiSprite(eventUserData.rewardType_2).uiSprite;
        image_3.sprite = Utils.GetUiSprite(eventUserData.rewardType_3).uiSprite;

        hide.SetActive(active);
    }
    public void Interaction(bool active)
    {
        this.active = active;
    }
    void Reward()
    {   
        if(!this.active)
        {
            return;
        }

        CheckAdResetTime dTime = new CheckAdResetTime();

        dTime.CheckOverEventNewUserDayTime(SetEventTime);

    }

    void SetEventTime(string resetTime)
    {
        
        PopupRewardInfoData data = new PopupRewardInfoData();

        var title = I2.Loc.LocalizationManager.GetTranslation("TitleEventNewUser");

        data.SetTitle(title);

        List<RewardInfoData> rewards = new List<RewardInfoData>();
        
        rewards.Add(new RewardInfoData((RewardTYPE)System.Enum.Parse(typeof(RewardTYPE), eventUserData.rewardType_1), eventUserData.amount_1));
        rewards.Add(new RewardInfoData((RewardTYPE)System.Enum.Parse(typeof(RewardTYPE), eventUserData.rewardType_2), eventUserData.amount_2));
        rewards.Add(new RewardInfoData((RewardTYPE)System.Enum.Parse(typeof(RewardTYPE), eventUserData.rewardType_3), eventUserData.amount_3));


        data.SetRewardInfoList(rewards);

        PopupController.instance.SetupPopupInfo(data);

        GameDataManager.instance.SetEventCount(1);
        GameDataManager.instance.GetSaveData.eventNewUserTime = resetTime;


        hide.SetActive(true);
        eventUI.CloseUI();

    }


   




}
