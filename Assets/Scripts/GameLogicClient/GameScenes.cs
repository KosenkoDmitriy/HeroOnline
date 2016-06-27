using UnityEngine;
using System.Collections;

public static class GameScenes
{
    public enum MyScene
    {
        None,
        MainMenu,
        ServerGame,
        Login,
        Zone,
        WorldMap,
        Lobby,
        ChargeShop,
        Ranking,
        Hero,
        HeroUpgrade,
        HeroUpStar,
        Battle,
        PVEMap,
        PVEBattle,
        GameOffline,
        Dungeon,
        ItemUpgrade,
        Arena,
        Pillage,
        Social,
        BuyHero,
        Empire
    }

    public static MyScene previousSence =  MyScene.None;
    public static MyScene currentSence =  MyScene.None;

    public static void ChangeScense(MyScene fromSence, MyScene toScene)
    {
        UINotificationManager.Instance = null;
        previousSence = fromSence;
        currentSence = toScene;
        Application.LoadLevel(toScene.ToString());
        Resources.UnloadUnusedAssets();
    }
}
