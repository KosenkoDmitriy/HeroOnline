using UnityEngine;
using System.Collections;
using DEngine.Unity.Photon;

public class MyInput {


	public static void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (GameScenes.currentSence)
            {
                case GameScenes.MyScene.MainMenu:
                    HandleExitGame();
                    break;
                case GameScenes.MyScene.ServerGame:
                    GameScenes.ChangeScense(GameScenes.MyScene.ServerGame, GameScenes.MyScene.MainMenu);
                    break;
                case GameScenes.MyScene.Login:
                    GameScenes.ChangeScense(GameScenes.MyScene.Login, GameScenes.MyScene.ServerGame);
                    break;
                case GameScenes.MyScene.Zone: 
                    GameScenes.ChangeScense(GameScenes.MyScene.Zone, GameScenes.MyScene.ServerGame);
                    break;
                case GameScenes.MyScene.Lobby:
                    GameScenes.ChangeScense(GameScenes.MyScene.Lobby, GameScenes.MyScene.MainMenu);
                    break;
                case GameScenes.MyScene.WorldMap:
                    HandleExitWorldMap();
                    break;
                case GameScenes.MyScene.Hero:
                    GameScenes.ChangeScense(GameScenes.MyScene.Hero, GameScenes.MyScene.WorldMap);
                    break;
                case GameScenes.MyScene.ChargeShop:
                    GameScenes.ChangeScense(GameScenes.MyScene.ChargeShop, GameScenes.MyScene.WorldMap);
                    break;
                case GameScenes.MyScene.Ranking:
                    GameScenes.ChangeScense(GameScenes.MyScene.Ranking, GameScenes.MyScene.WorldMap);
                    break;
                case GameScenes.MyScene.PVEMap:
                    GameScenes.ChangeScense(GameScenes.MyScene.PVEMap, GameScenes.MyScene.WorldMap);
                    break;
                case GameScenes.MyScene.Arena:
                  //  GameScenes.ChangeScense(GameScenes.MyScene.Arena, GameScenes.MyScene.WorldMap);
                    break;
                case GameScenes.MyScene.BuyHero:
                    GameScenes.ChangeScense(GameScenes.MyScene.BuyHero, GameScenes.MyScene.WorldMap);
                    break;
                case GameScenes.MyScene.HeroUpgrade:
                    GameScenes.ChangeScense(GameScenes.MyScene.HeroUpgrade, GameScenes.MyScene.WorldMap);
                    break;
                case GameScenes.MyScene.HeroUpStar:
                    GameScenes.ChangeScense(GameScenes.MyScene.HeroUpStar, GameScenes.MyScene.WorldMap);
                    break;
                case GameScenes.MyScene.ItemUpgrade:
                    GameScenes.ChangeScense(GameScenes.MyScene.ItemUpgrade, GameScenes.MyScene.WorldMap);
                    break;
                case GameScenes.MyScene.Pillage:
                    GameScenes.ChangeScense(GameScenes.MyScene.Pillage, GameScenes.MyScene.WorldMap);
                    break;
                case GameScenes.MyScene.Social:
                    GameScenes.ChangeScense(GameScenes.MyScene.Social, GameScenes.MyScene.WorldMap);
                    break;
                case GameScenes.MyScene.PVEBattle:
                case GameScenes.MyScene.Battle:
                case GameScenes.MyScene.GameOffline:
                    HandleExitBattle();
                    break;
                case GameScenes.MyScene.Dungeon:
                    break;
                default:
                    GameScenes.ChangeScense(GameScenes.currentSence, GameScenes.MyScene.WorldMap);
                    break;

            }
        }
    }

    public static void HandleExitBattle()
    {
        UINoticeManager.OnButtonCancel_click += new UINoticeManager.NoticeHandle(OnResumeBattle);
        UINoticeManager.OnButtonOK_click += new UINoticeManager.NoticeHandle(OnQuitBattle);

        MessageBox.ShowDialog(GameManager.localization.GetText("Dialog_Battle_Quit"), UINoticeManager.NoticeType.YesNo);
              
    }

    private static void OnQuitBattle()
    {
        GameplayManager.Instance.SendQuitBattle();
        GameManager.dungeonCurEvent = 0;
        GameManager.dungeonEventCount = 0;
        GameScenes.ChangeScense(GameScenes.MyScene.Battle, GameScenes.MyScene.WorldMap);
    }

    private static void OnResumeBattle()
    {

    }

    private static void HandleExitGame()
    {
        UINoticeManager.OnButtonOK_click += new UINoticeManager.NoticeHandle(OnQuitGame);
        MessageBox.ShowDialog(GameManager.localization.GetText("MainMenu_QuitGame"), UINoticeManager.NoticeType.YesNo);
    }

    private static void OnQuitGame()
    {
        Application.Quit();
    }

    private static void HandleExitWorldMap()
    {
        UINoticeManager.OnButtonOK_click += ExitWorldMap;
        MessageBox.ShowDialog(GameManager.localization.GetText("MainMenu_QuitGame"), UINoticeManager.NoticeType.YesNo);
    }

    private static void ExitWorldMap()
    {
        GameObject go = GameObject.Find("PhotonManager");
        if (go != null)
            go.GetComponent<PhotonManager>().Controller.Disconnect();
        GameScenes.ChangeScense(GameScenes.MyScene.WorldMap, GameScenes.MyScene.MainMenu);
    }
}
