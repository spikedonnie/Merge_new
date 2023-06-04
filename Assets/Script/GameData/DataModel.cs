using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[System.Serializable]
public class Alarm
{
    public string ALARM_TYPE;
    public string MESSAGE;
}

[System.Serializable]
public class Enemy
{
    public int ID;
    public float HP;
    public float GOLD;
    public int MONSTER_IMAGE_ID;
    public float STONE;
    public int DIAMOND;
    public string NAME;
    public int LEADER_STAGE;
    public int BG_ID;
    public EnemyTYPE EnemyTYPE;
}

[System.Serializable]
public class Player
{
    public string NAME;
    public float DAMAGE;
    public string ATTACK_TYPE;
    public int REWARD;
    public float AddDamage;
    public float Cost;
    public int MaxLevel;
    public string EffectType;
    public string ArrowType;
    public string Name_KR;

}

[System.Serializable]
public class ShopProduct
{
    public string PRODUCT_TYPE;
    public string PRICE;
    public string NAME;
    public string INFO_TEXT_1;
    public string INFO_TEXT_2;
    public string INFO_TEXT_3;
    public string INFO_ICON_1;
    public string INFO_ICON_2;
    public string INFO_ICON_3;
}

[System.Serializable]
public class Relic
{
    public string NAME;
    public string TYPE_1;
    public string TYPE_2;
    public float VALUE_1;
    public float VALUE_2;
    public int MAX_LEVEL;
    public string TYPE_1_NAME;
    public string TYPE_2_NAME;
    public string UNIT_NAME;
    public string Name_KR;
}

[System.Serializable]
public class TrainingCost
{
    public int LEVEL;
    public float DAMAGE;
    public float CRITICALCHANCE;
    public float CRITICALDAMAGE;
    public float GOLDBONUS;
    public float HEROLEVEL;
    public float MAXSUPPLY;
    public float MAXCOLLECT;
    public float COLLECTSPEED;
    public float AUTOCOLLECTTIME;
    public float AUTOMERGETIME;
    public float D_HEROLEVEL;

    public float S_SPAWN_JUMP_RATE;
    public float S_MERGE_JUMP_RATE;
    public float S_STAGE_JUMP_RATE;
    public float D_GODFINGER_DURATION;
}

[System.Serializable]
public class Training
{
    public string NAME;
    public string TYPE;
    public float ADD_VALUE;
    public string CURRENCY_TYPE;
    public int MAX_LEVEL;
    public float DEFAULT_VALUE;
    public string UNIT_NAME;
}

[System.Serializable]
public class Tutorial
{
    public string Description;
    public int Stage;
    public string TutorialType;

}

[System.Serializable]
public class EventNewUser
{
    public int day;
    public string rewardType_1;
    public string rewardType_2;
    public string rewardType_3;
    public int amount_1;
    public int amount_2;
    public int amount_3;
    
}
[System.Serializable]
public class ConvertText
{
    public string Kr;
    public string TextType;
}

