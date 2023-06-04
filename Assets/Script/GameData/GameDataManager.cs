using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;
using JetBrains.Annotations;
using Spine;
using System;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager instance;
    public List<TextAsset> sheetDatas = new List<TextAsset>();
    public LoadJsonData loadJsonData;
    [SerializeField] private SaveData saveData = new SaveData();
    public LoadJsonData SheetJsonLoader { get{ return loadJsonData; } }
    public SaveData GetSaveData { get{ return saveData; } }

    private void Awake()
    {
        instance = this;
        loadJsonData = new LoadJsonData();
        Screen.SetResolution(1080, 1920, true);
        Input.multiTouchEnabled = false;
        Application.targetFrameRate = 50;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        DontDestroyOnLoad(this.gameObject);
    }

    public IEnumerator SetGameData()
    {
        //ES3.DeleteKey("badgeDictionary");

        //시트 데이터 로드 후 Json 파일로 변환
        SheetJsonLoader.LoadSheetData(sheetDatas);
        //데이터 로드
        LoadData();
        //30초마다 데이터 저장
        InvokeRepeating("SaveData", 30f, 30f);
        yield return new WaitForSeconds(0.1f);

    }

    #region ===========   재화 데이터 추가/감소   ================
    public void SetTheBellOfCall(int value)
    {
        saveData.theBellOfCallCount += value;
        UIController.instance.UpdateTheBellOfCall();
    }
    public void SetContract(int value)
    {
        saveData.contract += value;
        if(saveData.contract < 0) saveData.contract = 0;
    }
    public void SetDungeonKey(int value)
    {
        saveData.dungeonKey += value;
        if(saveData.dungeonKey < 0) saveData.dungeonKey = 0;
    }
    public void SetEventCount(int value)
    {
        saveData.eventCount += value;
    }
    public float GetCurrency(RewardTYPE currency) 
    { 
        switch (currency)
        {
            case RewardTYPE.Gold:
                return saveData.currency[(int)currency];
            case RewardTYPE.Diamond:
                return saveData.currency[(int)currency];
            case RewardTYPE.SpiritStone:
                return saveData.currency[(int)currency];
            case RewardTYPE.Key:
                return saveData.dungeonKey;
            case RewardTYPE.Contract:
                return saveData.contract;
            case RewardTYPE.Bell:
                return saveData.theBellOfCallCount;
            case RewardTYPE.MAX:
                break;
            default:
                break;
        }

        return 0;
    }
    public void SetCurrency(RewardTYPE type, float value)
    {
        switch (type)
        {
            case RewardTYPE.Gold:
                GameDataManager.instance.AddGoldData(value);

                break;

            case RewardTYPE.Diamond:
                GameDataManager.instance.AddDiamondData(value);

                break;

            case RewardTYPE.SpiritStone:
                GameDataManager.instance.AddSpiritStone(value);

                break;
            case RewardTYPE.Key:
                saveData.dungeonKey += (int)value;
                if (saveData.dungeonKey < 0) saveData.dungeonKey = 0;
                break;
            case RewardTYPE.Contract:
                saveData.contract += (int)value;
                if (saveData.contract < 0) saveData.contract = 0;
                break;
            case RewardTYPE.Bell:
                saveData.theBellOfCallCount += (int)value;
                UIController.instance.UpdateTheBellOfCall();
                break;
            case RewardTYPE.MAX:
                break;

            default:
                break;
        }
    }
    public void SaveDataCurrency(RewardTYPE currency, float value)
    {
        if(saveData.currency[(int)currency] + value >= float.MaxValue)
        {
            saveData.currency[(int)currency] = float.MaxValue;
        }
        else
        {
            saveData.currency[(int)currency] += value;
        }
    }
    public bool CheckIsEnoughCurrency(RewardTYPE currency, float value)
    {
        if (saveData.currency[(int)currency] >= value)
        {
            return true;
        }

        switch (currency)
        {
            case RewardTYPE.Gold:
                UIController.instance.SendPopupMessage(AlarmTYPE.NOT_ENOUGH_GOLD);
                break;

            case RewardTYPE.Diamond:
                UIController.instance.SendPopupMessage(AlarmTYPE.NOT_ENOUGH_DIAMOND);
                break;

            case RewardTYPE.SpiritStone:
                UIController.instance.SendPopupMessage(AlarmTYPE.NOT_ENOUGH_SPRITESTONE);
                break;

            case RewardTYPE.Key:
                UIController.instance.SendPopupMessage(AlarmTYPE.NOT_ENOUGH_KEY);

                break;
            case RewardTYPE.Contract:
                

                break;
            case RewardTYPE.Bell:
                UIController.instance.SendPopupMessage(AlarmTYPE.NOT_ENOUGH_BELLOFCALL);

                break;

            default:
                break;
        }

        return false;
    }
    public void AddGoldData(float add)
    {
        //Debug.Log("드랍골드:"+add);
        //골드 보너스 증가량
        float rate = GameController.instance.abilityManager.TotalAbilityData(AbilityType.GoldBonus);
        //Debug.Log("골드 증가량:" + rate);
        
        var sum = add + (add * rate);
        //골드 2배확률
        var num = GameController.instance.abilityManager.TotalAbilityData(AbilityType.DoubleGoldBonus);
        
        var result = UnityEngine.Random.Range(0, 100);

        if(result < num)
        {
            sum = sum * 2;
        }

        //Debug.Log("추가 골드 : "+ add * rate);
        //버프 골드
        var total = GameController.instance.abilityManager.CheckGoldBuffByAdReward(sum);
        //Debug.Log("최종 골드 : " + total);
        SaveDataCurrency(RewardTYPE.Gold, total);
        
        var data = NumberToSymbol.ChangeNumber(GetCurrency(RewardTYPE.Gold));

        UIController.instance.UpdateGoldText(data, NumberToSymbol.ChangeNumber(total), Color.yellow);

        GameController.instance.trainingManager.CheckGoldForButtonSprite();

    }
    public void SubtractGoldData(float sub)
    {

        SaveDataCurrency(RewardTYPE.Gold, -sub);
        var data = NumberToSymbol.ChangeNumber(GetCurrency(RewardTYPE.Gold));
        UIController.instance.UpdateGoldText(data, NumberToSymbol.ChangeNumber(sub), Color.red);
    }
    public void SubCurrency(RewardTYPE type, float value)
    {
        switch (type)
        {
            case RewardTYPE.Gold:
                SubtractGoldData(value);
                break;
            case RewardTYPE.Diamond:
                SubtractDiamondData(value);

                break;
            case RewardTYPE.SpiritStone:
                SubtractSpiritStone(value);

                break;
            case RewardTYPE.MAX:
                break;
            default:
                break;
        }
    }
    public void AddDiamondData(float add)
    {
        SaveDataCurrency(RewardTYPE.Diamond, add);
        UIController.instance.UpdateDiamondText(GetCurrency(RewardTYPE.Diamond).ToString(), add.ToString(),Color.cyan);
    }
    public void SubtractDiamondData(float sub)
    {
        SaveDataCurrency(RewardTYPE.Diamond, -sub);
        UIController.instance.UpdateDiamondText(GetCurrency(RewardTYPE.Diamond).ToString(), string.Format("-{0:F0}",sub), Color.red);
    }

    public void AddSpiritStone(float add)
    {
        var total = CalcSpiritStoneBonus(add);


        SaveDataCurrency(RewardTYPE.SpiritStone, total);

        var symbol = NumberToSymbol.ChangeNumber(GetCurrency(RewardTYPE.SpiritStone));

        UIController.instance.UpdateSpiritStoneText(symbol, NumberToSymbol.ChangeNumber(total), Color.white);

        GameController.instance.trainingManager.CheckGoldForButtonSprite();
    }
    public float CalcSpiritStoneBonus(float amount)
    {
        float rate = GameController.instance.abilityManager.TotalAbilityData(AbilityType.SpiritStoneBonus);
        var sum = amount + (amount * rate);
        //Debug.Log($"기본 영혼석:{amount} / 추가 영혼석:{rate}배 / 총 영혼석:{sum}");

        var total = GameController.instance.abilityManager.CheckSpiritStoneBuffByAdReward(sum);
        //Debug.Log($"총 영혼석:{sum}/버프 영혼석:{total}");

        return total;
    }
    public void SubtractSpiritStone(float sub)
    {
        SaveDataCurrency(RewardTYPE.SpiritStone, -sub);
        var symbol = NumberToSymbol.ChangeNumber(GetCurrency(RewardTYPE.SpiritStone));
        UIController.instance.UpdateSpiritStoneText(symbol, "-" + NumberToSymbol.ChangeNumber(sub), Color.red);
    }
    #endregion 재화

    public void SetSaveData(SaveData data)
    {
        saveData = data;
    }
    public void SetTrainingLevel(TRAINING_TYPE type, int level)
    {
        saveData.levelData.trainingLevel[(int)type] = level;
        SaveData();
    }
    public void SetMercenaryLevel(MercenaryType type, int level)
    {
        saveData.levelData.mercenaryLevel[(int)type] = level;
        SaveData();
    }

    public void SetBadgeData(BADGE_TYPE type, bool have)
    {
        saveData.badgeDictionary[type] = have;
        SaveData();
    }
    public bool GetBadgePurchaseData(BADGE_TYPE type)
    {
        if(saveData.badgeDictionary.ContainsKey(type))
        {
            return saveData.badgeDictionary[type];
        }
        else
        {
            return false;
        }
    }

    public Dictionary<BADGE_TYPE,bool> GetBadgeDictionary()
    {
        return saveData.badgeDictionary;
    }    
    
    //클라우드 데이타 저장
    public void SaveCloudData()
    {
        GameController.instance.mergeController.SaveHeroList(saveData.battleSaveData.heroList);
        CloudManager.Instance.CloudSave();
    }


    //데이터 저장
    public void SaveData()
    {   
        if(GameController.instance == null) return;

        if(GameController.instance.mergeController != null)
        {
            GameController.instance.mergeController.SaveHeroList(saveData.battleSaveData.heroList);
            saveData.sortingCoolTime = GameController.instance.mergeController.GetSortingTime();
        } 
        
        if(GameController.instance.AdManager != null) GameController.instance.AdManager.SaveBuffData();


        ES3.Save("isFirstConnect", saveData.isFirstConnect);
        ES3.Save("isBgmOn", saveData.optionData.isBgmOn);
        ES3.Save("isEffectOn", saveData.optionData.isEffectOn);
        ES3.Save("currency", saveData.currency);
        ES3.Save("trainingLevel", saveData.levelData.trainingLevel);
        ES3.Save("relicLevel", saveData.levelData.relicLevel);
        ES3.Save("mercenaryLevel", saveData.levelData.mercenaryLevel);
        ES3.Save("adResetTime", saveData.adResetTime);
        ES3.Save("offlineGoldTime", saveData.offlineGoldTime);
        ES3.Save("playingTime", saveData.playingTime);
        ES3.Save("theBellOfCallCount", saveData.theBellOfCallCount);
        ES3.Save("tutorialStep", saveData.tutorialStep);
        ES3.Save("unLockLevel", saveData.unLockLevel);
        ES3.Save("heroTypeMaxIndex", saveData.heroTypeMaxIndex);
        ES3.Save("adMercenaryCount", saveData.advertiseSaveData.adMercenaryCount);
        ES3.Save("adRelicCount", saveData.advertiseSaveData.adRelicCount);
        ES3.Save("adFreeCallOfBell", saveData.advertiseSaveData.adFreeCallOfBell);
        ES3.Save("adFreeDiamondCount", saveData.advertiseSaveData.adFreeDiamondCount);
        ES3.Save("currentStage", saveData.battleSaveData.currentStage);
        ES3.Save("heroList", saveData.battleSaveData.heroList);
        ES3.Save("uniqueMonsterIndex", saveData.battleSaveData.uniqueMonsterIndex);
        ES3.Save("watingHero", saveData.battleSaveData.watingHero);
        ES3.Save("buyAdPassPackage", saveData.productPackage.buyAdPassPackage);
        ES3.Save("buyStartPackage", saveData.productPackage.buyStartPackage);
        ES3.Save("buyGoodPackage", saveData.productPackage.buyGoodPackage);
        ES3.Save("buyFastPackage", saveData.productPackage.buyFastPackage);
        ES3.Save("heroLock", saveData.heroLock);
        ES3.Save("dungeonKey", saveData.dungeonKey);
        ES3.Save("contract", saveData.contract);
        ES3.Save("eventCount", saveData.eventCount);
        ES3.Save("eventNewUserTime", saveData.eventNewUserTime);

        ES3.Save("damageBuffData", saveData.damageBuffData);
        ES3.Save("goldBuffData", saveData.goldBuffData);
        ES3.Save("stoneBuffData", saveData.stoneBuffData);
        ES3.Save("godFingerLevel", saveData.godFingerLevel);
        ES3.Save("godFingerTicket", saveData.godFingerTicket);
        ES3.Save("badgeDictionary", saveData.badgeDictionary);
        ES3.Save("buyKeyCount", saveData.buyKeyCount);
        ES3.Save("sortingCoolTime", saveData.sortingCoolTime);


    }

    //불러오기
    public void LoadData()
    {

        saveData.isFirstConnect = ES3.Load<bool>("isFirstConnect", true);

        saveData.optionData.isEffectOn = ES3.Load<bool>("isEffectOn", true);
        saveData.optionData.isBgmOn = ES3.Load<bool>("isBgmOn", true);

         
        saveData.advertiseSaveData.adMercenaryCount = ES3.Load<int>("adMercenaryCount",Define.MAX_SUMMON_MERCENARY);
        saveData.advertiseSaveData.adRelicCount = ES3.Load<int>("adRelicCount",Define.MAX_SUMMON_RELIC);
        saveData.advertiseSaveData.adFreeCallOfBell = ES3.Load<int>("adFreeCallOfBell",Define.MAX_FREE_BELL);
        saveData.advertiseSaveData.adFreeDiamondCount = ES3.Load<int>("adFreeDiamondCount",Define.MAX_FREE_DIAMOND);

        saveData.adResetTime = ES3.Load<string>("adResetTime",defaultValue:new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).ToString());

        saveData.offlineGoldTime = ES3.Load<string>("offlineGoldTime",defaultValue:System.DateTime.Now.ToString());

        saveData.playingTime = ES3.Load<float>("playingTime",0);

        saveData.theBellOfCallCount = ES3.Load<int>("theBellOfCallCount",0);
        saveData.tutorialStep = ES3.Load<TUTORIAL_STEP>("tutorialStep",TUTORIAL_STEP.NONE);
        saveData.unLockLevel = ES3.Load<int>("unLockLevel",0);

        saveData.heroTypeMaxIndex = ES3.Load<int>("heroTypeMaxIndex",0);
        saveData.battleSaveData.currentStage = ES3.Load<int>("currentStage",1);
        saveData.battleSaveData.currentStage = saveData.battleSaveData.currentStage >= Define.MAX_STAGE ? Define.MAX_STAGE : saveData.battleSaveData.currentStage;

        saveData.battleSaveData.uniqueMonsterIndex = ES3.Load<int>("uniqueMonsterIndex",0); 
        saveData.battleSaveData.uniqueMonsterIndex = saveData.battleSaveData.uniqueMonsterIndex >= Define.MAX_UNIQUE_INDEX ? Define.MAX_UNIQUE_INDEX : saveData.battleSaveData.uniqueMonsterIndex;

        saveData.battleSaveData.watingHero = ES3.Load<int>("watingHero",5);
        saveData.battleSaveData.heroList = ES3.Load<List<int>>("heroList",new List<int>());

        saveData.productPackage.buyAdPassPackage = ES3.Load<bool>("buyAdPassPackage",false);
        saveData.productPackage.buyStartPackage = ES3.Load<bool>("buyStartPackage",false);
        saveData.productPackage.buyGoodPackage = ES3.Load<bool>("buyGoodPackage",false);
        saveData.productPackage.buyFastPackage = ES3.Load<bool>("buyFastPackage",false);

        saveData.levelData.mercenaryLevel = Enumerable.Repeat<int>(0, (int)MercenaryType.Max).ToArray<int>();

        int[] mercenarySaveData =  ES3.Load<int[]>("mercenaryLevel",Enumerable.Repeat<int>(0, (int)MercenaryType.Max).ToArray<int>());

        for(int i = 0; i < mercenarySaveData.Length; i++)
        {
            saveData.levelData.mercenaryLevel[i] = mercenarySaveData[i];
        }

        saveData.levelData.trainingLevel = Enumerable.Repeat<int>(0, (int)TRAINING_TYPE.MAX).ToArray<int>();

        int[] trainingSaveData =  ES3.Load<int[]>("trainingLevel",Enumerable.Repeat<int>(0, (int)TRAINING_TYPE.MAX).ToArray<int>());

        for(int i = 0; i < trainingSaveData.Length; i++)
        {
            if(i < saveData.levelData.trainingLevel.Length)
            saveData.levelData.trainingLevel[i] = trainingSaveData[i];
        }


        saveData.levelData.relicLevel = ES3.Load<int[]>("relicLevel",Enumerable.Repeat<int>(0, (int)RelicType.MAX).ToArray<int>());



        saveData.levelData.relicLevel = Enumerable.Repeat<int>(0, (int)RelicType.MAX).ToArray<int>();

        int[] relicSaveData =  ES3.Load<int[]>("relicLevel",Enumerable.Repeat<int>(0, (int)RelicType.MAX).ToArray<int>());

        for(int i = 0; i < relicSaveData.Length; i++)
        {
            if(i < saveData.levelData.relicLevel.Length)
            saveData.levelData.relicLevel[i] = relicSaveData[i];
        }



        if (!ES3.KeyExists("heroLock"))
        {
            saveData.heroLock = Enumerable.Repeat<bool>(false, (int)MercenaryType.Max).ToArray<bool>();
            ES3.Save("heroLock", saveData.heroLock);
            saveData.heroLock = ES3.Load<bool[]>("heroLock");
        }
        else
        {
            bool[] heroLock = Enumerable.Repeat<bool>(false, (int)MercenaryType.Max).ToArray<bool>();
            bool[] lockData = ES3.Load<bool[]>("heroLock");

            for (int i = 0; i < lockData.Length; i++)
            {
                heroLock[i] = lockData[i];
            }
            saveData.heroLock = heroLock;
        }

        saveData.currency = ES3.Load<float[]>("currency", new float[3] { 0, 0, 0 });



        saveData.eventNewUserTime = ES3.Load<string>("eventNewUserTime",defaultValue:new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).ToString());
        saveData.dungeonKey = ES3.Load<int>("dungeonKey",0);
        saveData.contract = ES3.Load<int>("contract",0);
        saveData.eventCount = ES3.Load<int>("eventCount",0);

        saveData.damageBuffData = ES3.Load<BuffData>("damageBuffData",new BuffData(BuffType.DAMAGE,Define.MAX_FOOD_COUNT,0));
        saveData.goldBuffData = ES3.Load<BuffData>("goldBuffData",new BuffData(BuffType.DAMAGE,Define.MAX_FOOD_COUNT,0));
        saveData.stoneBuffData = ES3.Load<BuffData>("stoneBuffData",new BuffData(BuffType.DAMAGE,Define.MAX_FOOD_COUNT,0));

        saveData.godFingerLevel = ES3.Load<int>("godFingerLevel",0);
        saveData.godFingerTicket = ES3.Load<int>("godFingerTicket",0);

        saveData.badgeDictionary = ES3.Load<Dictionary<BADGE_TYPE,bool>>("badgeDictionary",new Dictionary<BADGE_TYPE,bool>());

        saveData.buyKeyCount = ES3.Load<int>("buyKeyCount",Define.MAX_BUY_DUNGEONKEY);
        saveData.sortingCoolTime = ES3.Load<float>("sortingCoolTime", 180f);

        string loadString = Newtonsoft.Json.JsonConvert.SerializeObject(saveData);
        Debug.Log(loadString);

    }

    




}


