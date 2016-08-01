namespace DEngine.Common
{
    public enum OperationCode : byte
    {
        None,

        World_Begin,
        Register,
        ChangePass,
        ZonesList,
        SignIn,
        SignOut,
        World_End,

        Zone_Begin,
        AdminCmd,
        SendChat,
        ShopBuy,
        UsersList,
        UserUpdate,
        RolesList,
        RoleUpdate,
        Battle,
        Dungeon,
        ChargeCash,
        Zone_End,

        SetBalance,
    }
}
