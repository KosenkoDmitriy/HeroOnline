using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DEngine.Common;
using ExitGames.Client.Photon;
using DEngine.Common.GameLogic;
using DEngine.Unity.Photon;
using System.Linq;

public class HeroNewController : PhotonController {

    private UIHeroNewManager _uiHeroManger;
    private bool _waitServer;

    public HeroNewController(UIHeroNewManager uiHeroManger)
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

    public void SendRequestSellItem(int ItemUid)
    {
        if (_waitServer) return;
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.ItemId, ItemUid);
        parameters.Add((byte)ParameterCode.SubCode, SubCode.ItemSell);
        SendOperation((byte)OperationCode.UserUpdate, parameters);
        Debug.Log("SendRequestResetRole");
    }

    public void SendRequestGetHireHero()
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.SetHire);
        parameters.Add((byte)ParameterCode.RoleId, 0);
        parameters.Add((byte)ParameterCode.TargetId, 0);
        parameters.Add((byte)ParameterCode.Param01, 0);
        parameters.Add((byte)ParameterCode.Param02, 0);
        SendOperation((byte)OperationCode.UserUpdate, parameters);
        Debug.Log("SendRequestHireHero");
    }

    public void SendRequestSetHire(int roleUid, int targetID, int gold, int silver)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.SetHire);
        parameters.Add((byte)ParameterCode.RoleId, roleUid);
        parameters.Add((byte)ParameterCode.TargetId, targetID);//0,1
        parameters.Add((byte)ParameterCode.Param01, gold);
        parameters.Add((byte)ParameterCode.Param02, silver);
        SendOperation((byte)OperationCode.UserUpdate, parameters);
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
                _uiHeroManger.OnResponseRoleUpdate();    
                break;
            case OperationCode.RolesList:
                HandleRoleList(response);
                break;
            case OperationCode.UserUpdate:
                {
                    SubCode subCode = (SubCode)response.Parameters[(byte)ParameterCode.SubCode];
                    
                    switch (subCode)
                    {
                        case SubCode.CheckCash:
                            // _uiHeroManger.OnRefreshGold();
                            break;
                        case SubCode.ItemSell:
                            HandleItemSell(response);
                            break;
                        case SubCode.RoleSetItem:
                             _uiHeroManger.OnResponseEquipment();
                            break;
                        //case SubCode.GetHires:
                        //    HandleGetHires(response);
                        //    break;
                        case SubCode.SetHire:
                            HandleSetHires(response);
                            break;
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

    private void HandleItemSell(OperationResponse response)
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

        _uiHeroManger.OnResponseSellItem();
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
                    UserItem[] roleItems = GameManager.GameUser.UserItems.Where(p => p.RoleUId == curRole.Id).ToArray();
                    //Debug.Log(curRole.Name + " " + roleItems.Length); 
                    curRole.RoleItems.AddRange(roleItems);
                }
                curRole.GameRole = (GameRole)GameManager.GameHeroes[curRole.Base.RoleId];


                for (int i = 0; i < curRole.RoleSkills.Count; i++)
                    curRole.RoleSkills[i].GameSkill = (GameSkill)GameManager.GameSkills[curRole.RoleSkills[i].SkillId];
            }

            GameManager.GameUser.UserRoles.Clear();
            GameManager.GameUser.UserRoles.AddRange(roles);

        }

        _uiHeroManger.OnResponseHeroListFormServer();
    }
      
    private void HandleSetHires(OperationResponse response)
    {
        byte[] objData = (byte[])response[(byte)ParameterCode.HireRoles];

        if (objData != null)
        {
            UserRoleHire[] roles = Serialization.LoadArray<UserRoleHire>(objData, true);

            if (roles != null)
            {
                GameManager.GameUser.HireRoles.Clear();
                GameManager.GameUser.HireRoles.AddRange(roles);
            }
        }

        _uiHeroManger.OnResponseSetHeroHire();
    }
}
