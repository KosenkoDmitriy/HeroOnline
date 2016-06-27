using DEngine.Common;
using DEngine.Common.GameLogic;
using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;
using UnityEngine;
using DEngine.Unity.Photon;

public class SocialController : PhotonController
{

    private UISocialManager _manager;
    public bool islock;

    public SocialController(UISocialManager manager)
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


    public void SendRequestFriendList()
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.GetFriend);
        SendOperation((byte)OperationCode.UserUpdate, parameters);
    }

    public void SendRequestAddFriend(string nickName)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.AddFriend);
        parameters.Add((byte)ParameterCode.FriendMode, 1);//1 = một chiều //2 = hai chiều
        parameters.Add((byte)ParameterCode.FriendName, nickName);
        SendOperation((byte)OperationCode.UserUpdate, parameters);
    }

    public void SendRequestAddEnemy(string nickName)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.AddFriend);
        parameters.Add((byte)ParameterCode.FriendMode, 0);
        parameters.Add((byte)ParameterCode.FriendName, nickName);
        SendOperation((byte)OperationCode.UserUpdate, parameters);
    }

    public void SendRequestRemoveFriend(string nickName)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.AddFriend);
        parameters.Add((byte)ParameterCode.FriendMode, -1);//1 = một chiều //2 = hai chiều
        parameters.Add((byte)ParameterCode.FriendName, nickName);
        SendOperation((byte)OperationCode.UserUpdate, parameters);
    } 

    public void SendRequestMailList()
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.CheckEmail);    
        SendOperation((byte)OperationCode.UserUpdate, parameters);
    }

    public void SendRequestReportList()
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.CheckReport);
        SendOperation((byte)OperationCode.UserUpdate, parameters);
    }

    public void SendRequestReadMail(int mailID)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.ReadEmail);
        parameters.Add((byte)ParameterCode.TargetId, mailID);
        SendOperation((byte)OperationCode.UserUpdate, parameters);
    }

    public void SendChat(int targetId, string chatmsg)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.TargetId, targetId);
        parameters.Add((byte)ParameterCode.Message, chatmsg);
        SendOperation((byte)OperationCode.SendChat, parameters);
    }
           
    
    public void SendChangeAvatar(int avatarID)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.UserAvatar);
        parameters.Add((byte)ParameterCode.AvatarId,avatarID);
        SendOperation((byte)OperationCode.UserUpdate, parameters);
    }

    public void SendRevenge(int id)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.RandomPvA);
        parameters.Add((byte)ParameterCode.TargetId, id);
        SendOperation((byte)OperationCode.Battle, parameters);
        Debug.Log("SendReplay");
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

            if (errCode == ErrorCode.TargetNotFound)
            {
                MessageBox.CloseDialog();
                MessageBox.ShowDialog(GameManager.localization.GetText("Social_FindUser_UserNotFound"), UINoticeManager.NoticeType.Message);
            }

            return;
        }

        switch (opCode)
        {
            case OperationCode.UserUpdate:
                {
                    SubCode subCode = (SubCode)parameter[(byte)ParameterCode.SubCode];
          
                    switch (subCode)
                    {

                        case SubCode.CheckReport:
                            HandleReponseCheckReport(response);
                            break;
                        case SubCode.ReadEmail:
                            HandleReponseReadMail(response);
                            break;
                        case SubCode.CheckEmail:
                            this.HandleReponseCheckMail(response);
                            _manager.OnResponseCheckMail();
                            break;
                        case SubCode.GetFriend:
                            this.HandleReponseGetFriendList(response);
                            _manager.OnResponseFriendList();
                            break;
                        case SubCode.AddFriend:
                            if (errCode == ErrorCode.Success)
                            {
                                HandleReponseAddFriend(response);
                            }
                            else
                            {
                                MessageBox.CloseDialog();
                                MessageBox.ShowDialog(GameManager.localization.GetText("Social_FindUser_UserNotFound"), UINoticeManager.NoticeType.Message);                               
                            }
                            break;
                    }
                }
                break;         
            default: break;
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
                _manager.OnReciveChat(idSender, chatMsg);
                this.HandleNotification(eventData);
                break;

            case EventCode.Battle:
                this.HandleBattle(eventData, GameStatus.PVA);
                break;

            case EventCode.PvPSearch:
                this.HandlePvPSearch(eventData);
                break;
        }
    }

   
    private void HandleReponseAddFriend(OperationResponse response)
    {
        byte[] friends = (byte[])response[(byte)ParameterCode.Friends];

        UserFriend[] userFriends = Serialization.LoadArray<UserFriend>(friends, true);

        GameManager.GameUser.UserFriends.Clear();

        if (userFriends != null)
            GameManager.GameUser.UserFriends.AddRange(userFriends);

        _manager.OnResponseAddFriend();
    }

  

    private void HandleReponseCheckReport(OperationResponse response)
    {
        byte[] userPvALog = (byte[])response[(byte)ParameterCode.UserPvALog];
        byte[] userPvPLog = (byte[])response[(byte)ParameterCode.UserPvPLog];

        if (userPvALog != null)
        {
            PvALog[] pvaLogs = Serialization.LoadArray<PvALog>(userPvALog, true);
            if (pvaLogs !=null)
            {
                GameManager.pvaLogs.Clear();
                GameManager.pvaLogs.AddRange(pvaLogs);

                GameManager.GameUser.PvALogs.Clear();
                GameManager.GameUser.PvALogs.AddRange(pvaLogs);
            }
        }

        if (userPvPLog != null)
        {
            PvPLog[] pvpLogs = Serialization.LoadArray<PvPLog>(userPvPLog, true);
            if (pvpLogs != null)
            {
                GameManager.pvpLogs.Clear();
                GameManager.pvpLogs.AddRange(pvpLogs);

                GameManager.GameUser.PvPLogs.Clear();
                GameManager.GameUser.PvPLogs.AddRange(pvpLogs);
            }
        }


        GameManager.InitReport();

        _manager.OnResponseReport();

    }


    private void HandleReponseReadMail(OperationResponse response)
    {

        byte[] itemData = (byte[])response[(byte)ParameterCode.UserItems];
        if (itemData != null)
        {
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


        byte[] mails = (byte[])response[(byte)ParameterCode.UserMails];  
        GameManager.GameUser.UserMails.Clear();
        if (mails != null)
        {
            UserMail[] userMail = Serialization.LoadArray<UserMail>(mails, true);
            if (userMail != null)
            {
                GameManager.GameUser.UserMails.AddRange(userMail);
            }
        }
        _manager.OnResponseCheckMail();
        _manager.OnResponseDeleteMail();

    }

    
}
