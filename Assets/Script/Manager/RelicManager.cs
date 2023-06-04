using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class RelicManager : UIManager
{
    public TextMeshProUGUI totalRelicDamage;

    private RelicSlot[] relicSlot;

    private List<RelicModel> relicDatalist = new List<RelicModel>();
    public RectTransform rect;

    private float resetPosition;

    private void Start()
    {
        relicSlot = GetComponentsInChildren<RelicSlot>();
        resetPosition = rect.localPosition.x;
    }

    public IEnumerator Initialize()
    {
        base.Init();

        int[] level = GameDataManager.instance.GetSaveData.levelData.relicLevel;

        for (RelicType i = 0; i < RelicType.MAX; i++)
        {
            RelicModel relicData = new RelicModel();

            Relic data = GameDataManager.instance.SheetJsonLoader.GetRelicData(i);
            relicData.relicTypeName = (RelicType)Enum.Parse(typeof(RelicType), data.NAME);
            relicData.type1 = (AbilityType)Enum.Parse(typeof(AbilityType), data.TYPE_1);
            relicData.type2 = (AbilityType)Enum.Parse(typeof(AbilityType), data.TYPE_2);
            relicData.type1_Value = data.VALUE_1;
            relicData.type2_Value = data.VALUE_2;
            relicData.relicMaxHaveCount = data.MAX_LEVEL;
            relicData.haveCount = level[(int)i];
            relicData.TYPE_1_NAME = data.TYPE_1_NAME;
            relicData.TYPE_2_NAME = data.TYPE_2_NAME;
            relicData.UNIT_NAME = data.UNIT_NAME;
            relicData.name_KR = data.Name_KR;
            relicSlot[(int)i].Init(relicData);
            relicDatalist.Add(relicData);
        }

        yield return null;
    }

    public override void OpenUI()
    {
        base.OpenUI();
        rect.localPosition = new Vector3(resetPosition, 0, 0);
        RefreshUI();
    }

    public override void CloseUI()
    {
        base.CloseUI();

        var tutoType = GameDataManager.instance.GetSaveData.tutorialStep;

        TutorialManager.instance.CheckTutorialStep(TUTORIAL_STEP.GIFT_CURRENCY);

    }

    private void RefreshUI()
    {
        for (int i = 0; i < relicSlot.Length; i++)
        {
            if (relicSlot[i].GetRelicModel().haveCount >= 1)
            {
                relicSlot[i].Show();
            }
            else
            {
                relicSlot[i].Hide();
            }
            relicSlot[i].RefreshUI();

        }

        totalRelicDamage.text = string.Format("{0}%",GameController.instance.abilityManager.GetRelicDamageRateData());
    }

    //뽑기 후에 나온 유물을 추가시켜줌
    public void AddRelic(RelicType type)
    {
        for (int i = 0; i < relicDatalist.Count; i++)
        {
            if (relicDatalist[i].relicTypeName.Equals(type))
            {
                relicDatalist[i].haveCount++;

                GameController.instance.abilityManager.SetRelic(type, relicDatalist[i].haveCount);

                break;
            }
        }



    }

    public bool IsCheckMaxLevel(RelicType type)
    {
        return relicSlot[(int)type].IsCheckMaxLevel();
    }
}

[Serializable]
public class RelicModel
{
    public int relicMaxHaveCount;
    public RelicType relicTypeName;
    public AbilityType type1;
    public AbilityType type2;
    public float type1_Value;
    public float type2_Value;
    public int haveCount;
    public string TYPE_1_NAME;
    public string TYPE_2_NAME;
    public string UNIT_NAME;
    public string name_KR;

}