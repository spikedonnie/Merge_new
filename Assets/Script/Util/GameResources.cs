using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterSpriteInfo
{
    public string characterName;
    public List<Sprite> idleSprite;
    public List<Sprite> WalkSprite;
    public List<Sprite> hurtSprite;
    public List<Sprite> AttackSprite;
    public List<Sprite> dieSprite;
}

[System.Serializable]
public class SpriteInfo
{
    public string spriteName;
    public Sprite uiSprite;
}

[CreateAssetMenu]
public class GameResources : ScriptableObject
{
    public List<CharacterSpriteInfo> spriteInfoList = new List<CharacterSpriteInfo>();

    public List<GameObject> normalMonsterList = new List<GameObject>();

    public List<GameObject> uniqueMonsterList = new List<GameObject>();

    public List<Sprite> background_List = new List<Sprite>();

    public List<SpriteInfo> uiSprite_List = new List<SpriteInfo>();

    public List<SpriteInfo> onOff_List = new List<SpriteInfo>();

    public List<SpriteInfo> document_Hero_List = new List<SpriteInfo>();

    public List<Sprite> relic_List = new List<Sprite>();

    public List<Sprite> raidBossSprite_List = new List<Sprite>();
    public List<SpriteInfo> projectile_List = new List<SpriteInfo>();
    
    public List<SpriteInfo> badgeSpriteList = new List<SpriteInfo>();
}