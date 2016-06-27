using DEngine.Common;
using DEngine.Common.GameLogic;
using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;
using UnityEngine;
using DEngine.Unity.Photon;
using DEngine.Common.Config;
using System.Linq;

public class WorldMapController : PhotonController
{   
    private UIWorldMapManager _uiManager;
    public bool islock;

    public WorldMapController(UIWorldMapManager uiLobbyManager)
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

    public void SendChangeAvatar(int avatarID)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.UserAvatar);
        parameters.Add((byte)ParameterCode.AvatarId, avatarID);
        SendOperation((byte)OperationCode.UserUpdate, parameters);
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

    public void SendPVAMode()
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.RandomPvA);
        SendOperation((byte)OperationCode.Battle, parameters);
        //Debug.Log("SendRandomBattle");
    }

    public void SendRequestHire()
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.GetHires);
        SendOperation((byte)OperationCode.UserUpdate, parameters);
    }

    public void SendDungeonMode(int RoleUIDHire)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.DungeonBegin);
        parameters.Add((byte)ParameterCode.RoleId, RoleUIDHire);
        SendOperation((byte)OperationCode.Battle, parameters);
        //Debug.Log("SendRandomBattle");
    }

    public void SendRequestMailList()
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.CheckEmail);
        SendOperation((byte)OperationCode.UserUpdate, parameters);
    }

    public void SendBuyPackage(int targetID)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.TargetId, targetID);
        SendOperation((byte)OperationCode.ShopBuy, parameters);
    }

    public void SendRequestOnlineReward(int type = 0)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.OnlineAward);
        parameters.Add((byte)ParameterCode.Param01, type);
        SendOperation((byte)OperationCode.UserUpdate, parameters);
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
                        case SubCode.CheckEmail:
                            HandleReponseCheckMail(response);
                            break;
                        case SubCode.OnlineAward:
                            HandleOnlineAward(response);
                            break;
                        case SubCode.GetHires:
                            {
                                HandleGetHires(response);
                            }
                            break;
                    }
                }
                break;
            case OperationCode.ShopBuy:
                {
                    HandleBuyPackage(response);
                }
                break;
            default: break;
        }
    }

    private void HandleOnlineAward(OperationResponse response)
    {
        GameAward gameAward = null;
        if (response.Parameters.ContainsKey((byte)ParameterCode.GameAward))
        {
            byte[] data = (byte[])response[(byte)ParameterCode.GameAward];
            gameAward = Serialization.LoadStruct<GameAward>(data);
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
        }
        
        byte[] userData = (byte[])response[(byte)ParameterCode.UserBase];
        GameUser.UserBase userBase = Serialization.LoadStruct<GameUser.UserBase>(userData);
        GameManager.GameUser.Base = userBase;
           

        _uiManager.OnResponseOnlineReward(gameAward);
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
                _uiManager.uiChatWindow.OnReciveChat(idSender, chatMsg);     
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

   

    private void HandleReponseCheckMail(OperationResponse response)
    {
        byte[] mails = (byte[])response[(byte)ParameterCode.UserMails];
        if (mails == null) return;

        UserMail[] userMail = Serialization.LoadArray<UserMail>(mails, true);

        if (userMail == null) return;

        _uiManager.OnResponseCheckMail(userMail);

    }

    private void HandleGetHires(OperationResponse response)
    {
        byte[] objData = (byte[])response[(byte)ParameterCode.HireRoles];

        if (objData != null)
        {
            UserRoleHire[] roles = Serialization.LoadArray<UserRoleHire>(objData, true);


            _uiManager.OnResponseHeroHire(roles);

        }

    }

    private void HandleBuyPackage(OperationResponse response)
    {
        byte[] itemData = (byte[])response[(byte)ParameterCode.UserItems];
        byte[] roleData = (byte[])response[(byte)ParameterCode.UserRoles];

        UserItem[] items = Serialization.LoadArray<UserItem>(itemData, true);
        UserRole[] roles = Serialization.LoadArray<UserRole>(roleData, true);

        if (items != null && roles != null)
        {
            int newItemCount = items.Length - GameManager.GameUser.UserItems.Count;
            int newRoleCount = roles.Length - GameManager.GameUser.UserRoles.Count;

            if (items != null)
            {
                foreach (UserItem item in items)
                {
                    item.GameItem = (GameItem)GameManager.GameItems[item.ItemId];
                }

                GameManager.GameUser.UserItems.Clear();
                GameManager.GameUser.UserItems.AddRange(items);
            }

            if (roles != null)
            {
                foreach (UserRole curRole in roles)
                {
                    if (roles != null)
                    {
                        UserItem[] roleItems = GameManager.GameUser.UserItems.Where(p => p.RoleUId == curRole.Id).ToArray();
                        curRole.RoleItems.AddRange(roleItems);
                    }
                    curRole.GameRole = (GameRole)GameManager.GameHeroes[curRole.Base.RoleId];


                    for (int i = 0; i < curRole.RoleSkills.Count; i++)
                        curRole.RoleSkills[i].GameSkill = (GameSkill)GameManager.GameSkills[curRole.RoleSkills[i].SkillId];
                }

                GameManager.GameUser.UserRoles.Clear();
                GameManager.GameUser.UserRoles.AddRange(roles);

            }
                        

            _uiManager.OnResponseBuyPackage(newItemCount, newRoleCount);
        }
    }
}
