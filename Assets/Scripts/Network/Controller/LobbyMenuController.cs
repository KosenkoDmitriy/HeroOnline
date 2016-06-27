using DEngine.Common;
using DEngine.Common.GameLogic;
using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;
using UnityEngine;
using DEngine.Unity.Photon;

public class LobbyMenuController : PhotonController
{
   
    private GameUser _inviterBattle;
    private UILobbyManager _uiLobbyManager;
    public bool islock;

    public LobbyMenuController(UILobbyManager uiLobbyManager)
        : base()
    {
        _uiLobbyManager = uiLobbyManager;
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
    public void SendZonesLeave()
    {
       // SendOperation((byte)OperationCode.ZoneLeave);
        //Debug.Log("SendZonesLeave ");
    }

    public void SendChat(int targetId, string chatmsg)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.TargetId, targetId);
        parameters.Add((byte)ParameterCode.Message, chatmsg);
        SendOperation((byte)OperationCode.SendChat, parameters);
        //Debug.Log("SendChat " + chatmsg);
    }

    public void SendRequestInviteToBattle(GameUser user)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.Invite);
        parameters.Add((byte)ParameterCode.TargetId, user.Id);
        SendOperation((byte)OperationCode.Battle, parameters);
        GameManager.IsInviter = true;
        //Debug.Log("SendRequestInviteToBattle");
    }

    public void SendRequestCancelBattle(GameUser user)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.Refuse);
        parameters.Add((byte)ParameterCode.TargetId, user.Id);
        SendOperation((byte)OperationCode.Battle, parameters);
        //Debug.Log("SendRequestCancelBattle");
    }

    public void SendRequestAcceptBattle(GameUser user)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.Accept);
        parameters.Add((byte)ParameterCode.TargetId, user.Id);
        SendOperation((byte)OperationCode.Battle, parameters);
        //Debug.Log("SendRequestAcceptBattle");
    }

    public void SendRandomBattle()
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.RandomPvP);
        SendOperation((byte)OperationCode.Battle, parameters);
        //Debug.Log("SendRandomBattle");
    }

    public void SendChangeAvatar(int avatarID)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.UserAvatar);
        parameters.Add((byte)ParameterCode.AvatarId,avatarID);
        SendOperation((byte)OperationCode.UserUpdate, parameters);
        //Debug.Log("SendChangeAvatar");
    }
  

    public void SendPVAMode()
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.RandomPvA);
        SendOperation((byte)OperationCode.Battle, parameters);
        //Debug.Log("SendRandomBattle");
    }

    public override void OnResponse(OperationResponse response)
    {
        base.OnResponse(response);

        OperationCode opCode = (OperationCode)response.OperationCode;
        ErrorCode errCode = (ErrorCode)response.ReturnCode;

        if (errCode != ErrorCode.Success)
        {
            Debug.Log(string.Format("ResponseReceived, OperationCode = {0}, ReturnCode = {1}, DebugMsg = {2}", opCode, errCode, response.DebugMessage));
            this.HandleErrorCode(errCode);
            return;
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
                int idSender = (int)eventData[(byte)ParameterCode.UserId];

                GameManager.ReciveChat(chatMsg, eventData);
                _uiLobbyManager.uiChatWindow.OnReciveChat(idSender, chatMsg);
                break;

            case EventCode.Battle:
                //this.HandleBattle(eventData, GameStatus.InBattle);
                break;
        }
    }    
}
