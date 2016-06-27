using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DEngine.Common;
using ExitGames.Client.Photon;
using DEngine.Common.GameLogic;
using DEngine.Unity.Photon;
using System.Linq;

public class HeroMenuController : PhotonController {

    private UIHeroManager _uiHeroManger;
    private bool _waitServer;

    public HeroMenuController(UIHeroManager uiHeroManger)
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

    public void SendRequestUpdateRole(UserRole role)
    {
        if (_waitServer) return;
        _waitServer = true;
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.RoleId, role.Id);
        parameters.Add((byte)ParameterCode.Param01, role.Base.Status);
        parameters.Add((byte)ParameterCode.Param02, role.Base.AIMode);
        SendOperation((byte)OperationCode.RoleUpdate, parameters);
        //Debug.Log("SendRequestUpdateRole");
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

    public void SendRequestRoleList()
    {
        if (_waitServer) return;
        _waitServer = true;
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();      
        SendOperation((byte)OperationCode.RolesList, parameters);
        //Debug.Log("SendRequestRoleList");
    }

    public void SendRequestResetRole(UserRole role)
    {
        if (_waitServer) return;
        _waitServer = true;
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.RoleId, role.Id);
        SendOperation((byte)OperationCode.RoleUpdate, parameters);
        //Debug.Log("SendRequestResetRole");
    }

    public void SendRequestEquipItem(int roleUId, int ItemUid, int amount = 1)
    {
        if (_waitServer) return;
        if (roleUId != 0)
            _waitServer = true;
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.RoleSetItem);
        parameters.Add((byte)ParameterCode.RoleId, roleUId);
        parameters.Add((byte)ParameterCode.ItemId, ItemUid);
        parameters.Add((byte)ParameterCode.ItemCount, amount);
        SendOperation((byte)OperationCode.UserUpdate, parameters);
        Debug.Log("SendRequestResetRole");
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
            this.HandleErrorCode(errCode);
            return;
        }

        switch (opCode)
        {
            case OperationCode.RoleUpdate:
                if (errCode == ErrorCode.Success)
                {
                    _uiHeroManger.OnReciveUserUpdate();
                    //byte[] data = (byte[])response.Parameters[(byte)ParameterCode.RoleBase];

                  //  _uiHeroManger.OnResponseResetRole(data);
                }
                else
                {
                  /*  byte[] data = null;

                    if (response.Parameters.ContainsKey((byte)ParameterCode.RoleBase))
                        data = (byte[])response.Parameters[(byte)ParameterCode.RoleBase];

                    _uiHeroManger.OnResponseResetRole(data);*/

                }
                break;
            case OperationCode.RolesList:
                if (errCode == ErrorCode.Success)
                {
                    HandleRoleList(response);
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
                                _uiHeroManger.OnRefreshGold();
                                break;
                            case SubCode.RoleSetItem:
                                _uiHeroManger.OnResponseEquipment();
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
        }
    }

    private void HandleRoleList(OperationResponse response)
    {

        byte[] objData = (byte[])response[(byte)ParameterCode.UserRoles];
        byte[] itemData = (byte[])response[(byte)ParameterCode.UserItems];

        UserRole[] roles = Serialization.LoadArray<UserRole>(objData, true);

        
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

        if (roles != null)
        {
            foreach (UserRole curRole in roles)
            {
                if (userItems != null)
                {
                    var roleItems = userItems.Where(p => p.RoleUId == curRole.Id);
                    curRole.RoleItems.AddRange(roleItems);
                }
                curRole.GameRole = (GameRole)GameManager.GameHeroes[curRole.Base.RoleId];
            }

            GameManager.GameUser.UserRoles.Clear();
            GameManager.GameUser.UserRoles.AddRange(roles);

        }

        _uiHeroManger.LoadHeroSlots();
    }

}
