using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AbilityManager : MonoBehaviour
{
    private BuffTag buffTag = BuffTag.None;

    private Dictionary<RelicType, RelicModel> relicDictionary = new Dictionary<RelicType, RelicModel>();

    [SerializeField]
    LevelData levelData;


    public void AddBuff(BuffTag tag)
    { buffTag |= tag; }

    public void RemoveBuff(BuffTag tag)
    { buffTag &= ~tag; }

    #region ???�׷�?????? ???��Ʈ �ݹ�??? �޾� UI ????????? ó��

    public void SetTrainingUpgrade(TRAINING_TYPE type, int level)
    {
        levelData.trainingLevel[(int)type] = level;
        GameDataManager.instance.SetTrainingLevel(type, level);

        if (type.Equals(TRAINING_TYPE.S_WAIT_MERCENARY) || type.Equals(TRAINING_TYPE.S_BATTLE_MERCENARY))
        {
            UIController.instance.UpdateReserveUI();
            UIController.instance.UpdateSupplyUI();
        }

        if (type.Equals(TRAINING_TYPE.D_AUTO_COLLECT_TIME) || type.Equals(TRAINING_TYPE.D_AUTO_MERGE_TIME) || type.Equals(TRAINING_TYPE.S_COLLECT_SPEED))
        {
            UIController.instance.UpdateTimeUI();
        }

        if(type.Equals(TRAINING_TYPE.D_HEROLEVEL) || type.Equals(TRAINING_TYPE.S_HERO_START_LEVEL))
        {
            //��� �ʱ�????? ???����
            GameController.instance.mergeController.ChangeHeroStartLevel((int)GetTotalAbility(AbilityType.HeroStartLevel));
        }
    }

    public void SetMercenaryUpgrade(MercenaryType type, int level)
    {
        levelData.mercenaryLevel[(int)type] = level;
        GameDataManager.instance.SetMercenaryLevel(type, level);
    }

    public void SetRelic(RelicType type, int level)
    {
        levelData.relicLevel[(int)type] = level;

        RelicModel value;

        if (relicDictionary.TryGetValue(type, out value))
        {
            value.haveCount = level;

            //????? ��?????? �ٷ� ?????????�Ѿ�??????�͵�??? ??????
            if (value.type2.Equals(AbilityType.MaxSupply) || value.type2.Equals(AbilityType.MaxCollect))
            {
                UIController.instance.UpdateReserveUI();
                UIController.instance.UpdateSupplyUI();
            }

            if (value.type2.Equals(AbilityType.AutoCollectTime) || value.type2.Equals(AbilityType.AutoMergeTime) || value.type2.Equals(AbilityType.CollectSpeed))
            {
                UIController.instance.UpdateTimeUI();
            }

        }
    }

    #endregion ???�׷�?????? ???��Ʈ �ݹ�??? �޾� UI ????????? ó��


    public IEnumerator SetAbilityManager()
    {

        levelData = GameDataManager.instance.GetSaveData.levelData;

        for (RelicType i = 0; i < RelicType.MAX; i++)
        {
            Relic data = GameDataManager.instance.SheetJsonLoader.GetRelicData(i);
            relicDictionary.Add(i, new RelicModel
            {
                relicTypeName = Enum.Parse<RelicType>(data.NAME),
                type1 = Enum.Parse<AbilityType>(data.TYPE_1),
                type2 = Enum.Parse<AbilityType>(data.TYPE_2),
                type1_Value = data.VALUE_1,
                type2_Value = data.VALUE_2,
                relicMaxHaveCount = data.MAX_LEVEL,
                haveCount = levelData.relicLevel[(int)i]
            });
        }

        yield return null;
    }

    public float GetBadgeBonus(AbilityType type)
    {
        float addPower = 0;

        for(BADGE_TYPE i = 0; i < BADGE_TYPE.MAX; i++)
        {
            if(GameDataManager.instance.GetBadgePurchaseData(i))
            {
                var model = GameDataManager.instance.SheetJsonLoader.GetBadgeData(i);

                if(model.abilityType == type.ToString())   
                {
                    addPower += model.power;
                }
            }
        }

        //Debug.Log($"Badge {type} Bonus {addPower} % ");

        return addPower;
    }
    



    //????? ����??? ��?????? �޾�?????
    public float GetRelicDamageRateData()
    {
        int total = 0;

        for (int i = 0; i < levelData.relicLevel.Length; i++)
        {
            total += levelData.relicLevel[i];
        }

        //Debug.Log("???��??? ��??? ���� ���� " + total * 10 + " %");

        return total * Define.ADD_DAMAGERATE_RELIC;
    }

    //��� ???????? ����
    public float TotalAbilityData(AbilityType ability)
    {
        return GetTotalAbility(ability);
    }

    float GetTotalAbility(AbilityType ability)
    {
        switch (ability)
        {
            case AbilityType.Damage: return (GetTrainingDataByLevel(TRAINING_TYPE.DAMAGE) + GetRelicDamageRateData() + GetBadgeBonus(AbilityType.Damage)) * 0.01f;

            case AbilityType.CriticalChance: return GetTrainingDataByLevel(TRAINING_TYPE.CRITICAL_CHANCE) + FindAbilityToDictionary(AbilityType.CriticalChance);

            case AbilityType.CriticalDamage: return (GetTrainingDataByLevel(TRAINING_TYPE.CRITICAL_DAMAGE) + FindAbilityToDictionary(AbilityType.CriticalDamage)) * 0.01f;

            case AbilityType.GoldBonus: return (GetTrainingDataByLevel(TRAINING_TYPE.GOLD_BONUS) + FindAbilityToDictionary(AbilityType.GoldBonus)+ GetBadgeBonus(AbilityType.GoldBonus)) * 0.01f;

            case AbilityType.MaxSupply: return GetTrainingDataByLevel(TRAINING_TYPE.S_WAIT_MERCENARY);

            case AbilityType.MaxCollect: return GetTrainingDataByLevel(TRAINING_TYPE.S_BATTLE_MERCENARY);

            case AbilityType.CollectSpeed: return GetTrainingDataByLevel(TRAINING_TYPE.S_COLLECT_SPEED) + FindAbilityToDictionary(AbilityType.CollectSpeed);

            case AbilityType.AutoCollectTime: return GetTrainingDataByLevel(TRAINING_TYPE.D_AUTO_COLLECT_TIME) + FindAbilityToDictionary(AbilityType.AutoCollectTime);

            case AbilityType.AutoMergeTime: return GetTrainingDataByLevel(TRAINING_TYPE.D_AUTO_MERGE_TIME) + FindAbilityToDictionary(AbilityType.AutoMergeTime);

            case AbilityType.RecoveryHeroHP: return Define.WAKE_TIME - FindAbilityToDictionary(AbilityType.RecoveryHeroHP);

            case AbilityType.BossDamage: return FindAbilityToDictionary(AbilityType.BossDamage) * 0.01f;

            case AbilityType.SpiritStoneBonus: 
                // Debug.Log($"FindAbilityToDictionary : {FindAbilityToDictionary(AbilityType.SpiritStoneBonus)}");
                // Debug.Log($"GetBadgeBonus : {GetBadgeBonus(AbilityType.SpiritStoneBonus)}");
            return (FindAbilityToDictionary(AbilityType.SpiritStoneBonus) + GetBadgeBonus(AbilityType.SpiritStoneBonus)) * 0.01f;

            case AbilityType.WarriorDamage: return FindAbilityToDictionary(AbilityType.WarriorDamage) * 0.01f;

            case AbilityType.RangerDamage: return FindAbilityToDictionary(AbilityType.RangerDamage) * 0.01f;

            case AbilityType.MagicianDamage: return FindAbilityToDictionary(AbilityType.MagicianDamage) * 0.01f;

            case AbilityType.EnemyHP: return FindAbilityToDictionary(AbilityType.EnemyHP) * 0.01f;

            case AbilityType.OfflineBonus: return FindAbilityToDictionary(AbilityType.OfflineBonus) * 0.01f;

            case AbilityType.DoubleGoldBonus: return FindAbilityToDictionary(AbilityType.DoubleGoldBonus);

            case AbilityType.AllDamage: return FindAbilityToDictionary(AbilityType.AllDamage) * 0.01f;

            case AbilityType.HeroStartLevel: return GetTrainingDataByLevel(TRAINING_TYPE.S_HERO_START_LEVEL) + GetTrainingDataByLevel(TRAINING_TYPE.D_HEROLEVEL);

            case AbilityType.MergeSpeed: return GetBadgeBonus(AbilityType.MergeSpeed) * 0.01f;

            case AbilityType.SpawnJump: 
            //Debug.Log($"SpawnJump: {GetTrainingDataByLevel(TRAINING_TYPE.S_SPAWN_JUMP_RATE) + FindAbilityToDictionary(AbilityType.SpawnJump)}");
            return GetTrainingDataByLevel(TRAINING_TYPE.S_SPAWN_JUMP_RATE) + FindAbilityToDictionary(AbilityType.SpawnJump);

            case AbilityType.MergeJump: 
            //Debug.Log($"MergeJump: { GetTrainingDataByLevel(TRAINING_TYPE.S_MERGE_JUMP_RATE) + FindAbilityToDictionary(AbilityType.MergeJump)}");

            return GetTrainingDataByLevel(TRAINING_TYPE.S_MERGE_JUMP_RATE) + FindAbilityToDictionary(AbilityType.MergeJump);

            case AbilityType.GoldFingerDuration: 
            //Debug.Log($"GoldFingerDuration: {  GetTrainingDataByLevel(TRAINING_TYPE.D_GODFINGER_DURATION) + FindAbilityToDictionary(AbilityType.GoldFingerDuration)}");

            return GetTrainingDataByLevel(TRAINING_TYPE.D_GODFINGER_DURATION) + FindAbilityToDictionary(AbilityType.GoldFingerDuration);


            case AbilityType.MAX:
                break;
            default:
                break;
        }

        return 0;
    }

    float FindAbilityToDictionary(AbilityType type)
    {
        //Debug.Log(type + " ã��");

        float result = 0;

        for (int i = 0; i < relicDictionary.Count; i++)
        {
            if (relicDictionary[(RelicType)i].type2.Equals(type))
            {
                //Debug.Log((RelicType)i + " ã��!!!");

                if (relicDictionary[(RelicType)i].haveCount > 0)
                {
                    result = relicDictionary[(RelicType)i].type2_Value * relicDictionary[(RelicType)i].haveCount;
                }

                break;
            }
        }
        //Debug.Log("��� " + result);
        return result;
    }

    int GetTrainingLevel(TRAINING_TYPE type)
    {
        return levelData.trainingLevel[(int)type];
    }

    float GetTrainingDataByLevel(TRAINING_TYPE type)
    {
        var data = GameDataManager.instance.SheetJsonLoader.GetTrainingData(type);

        return data.DEFAULT_VALUE + ((GetTrainingLevel(type) * data.ADD_VALUE));

    }

    #region ���� ����?? ???????? ??????

    private float CheckDamageBuffByAdReward(float dmg)
    {
        if (buffTag.HasFlag(BuffTag.Damage))
        {
            return dmg * Define.ADD_BUFF_POWER_RATE;
        }
        return dmg;
    }

    public float CheckGoldBuffByAdReward(float value)
    {
        if (buffTag.HasFlag(BuffTag.Gold))
        {
            return value * Define.ADD_BUFF_GOLD_RATE;
        }
        return value;
    }

    public float CheckSpiritStoneBuffByAdReward(float value)
    {
        if (buffTag.HasFlag(BuffTag.Stone))
        {
            return value * Define.ADD_BUFF_STONE_RATE;
        }
        return value;
    }

    #endregion ���� ����?? ???????? ??????

    #region Damage ??????

    //ġ��????? ???����?? ??????
    public bool IsCritical()
    {
        float random = UnityEngine.Random.Range(0f, 100f);
        //Debug.Log("????????:" + random + " ġ��??? ?????:" + TotalAbilityData(AbilityType.CriticalChance));
        if (random < TotalAbilityData(AbilityType.CriticalChance))
        {
            return true;
        }
        return false;
    }
    //ġ��??? ???��??? ���
    public float CalcCriticalDamage(float damage)
    {
        //Debug.Log("???��???:" + damage + " ?????? : " + (damage * (1 + (GetCommonCriticalDamageRateData() * 0.01f))));
        return damage * (1 + (TotalAbilityData(AbilityType.CriticalDamage)));
    }
    public float FinalTotalDamage(MercenaryType mercenaryType, EnemyTYPE enemy)
    {
        //???���� �⺻ ����???
        PlayerDamageModel playerData = GameController.instance.DamageManager.GetPlayerModelByPlayerType(mercenaryType);
        //???���� ???���� ?????? ����???
        var add = GameDataManager.instance.SheetJsonLoader.GetPlayerData(mercenaryType).AddDamage;
        //???�׷�?????? ���� ����???
        var playerDamage = playerData.baseDamage + (add * levelData.mercenaryLevel[(int)mercenaryType]);
        //Debug.Log("????? �⺻ ����???  :  " + playerData.baseDamage);
        //Debug.Log("???����?? ����??? :  " + add);
        //Debug.Log("???����?? ����??? ??? :  " + add * levelData.mercenaryLevel[(int)mercenaryType]);
        //Debug.Log("????? �⺻ ����??? + ???����?? ����??? :  " + (long)playerDamage);

        float addTypeDamage = 0;
        //����?????? ��??? ����???
        switch (playerData.attackType)
        {
            case AttackType.WARRIOR:
                addTypeDamage = TotalAbilityData(AbilityType.WarriorDamage);
                break;
            case AttackType.RANGER:
                addTypeDamage = TotalAbilityData(AbilityType.RangerDamage);
                break;
            case AttackType.MAGIC:
                addTypeDamage = TotalAbilityData(AbilityType.MagicianDamage);
                break;
            default:
                break;
        }

        //����?? ��??? ???��??? ó��
        if (enemy == EnemyTYPE.Elite || enemy == EnemyTYPE.Unique)
        {
            //Debug.Log("����????? - " + playerDamage * GameController.instance.abilityManager.TotalAbilityData(AbilityType.BossDamage) + " - ��??? ???��???");
            playerDamage = playerDamage * (1 + (TotalAbilityData(AbilityType.Damage) + addTypeDamage + TotalAbilityData(AbilityType.AllDamage)+ TotalAbilityData(AbilityType.BossDamage)));
        }
        else
        {
            //Debug.Log("��??? ����???(??????:" + GetTrainingDataByLevel(TRAINING_TYPE.DAMAGE) + "%+?????:"+ GetRelicDamageRateData()+"%+??????:"+ addTypeDamage*100+"%+���?????:"+ TotalAbilityData(AbilityType.AllDamage)*100+"%)");
            playerDamage = playerDamage * (1 + (TotalAbilityData(AbilityType.Damage) + addTypeDamage + TotalAbilityData(AbilityType.AllDamage)));
        }


        //Debug.Log("?????? ???��??? : " + (long)playerDamage);

        return CheckDamageBuffByAdReward(playerDamage);
    }

    #endregion Damage ??????
}