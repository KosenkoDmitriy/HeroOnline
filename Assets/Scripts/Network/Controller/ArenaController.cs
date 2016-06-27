using DEngine.Common;
using DEngine.Common.GameLogic;
using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;
using UnityEngine;
using DEngine.Unity.Photon;

public class ArenaController : PhotonController
{

    private UIArenaManager _uiManager;
    public bool islock;

    public ArenaController(UIArenaManager uiLobbyManager)
        : base()
    {
        _uiManager = uiLobbyManager;
        islock = false;
    }

    public void Disponse()
    {

    }


    public override void OnDisconnect(StatusCode statusCode)
    {
        base.OnDisconnect(statusCode);
        this.HandleDisConnect();
    }

    public void SendRequestReportList()
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.CheckReport);
        SendOperation((byte)OperationCode.UserUpdate, parameters);
    }

    public void SendRequestAcceptBattle(GameUser user)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.Accept);
        parameters.Add((byte)ParameterCode.TargetId, user.Id);
        SendOperation((byte)OperationCode.Battle, parameters);
        //Debug.Log("SendRequestAcceptBattle");
    }

    public void SendQuitArena()
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.Abort);
        SendOperation((byte)OperationCode.Battle, parameters);
        Debug.Log("SendQuitArena");
    }

    public void SendRandomBattle()
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.RandomPvP);
        SendOperation((byte)OperationCode.Battle, parameters);
        Debug.Log("SendRandomBattle");
    }

    public void SendReplay(int id)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.RandomPvA);
        parameters.Add((byte)ParameterCode.TargetId, id);
        SendOperation((byte)OperationCode.Battle, parameters);
        Debug.Log("SendReplay");
    }
      
    public void SendRequestTopList()
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.Param01, (int)UserListType.TopHonor);
        SendOperation((byte)OperationCode.UsersList, parameters);
        Debug.Log("SendRequestTopList");
    }

    public override void OnResponse(OperationResponse response)
    {
        base.OnResponse(response);

        OperationCode opCode = (OperationCode)response.OperationCode;
        ErrorCode errCode = (ErrorCode)response.ReturnCode;
        Dictionary<byte, object> parameter = response.Parameters;

        if (errCode != ErrorCode.Success)
        {
            Debug.Log(string.Format("ResponseReceived, OperationCode = {0}, ReturnCode = {1}, DebugMsg = {2}", opCode, errCode, response.DebugMessage));
            this.HandleErrorCode(errCode);
            return;
        }

        switch (opCode)
        {
            case OperationCode.UserUpdate:
                {
                    SubCode subCode = (SubCode)parameter[(byte)ParameterCode.SubCode];

                    switch (subCode)
                    {
                        case SubCode.CheckReport:
                            HandleReponseCheckReport(response);
                            _uiManager.OnResponseReport();
                            break;
                    }
                }
                break;
            case OperationCode.UsersList:
                OnReciveTopList(response);
                break;            
            default: break;

        }

    }

    public override void OnEvent(EventData eventData)
    {
        base.OnEvent(eventData);

        EventCode evCode = (EventCode)eventData.Code;

        //Debug.Log("OnEvent " + evCode);
        switch (evCode)
        {           

            case EventCode.SendChat:
                string chatMsg = (string)eventData[(byte)ParameterCode.Message];
                GameManager.ReciveChat(chatMsg, eventData);  
                this.HandleNotification(eventData);
                break;

            case EventCode.Battle:
                {
                    if (_uiManager.type == UIArenaManager.Type.PVA)
                    {
                        this.HandleBattle(eventData, GameStatus.PVP);
                    }
                    else
                    {
                        SubCode subCode = (SubCode)eventData[(byte)ParameterCode.SubCode];
                        if (subCode == SubCode.Begin)
                        {
                            OnReciveRandomPVP(eventData);
                        }
                    }
                }
                break;

            case EventCode.PvPSearch:
                this.HandlePvPSearch(eventData);
                break;
        }
    }

    private void OnReciveTopList(OperationResponse response)
    {

        byte[] userData = (byte[])response[(byte)ParameterCode.ZoneUsers];
        GameObjList gameUserMini = Serialization.Load<GameObjList>(userData, true);

        _uiManager.OnReciveListTopUser(gameUserMini);
    }

    private void OnReciveRandomPVP(EventData eventData)
    {
        byte[] rolesData = (byte[])eventData[(byte)ParameterCode.BattleRoles];
        byte[] battleData = (byte[])eventData[(byte)ParameterCode.BattleData];

        GameManager.battleTime = (float)eventData[(byte)ParameterCode.BattleTime];

        GameBattle gameBattle = Serialization.Load<GameBattle>(battleData);

        GameManager.battleType = gameBattle.Mode;

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
              

        GameManager.Status = GameStatus.PVP;

        //GameplayManager.isBattleStart = false;

        _uiManager.OnReciveBattleInfo();

       
    }


    private void HandleReponseCheckReport(OperationResponse response)
    {
        byte[] userPvPLog = (byte[])response[(byte)ParameterCode.UserPvPLog];

        if (userPvPLog != null)
        {
            PvPLog[] pvpLogs = Serialization.LoadArray<PvPLog>(userPvPLog, true);
            if (pvpLogs != null)
            {
                GameManager.pvpLogs.Clear();
                GameManager.pvpLogs.AddRange(pvpLogs);
                GameManager.GameUser.PvPLogs.Clear();
                GameManager.GameUser.PvPLogs.AddRange(pvpLogs);
            }

        }

    }
}
