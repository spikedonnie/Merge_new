using UnityEngine;

public static class Utils
{
    public static GameResources gameResources;

    public static GameResources InitResources()
    {
        return gameResources = Resources.Load<GameResources>("GameResources");
    }

    public static CharacterSpriteInfo GetItemVisualByName(string name)
    {
        for (int i = 0; i < gameResources.spriteInfoList.Count; i++)
        {
            if (name.Equals(gameResources.spriteInfoList[i].characterName))
                return gameResources.spriteInfoList[i];
        }
        return null;
    }

    public static Sprite GetRelicSpriteByName(string name)
    {
        for (int i = 0; i < gameResources.relic_List.Count; i++)
        {
            if (name.Equals(gameResources.relic_List[i].name))
                return gameResources.relic_List[i];
        }
        return null;
    }

    public static GameObject GetNormalMonsterName(int index)
    {
        return gameResources.normalMonsterList[index];
    }

    public static GameObject GetUniqueMonsterName(int index)
    {
        return gameResources.uniqueMonsterList[index];
    }

    public static Sprite GetRaidBossSprite(int type)
    {
        return gameResources.raidBossSprite_List[type];
    }

    public static Sprite GetBackground(int index)
    {
        return gameResources.background_List[index];
    }

    public static SpriteInfo GetProjectileSprite(string name)
    {
        for (int i = 0; i < gameResources.projectile_List.Count; i++)
        {
            if (name.Equals(gameResources.projectile_List[i].spriteName))
                return gameResources.projectile_List[i];
        }
        return null;
    }

    public static SpriteInfo GetUiSprite(string name)
    {
        for (int i = 0; i < gameResources.uiSprite_List.Count; i++)
        {
            if (name.Equals(gameResources.uiSprite_List[i].spriteName))
                return gameResources.uiSprite_List[i];
        }
        return null;
    }

    public static SpriteInfo GetOnOffButtonSprite(string name)
    {
        for (int i = 0; i < gameResources.onOff_List.Count; i++)
        {
            if (name.Equals(gameResources.onOff_List[i].spriteName))
                return gameResources.onOff_List[i];
        }
        return null;
    }

    public static SpriteInfo GetDocumentHeroSprite(string name)
    {
        for (int i = 0; i < gameResources.document_Hero_List.Count; i++)
        {
            if (name.Equals(gameResources.document_Hero_List[i].spriteName))
                return gameResources.document_Hero_List[i];
        }
        return null;
    }

    public static Sprite GetBadgeSprite(string name)
    {
        for (int i = 0; i < gameResources.badgeSpriteList.Count; i++)
        {
            if (name.Equals(gameResources.badgeSpriteList[i].spriteName))
                return gameResources.badgeSpriteList[i].uiSprite;
        }
        return null;
    }
}