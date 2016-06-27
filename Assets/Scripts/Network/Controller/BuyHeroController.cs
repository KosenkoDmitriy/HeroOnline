using DEngine.Common;
using DEngine.Common.GameLogic;
using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;
using UnityEngine;
using DEngine.Unity.Photon;
using System.Linq;

public class BuyHeroController : PhotonController
{

    private bool _waitServer;
    private UIBuyHeroManager _manager;

    public BuyHeroController(UIBuyHeroManager manager)
        : base()
    {
        _manager = manager;
    }

    public void Disponse()
    {

    }

    public void SendRequestBuyItem(int shopID, int shopMode)
    {
        if (_waitServer) return;
        _waitServer = true;
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.TargetId, shopID);
        //0: Nhu cu
        //1: Silver only
        //2: Gold only
        parameters.Add((byte)ParameterCode.ShopMode, shopMode);
        SendOperation((byte)OperationCode.ShopBuy, parameters);

        Debug.Log("SendRequestBuyItem " + shopMode);
    }

   
    public override void OnDisconnect(StatusCode statusCode)
    {
        base.OnDisconnect(statusCode);
        this.HandleDisConnect();
    }

    public void SendRequestRefreshGold()
    {
        if (_waitServer) return;
        _waitServer = true;
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.CheckCash);
        SendOperation((byte)OperationCode.UserUpdate, parameters);
        //Debug.Log("SendRequestRoleList");
    }

    public override void OnResponse(OperationResponse response)
    {
        base.OnResponse(response);
        _waitServer = false;
        OperationCode opCode = (OperationCode)response.OperationCode;
        ErrorCode errCode = (ErrorCode)response.ReturnCode;

        if (errCode != ErrorCode.Success)
        {
            Debug.Log(string.Format("ResponseReceived, OperationCode = {0}, ReturnCode = {1}, DebugMsg = {2}", opCode, errCode, response.DebugMessage));
            this.HandleErrorCode(errCode);
            return;
        }

        switch (opCode)
        {
            case OperationCode.ShopBuy:
                if (errCode == ErrorCode.Success)
                {
                    byte[] userData = (byte[])response[(byte)ParameterCode.UserBase];
                    int itemKind = (int)response[(byte)ParameterCode.ItemKind];
                    byte[] objData = (byte[])response[(byte)ParameterCode.ShopItem];

                    GameUser.UserBase miniUser = Serialization.LoadStruct<GameUser.UserBase>(userData);

                    GameManager.GameUser.Base.Gold = miniUser.Gold;
                    GameManager.GameUser.Base.Silver = miniUser.Silver;

                    if (itemKind == (int)ItemKind.Hero)
                    {
                        UserRole userRole = Serialization.Load<UserRole>(objData);
                        userRole.GameRole = (GameRole)GameManager.GameHeroes[userRole.Base.RoleId];
                        GameManager.GameUser.UserRoles.Add(userRole);
                    }
                    _manager.OnResponseBuyHero();
                    
                }
                break;

            case OperationCode.UserUpdate:
                {
                    SubCode subCode = (SubCode)response.Parameters[(byte)ParameterCode.SubCode];

                    if (errCode == ErrorCode.Success)
                    {

                        switch (subCode)
                        {
                            case SubCode.CheckCash:
                                {
                                    byte[] userData = (byte[])response[(byte)ParameterCode.UserBase];
                                    GameUser.UserBase miniUser = Serialization.LoadStruct<GameUser.UserBase>(userData);
                                    GameManager.GameUser.Base = miniUser;
                                    _manager.OnRefreshGold();
                                }
                                break;

                        }
                    }
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

    



}
