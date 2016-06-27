using DEngine.Common.GameLogic;
using System.Collections.Generic;
using System.Collections;
using DEngine.Common;
using System.Linq;
using UnityEngine;
using ExitGames.Client.Photon;
using DEngine.Unity.Photon;
using DEngine.Common.Config;
using System;

public enum GameStatus
{
    Start,

    Lab,
    Storage,
    Blacksmith,
    UpStar,
    UpLevel,
    Hero,
    Quest,
    Arena,
    Social,
    Shop,  
    ShopHero,
 
    Mission,
    Dungeon,
    PVP,
    PVA,
    Pillage
}



public static class GameManager
{
    public static Guid ZoneID;
    public static string PlayerTagName = "Player";
    public static string EnemyTagName = "Enemy";
    public static bool logined;
    public static string LastError;
    public static GameStatus Status;
    public static GameObjCollection GameHeroes;
    public static GameObjCollection GameItems;
    public static GameObjCollection GameSkills;
    public static GameObjCollection ChargeShop;
    public static List<GameZone> GameZones;
    public static Dictionary<int, int> itemReward;

    public static int slotHeroCountLevel1 = 6;
    public static int maxSlotHero
    {
        get { return slotHeroCountLevel1 + GameUser.Land.Houses.Count(p => p.HouseId == 4) * 2; }//GameUser.Base.Level +
    }

    public static GameUser GameUser;
    public static GameUser EnemyUser;
    public static bool IsInviter;
    public static MyLocalization localization;
    public static List<string> oldChat = new List<string>();
    public static BattleMode battleType;
    public static float battleTime;
    public static int maxSlotInventory = 30;
    public static int dungeonEventCount = 0;
    public static int dungeonCurEvent = 0;
    public static List<PvALog> pvaLogs;
    public static List<PvPLog> pvpLogs;
    public static List<UIReport> uiReport;
    public static TutorialManager tutorial;
    public static UserRole CurRoleSelectedUpGrade;
    public static AutoSkillConfig autoSkillConfig;
    public static bool takeLoginReward;
    public static bool viewedIntro = false;
    public static string serverName = "";
    public static int curMission = 0;

    static GameManager()
    {

    }

    public static void Init()
    {
        takeLoginReward = false;
        oldChat = new List<string>();
        logined = false;
        IsInviter = false;
        Status = GameStatus.Start;
        GameZones = new List<GameZone>();
        localization = new MyLocalization();
        battleType = BattleMode.None;
        itemReward = new Dictionary<int, int>();
        pvaLogs = new List<PvALog>();
        pvpLogs = new List<PvPLog>();
        uiReport = new List<UIReport>();
        dungeonCurEvent = 0;
        dungeonEventCount = 0;
        battleTime = 0;
        tutorial = new TutorialManager();
        autoSkillConfig = new AutoSkillConfig();
    }

    public static void InitReport()
    {
        uiReport.Clear();
        foreach (PvALog log in pvaLogs)
        {
            UIReport report = new UIReport()
            {
                id = log.LogId,
                type = 0,
                nickName = log.Opponent.Base.NickName,
                time = log.LogTime.ToString(localization.GetText("Arena_LastMatchFormatDate")),
                where = localization.GetText("WorldMap_Pillage"),
            };

            if (log.Result < 0)
            {
                report.status = string.Format("[00FF00]{0}[-]", GameManager.localization.GetText("Dialog_Battle_Win"));
            }
            else if (log.Result == 0)
            {
                report.status = GameManager.localization.GetText("Dialog_Battle_Draw");
            }
            else
            {
                report.status = string.Format("[FF0000]{0}[-]", GameManager.localization.GetText("Dialog_Battle_Lose"));
            }
            uiReport.Add(report);

        }

        foreach (PvPLog log in pvpLogs)
        {
            UIReport report = new UIReport()
            {
                id = log.LogId,
                type = 1,
                nickName = log.Opponent.Base.NickName,
                time = log.LogTime.ToString(localization.GetText("Arena_LastMatchFormatDate")),
                where = localization.GetText("WorldMap_Arena"),
            };
            if (log.Result < 0)
            {
                report.status = string.Format("[00FF00]{0}[-]", GameManager.localization.GetText("Dialog_Battle_Win"));
            }
            else if (log.Result == 0)
            {
                report.status = GameManager.localization.GetText("Dialog_Battle_Draw");
            }
            else
            {
                report.status = string.Format("[FF0000]{0}[-]", GameManager.localization.GetText("Dialog_Battle_Lose"));
            }
            uiReport.Add(report);

        }
    }

    public static void ReciveChat(string message, EventData eventData)
    {
        int idSender = (int)eventData[(byte)ParameterCode.UserId];

        if (idSender == 0)
        {
            message = string.Format(GameManager.localization.GetText("SystemMessage_System"), getFormatMessageSystem(message));
        }

        if (oldChat.Count > 30)
            oldChat.RemoveAt(0);
        oldChat.Add(message);
    }

    public static void EndBattle()
    {
        IsInviter = false;
        GameplayManager.allRoles.Clear();
        GameplayManager.playerRoles.Clear();
        GameplayManager.enemyRoles.Clear();
    }

    public static void InitGameUser(byte[] objData)
    {
        GameManager.GameUser = Serialization.Load<GameUser>(objData, true);
        GameManager.GameUser.Tag = new GameUserClient();
        foreach (UserItem userItem in GameManager.GameUser.UserItems)
        {
            userItem.GameItem = (GameItem)GameManager.GameItems[userItem.ItemId];
        }

        foreach (UserRole role in GameManager.GameUser.UserRoles)
        {
            role.GameRole = (GameRole)GameManager.GameHeroes[role.Base.RoleId];

            role.RoleItems.Clear();
            var roleItems = GameManager.GameUser.UserItems.Where(p => p.RoleUId == role.Id);
            role.RoleItems.AddRange(roleItems);


            for (int i = 0; i < role.RoleSkills.Count; i++)
                role.RoleSkills[i].GameSkill = (GameSkill)GameManager.GameSkills[role.RoleSkills[i].SkillId];
        }

    }

    #region BattleHandle
    public static void HandleBattle(this PhotonController controller, EventData eventData, GameStatus type)
    {
        SubCode subCode = (SubCode)eventData[(byte)ParameterCode.SubCode];

        //Debug.Log("subCode " + subCode);
        switch (subCode)
        {
            case SubCode.Invite:
                {
                    //int senderID = (int)eventData[(byte)ParameterCode.UserId];

                    //GameUser inviterBattle = (GameUser)GameManager.ZoneUsers[senderID];
                    //UIInviterManager.Instance.AddInviter(inviterBattle);
                }
                break;
            case SubCode.Refuse:
                {
                    MessageBox.CloseDialog();
                    MessageBox.ShowDialog(GameManager.localization.GetText("Dialog_Refuse"), UINoticeManager.NoticeType.Message);
                }
                break;
            case SubCode.Begin:
                {
                    GameplayManager.battleStatus = GameplayManager.BattleStatus.Start;

                    byte[] rolesData = (byte[])eventData[(byte)ParameterCode.BattleRoles];
                    byte[] battleData = (byte[])eventData[(byte)ParameterCode.BattleData];

                    battleTime = (float)eventData[(byte)ParameterCode.BattleTime];



                    GameBattle gameBattle = Serialization.Load<GameBattle>(battleData);


                    if (gameBattle.User01.Id != GameManager.GameUser.Id)
                    {
                        GameManager.EnemyUser = gameBattle.User01;
                        GameManager.GameUser.Position = gameBattle.User02.Position;
                    }
                    else
                    {
                        GameManager.GameUser.Position = gameBattle.User01.Position;
                        GameManager.EnemyUser = gameBattle.User02;
                    }

                    GameObjCollection battleRoles = Serialization.Load<GameObjCollection>(rolesData, true);

                    GameplayManager.InitRoles(battleRoles);

                    MessageBox.CloseDialog();

                    GameScenes.ChangeScense(GameScenes.previousSence, GameScenes.MyScene.Battle);

                    GameManager.Status = type;
                    battleType = gameBattle.Mode;
                }
                break;

        }
    }

    public static void SendRequestCancelBattle(this PhotonController controller, GameUser user)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.Refuse);
        parameters.Add((byte)ParameterCode.TargetId, user.Id);
        controller.SendOperation((byte)OperationCode.Battle, parameters);
        //Debug.Log("SendRequestCancelBattle");
    }

    public static void SendRequestAcceptBattle(this PhotonController controller, GameUser user)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.Accept);
        parameters.Add((byte)ParameterCode.TargetId, user.Id);
        controller.SendOperation((byte)OperationCode.Battle, parameters);
        //Debug.Log("SendRequestAcceptBattle " + user.Id);
    }

    public static void HandleErrorCode(this PhotonController controller, ErrorCode errCode)
    {
        switch (errCode)
        {
            case ErrorCode.CashInsufficient:
                Helper.HandleCashInsufficient();
                break;
            case ErrorCode.DuplicateLogin:
                MessageBox.ShowDialog(GameManager.localization.GetText("Login_DuplicateLogin"), UINoticeManager.NoticeType.Message);
                break;
            case ErrorCode.DuplicateUserName:
                MessageBox.ShowDialog(GameManager.localization.GetText("Register_ExitsUsername"), UINoticeManager.NoticeType.Message);
                break;
            case ErrorCode.DuplicateNickName:
                MessageBox.ShowDialog(GameManager.localization.GetText("Register_ExitsNickName"), UINoticeManager.NoticeType.Message);
                break;
            case ErrorCode.EnergyInsufficient:
                MessageBox.ShowDialog(GameManager.localization.GetText("ErrorCode_NotEnoghtEnery"), UINoticeManager.NoticeType.Message);
                break;
            case ErrorCode.TargetNotReady://Trong tran, chua chon tuong
                MessageBox.ShowDialog(GameManager.localization.GetText("Dialog_TargetNotReady"), UINoticeManager.NoticeType.Message);
                break;
            case ErrorCode.UserNotReady://Trong tran, chua chon tuong               
                MessageBox.ShowDialog(GameManager.localization.GetText("ErrorCode_NotEnough3Hero"), UINoticeManager.NoticeType.Message);
                break;
            case ErrorCode.TargetNotFound:
                break;
            case ErrorCode.InvalidPassword:
                MessageBox.ShowDialog(GameManager.localization.GetText("Login_IncorrectUserNamePass"), UINoticeManager.NoticeType.Message);
                break;
            case ErrorCode.UserLevelNotEnough:
                if (GameManager.Status == GameStatus.Dungeon)
                    MessageBox.ShowDialog(GameManager.localization.GetText("Dungeon_NotLevel"), UINoticeManager.NoticeType.Message);
                else if (GameManager.Status == GameStatus.Arena)
                    MessageBox.ShowDialog(GameManager.localization.GetText("Arena_NotLevel"), UINoticeManager.NoticeType.Message);
                else
                {
                    int levelPillage = GameConfig.PILLAGELEVEL;
                    MessageBox.ShowDialog(string.Format(GameManager.localization.GetText("Pillage_NotLevel"), levelPillage), UINoticeManager.NoticeType.Message);
                }
                break;
            case ErrorCode.MobileCardChargeFailed:
                MessageBox.ShowDialog(GameManager.localization.GetText("ErrorCode_MobileCardChargeFailed"), UINoticeManager.NoticeType.Message);
                break;
            case ErrorCode.InvalidParams:
            case ErrorCode.ItemNotAvailable:
            case ErrorCode.ItemsInsufficient:
            case ErrorCode.OperationDedined:
            case ErrorCode.RoleInBattle:
            case ErrorCode.RoleIsDeath:
            case ErrorCode.RoleNotFound:
            case ErrorCode.RoleNotReady:
            case ErrorCode.SkillHPInsufficient:
            case ErrorCode.SkillInvalidCost:
            case ErrorCode.SkillInvalidTarget:
            case ErrorCode.SkillMPInsufficient:
            case ErrorCode.SkillNotAvaiable:
            case ErrorCode.TargetNotAvaiable:
            case ErrorCode.UserInBattle:
                break;                
            default:
                MessageBox.ShowDialog(errCode.ToString(), UINoticeManager.NoticeType.Message);
                break;
        }
    }

    public static bool CheckHeroToBattle()
    {
        //requires three heroes are selected
        if (GameManager.GameUser.UserRoles.Count(p => p.Base.Status == RoleStatus.Active) < 3)
        {
            MessageBox.ShowDialog(GameManager.localization.GetText("ErrorCode_NotEnough3Hero"), UINoticeManager.NoticeType.Message);
            return false;
        }
        //check require enery for each hero 
        if (GameManager.GameUser.UserRoles.Count(p => p.Base.Status == RoleStatus.Active && p.Base.Energy >= RoleConfig.ENERGY_MIN) < 3)
        {
            MessageBox.ShowDialog(GameManager.localization.GetText("ErrorCode_NotEnoghtEnery"), UINoticeManager.NoticeType.Message);
            return false;
        }

        return true;
    }

    #endregion

    #region Hanle User Zone
    public static GameUser HandleZoneEnter(this PhotonController controller, EventData eventData)
    {
        int userId = (int)eventData[(byte)ParameterCode.UserId];
        // string userInfo = (string)eventData[(byte)ParameterCode.UserInfo];
        GameUser userMini = new GameUser() { Id = userId };
        userMini.Tag = new GameUserClient();
        //userMini.InitBase(userInfo);
        //GameManager.OnUserEnter(userMini);
        return userMini;
    }

    public static int HandleZoneExit(this PhotonController controller, EventData eventData)
    {
        int userId = (int)eventData[(byte)ParameterCode.UserId];
        //GameManager.OnUserLeave(userId);
        return userId;
    }
    #endregion

    #region HandleDisConnect
    public static void HandleDisConnect(this PhotonController controller)
    {
        UINoticeManager.OnButtonOK_click += new UINoticeManager.NoticeHandle(OnButtionOkClick);

        MessageBox.ShowDialog(GameManager.localization.GetText("Dialog_DisConnect"), UINoticeManager.NoticeType.Message);

    }
    private static void OnButtionOkClick()
    {
        GameScenes.ChangeScense(GameScenes.currentSence, GameScenes.MyScene.MainMenu);
    }
    #endregion

    #region CheckMail
    public static void HandleReponseCheckMail(this PhotonController controller, OperationResponse response)
    {
        byte[] mails = (byte[])response[(byte)ParameterCode.UserMails];
        if (mails == null) return;

        UserMail[] userMail = Serialization.LoadArray<UserMail>(mails, true);

        if (userMail == null) return;
        GameManager.GameUser.UserMails.Clear();
        GameManager.GameUser.UserMails.AddRange(userMail);

    }
    #endregion

    public static void HandlePvPSearch(this PhotonController controller, EventData eventData)
    {
        //string userName = (string)eventData[(byte)ParameterCode.UserName];

        if (UIPVPSearchManager.Instance != null) return;

        GameObject prefab = Resources.Load("Prefabs/UI/PVPSearch") as GameObject;

        GameObject.Instantiate(prefab);
    }

    public static void HandleNotification(this PhotonController controller, EventData eventData)
    {
        string chatMsg = (string)eventData[(byte)ParameterCode.Message];
        int idSender = (int)eventData[(byte)ParameterCode.UserId];

        //SubCode subCode = (SubCode)eventData[(byte)ParameterCode.ev];
        bool system = false;
        if (idSender == 0)
        {
            chatMsg = getFormatMessageSystem(chatMsg);
            if (chatMsg != "")
                system = true;
        }
        else
        {
            if (idSender == GameManager.GameUser.Id)
                return;
        }

        if (UINotificationManager.Instance == null)
        {
            GameObject prefab = Resources.Load("Prefabs/UI/UINotification") as GameObject;
            GameObject go = GameObject.Instantiate(prefab) as GameObject;

            UINotificationManager.Instance = go.GetComponent<UINotificationManager>();
        }

        if (chatMsg != "")
        {
            UINotificationManager.Instance.AddText(chatMsg, system);
            
        }
    }

    public static string getFormatMessageSystem(string chatMsg)
    {
        string s = "";
        string[] messageParam = chatMsg.Split('\n');
        try
        {
            if (messageParam.Length > 0)
            {
                int worlMessageID = int.Parse(messageParam[0]);
                WorldMessage messageType = (WorldMessage)worlMessageID;

                switch (messageType)
                {
                    case WorldMessage.Sommon:
                        s = GameManager.localization.GetText("SystemMessage_BuyHero");
                        s = string.Format(s, messageParam[1], messageParam[2], messageParam[3]);
                        break;

                    case WorldMessage.Dungeon:
                        s = GameManager.localization.GetText("SystemMessage_Dungeon");
                        s = string.Format(s, messageParam[1]);
                        break;

                    case WorldMessage.PillageFree:
                        s = GameManager.localization.GetText("SystemMessage_PillageFree");
                        s = string.Format(s, messageParam[1], messageParam[2]);
                        break;

                    case WorldMessage.PillageSlave:
                        s = GameManager.localization.GetText("SystemMessage_PillageSlave");
                        s = string.Format(s, messageParam[1], messageParam[2]);
                        break;

                    case WorldMessage.Mission:
                        s = GameManager.localization.GetText("SystemMessage_Mission");
                        string hard = "";
                        int param3 = int.Parse(messageParam[3]);
                        if (param3 == 1)
                            hard = GameManager.localization.GetText("PVE_Mission_Standard_Easy");
                        if (param3 == 2)
                            hard = GameManager.localization.GetText("PVE_Mission_Standard_Normal");
                        if (param3 == 3)
                            hard = GameManager.localization.GetText("PVE_Mission_Standard_Hard");

                        s = string.Format(s, messageParam[1], messageParam[2], hard);
                        break;

                    case WorldMessage.RoleUpgrade:
                        s = GameManager.localization.GetText("SystemMessage_Upstart");
                        s = string.Format(s, messageParam[1], messageParam[2], messageParam[3]);
                        break;

                }
            }
        }
        catch
        {
            s = "";
        }
        return s;
    }

    public static void HandleReponseGetFriendList(this PhotonController controller, OperationResponse response)
    {
        byte[] friends = (byte[])response[(byte)ParameterCode.Friends];

        UserFriend[] userFriends = Serialization.LoadArray<UserFriend>(friends, true);

        if (userFriends == null) return;
        GameManager.GameUser.UserFriends.Clear();
        GameManager.GameUser.UserFriends.AddRange(userFriends);

    }

    public static void SendSaveTutorialStep(this PhotonController controller)
    {        
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.TutorStep); 
        parameters.Add((byte)ParameterCode.TargetId, GameManager.GameUser.Base.TutorStep);
        controller.SendOperation((byte)OperationCode.UserUpdate, parameters);
        Debug.Log("SendSaveTutorialStep " + (int)GameManager.GameUser.Base.TutorStep);

    }
    
}
