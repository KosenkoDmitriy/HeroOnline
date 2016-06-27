using DEngine.Common;
using DEngine.Common.GameLogic;
using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;
using UnityEngine;
using DEngine.Unity.Photon;

public class BattleResultController : PhotonController
{

    private UIBattleResultNew _manager;
    public bool islock;

    public BattleResultController(UIBattleResultNew manager)
        : base()
    {
        _manager = manager;
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

    public void SendRequestBattle(int level, int difficulty)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.MissionBegin);
        parameters.Add((byte)ParameterCode.TargetId, level);
        parameters.Add((byte)ParameterCode.RoleId, 0);
        parameters.Add((byte)ParameterCode.Difficulty, difficulty);
        SendOperation((byte)OperationCode.Battle, parameters);

    }

    internal void SendAddFriend(string nickName)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.AddFriend);
        parameters.Add((byte)ParameterCode.FriendMode, 1);//1 = một chiều //2 = hai chiều
        parameters.Add((byte)ParameterCode.FriendName, nickName);
        SendOperation((byte)OperationCode.UserUpdate, parameters);
    }
    
    public override void OnEvent(EventData eventData)
    {
        EventCode evCode = (EventCode)eventData.Code;

//        Debug.Log("evCode " + evCode);
        switch (evCode)
        {
            case EventCode.SendChat:
                string chatMsg = (string)eventData[(byte)ParameterCode.Message];
                GameManager.ReciveChat(chatMsg, eventData);
                break;     
            case EventCode.Battle:
                this.HandleBattle(eventData, GameStatus.Mission);
                break;
        }
    }



    public override void OnResponse(OperationResponse response)
    {
        base.OnResponse(response);

        OperationCode opCode = (OperationCode)response.OperationCode;
        ErrorCode errCode = (ErrorCode)response.ReturnCode;
        Dictionary<byte, object> parameter = response.Parameters;

        if (errCode != ErrorCode.Success)
        {
            this.HandleErrorCode(errCode);
            Debug.Log(string.Format("ResponseReceived, OperationCode = {0}, ReturnCode = {1}, DebugMsg = {2}", opCode, errCode, response.DebugMessage));
            return;
        }


        switch (opCode)
        {
            case OperationCode.UserUpdate:
                {
                    SubCode subCode = (SubCode)parameter[(byte)ParameterCode.SubCode];

                    switch (subCode)
                    {
                        case SubCode.AddFriend:
                            HandleReponseAddFriend(response);
                            break;
                    }
                }
                break;
            default: break;
        }
    }

    private void HandleReponseAddFriend(OperationResponse response)
    {
        _manager.OnResponseAddFriendSuccess();
    }
}
