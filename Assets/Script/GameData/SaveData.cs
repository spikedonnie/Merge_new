
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public TUTORIAL_STEP tutorialStep;
    public float[] currency;

    public BattleSaveData battleSaveData = new BattleSaveData();
    public AdvertiseSaveData advertiseSaveData = new AdvertiseSaveData();
    public LevelData levelData = new LevelData();
    public ProductPackage productPackage = new ProductPackage();
    public OptionData optionData = new OptionData();
    public bool[] heroLock;
    public int theBellOfCallCount = 0;
    public string offlineGoldTime;
    public string adResetTime;
    public string eventNewUserTime;
    public float playingTime;
    public int heroTypeMaxIndex;
    public int unLockLevel;
    public bool isFirstConnect;
    public int dungeonKey;//던전입장열쇠
    public int contract;//영웅소환권
    public int godFingerTicket;
    public int eventCount;
    public BuffData damageBuffData;
    public BuffData goldBuffData;
    public BuffData stoneBuffData;

    public int godFingerLevel;
    public int buyKeyCount;

    public float sortingCoolTime;
    public Dictionary<BADGE_TYPE,bool> badgeDictionary = new Dictionary<BADGE_TYPE, bool>();

}

[System.Serializable]
public class BattleSaveData
{
    public List<int> heroList;
    public int currentStage;
    public int uniqueMonsterIndex;
    public int watingHero;

}
[System.Serializable]
public class AdvertiseSaveData
{
    public int adRelicCount;
    public int adMercenaryCount;
    public int adFreeDiamondCount;
    public int adFreeCallOfBell;
}
[System.Serializable]
public class LevelData
{
    public int[] trainingLevel;
    public int[] relicLevel;
    public int[] mercenaryLevel;
}

[System.Serializable]
public class ProductPackage
{
    public bool buyStartPackage;
    public bool buyGoodPackage;
    public bool buyFastPackage;
    public bool buyAdPassPackage;

}
[System.Serializable]
public class OptionData
{
    public bool isEffectOn;
    public bool isBgmOn;
}