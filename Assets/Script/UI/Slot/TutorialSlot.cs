using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class TutorialSlot : MonoBehaviour
{
    public TutorialModel tutorialModel;

    private void Awake()
    {
        tutorialModel.canvasGroup = GetComponent<CanvasGroup>();
        tutorialModel.btn = GetComponentInChildren<Button>();
        tutorialModel.text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetSlot(UnityAction action, TUTORIAL_STEP step)
    {
        var sheetData = GameDataManager.instance.SheetJsonLoader.GetTutorialData(step);

        tutorialModel.btn.onClick.AddListener(action);
        tutorialModel.tutorialStep = step;
        tutorialModel.unLockStageLevel = sheetData.Stage;
        tutorialModel.description = sheetData.Description;
        tutorialModel.text.text = tutorialModel.description;
    }

    public void StartTutorial()
    {
        tutorialModel.canvasGroup.alpha = 1;
        tutorialModel.canvasGroup.blocksRaycasts = true;
    }

    public void EndTutorial()
    {
        tutorialModel.canvasGroup.alpha = 0;
        tutorialModel.canvasGroup.blocksRaycasts = false;
    }
}