using UnityEngine;
using UnityEngine.UI;

public class EventUI : UIManager
{
    [SerializeField] private CanvasGroup[] canvasElements = null;
    [SerializeField] private Button[] menuButtons = null;

    public RewardNewUser newUser;

    public override void Init()
    {
        base.Init();
        menuButtons[0].onClick.AddListener(() => OpenPanel(0));
        menuButtons[1].onClick.AddListener(() => OpenPanel(1));
        menuButtons[2].onClick.AddListener(() => OpenPanel(2));
        OpenPanel(0);
    }

    private void OpenPanel(int index)
    {
        for (int i = 0; i < canvasElements.Length; i++)
        {
            canvasElements[i].alpha = 0;
            canvasElements[i].blocksRaycasts = false;

            menuButtons[i].image.sprite = Utils.GetOnOffButtonSprite("ImPossible").uiSprite;
        }
        menuButtons[index].image.sprite = Utils.GetOnOffButtonSprite("Possible").uiSprite;

        canvasElements[index].alpha = 1;
        canvasElements[index].blocksRaycasts = true;
    }

    public override void CloseUI()
    {
        base.CloseUI();
    }

    public override void OpenUI()
    {
        base.OpenUI();
        newUser.Init(this);

    }






}