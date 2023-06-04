using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonUI : UIManager
{
    [SerializeField]
    private UISlot slot = new UISlot();


    [SerializeField] private GameObject leftButtonGameObject;
    [SerializeField] private GameObject rightButtonGameObject;

    [SerializeField] private Button challengeButton;
    [SerializeField] private Button keyButton;

    private Button leftButton = null;
    private Button rightButton = null;
    [SerializeField] private Image bossIcon;

    [SerializeField] private Image clearRewardImage;
    [SerializeField] private Image sweepRewardImage;

    [SerializeField] private TextMeshProUGUI clearText;
    [SerializeField] private TextMeshProUGUI sweepText;
    [SerializeField] private TextMeshProUGUI currntStageText;
    [SerializeField] private TextMeshProUGUI raidBossNameText;

    private TextMeshProUGUI leftStageText = null;
    private TextMeshProUGUI rightStageText = null;


    [SerializeField] private TextMeshProUGUI keyText;

    private int savedUniqueMonsterIndex;
    private int currentMonsterIndex;


    [SerializeField] private DropTable dropTable;
    [SerializeField] private RecordBattle recordBattle;
    private Enemy enemy;



    private void Start()
    {
        leftStageText = leftButtonGameObject.GetComponentInChildren<TextMeshProUGUI>();
        rightStageText = rightButtonGameObject.GetComponentInChildren<TextMeshProUGUI>();
        leftButton = leftButtonGameObject.GetComponent<Button>();
        rightButton = rightButtonGameObject.GetComponent<Button>();
        leftButton.onClick.AddListener(ClickLeftButton);
        rightButton.onClick.AddListener(ClickRightButton);
        challengeButton.onClick.AddListener(Challenge);
        keyButton.onClick.AddListener(KeyButton);
    }

    public override void Init()
    {
        base.Init();
    }
    public override void OpenUI()
    {
        base.OpenUI();

        GameController.instance.IsGameStop = true;

        savedUniqueMonsterIndex = GameDataManager.instance.GetSaveData.battleSaveData.uniqueMonsterIndex;
        currentMonsterIndex = savedUniqueMonsterIndex;

        if (currentMonsterIndex > Define.MAX_UNIQUE_INDEX)
        {
            currentMonsterIndex = Define.MAX_UNIQUE_INDEX;
        }

        SetDungeonData(currentMonsterIndex);
        SetButtons(currentMonsterIndex);
        
        CheckChallangeInteractable();
        CheckKeyInteractable();
    }
    void SetDungeonData(int uniqueIndex)
    {
        //1. ����ũ �ε����� �� ������ �����ϱ�
        enemy = GameDataManager.instance.SheetJsonLoader.GetEnemyData(EnemyTYPE.Unique, uniqueIndex);
        //2. ����ũ ���� UI ����
        MonsterInfoUI(enemy);
        //3. ������̺� ����
        SetDropTable(enemy.ID);
        //4. �ؽ�Ʈ ����
        UpdateTextUI(enemy.ID);
    }
    private void UpdateTextUI(int currentStage)
    {
        

        var getStone = GameDataManager.instance.CalcSpiritStoneBonus(dropTable.typeValue_2);

        clearText.text = string.Format("{0}", dropTable.typeValue_1);
        sweepText.text = string.Format("{0:F0}", NumberToSymbol.ChangeNumber(getStone));
        currntStageText.text = string.Format("{0}", currentStage + 1);
        leftStageText.text = string.Format("{0}", currentStage);
        rightStageText.text = string.Format("{0}", currentStage + 2);
        keyText.text = string.Format("{0}", GameDataManager.instance.GetSaveData.dungeonKey);
    }
    void MonsterInfoUI(Enemy enemy)
    {
        bossIcon.sprite = Utils.GetRaidBossSprite(enemy.ID);
        raidBossNameText.text = I2.Loc.LocalizationManager.GetTranslation(enemy.NAME);
    }
    void SetDropTable(int uniqueIndex)
    {
        var enemy = GameDataManager.instance.SheetJsonLoader.GetEnemyData(EnemyTYPE.Unique, uniqueIndex);
        DropTable dt = new DropTable(RewardTYPE.Diamond, RewardTYPE.SpiritStone, enemy.DIAMOND, enemy.STONE);
        this.dropTable = dt;
    }
    void ClickLeftButton()
    {
        currentMonsterIndex--;
        SetButtons(currentMonsterIndex);
        SetDungeonData(currentMonsterIndex);
    }
    void ClickRightButton()
    {
        currentMonsterIndex++;
        SetButtons(currentMonsterIndex);
        SetDungeonData(currentMonsterIndex);
    }
    void SetButtons(int currentStage)
    {
        SetLeftBtn(currentStage);
        SetRightBtn(currentStage);
        CheckChallangeInteractable();
        CheckKeyInteractable();
    }
    void SetLeftBtn(int currentStage)
    {   
        if (currentStage == 0)
        {
            leftButtonGameObject.SetActive(false);
        }
        else
        {
            leftButtonGameObject.SetActive(true);
        }
    }
    void SetRightBtn(int currentStage)
    {
        if (currentStage >= Define.MAX_UNIQUE_INDEX || currentStage >= savedUniqueMonsterIndex)
        {
            rightButtonGameObject.SetActive(false);
        }
        else
        {
            rightButtonGameObject.SetActive(true);
        }
    }
    void CheckChallangeInteractable()
    {
        if (currentMonsterIndex >= savedUniqueMonsterIndex)
        {
            challengeButton.interactable = true;
            challengeButton.GetComponentInChildren<TextMeshProUGUI>().text = I2.Loc.LocalizationManager.GetTranslation("enter");
        }
        else
        {
            challengeButton.interactable = false;
            challengeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Clear";
        }

    }

    void CheckKeyInteractable()
    {
        if (currentMonsterIndex < savedUniqueMonsterIndex)
        {
            keyButton.interactable = true;
        }
        else
        {
            keyButton.interactable = false;
        }

    }


    void KeyButton()
    {
       var keyCount = GameDataManager.instance.GetSaveData.dungeonKey;

       if (keyCount <= 0)
       {
           UIController.instance.SendPopupMessage(AlarmTYPE.NOT_ENOUGH_KEY);
           return;
       }

       GameDataManager.instance.SetDungeonKey(-1);
       GameController.instance.BattleManager.IsUseDungeonKey = true;
       Challenge();
    }
    public override void CloseUI()
    {
        base.CloseUI();
        GameController.instance.IsGameStop = false;
        GameController.instance.BattleManager.IsBoss = false;
    }
    bool CheckMaxUniqueIndex()
    {
        if (currentMonsterIndex < Define.MAX_UNIQUE_INDEX)
        {
            return false;
        }
        return true;
    }
    //���� ��ư
    public void Challenge()
    {
        GameController.instance.BattleManager.CurrentStage = currentMonsterIndex;
        GameController.instance.BattleManager.IsBoss = true;
        UIController.instance.SetGiveUpButton(GiveUp);
        GameController.instance.FadeInOut(1f, CallBackChallenge);
        TutorialManager.instance.CheckTutorialStep(TUTORIAL_STEP.CHALLENGE_RAID_BOSS);//Ʃ�丮��
    }
    private void CallBackChallenge()
    {
        base.CloseUI();
        GameController.instance.BattleManager.SpawnEnemy(EnemyTYPE.Unique);
    }
    //���� ��ư
    public void GiveUp()
    {
        //Unique ���� ������ ���� ��ư�� ������ �� ���� �����ߴ� Common����̳� Elite���Ͱ� �ٽ� ���´�

        
        GameController.instance.FadeInOut(1f, CallBackGiveUp);
    }
    private void CallBackGiveUp()
    {
        CloseUI();
        if (GameController.instance.BattleManager.enemyBase != null)
        {
            GameController.instance.BattleManager.enemyBase.ForceUniqueKill();
        }
        GameController.instance.BattleManager.IsUseDungeonKey = false;
        GameController.instance.BattleManager.SpawnEnemy(EnemyTYPE.Common);
    }



}