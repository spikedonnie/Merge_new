using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using System.Collections;

public class TrainingSlot : MonoBehaviour
{
    [SerializeField] public TrainingModel trainingModel;

    public Image currencyIcon;
    [SerializeField] private Image skillIcon = null;
    [SerializeField] private Button upgradeButton = null;
    [SerializeField] private TextMeshProUGUI skillNameText = null;
    [SerializeField] private TextMeshProUGUI skillPriceText = null;
    [SerializeField] private TextMeshProUGUI skillMaxLevelText = null;
    [SerializeField] private TextMeshProUGUI skillBeforeValueText = null;
    [SerializeField] private TextMeshProUGUI skillAfterValueText = null;
    [SerializeField] private TextMeshProUGUI skillCurrentLevelText = null;

    [SerializeField] private ParticleSystem particle;
    [SerializeField] UpgradeEffect upgradeEffect;
    private TrainingManager trainingManager;

    private float upgradeCost;

    private float applyValue;

    private void Awake()
    {
        upgradeButton.onClick.AddListener(Upgrade);
    }

    public void Init(TrainingManager tm, TrainingModel info)
    {
        trainingManager = tm;
        trainingModel = info;
        currencyIcon.sprite = Utils.GetUiSprite(trainingModel.RewardTYPE.ToString()).uiSprite;
        skillIcon.sprite = Utils.GetUiSprite(trainingModel.trainingType.ToString()).uiSprite;
        RefreshButtonInfoUI();
    }

    public void ApplyButton()
    {
        if (CheckMaxLevel())
        {
            upgradeButton.image.sprite = Utils.GetOnOffButtonSprite("ImPossible").uiSprite;
            return;
        }

        upgradeCost = GetCost(GameDataManager.instance.SheetJsonLoader.GetTrainingCostData(trainingModel.currentLevel));

        if (upgradeCost <= GameDataManager.instance.GetCurrency(trainingModel.RewardTYPE))
        {
            upgradeButton.image.sprite = Utils.GetOnOffButtonSprite("Possible").uiSprite;
        }
        else
        {
            upgradeButton.image.sprite = Utils.GetOnOffButtonSprite("ImPossible").uiSprite;
        }
    }

    //UI 및 버튼컬러 변경
    public void RefreshButtonInfoUI()
    {
        if (CheckMaxLevel())
        {
            upgradeCost = 0;

            var before = trainingModel.defaultVALUE + (trainingModel.addVALUE * (trainingModel.currentLevel));

            skillPriceText.text = string.Format("");

            skillAfterValueText.text = string.Format("MAX");

            skillCurrentLevelText.text = string.Format("LV {0}", trainingModel.currentLevel);

            skillBeforeValueText.text = string.Format("{0:0.#}{1}", before, trainingModel.unitName);

            currencyIcon.enabled = false;

        }
        else
        {
            upgradeCost = GetCost(GameDataManager.instance.SheetJsonLoader.GetTrainingCostData(trainingModel.currentLevel));

            var before = trainingModel.defaultVALUE + (trainingModel.addVALUE * (trainingModel.currentLevel));

            var after = trainingModel.defaultVALUE + (trainingModel.addVALUE * (trainingModel.currentLevel + 1));

            skillPriceText.text = string.Format("{0}", StringReturnType(upgradeCost));

            skillCurrentLevelText.text = string.Format("LV {0}", trainingModel.currentLevel);

            skillBeforeValueText.text = string.Format("{0:0.#}{1}", before, trainingModel.unitName);

            skillAfterValueText.text = string.Format("{0:0.#}{1}", after, trainingModel.unitName);
        }

        //skillNameText.text = trainingModel.skillName;
        skillNameText.text = I2.Loc.LocalizationManager.GetTranslation(trainingModel.skillName);

        skillMaxLevelText.text = string.Format("MAX {0}", trainingModel.maxLevel);

        trainingManager.CheckGoldForButtonSprite();
    }

    string StringReturnType(float value)
    {

        switch (trainingModel.trainingType)
        {
            case TRAINING_TYPE.DAMAGE:
                return NumberToSymbol.ChangeNumber(value);
            case TRAINING_TYPE.CRITICAL_CHANCE:
                return NumberToSymbol.ChangeNumber(value);
            case TRAINING_TYPE.CRITICAL_DAMAGE:
                return NumberToSymbol.ChangeNumber(value);
            case TRAINING_TYPE.GOLD_BONUS:
                return NumberToSymbol.ChangeNumber(value);
            case TRAINING_TYPE.S_WAIT_MERCENARY:
                return NumberToSymbol.ChangeNumber(value);
            case TRAINING_TYPE.S_BATTLE_MERCENARY:
                return NumberToSymbol.ChangeNumber(value);
            case TRAINING_TYPE.S_COLLECT_SPEED:
                return NumberToSymbol.ChangeNumber(value);
            case TRAINING_TYPE.S_HERO_START_LEVEL:
                return NumberToSymbol.ChangeNumber(value);
            case TRAINING_TYPE.D_AUTO_COLLECT_TIME:
                return value.ToString();
            case TRAINING_TYPE.D_AUTO_MERGE_TIME:
                return value.ToString();
            case TRAINING_TYPE.D_HEROLEVEL:
                return value.ToString();
            case TRAINING_TYPE.S_SPAWN_JUMP_RATE:
                return NumberToSymbol.ChangeNumber(value);
            case TRAINING_TYPE.S_MERGE_JUMP_RATE:
                return NumberToSymbol.ChangeNumber(value);
            case TRAINING_TYPE.D_GODFINGER_DURATION:
                return value.ToString();
            case TRAINING_TYPE.MAX:
                break;
            default:
                break;
        }
        return null;
    }

    private float GetCost(TrainingCost tc)
    {
        switch (trainingModel.trainingType)
        {
            case TRAINING_TYPE.DAMAGE:
                return tc.DAMAGE;

            case TRAINING_TYPE.CRITICAL_CHANCE:
                return tc.CRITICALCHANCE;

            case TRAINING_TYPE.CRITICAL_DAMAGE:
                return tc.CRITICALDAMAGE;

            case TRAINING_TYPE.GOLD_BONUS:
                return tc.GOLDBONUS;

            case TRAINING_TYPE.S_HERO_START_LEVEL:
                return tc.HEROLEVEL;

            case TRAINING_TYPE.S_WAIT_MERCENARY:
                return tc.MAXSUPPLY;

            case TRAINING_TYPE.S_BATTLE_MERCENARY:
                return tc.MAXCOLLECT;

            case TRAINING_TYPE.S_COLLECT_SPEED:
                return tc.COLLECTSPEED;

            case TRAINING_TYPE.D_AUTO_COLLECT_TIME:
                return tc.AUTOCOLLECTTIME;

            case TRAINING_TYPE.D_AUTO_MERGE_TIME:
                return tc.AUTOMERGETIME;
            case TRAINING_TYPE.D_HEROLEVEL:
                return tc.D_HEROLEVEL;

            case TRAINING_TYPE.S_SPAWN_JUMP_RATE:
                return tc.S_SPAWN_JUMP_RATE;
            case TRAINING_TYPE.S_MERGE_JUMP_RATE:
                return tc.S_MERGE_JUMP_RATE;
            case TRAINING_TYPE.D_GODFINGER_DURATION:
                return tc.D_GODFINGER_DURATION;
            case TRAINING_TYPE.MAX:
                break;

            default:
                break;
        }

        return 0;
    }



    //강화
    public void Upgrade()
    {
        if (CheckMaxLevel() || upgradeEffect.isPlayUpgrade) return;

        StartCoroutine(StartUpgrade());
    }


    IEnumerator StartUpgrade()
    {
        yield return null;
        upgradeEffect.isPlayUpgrade = true;

        if (GameDataManager.instance.CheckIsEnoughCurrency(trainingModel.RewardTYPE, upgradeCost))
        {
            GameDataManager.instance.SubCurrency(trainingModel.RewardTYPE, upgradeCost);
            trainingModel.currentLevel++;
            particle.Stop(true);
            particle.Play(true);
            AudioManager.Instance.PlaySound(PoolAudio.UPGRADE);
            GameController.instance.abilityManager.SetTrainingUpgrade(trainingModel.trainingType, trainingModel.currentLevel);
            RefreshButtonInfoUI();
            yield return StartCoroutine(upgradeEffect.ClickEffectProcess());
        }
        upgradeEffect.isPlayUpgrade = false;

    }

    private bool CheckMaxLevel()
    {
        if (trainingModel.currentLevel >= trainingModel.maxLevel)
        {
            return true;
        }
        return false;
    }
}