using System;

namespace DEngine.Common.GameLogic
{
    public enum BattleMode
    {
        None,
        Challenge,
        RandomPvP,
        RandomPvA,
        RandomPvE,
    }

    public enum UserType
    {
        Unknown = -1,
        Default = 0,
        Bing,
        Google,
        Yahoo,
        FaceBook,
        Twitter,
    }

    public enum UserStatus
    {
        Default = 0,
        Ready,
        InBattle,
    }

    public enum RoleType
    {
        Default = 0,
        Hero,
        Mob,
        Elite,
        Boss,
        Hostage,
        HiredHero,
    }

    public enum RoleStatus
    {
        Default = 0,
        Active,
    }

    public enum RoleClass
    {
        None = 0,
        Warrior,
        Tanker,
        Assasin,
        Ranger,
        Elf,
        Sharpshooter,
        Mage,
        Sorceres,
        Healer,
        Count,
    }

    public enum RoleAction
    {
        Idle,
        Move,
        Action,
        Skill,
        Dead,
    }

    public enum TargetType
    {
        None,
        Self,//chính nó
        AllyOne,//1 đồng đội
        AllyGroup,//tất cả đồng đội
        EnemyOne,//1 kẻ thù
        EnemyGroup,//tất cả kẻ thù
        AreaEffect,//tác động vào tất cả kẻ thù trong phạm vi bán kính gần mục tiêu đó
        AreaAroundSelf,//tác động những kẻ thù gần nó
        SuicideBombArea,
        SuicideBombFull,
    }

    public enum CostType
    {
        None,
        HPValue,
        MPValue,
        HPPercent,
        MPPercent,
        HPMaxPercent,
        MPMaxPercent,
    }

    public enum AttribType
    {
        None,
        Strength,
        Agility,
        Intelligent,
        HPValue,
        MPValue,
        HPPercent,
        MPPercent,
        HPMaxValue,
        MPMaxValue,
        HPMaxPercent,
        MPMaxPercent,
        HPRegenValue,
        MPRegenValue,
        HPRegenPercent,
        MPRegenPercent,
        AttackValue,
        AttackPercent,
        DefenceMetal,
        DefenceWood,
        DefenceWater,
        DefenceFire,
        DefenceEarth,
        DefenceValue,
        DefencePercent,
        AttackSpeedPercent,
        MoveSpeedPercent,
        SkillDisable,
        ItemDisable,
        EnergyMax,
        EnergyRegen,
        HitRate,
        EvasionRate,
        BlockRate,
        CriticalRate,
        CriticalPower,
        ConvertOutputDamageToHP,
        ConvertOutputDamageToMP,
        ConvertInputDamageToMP,
        FeedbackDamage,
        RoleExpValue,
        RoleEngValue,
        BuffClear,
        DebuffClear,
        IgnoreDefense,
        ManaShield,
        DecreaseDamage,
        AttribCount,
    }

    public enum EffectType
    {
        None,
        Stun,
        Blind,
        Silent,
        Shield,
        Freeze,
        Poison,
        SpiritBurn,
        Bleed,
        Cripple,
        Lame,
        Break,
        Cleansing,
        BattleMage,
        Attackup,
        ATSup,
        MVSup,
        ATSdown,
        MVSdown,
        Slow,
        Dizzy,
        Hitrateup,
        Hitratedown,
        CRTRup,
        CRTRdown,
        CRTPup,
        CRTPdown,
        SuckHP,
        SuckMP,
        Blockup,
        Blockdown,
        Evadeup,
        Evadedown,
        Haste,
        Feedback,
        Hpregenup,
        Mpregenup,
        Slow2,
        ConvertMP,
        Bloodlust,
        Rage,
    }

    public enum SkillType
    {
        None,
        Normal,
        Special,
        Aura,
    }

    [Flags]
    public enum EffectMask
    {
        None,
        HitRate = 1,
        Evasion = 2,
        Critical = 4,
        Block = 8,
        Feedback = 16,
    }

    public enum ItemKind
    {
        None,
        Hero,
        Ring,
        Armor,
        Medal,
        Support,
        Material,
        Consume,
        Silver,
        Gold,
        Count,
    }

    public enum ItemSubKind
    {
        Equipment,
        MedicineHP,
        MedicineMP,
        MedicineSP,
        ItemUpgrade,
        HeroUpgrade,
        HeroBook,
    }

    public enum ItemGrade
    {
        None,
        White,
        Green,
        Blue,
        Yellow,
        Golden,
        Count,
    }

    public enum UserListType
    {
        None,
        Pillage,
        TopLevel,
        TopHonor,
        TopGold,
        TopSilver,
    }

    public enum ElemType
    {
        None,
        Metal,
        Wood,
        Water,
        Fire,
        Earth,
        Count,
    }

    public enum GiftType
    {
        Default,
        NormalEvent,
        DailyLogin,
    }

    public enum UserRelation
    {
        Enemy,
        FriendOne,
        FriendTwo,
        Master,
        Slave,
    }

    public enum ChargeType
    {
        None,
        MobileCard,
        GoogleStore,
        AppleStore,
        MicrosoftStore,
        Count,
    }

    public enum CardType
    {
        None,
        Vinaphone,
        Mobilefone,
        Viettel,
        Count,
    }

    public enum WorldMessage
    {
        None,
        LevelUp,
        Sommon,
        Mission,
        Dungeon,
        Arena,
        PillageSlave,
        PillageFree,
        ItemGold,
        RoleUpgrade,
        ItemUpgrade,
        PillageDefence,
    }
}
