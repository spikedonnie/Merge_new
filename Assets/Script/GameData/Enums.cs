    public enum BADGE_TYPE
    {
        Hammer,
        Castle,
        Spider,
        Fist,
        Lion,
        Wolves,
        Tooth,
        Blood,
        MAX
    }
    [System.Flags]
    public enum BuffTag
    {
        None = 0,
        Damage = 1 << 0,
        Gold = 1 << 1,
        Stone = 1 << 2,
        All = int.MaxValue
    }
    public enum AnimCurveType
    {
        EaseInQuad,
        EaseOutQuad,
        EaseInOutQuad,
        EaseOutCubic,
        EaseInOutCubic,
        Spring,
        EaseInQuint,
        EaseInOutSine,
        EaseOutExpo
    }

    public enum SheetDataTYPE
    {
        CommonMonsterData,
        EliteMonsterData,
        UniqueMonsterData,
        PlayerData,
        AlarmData,
        TutorialData,
        TrainingData,
        TrainingCostData,
        RelicData,
        ProductData,
        EventNewUser,
        TextData,
        GodFingerData,
        BadgeData
    }

    public enum TextType
    {
        EndGame,
        TitleOffLine,
        TitleEventNewUser,
        CloseTab
    }

    public enum BuffType
    {
        DAMAGE,
        GOLD,
        STONE,
        MAX
    }

 
    public enum RewardTYPE
    {
        Gold,
        Diamond,
        SpiritStone,
        Key,
        Contract,
        Bell,
        MAX
    }


 

    public enum TRAINING_TYPE
    {
        DAMAGE,
        CRITICAL_CHANCE,
        CRITICAL_DAMAGE,
        GOLD_BONUS,
        S_WAIT_MERCENARY,
        S_BATTLE_MERCENARY,
        S_COLLECT_SPEED,
        S_HERO_START_LEVEL,
        D_AUTO_COLLECT_TIME,
        D_AUTO_MERGE_TIME,
        D_HEROLEVEL,
        S_SPAWN_JUMP_RATE,
        S_MERGE_JUMP_RATE,
        D_GODFINGER_DURATION,
        MAX
    }

    public enum MenuType
    {
        MERCENARY,
        TRAINING,
        BOSSRAID,
        RELIC,
        SHOP,
        ADVERTISE,
        OPTION,
        EVENT
    }

    public enum ActionStateType
    {
        Walk,
        Attack,
        Hit,
        Die
    }

    public enum AttackType
    {
        WARRIOR,
        RANGER,
        MAGIC
    }

    public enum EnemyTYPE
    {
        Common,
        Elite,
        Unique,
        Max
    }

    public enum ShopProductType
    {
        AD_FREE_PASS,
        START_PACKAGE,
        GOOD_PACKAGE,
        FAST_PACKAGE,
        GOODS_1,
        GOODS_2,
        GOODS_3,
        GOODS_4
    }
    //알랍 메세지 타입
    public enum AlarmTYPE
    {
        NOT_ENOUGH_GOLD,
        NOT_ENOUGH_DIAMOND,
        NOT_ENOUGH_SPRITESTONE,
        FULL_BATTLE,
        FAIL_NETWORK,
        COMPLETE_AD_DAMAGE,
        COMPLETE_AD_GOLD,
        COMPLETE_AD_STONE,
        ACTIVE_BUFF,
        NOT_ENOUGH_ADCOUNT,
        CAN_NOT_OPEN_CHALLENGE,
        FULL_HERO,
        NOT_ENOUGH_BELLOFCALL,
        RELIC_ALL_MAX_LEVEL,
        HAVE_PACKAGE,
        NOT_LOAD_AD,
        HERO_LOCK,
        NOT_ENOUGH_KEY,
        NOT_ENOUGH_CONTRACT,
        SAVED_CLOUD_DATA,
        PURCHASED_BADGE
    }

    public enum AbilityType
    {
        Damage,//공격력 IgnisArmor
        CriticalChance,//치명타 확률 DarkLordHelmet
        CriticalDamage,//치명타 공격력 TomeOfDemise
        GoldBonus,//골드 보너스 HelmOfEternalLight
        MaxSupply,//최대 저장 용병 수(최대?명까지 보관)
        MaxCollect,//최대 용병 고용 수(최대 ?명까지 고용)
        CollectSpeed,//용병 생성 시간
        AutoCollectTime,//자동 용병 모집 시간(?초마다 소환)
        AutoMergeTime,//자동 합치기 시간(?초마다 합치기)
        RecoveryHeroHP,//영웅회복속도 ObsidianShield
        BossDamage,//보스 추가 데미지 SteampunkSword
        SpiritStoneBonus,//영혼석 보너스  ArmorOfRelentlessNightmares
        WarriorDamage,//전사형 추가대미지 FrozenDragonAxe
        RangerDamage,//궁수형 추가대미지 CherufeArc
        MagicianDamage,//법사형 추가대미지 CaneOfIgnis
        EnemyHP,//전사형 공속 DivineDragoonSword
        OfflineBonus,//궁수형 공속 ElementalBow2
        DoubleGoldBonus,//법사형 공속 TeraStaff
        HeroStartLevel,
        SpawnJump,
        MergeJump,
        GoldFingerDuration,
        AllDamage,//모든 용병 추가 데미지
        MergeSpeed,
        MAX
    }

    public enum RelicType
    {
        IgnisArmor,
        DarkLordHelmet,
        TomeOfDemise,
        HelmOfEternalLight,
        ObsidianShield,
        SteampunkSword,
        ArmorOfRelentlessNightmares,
        FrozenDragonAxe,
        CherufeArc,
        CaneOfIgnis,
        DivineDragoonSword,
        ElementalBow2,
        TeraStaff,
        DragonTempestSteelGarb,
        TerraLongsword,
        VoidDragonHelmet,
        RisenBow,
        BloodSpirit,
        Blazewing,
        MAX
    }

    public enum MercenaryType
    {
        Warrior,
        WarriorGirl,
        MonkGuy,
        HumanArcher,
        RomanianSettler,
        OldGuy,
        ElfArcher,
        Sergeant,
        WhiteNinja,
        Assassin,
        BlackWizard,
        BlackNinja,
        PersianWarrior,
        IndianTribeKnight,
        MayanTribeKnight,
        HumanWindArcher,
        IncaTribe,
        Orc,
        MaskedGuy,
        DevilMaskedGuy,
        HumanMaskArcher,
        GreekWarrior,
        RedArcher,
        Pirate,
        HeavyArmoredDefenderKnight,
        WhiteArmoredKnight,
        SpartanKnightwithSpear,
        MaskMagic,
        MedievalKing,
        Golem,
        FrontierDefenderSpartanKnight,
        TemplarKnight,
        VeryHeavyArmoredFrontierDefender,
        FallenWhiteAngel,
        EgyptianSentry,
        EgyptianMummy,
        Gnoll,
        Anubis,
        EliteElfArcher,
        Satyr,
        Shamen,
        GreenMagic,
        PumpkinHeadGuy,
        UndeadMagic1,
        SkullKnight,
        DemonArcher1,
        DemonsOfDarkness,
        DeathKnight,
        DemonsOfDark,
        DemonMagic2,
        Succubus,//1차업데이트
        Vampire,
        Paladin_3,
        Paladin_2,
        Paladin_1,
        Satyr_1,
        Goblin_1,
        DemonArcher2,
        Devil,
        DemonMagic3,
        Max
    }

    public enum RAIDBOSS_TYPE
    {
        BigMouthBat,
        Ground,
        Pig,
        Emerald,
        BirdOrc,
        Thornado,
        Shadow,
        Stormfly,
        Claw,
        Toxic,
        Ankylosaurus,
        Topaz,
        Lizard,
        Twin,
        Caveman,
        Velociraptor,
        Ghost,
        Magma,
        EvilEye,
        Death,
        Mummy,
        DragonOfRevenge,
        Zombie,
        Terror,
        CentaurGeneral,
        Crystal,
        Kobold,
        Thunder,
        Quilboar,
        Fiery,
        Tauren,
        Black,
        Skeleton,
        ShadowFiend,
        Manticore,
        Vampires,
        Medusa,
        Harpy,
        BlackKnight,
        ElfMage,

        MAX
    }