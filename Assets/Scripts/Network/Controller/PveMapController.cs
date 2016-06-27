using DEngine.Common;
using DEngine.Common.GameLogic;
using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;
using UnityEngine;
using DEngine.Unity.Photon;

public class PveMapController : PhotonController
{

    private UIPVEMapManager _manager;
    public bool islock;

    public PveMapController(UIPVEMapManager manager)
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

    public void SendRequestHire()
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.GetHires);
        SendOperation((byte)OperationCode.UserUpdate, parameters);
    }

    public void SendRequestBattle(int level,int RoleUIDHire, int difficulty)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.MissionBegin);
        parameters.Add((byte)ParameterCode.TargetId, level);
        parameters.Add((byte)ParameterCode.Difficulty, difficulty);
        parameters.Add((byte)ParameterCode.RoleId, RoleUIDHire);
        SendOperation((byte)OperationCode.Battle, parameters);
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
                this.HandleNotification(eventData);
                break;     
            case EventCode.Battle:
                this.HandleBattle(eventData, GameStatus.Mission);
                break;

            case EventCode.PvPSearch:
                this.HandlePvPSearch(eventData);
                break;
        }
    }



    public override void OnResponse(OperationResponse response)
    {
        base.OnResponse(response);

        OperationCode opCode = (OperationCode)response.OperationCode;
        ErrorCode errCode = (ErrorCode)response.ReturnCode;

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
                    SubCode subCode = (SubCode)response.Parameters[(byte)ParameterCode.SubCode];
                    switch (subCode)
                    {
                        case SubCode.GetHires:
                            {
                                HandleGetHires(response);
                            }
                            break;
                    }
                }
                break;
            default: break;
        }
    }

    private void HandleGetHires(OperationResponse response)
    {
        byte[] objData = (byte[])response[(byte)ParameterCode.HireRoles];

        if (objData != null)
        {
            UserRoleHire[] roles = Serialization.LoadArray<UserRoleHire>(objData, true);


            _manager.OnResponseHeroHire(roles);
            
        }

    }
}
