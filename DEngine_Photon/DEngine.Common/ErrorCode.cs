﻿namespace DEngine.Common
{
    public enum ErrorCode : short
    {
        UnknownRequest = -3,
        OperationDedined = -2,
        InvalidParams = -1,
        Success = 0,
        InvalidPassword,
        DuplicateUserName,
        DuplicateLogin,
        UserNotFound,
        UserNotReady,
        UserInBattle,
        RoleNotFound,
        RoleNotReady,
        RoleInBattle,
        RoleIsDeath,
        CashInsufficient,
        TargetNotFound,
        TargetNotReady,
        TargetNotAvaiable,
        ItemNotAvailable,
        SkillNotAvaiable,
        SkillInvalidCost,
        SkillHPInsufficient,
        SkillMPInsufficient,
        SkillInvalidTarget,
        EnergyInsufficient,
        ItemsInsufficient,
        UserLevelNotEnough,
        RoleLevelNotEnough,
        MissionLevelNotEnough,
        ItemsUpgradeFailed,
        MobileCardChargeFailed,
        GoogleStoreChargeFailed,
        DuplicateNickName,
        BattleNotAvailable,
        AppleStoreChargeFailed,
    }
}
