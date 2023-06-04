using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[Serializable]
public class PlayerDamageModel
{
    public MercenaryType mercenaryType;
    public AttackType attackType;
    public int level;
    public float baseDamage = 1;
}

public class DamageManager : MonoBehaviour
{
    [SerializeField] public List<PlayerDamageModel> playerDamageModel = new List<PlayerDamageModel>();

    private List<int> playerDamage = new List<int>();

    public void SetPlayerDamageModelList()
    {
        for (MercenaryType i = 0; i < MercenaryType.Max; i++)
        {
            PlayerDamageModel playerModel = new PlayerDamageModel();
            playerModel.mercenaryType = i;
            playerModel.baseDamage = GameDataManager.instance.SheetJsonLoader.GetPlayerData(i).DAMAGE;
            playerModel.level = 1;
            playerModel.attackType = (AttackType)Enum.Parse(typeof(AttackType), GameDataManager.instance.SheetJsonLoader.GetPlayerData(i).ATTACK_TYPE);
            playerDamageModel.Add(playerModel);
        }
    }

    public PlayerDamageModel GetPlayerModelByPlayerType(MercenaryType type)
    {
        if (type >= MercenaryType.DemonMagic3)
        {
            type = MercenaryType.DemonMagic3;
        }
        return playerDamageModel?.FirstOrDefault(f => f.mercenaryType == type);
    }

    public AttackType GetAttackTypeByPlayerType(MercenaryType type)
    {
        if(type >= MercenaryType.DemonMagic3)
        {
            type = MercenaryType.DemonMagic3;
        }

        var playerData = GetPlayerModelByPlayerType(type);
        return playerData.attackType;
    }

    public float BaseDamageByPlayer(MercenaryType type)
    {
        if (type >= MercenaryType.DemonMagic3)
        {
            type = MercenaryType.DemonMagic3;
        }
        PlayerDamageModel playerData = GetPlayerModelByPlayerType(type);
        return playerData.baseDamage;
    }
}