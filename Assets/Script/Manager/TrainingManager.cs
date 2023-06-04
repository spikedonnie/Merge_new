using System;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class TrainingManager : UIManager
{
    [SerializeField] private TrainingSlot[] trainingButtonInfo = null;
    [SerializeField] private ContentSizeFitter fitter;

    public RectTransform rect;

    private float resetPosition;

    private void Start()
    {
        resetPosition = rect.localPosition.x;
    }

    //Test Code
    public void TestUpgrade()
    {
        for (int i = 0; i < trainingButtonInfo.Length; i++)
        {
            trainingButtonInfo[i].Upgrade();
        }
    }
    //test code
    public void SetCustomLevel(TRAINING_TYPE type,int level)
    {
        trainingButtonInfo[(int)type].trainingModel.currentLevel = level;
        CheckGoldForButtonSprite();

        for (int i = 0; i < trainingButtonInfo.Length; i++)
        {
            trainingButtonInfo[i].RefreshButtonInfoUI();
        }
    }

    public IEnumerator SetTrainingManager()
    {
        base.Init();

        for (TRAINING_TYPE i = 0; i < TRAINING_TYPE.MAX; i++)
        {
            TrainingModel model = new TrainingModel();

            Training data = GameDataManager.instance.SheetJsonLoader.GetTrainingData(i);
            model.currentLevel = GameDataManager.instance.GetSaveData.levelData.trainingLevel[(int)i];
            model.trainingType = (TRAINING_TYPE)Enum.Parse(typeof(TRAINING_TYPE), data.TYPE);
            model.skillName = data.NAME;
            model.addVALUE = data.ADD_VALUE;
            model.defaultVALUE = data.DEFAULT_VALUE;
            model.maxLevel = data.MAX_LEVEL;
            model.RewardTYPE = (RewardTYPE)Enum.Parse(typeof(RewardTYPE), data.CURRENCY_TYPE);
            model.unitName = data.UNIT_NAME;
            trainingButtonInfo[(int)i].Init(this, model);
        }

        CheckGoldForButtonSprite();

        yield return null;
    }


    public override void OpenUI()
    {
        base.OpenUI();
        rect.localPosition = new Vector3(resetPosition, 0, 0);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)fitter.transform);

        for (int i = 0; i < trainingButtonInfo.Length; i++)
        {
            trainingButtonInfo[i].RefreshButtonInfoUI();
        }
    }

    public void CheckGoldForButtonSprite()
    {
        for (int i = 0; i < trainingButtonInfo.Length; i++)
        {
            trainingButtonInfo[i].ApplyButton();
        }
    }

    
    public void TutorialUpgrade()
    {
        trainingButtonInfo[0].Upgrade();
    }


}

[Serializable]
public class TrainingModel
{
    public int currentLevel;
    public TRAINING_TYPE trainingType;
    public RewardTYPE RewardTYPE;
    public string skillName;
    public float addVALUE;
    public float defaultVALUE;
    public int maxLevel;
    public string unitName;
    
}