﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DEngine.Common;
using ExitGames.Client.Photon;
using DEngine.Common.GameLogic;
using DEngine.Unity.Photon;
using System.Linq;

public class HeroUpStarController : PhotonController {

    private UIUpStarManager _uiHeroManger;
    private bool _waitServer;

    public HeroUpStarController(UIUpStarManager uiHeroManger)
        : base()
    {
        _uiHeroManger = uiHeroManger;
        _waitServer = false;
    }

    public void Disponse()
    {

    }

    public override void OnDisconnect(StatusCode statusCode)
    {
        base.OnDisconnect(statusCode);
        this.HandleDisConnect();
    }

    public void SendRequestUpStar(UserRole role, int[] items)
    {
        if (_waitServer) return;
        _waitServer = true;
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.RoleUpStar);
        parameters.Add((byte)ParameterCode.RoleId, role.Id);
        SendOperation((byte)OperationCode.UserUpdate, parameters);
        Debug.Log("SendRequestUpStar " + items.Length);
    }

    
    public override void OnResponse(OperationResponse response)
    {
        _waitServer = false;
        base.OnResponse(response);

        OperationCode opCode = (OperationCode)response.OperationCode;
        ErrorCode errCode = (ErrorCode)response.ReturnCode;

        if (errCode != ErrorCode.Success)
        {
            Debug.Log("errCode" + errCode + " - opCode " + opCode);
            this.HandleErrorCode(errCode);
            return;
        }

        switch (opCode)
        {
            case OperationCode.UserUpdate:
                if (errCode == ErrorCode.Success)
                {
                    HandleUserUpdate(response);
                }
                break;
          
            default: break;

        }
    }

    public override void OnEvent(EventData eventData)
    {
        EventCode evCode = (EventCode)eventData.Code;

        switch (evCode)
        {
            case EventCode.SendChat:
                string chatMsg = (string)eventData[(byte)ParameterCode.Message];
                GameManager.ReciveChat(chatMsg, eventData);
                this.HandleNotification(eventData);
                break;
            case EventCode.Battle:
                //this.HandleBattle(eventData, GameStatus.InBattle);
                break;

            case EventCode.PvPSearch:
                this.HandlePvPSearch(eventData);
                break;
           
        }
    }

    private void HandleUserUpdate(OperationResponse response)
    {
        _uiHeroManger.OnServerResponse();        
    }

}
