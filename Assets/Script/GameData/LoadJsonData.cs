using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class LoadJsonData
{
    List<TextAsset> sheetDatas;
    [SerializeField] private List<DataList<Enemy>> enemyDataList = new List<DataList<Enemy>>();
    [SerializeField] private DataList<Alarm> alarmDataList;
    [SerializeField] private DataList<ShopProduct> productDataList;
    [SerializeField] private DataList<Player> playerDataList;
    [SerializeField] private DataList<Training> trainingDataList;
    [SerializeField] private DataList<TrainingCost> trainingCostDataList;
    [SerializeField] private DataList<Tutorial> tutorialDataList;
    [SerializeField] private DataList<Relic> relicDataList;
    [SerializeField] private DataList<EventNewUser> eventNewUserDataList;
    [SerializeField] private DataList<ConvertText> convertTextDataList;
    [SerializeField] private DataList<GodFingerModel> godFingerDataList;
    [SerializeField] private DataList<BadgeModel> badgeDataList;





    public void LoadSheetData(List<TextAsset> asset)
    {
        sheetDatas = asset;
        SetAlarmData();
        SetPlayerData();
        SetProductData();
        SetTrainingData();
        SetTrainingCostData();
        SetTutorialData();
        SetRelicData();
        SetEnemyData();
        SetEventNewUserData();
        SetTextTypeData();
        SetGodFingerData();
        SetBadgeData();
    }

    private void SetEnemyData()
    {
        var common = GetJsonData<DataList<Enemy>>(SheetDataTYPE.CommonMonsterData);
        var elite = GetJsonData<DataList<Enemy>>(SheetDataTYPE.EliteMonsterData);
        var unique = GetJsonData<DataList<Enemy>>(SheetDataTYPE.UniqueMonsterData);

        // set type
        common.data.ForEach(f => f.EnemyTYPE = EnemyTYPE.Common);
        elite.data.ForEach(f => f.EnemyTYPE = EnemyTYPE.Elite);
        unique.data.ForEach(f => f.EnemyTYPE = EnemyTYPE.Unique);

        enemyDataList.Add(common);
        enemyDataList.Add(elite);
        enemyDataList.Add(unique);
    }
    private void SetAlarmData() { alarmDataList = GetJsonData<DataList<Alarm>>(SheetDataTYPE.AlarmData); }
    private void SetPlayerData() { playerDataList = GetJsonData<DataList<Player>>(SheetDataTYPE.PlayerData); }
    private void SetProductData() { productDataList = GetJsonData<DataList<ShopProduct>>(SheetDataTYPE.ProductData); }
    private void SetTrainingData() { trainingDataList = GetJsonData<DataList<Training>>(SheetDataTYPE.TrainingData); }
    private void SetTrainingCostData() { trainingCostDataList = GetJsonData<DataList<TrainingCost>>(SheetDataTYPE.TrainingCostData); }
    private void SetTutorialData() { tutorialDataList = GetJsonData<DataList<Tutorial>>(SheetDataTYPE.TutorialData); }
    private void SetRelicData() { relicDataList = GetJsonData<DataList<Relic>>(SheetDataTYPE.RelicData); }
    private void SetEventNewUserData() { eventNewUserDataList = GetJsonData<DataList<EventNewUser>>(SheetDataTYPE.EventNewUser); }
    private void SetTextTypeData() { convertTextDataList = GetJsonData<DataList<ConvertText>>(SheetDataTYPE.TextData); }
    void SetGodFingerData() { godFingerDataList = GetJsonData<DataList<GodFingerModel>>(SheetDataTYPE.GodFingerData);  }
    void SetBadgeData() { badgeDataList = GetJsonData<DataList<BadgeModel>>(SheetDataTYPE.BadgeData);  }

    private T GetJsonData<T>(SheetDataTYPE sheetDataType)
    {
        var data = sheetDatas[(int)sheetDataType];
        return JsonUtility.FromJson<T>(data.text);
    }


    [System.Serializable]
    public class DataList<T>
    {
        public List<T> data = new List<T>();
    }

    public Alarm GetAlarmData(AlarmTYPE alarmType)
    {
        var data = alarmDataList.data?.FirstOrDefault(f => f.ALARM_TYPE == alarmType.ToString());

        if (data == null)
        {
            Debug.LogError($"{alarmType.ToString()} 데이터 리스트에 {alarmType} 인덱스를 가진 데이터가 없습니다.");
            return null;
        }
        else
        {
            return data;
        }
    }

    public Enemy GetEnemyData(EnemyTYPE monsterType, int stage)
    {
        var data = enemyDataList[(int)monsterType].data?.FirstOrDefault(f => f.ID == stage);
        if (data == null)
        {
            Debug.LogError($"{monsterType.ToString()} 데이터 리스트에 {stage} 인덱스를 가진 데이터가 없습니다.");
            return null;
        }
        else
        {
            return data;
        }
    }

    public Player GetPlayerData(MercenaryType mercenaryType)
    {
        var data = playerDataList.data?.FirstOrDefault(f => f.NAME == mercenaryType.ToString());

        if (data == null)
        {
            Debug.LogError($"{mercenaryType.ToString()} 데이터 리스트에 {mercenaryType} 인덱스를 가진 데이터가 없습니다.");
            return null;
        }
        else
        {
            return data;
        }
    }

    public ShopProduct GetProductData(ShopProductType productType)
    {
        var data = productDataList?.data?.FirstOrDefault(f => f.PRODUCT_TYPE == productType.ToString());

        if (data == null)
        {
            Debug.LogError($"{productType.ToString()} 데이터 리스트에 {productType} 인덱스를 가진 데이터가 없습니다.");
            return null;
        }
        else
        {
            return data;
        }
    }

    public Relic GetRelicData(RelicType relicType)
    {
        var data = relicDataList.data?.FirstOrDefault(f => f.NAME == relicType.ToString());

        if (data == null)
        {
            Debug.LogError($"{relicType.ToString()} 데이터 리스트에 {relicType} 인덱스를 가진 데이터가 없습니다.");
            return null;
        }
        else
        {
            return data;
        }
    }

    public TrainingCost GetTrainingCostData(int level)
    {
        var data = trainingCostDataList.data?.FirstOrDefault(f => f.LEVEL == level + 1);

        if (data == null)
        {
            Debug.LogError($"{level + 1} 데이터 리스트에 {level + 1} 인덱스를 가진 데이터가 없습니다.");
            return null;
        }
        else
        {
            return data;
        }
    }

    public Training GetTrainingData(TRAINING_TYPE type)
    {
        var data = trainingDataList.data?.FirstOrDefault(f => f.TYPE == type.ToString());

        if (data == null)
        {
            Debug.LogError($"{type.ToString()} 데이터 리스트에 {type} 인덱스를 가진 데이터가 없습니다.");
            return null;
        }
        else
        {
            return data;
        }
    }

    public Tutorial GetTutorialData(TUTORIAL_STEP step)
    {
        var data = tutorialDataList.data?.FirstOrDefault(f => f.TutorialType == step.ToString());

        if (data == null)
        {
            Debug.LogError($"{step.ToString()} 데이터 리스트에 {step} 인덱스를 가진 데이터가 없습니다.");
            return null;
        }
        else
        {
            return data;
        }
    }

    public EventNewUser GetEventNewUserData(int day)
    {
        var data = eventNewUserDataList.data?.FirstOrDefault(f => f.day == day);

        if (data == null)
        {
            Debug.LogError($"{day.ToString()} 데이터 리스트에 {day} 인덱스를 가진 데이터가 없습니다.");
            return null;
        }
        else
        {
            return data;
        }
    }
    public ConvertText GetConvertTextData(TextType type)
    {
        var data = convertTextDataList.data?.FirstOrDefault(f => f.TextType == type.ToString());

        if (data == null)
        {
            Debug.LogError($"{type.ToString()} 데이터 리스트에 {type} 인덱스를 가진 데이터가 없습니다.");
            return null;
        }
        else
        {
            return data;
        }
    }

    public GodFingerModel GetGodFingerData(int count)
    {
        var data = godFingerDataList.data?.FirstOrDefault(f => f.count == count);

        if (data == null)
        {
            Debug.LogError($"{count.ToString()} 데이터 리스트에 {count} 인덱스를 가진 데이터가 없습니다.");
            return null;
        }
        else
        {
            return data;
        }
    }
    public BadgeModel GetBadgeData(BADGE_TYPE type)
    {
        var data = badgeDataList.data?.FirstOrDefault(f => f.badgeType == type.ToString());

        if (data == null)
        {
            Debug.LogError($"{type} 데이터 리스트에 {type} 인덱스를 가진 데이터가 없습니다.");
            return null;
        }
        else
        {
            return data;
        }
    }


}
