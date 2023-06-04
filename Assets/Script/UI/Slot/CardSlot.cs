using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class CardSlot : MonoBehaviour
{
    [SerializeField] private UISlot slot = new UISlot();
    [SerializeField] private TextMeshProUGUI heroName;
    [SerializeField] private TextMeshProUGUI indexHero;
    [SerializeField] private TextMeshProUGUI cardDamage;
    [SerializeField] private TextMeshProUGUI probability;
    [SerializeField] private TextMeshProUGUI probabilityName;
    [SerializeField] private TextMeshProUGUI cost;

    [SerializeField] private Button gradeBtn;
    [SerializeField] private Button lockBtn;
    [SerializeField] private CardData cardData;
    [SerializeField] private Image coinImage;

    public GameObject unLockPanel;
    public ParticleSystem particle;
    [SerializeField] UpgradeEffect upgradeEffect;
    CardManager cardManager;
    public Image lockImage;



    private void Awake()
    {
        gradeBtn.onClick.AddListener(UpgradeHero);
        lockBtn.onClick.AddListener(ActiveLock);
    }

    public void Init(CardManager manager, CardData card)
    {
        cardManager = manager;
        cardData = card;
        slot.iconImage.sprite = Utils.GetDocumentHeroSprite(cardData.cardType.ToString()).uiSprite;
        heroName.text = I2.Loc.LocalizationManager.GetTranslation(cardData.name);
        RefreshUI();
    }

    public void ActiveLock()
    {
        if(cardData.isMergeLock)
        {
            cardData.isMergeLock = false;
            lockImage.sprite = Utils.GetUiSprite("UnLock").uiSprite;
            cardManager.HeroLock(cardData.cardType, false);
        }
        else
        {
            cardData.isMergeLock = true;
            lockImage.sprite = Utils.GetUiSprite("Lock").uiSprite;
            cardManager.HeroLock(cardData.cardType, true);
        }
    }

    public void UpgradeHero()
    {
        if (CheckMaxLevel() || upgradeEffect.isPlayUpgrade) return;

        StartCoroutine(StartUpgradeHero());

    }

    IEnumerator StartUpgradeHero()
    {
        upgradeEffect.isPlayUpgrade = true;
        if (GameDataManager.instance.CheckIsEnoughCurrency(RewardTYPE.Gold, cardData.cost))
        {
            GameDataManager.instance.SubtractGoldData(cardData.cost);

            if (CalcProbbability())
            {
                AudioManager.Instance.PlaySound(PoolAudio.UPGRADE);
                cardData.level++;
                cardData.totalDamage = cardData.damage + (cardData.addDamage * cardData.level);
                particle.Stop(true);
                particle.Play(true);
                GameController.instance.abilityManager.SetMercenaryUpgrade(cardData.cardType, cardData.level);
                RefreshUI();
                yield return StartCoroutine(upgradeEffect.ClickEffectProcess());
            }
        }
        upgradeEffect.isPlayUpgrade = false;

    }
    private bool CheckMaxLevel()
    {
        if (cardData.level >= cardData.maxLevel)
        {
            return true;
        }
        return false;
    }

    public void RefreshUI()
    {

        int unLock = GameDataManager.instance.GetSaveData.unLockLevel + 1;
        cardData.unLock = (int)cardData.cardType >= unLock ? true : false;
        indexHero.text = string.Format("Lv {0}", cardData.level);
        unLockPanel.SetActive(cardData.unLock);
        cardData.totalDamage = cardData.damage + (cardData.addDamage * cardData.level);
        cardDamage.text = string.Format("{0} >> {1}", NumberToSymbol.ChangeNumber(cardData.totalDamage), NumberToSymbol.ChangeNumber(cardData.damage + (cardData.addDamage * (cardData.level + 1))));
        heroName.text = I2.Loc.LocalizationManager.GetTranslation(cardData.name);
        probabilityName.text = I2.Loc.LocalizationManager.GetTranslation("강화확률 텍스트");
        if (cardData.isMergeLock)
        {
            lockImage.sprite = Utils.GetUiSprite("Lock").uiSprite;
        }
        else
        {
            lockImage.sprite = Utils.GetUiSprite("UnLock").uiSprite;
        }

        if (CheckMaxLevel())
        {
            probability.text = string.Format("MAX");
            cost.text = string.Format("");
            coinImage.enabled = false;
        }
        else
        {
            probability.text = string.Format("{0:F0}%", 100 - cardData.level);
            cost.text = NumberToSymbol.ChangeNumber(cardData.cost);
        }

        if (cardData.cost <= GameDataManager.instance.GetCurrency(RewardTYPE.Gold) && !CheckMaxLevel())
        {
            gradeBtn.image.sprite = Utils.GetOnOffButtonSprite("Possible").uiSprite;

        }
        else
        {
            gradeBtn.image.sprite = Utils.GetOnOffButtonSprite("ImPossible").uiSprite;
        }
    }

    //확률 계산
    bool CalcProbbability()
    {
        int ran = Random.Range(0, 100);

        if(ran < 100 - cardData.level)
        {
            return true;
        }

        return false;
    }

}