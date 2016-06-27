using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DEngine.Common;
using ExitGames.Client.Photon;
using DEngine.Common.GameLogic;
using DEngine.Unity.Photon;
using System.Linq;

public class ItemUpgradeController : PhotonController
{

    private UIItemUpgradeManager _uiHeroManger;
    private bool _waitServer;

    public ItemUpgradeController(UIItemUpgradeManager uiHeroManger)
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

    public void SendUpgradeItem(UserItem items, int row, bool useGold)
    {
        if (_waitServer) return;
        _waitServer = true;
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.ItemUpgrade);
        parameters.Add((byte)ParameterCode.ItemId, items.Id);
        parameters.Add((byte)ParameterCode.Param01, row);
        parameters.Add((byte)ParameterCode.Param02, useGold ? 1 : 0);
        SendOperation((byte)OperationCode.UserUpdate, parameters);
        Debug.Log("SendUpgradeItem ");
    }

    
    public override void OnResponse(OperationResponse response)
    {
        _waitServer = false;
        base.OnResponse(response);

        OperationCode opCode = (OperationCode)response.OperationCode;
        ErrorCode errCode = (ErrorCode)response.ReturnCode;

        if (errCode != ErrorCode.Success)
        {
            Debug.Log("errCode" + errCode + " - opCode " + opCode + " " + response.DebugMessage);
            //this.HandleErrorCode(errCode);          
        }

        switch (opCode)
        {
            case OperationCode.UserUpdate:
                {
                    if (errCode == ErrorCode.Success)
                    {                        
                        //MessageBox.ShowDialog(GameManager.localization.GetText("ItemUpgrade_Success"), UINoticeManager.NoticeType.Message);
                    }                 
                    if (errCode == ErrorCode.ItemsUpgradeFailed)
                    {
                        MessageBox.ShowDialog(GameManager.localization.GetText("ItemUpgrade_Fail"), UINoticeManager.NoticeType.Message);
                    }
                    HandleUserUpdate(response, errCode);
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

    private void HandleUserUpdate(OperationResponse response,ErrorCode errorCode)
    {

        byte[] itemData = (byte[])response[(byte)ParameterCode.UserItems];

        UserItem[] userItems = Serialization.LoadArray<UserItem>(itemData, true);
        if (userItems != null)
        {
            foreach (UserItem item in userItems)
            {
                item.GameItem = (GameItem)GameManager.GameItems[item.ItemId];
            }

            GameManager.GameUser.UserItems.Clear();
            GameManager.GameUser.UserItems.AddRange(userItems);
        }

        if (errorCode == ErrorCode.Success)
        {
            _uiHeroManger.OnResponseFromServer();
        }
        else
        {
            _uiHeroManger.OnFinishedEffect();
        }
    }
}
