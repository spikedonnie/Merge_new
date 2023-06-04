using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CardManager : UIManager
{
    [SerializeField] private CardSlot[] cardSlots;

    private List<CardData> cardDatalist = new List<CardData>();

    public Transform heroParent;

    //public int[] gradeLevel = new int[3] { 30, 20, 10 };

    //private CardData cardData;

    public RectTransform rect;

    private float resetPosition;


    //test code
    public void TestUpgrade()
    {
        for (int i = 0; i < cardSlots.Length; i++)
        {
            cardSlots[i].UpgradeHero();
        }
    }

    //튜토리얼
    public void UpGradeHero()
    {
        cardSlots[0].UpgradeHero();
    }

    private void Start()
    {
        resetPosition = rect.localPosition.x;
        cardSlots = new CardSlot[Define.MAX_HERO_LEVEL];
        cardSlots = heroParent.GetComponentsInChildren<CardSlot>();
    }

    public IEnumerator Initialize()
    {
        base.Init();

        int [] dataLevel = GameDataManager.instance.GetSaveData.levelData.mercenaryLevel;

        bool[] lockData = GameDataManager.instance.GetSaveData.heroLock;


        for (MercenaryType i = 0; i < MercenaryType.Max; i++)
        {
            CardData cardData = new CardData();
            Player player = GameDataManager.instance.SheetJsonLoader.GetPlayerData(i);
            cardData.cardType = i;
            //cardData.name = string.Format("{0}.{1}", (int)i + 1, player.Name_KR);
            cardData.name = player.Name_KR;
            cardData.index = (int)i;
            cardData.level = dataLevel[(int)i];
            cardData.addDamage = player.AddDamage;
            cardData.damage = player.DAMAGE;
            cardData.cost = player.Cost;
            cardData.totalDamage = cardData.damage + (cardData.addDamage * cardData.level);
            cardData.maxLevel = player.MaxLevel;
            cardData.isMergeLock = lockData[(int)i];
            //cardData.totalDamage = NumberToSymbol.ChangeNumber(totalDmg);
            cardSlots[(int)i].Init(this, cardData);
            cardDatalist.Add(cardData);
        }

        yield return null;
    }

    public void HeroLock(MercenaryType type, bool isLock)
    {
        GameDataManager.instance.GetSaveData.heroLock[(int)type] = isLock;
    }

    public bool GetHeroLockState(MercenaryType type)
    {
        return GameDataManager.instance.GetSaveData.heroLock[(int)type];
    }


    public override void OpenUI()
    {
        base.OpenUI();
        //rect.localPosition = new Vector3(resetPosition, 0, 0);

        RefreshCardUI();
    }

    public void RefreshCardUI()
    {
        for (int i = 0; i < cardSlots.Length; i++)
        {
            cardSlots[i].RefreshUI();
        }
    }

}

[System.Serializable]
public class CardData
{
    public MercenaryType cardType;
    public string name;
    public int index;
    public float damage;
    public float cost;
    public int level;
    public float addDamage;
    public float totalDamage;
    public bool unLock;
    public int maxLevel;
    public bool isMergeLock;
}