using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DEngine.Common;
using ExitGames.Client.Photon;
using DEngine.Common.GameLogic;
using DEngine.Unity.Photon;
using System.Linq;
using DEngine.Common.Config;

public class BattleContoller : PhotonController
{

    GameplayManager _gamePlayManager;

    private int ReviveCount = 0;

    public BattleContoller(GameplayManager gamePlayManager)
        : base()
    {
        _gamePlayManager = gamePlayManager;
        ReviveCount = 0;
    }

    public void Disponse()
    {

    }

    public void SendBattleReady()
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.Ready);
        SendOperation((byte)OperationCode.Battle, parameters);
        // Debug.Log("SendQuit ");
    }

    public override void OnDisconnect(StatusCode statusCode)
    {
        base.OnDisconnect(statusCode);
        this.HandleDisConnect();
    }

    public void SendQuit()
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.Quit);
        SendOperation((byte)OperationCode.Battle, parameters);
       // Debug.Log("SendQuit ");
    }

    //index not used by old version
    public void SendMove(int roleID, int index, float[] targetPos)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.Move);
        parameters.Add((byte)ParameterCode.RoleId, roleID);
        parameters.Add((byte)ParameterCode.TargetPos, targetPos);
        SendOperation((byte)OperationCode.Battle, parameters);
        //Debug.Log("SendMove " + roleID);
    }

    //roleIndex not used by old version
    public void SendAction(int roleID, int roleIndex, int targetID)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.Action);
        parameters.Add((byte)ParameterCode.RoleId, roleID);
        parameters.Add((byte)ParameterCode.TargetId, targetID);
        SendOperation((byte)OperationCode.Battle, parameters);
       // Debug.Log("SendAction " + roleID + " " + targetID);
    }

    public void SendSkillCast(int roleID, int SkillID)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.SkillCast);
        parameters.Add((byte)ParameterCode.SkillId, SkillID);
        parameters.Add((byte)ParameterCode.RoleId, roleID);
        SendOperation((byte)OperationCode.Battle, parameters);
       // Debug.Log("SendSkillCast " + roleID + " " + SkillID);
    }

    public void SendSkillHit(int roleID, int roleIndex, int targetID, int SkillID)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.SkillHit);
        parameters.Add((byte)ParameterCode.RoleId, roleID);
        parameters.Add((byte)ParameterCode.TargetId, targetID);
        parameters.Add((byte)ParameterCode.SkillId, SkillID);
        SendOperation((byte)OperationCode.Battle, parameters);
        //Debug.Log("SendSkillHit " + roleID + " " + SkillID);
        //Debug.Log("SendSkillHit " + SkillID);
    }

    public void SendUseItem(int roleID, int itemID)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.ItemEat);
        parameters.Add((byte)ParameterCode.RoleId, roleID);
        parameters.Add((byte)ParameterCode.TargetId, itemID);
        SendOperation((byte)OperationCode.Battle, parameters);
        //Debug.Log("SendSkillHit " + roleID + " " + SkillID);
        //Debug.Log("SendSkillHit " + SkillID);
    }

    public void SendRequestBattle(int level, int difficulty)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.MissionBegin);
        parameters.Add((byte)ParameterCode.TargetId, level);
        parameters.Add((byte)ParameterCode.Difficulty, difficulty);
        SendOperation((byte)OperationCode.Battle, parameters);

        //Debug.Log("SendRequestInviteToBattle " + level);
    }

    public void SendResumeBattle(int type)
    {
        //0=thua
        //1=Revive
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.MissionResume);
        parameters.Add((byte)ParameterCode.TargetId, type);
        SendOperation((byte)OperationCode.Battle, parameters);
        Debug.Log("SendResumeBattle " + type);
    }

    public override void OnResponse(OperationResponse response)
    {
        base.OnResponse(response);

        OperationCode opCode = (OperationCode)response.OperationCode;
        ErrorCode errCode = (ErrorCode)response.ReturnCode;
        Dictionary<byte, object> parameter = response.Parameters;

        SubCode subCode = SubCode.Abort;
        if (parameter.ContainsKey((byte)ParameterCode.SubCode))
            subCode = (SubCode)parameter[(byte)ParameterCode.SubCode];

        if (errCode != ErrorCode.Success)
        {
            Debug.Log("errCode" + errCode + " - opCode " + opCode + " subCode " + subCode);
            if (errCode == ErrorCode.BattleNotAvailable)
            {
                HandleEnemyQuitBattle();
            }
            else
            {
                this.HandleErrorCode(errCode);
            }
            return;
        }

        switch (opCode)
        {
            case OperationCode.Battle:
                {                    
                    switch (subCode)
                    {
                        case SubCode.MissionResume:
                            Debug.Log("MissionResume");
                            OnResponseRevive(response);
                            break;
                    }
                }
                break;
        }


    
    }

    public override void OnEvent(EventData eventData)
    {
        EventCode evCode = (EventCode)eventData.Code;
        //Debug.Log("evCode " + evCode);
        switch (evCode)
        {
            case EventCode.UserSync:
                HandleUserSync(eventData);
                Debug.Log("UserSync");
                break;
           
            case EventCode.Battle:
                SubCode subCode = (SubCode)eventData.Parameters[(byte)ParameterCode.SubCode];
                switch (subCode)
                {
                    //dau tiep mission
                    case SubCode.MissionResume:
                        Debug.Log("MissionResume");
                        OnResponseRevive(null);
                        break;
                    case SubCode.MissionEnd:
                        Debug.Log("MissionEnd");
                        OnResponseMissionEnd(eventData);
                        break;
                    case SubCode.Ready:
                        Debug.Log("Ready");
                        HandleBattleReady();
                        break;
                    case SubCode.Move:
                        HandleReciveTarget(eventData);
                        break;
                    case SubCode.Action:
                        HanleReciveAction(eventData);
                        break;
                    case SubCode.SkillCast:
                        HandleSkillCast(eventData);
                        break;
                    case SubCode.End:
                        Debug.Log("End");
                        HandleBattleEnd(eventData);
                        break;
                    case SubCode.SkillMiss:
                        HandleSkillMiss(eventData);
                        break;
                    case SubCode.SkillCrit:
                        HandleSkillCrit(eventData);
                        break;                  
                    case SubCode.SkillEvas:
                        HandleSkillEvas(eventData);
                        break;
                    case SubCode.MobCreate:
                        HandleMobCreate(eventData);
                        break;
                    case SubCode.ItemDrop:
                        HandleItemDrop(eventData);
                        break;
                }
                break;
            case EventCode.RoleSync:
               // HandleSyncTransform(eventData);
                break;
            case EventCode.BattleSync:
                HandleBattleSync(eventData);
                break;
            case EventCode.SendChat:
                string chatMsg = (string)eventData[(byte)ParameterCode.Message];
                GameManager.ReciveChat(chatMsg, eventData);
                this.HandleNotification(eventData);
                break;
        }
    }

    private void HandleBattleReady()
    {
        if (GameManager.battleType == BattleMode.Challenge || GameManager.battleType == BattleMode.RandomPvA || GameManager.battleType == BattleMode.RandomPvP)
        {
            GameplayManager.battleStatus = GameplayManager.BattleStatus.Begin;
        }
        else
        {
            GameplayManager.battleStatus = GameplayManager.BattleStatus.Playing;
        }  
    }

    private void HandleItemDrop(EventData eventData)
    {
        int itemGrade = (int)eventData.Parameters[(byte)ParameterCode.ItemData];
        //int ItemKind = (int)eventData.Parameters[(byte)ParameterCode.ItemKind];
        float[] targetPos = (float[])eventData.Parameters[(byte)ParameterCode.TargetPos];
        Vector3 pos = new Vector3(targetPos[0], targetPos[1], targetPos[2]);
        if (itemGrade == 1)
        {
            UIChestManager.Instance.CreateDropChest(1, pos);
        }
        else if (itemGrade == 2)
        {
            UIChestManager.Instance.CreateDropChest(2, pos);
        }
        else if (itemGrade == 3)
        {
            UIChestManager.Instance.CreateDropChest(3, pos);
        }
    }


    //create mob for wave
    private void HandleMobCreate(EventData eventData)
    {
        byte[] roleData = (byte[])eventData.Parameters[(byte)ParameterCode.BattleMobs];
        UserRole[] mobs = Serialization.LoadArray<UserRole>(roleData, true);
        int wave = (int)eventData.Parameters[(byte)ParameterCode.Param01];
        int maxWave = (int)eventData.Parameters[(byte)ParameterCode.Param02];

        // Debug.Log("HandleMobCreate " + mobs.Length);
        foreach (UserRole mob in mobs)
        {
            for (int i = 0; i < mob.RoleSkills.Count; i++)
            {
                //Debug.Log(i + " " + mob.RoleSkills[i].Name + " " + mob.RoleSkills[i].SkillId);
                mob.RoleSkills[i].GameSkill = (GameSkill)GameManager.GameSkills[mob.RoleSkills[i].SkillId];
                // Debug.Log("game skill " + mob.RoleSkills[i].GameSkill);
            }
            //mob.Base.RoleId =character type , get base gamerole  in game by mob.Base.RoleId
            mob.GameRole = (GameRole)GameManager.GameHeroes[mob.Base.RoleId];
        }

        UIBattleManager.Instance.lblWave.text = string.Format("Wave: {0}/{1}", wave + 1, maxWave);
        _gamePlayManager.mobsNextwave.Clear();
        _gamePlayManager.mobsNextwave.AddRange(mobs);

        if (GameManager.curMission == 10)//not stop gameplay
        {
            GameplayManager.Instance.StartCoroutine(GameplayManager.Instance.CreateMobsNextWave());
        }
        else
        {
            //stop update gameplay
            GameplayManager.Instance.PauseGame();

            if (wave == 0)
            {
                UIWaveManager.Instance.Show();
            }
            else
            {
                if (GameManager.Status == GameStatus.Mission)
                {
                    UIWaveManager.Instance.Show();
                }
                else
                {
                    _gamePlayManager.MoveAllHeroToStartPos();
                }
            }
        }

    }
    
    private void HandleUserSync(EventData eventData)
    {
        byte[] gameAward = (byte[])eventData.Parameters[(byte)ParameterCode.GameAward];
        byte[] userData = (byte[])eventData.Parameters[(byte)ParameterCode.UserData];

        int userLevelOld = GameManager.GameUser.Base.Level;

        GameManager.InitGameUser(userData);       

        if (gameAward != null)
        {
            GameAward award = Serialization.LoadStruct<GameAward>(gameAward);

            GameManager.itemReward = award.Items;

            UIBattleManager.Instance.battleResult.battleAward = new UIBattleResultNew.BattleAward()
            {
                name = GameManager.GameUser.Base.NickName,
                level = GameManager.GameUser.Base.Level,
                Exp = award.UserExp,
                silver = award.Silver,
                gold = award.Gold,
                honor = award.Honor,
                HeroEXP = award.RoleExp,
                EndDungeon = false,
                silverEx = award.SilverEx
            };


            if (award.Silver < 0)
            {
                UIBattleManager.Instance.battleResult.battleAward.silverSign = -1;
            }
            else
            {
                UIBattleManager.Instance.battleResult.battleAward.silverSign = 1;
            }
        }

        if (GameManager.GameUser.Base.Level > userLevelOld)
        {
            UIBattleManager.Instance.battleResult.battleAward.isLevelUp = true;
        }

        //Debug.Log("Honor = " + UIBattleManager.Instance.battleResult.userResult.honor);   
    }
     
    //
    private void HandleSkillCrit(EventData eventData)
    {

        int roleUID = (int)eventData.Parameters[(byte)ParameterCode.TargetId];
        _gamePlayManager.OnReciveSkillCrit(roleUID);

    }

    private void HandleSkillMiss(EventData eventData)
    {
        int roleUID = (int)eventData.Parameters[(byte)ParameterCode.TargetId];
        _gamePlayManager.OnRecivedSkillMiss(roleUID);
    }

    private void HandleBattleEnd(EventData eventData)
    {
        int gameUser1 = (int)eventData.Parameters[(byte)ParameterCode.Param01];
        int gameUser2 = (int)eventData.Parameters[(byte)ParameterCode.Param02];
        int battleResult = (int)eventData.Parameters[(byte)ParameterCode.BattleRes];
              

        if (battleResult == 0 && (gameUser1 == GameManager.GameUser.Id || gameUser2 == GameManager.GameUser.Id))
        {
            GameplayManager.battleStatus = GameplayManager.BattleStatus.Draw;
            _gamePlayManager.StartCoroutine(_gamePlayManager.OnDrawBattle());
        }
        
        int idWin = battleResult < 0 ? gameUser1 : gameUser2;

        int idLose = battleResult > 0 ? gameUser1 : gameUser2;



        if (idWin == GameManager.GameUser.Id)
        {
            GameplayManager.battleStatus = GameplayManager.BattleStatus.Win;
            _gamePlayManager.MoveAllHeroToStartPos();
        }
        else if (idLose == GameManager.GameUser.Id)
        {
            if (GameManager.battleType == BattleMode.RandomPvE && GameManager.Status != GameStatus.Dungeon)
            {
                _gamePlayManager.StartCoroutine(HandleLoseBattlePVE());
            }
            else
            {
                _gamePlayManager.StartCoroutine(_gamePlayManager.OnLoseBattle());
            }
        }

    }
    
    private void HandleBattleSync(EventData eventData)
    {
       
        //Debug.Log("HandleBattleSync");
        byte[] data = (byte[])eventData.Parameters[(byte)ParameterCode.RoleState];
        UserRole.RoleState[] roleStates = Serialization.LoadStructArray<UserRole.RoleState>(data);
        float battleTime = (float)eventData.Parameters[(byte)ParameterCode.BattleTime];

        if (roleStates != null)
            _gamePlayManager.OnReciveBattleSync(roleStates, battleTime);


       /* if (GameManager.battleType == BattleMode.RandomPvE)
        {

        }*/
    }

    private void HandleSkillCast(EventData eventData)
    {
        int userID = (int)eventData.Parameters[(byte)ParameterCode.UserId];
        int RoleId = (int)eventData.Parameters[(byte)ParameterCode.RoleId];
        int skillID = (int)eventData.Parameters[(byte)ParameterCode.SkillId];
        _gamePlayManager.OnReciveSkillCast(userID, RoleId, skillID);
    }

    private void HanleReciveAction(EventData eventData)
    {
        int userID = (int)eventData.Parameters[(byte)ParameterCode.UserId];
        int RoleId = (int)eventData.Parameters[(byte)ParameterCode.RoleId];
        int targetID = (int)eventData.Parameters[(byte)ParameterCode.TargetId];
        _gamePlayManager.OnRecivedAction(userID, RoleId, targetID);
    }

    private void HandleReciveTarget(EventData eventData)
    {
        int userID = (int)eventData.Parameters[(byte)ParameterCode.UserId];
        int RoleId = (int)eventData.Parameters[(byte)ParameterCode.RoleId];
        float[] targetPos = (float[])eventData.Parameters[(byte)ParameterCode.TargetPos];
        _gamePlayManager.OnRecivedTarget(userID, RoleId, targetPos);
    }

    //not used
    private void HandleSyncTransform(EventData eventData)
    {
        int roleId = (int)eventData.Parameters[(byte)ParameterCode.RoleId];
        UserRole.RoleState roleState = Serialization.LoadStruct<UserRole.RoleState>((byte[])eventData.Parameters[(byte)ParameterCode.RoleState]);
        _gamePlayManager.OnSyncTransform(roleId, roleState.CurrentPos);
    }

    private void HandleSkillEvas(EventData eventData)
    {
        int roleUID = (int)eventData.Parameters[(byte)ParameterCode.TargetId];
        _gamePlayManager.OnRecivedSkillEvas(roleUID);
    }

    private void HandleEnemyQuitBattle()
    {
        UINoticeManager.OnButtonOK_click += ExitBattle;
        MessageBox.ShowDialog(GameManager.localization.GetText("ErrorCode_BattleNotAvailable"), UINoticeManager.NoticeType.Message);
    }

    private void ExitBattle()
    {
        GameScenes.ChangeScense(GameScenes.MyScene.Battle, GameScenes.MyScene.Arena);
    }

    #region HandleBattleResume

    //server hoi co resume game ko ?
    private IEnumerator HandleLoseBattlePVE()
    {
        Debug.Log("HandleLoseBattlePVE");
        yield return new WaitForSeconds(4f);
        GameplayManager.battleStatus = GameplayManager.BattleStatus.Pause;
        int goldForRevive = 5;
        if (GameManager.GameUser.Base.Gold >= goldForRevive && ReviveCount < 3)
        {
            UINoticeManager.OnButtonOK_click += OnAcceptRevive;
            UINoticeManager.OnButtonCancel_click += OnCloseRevive;
            MessageBox.ShowDialog(string.Format(GameManager.localization.GetText("BatlteResult_LoseAndRevive"), goldForRevive), UINoticeManager.NoticeType.YesNo);
        }
        else
        {
            SendResumeBattle(0);
            _gamePlayManager.StartCoroutine(_gamePlayManager.OnLoseBattle());
        }
    }
    private void OnAcceptRevive()
    {
        SendResumeBattle(1);
    }
    private void OnCloseRevive()
    {
        SendResumeBattle(0);
    }
    private void OnResponseMissionEnd(EventData eventData)
    {//>0 thua
        int param = (int)eventData[(byte)ParameterCode.BattleRes];
        Debug.Log("OnResponseMissionEnd " + param);
        if (param > 0)
            _gamePlayManager.StartCoroutine(_gamePlayManager.OnLoseBattle());
    }
    private void OnResponseRevive(OperationResponse  response)
    {
        GameplayManager.battleStatus = GameplayManager.BattleStatus.Playing;
        _gamePlayManager.ReviveAlHero();
        ReviveCount++;
    }
    #endregion
}
