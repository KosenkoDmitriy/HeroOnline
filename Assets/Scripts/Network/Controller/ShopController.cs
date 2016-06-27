using DEngine.Common;
using DEngine.Common.GameLogic;
using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;
using UnityEngine;
using DEngine.Unity.Photon;
using System.Linq;

public class ShopController : PhotonController
{

    private GameUser _inviterBattle;
    private UIShopManager _uiShopManager;
    private bool _waitServer;

    public ShopController(UIShopManager uiShopManager)
        : base()
    {
        _uiShopManager = uiShopManager;
    }

    public void Disponse()
    {

    }

    public void SendChargeCashPlayStore(ChargeType chargeType, int ShopItemId, string productID, string token)
    {
        if (_waitServer) return;
        _waitServer = true;
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.Param01, (int)chargeType);
        parameters.Add((byte)ParameterCode.Param02, ShopItemId);
        parameters.Add((byte)ParameterCode.Param03, productID);
        parameters.Add((byte)ParameterCode.Param04, token);
        SendOperation((byte)OperationCode.ChargeCash, parameters);
    }

    public void SendChargeCashCard(CardType cardType, string series, string cardCode)
    {
        if (_waitServer) return;
        _waitServer = true;
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.Param01, (int)ChargeType.MobileCard);
        parameters.Add((byte)ParameterCode.Param02, (int)cardType);
        parameters.Add((byte)ParameterCode.Param03, series);
        parameters.Add((byte)ParameterCode.Param04, cardCode);
        SendOperation((byte)OperationCode.ChargeCash, parameters);

    }

    public void SendRequestBuyItem(int shopID, int shopMode)
    {
        if (_waitServer) return;
        _waitServer = true;
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.TargetId, shopID);
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
                    else
                    {
                        UserItem userItem = Serialization.Load<UserItem>(objData);
                        userItem.GameItem = (GameItem)GameManager.GameItems[userItem.ItemId];
                        UserItem itemInInventory = GameManager.GameUser.UserItems.FirstOrDefault(p => p.ItemId == userItem.ItemId);
                        if (itemInInventory == null)
                            GameManager.GameUser.UserItems.Add(userItem);
                        else
                            itemInInventory.Count = userItem.Count;
                    }
                    
                }
                _uiShopManager.OnBuyItemResponse(errCode);
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
                                    _uiShopManager.OnRefreshGold();
                                }
                                break;

                        }
                    }
                }
                break;
            case OperationCode.ChargeCash:
                HandleChargeCash(response);
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

    private void HandleChargeCash(OperationResponse response)
    {
        byte[] userData = (byte[])response[(byte)ParameterCode.UserBase];
        int cardAmount = (int)response[(byte)ParameterCode.CardAmount];
        int goldAdd = (int)response[(byte)ParameterCode.ChargeGold];
        int silverAdd = (int)response[(byte)ParameterCode.ChargeSilver];
        if (userData != null)
        {
            GameUser.UserBase miniUser = Serialization.LoadStruct<GameUser.UserBase>(userData);
            GameManager.GameUser.Base = miniUser;
        }
        _uiShopManager.OnResponseCashCard(cardAmount, goldAdd, silverAdd);
    }



}
