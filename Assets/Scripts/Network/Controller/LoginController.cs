using DEngine.Common;
using DEngine.Common.GameLogic;
using ExitGames.Client.Photon;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using DEngine.Unity.Photon;


public class LoginController : PhotonController
{
    private UILoginManager _uiLoginManager;

    public LoginController(UILoginManager uiMainMenuManager)
        :base()
    {
        _uiLoginManager = uiMainMenuManager;
    }

    public void SendSignIn(string username, string password)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.UserType, (int)UserType.Default);
        parameters.Add((byte)ParameterCode.UserName, username);
        parameters.Add((byte)ParameterCode.Password, password);
		parameters.Add((byte)ParameterCode.ZoneId, GameManager.ZoneID.ToByteArray());

		//send 
        SendOperation((byte)OperationCode.SignIn, parameters);
    }

    public void SendRegister(Dictionary<byte, object> parameters)
    {
        SendOperation((byte)OperationCode.Register, parameters);
        Debug.Log("SendRegister");
    }

    public void SendSignInWithFB(Dictionary<byte, object> parameters)
    {
        SendOperation((byte)OperationCode.SignIn, parameters);
    }

    public void SendSignOut()
    {
        SendOperation((byte)OperationCode.SignOut);
    }

	public void SendSetBalance (string username, string password, string balance)
	{
		//Debug.Log("SendSetBalance");
		Dictionary<byte, object> parameters = new Dictionary<byte, object>();
		parameters.Add((byte)ParameterCode.UserName, username);
		parameters.Add((byte)ParameterCode.Password, password);
//		parameters.Add((byte)ParameterCode.Balance, balance);
//		SendOperation((byte)OperationCode.SetBalance, parameters);
	}

    public void SendRequestZones()
    {
        SendOperation((byte)OperationCode.ZonesList);
    }

    public override void OnConnect()
    {
        //Debug.Log(" OnConnect()");
        base.OnConnect();
       // _uiMainMenuManager.OnPhotonConnected();
    }

    public override void OnDisconnect(StatusCode statusCode)
    {
        base.OnDisconnect(statusCode);
        this.HandleDisConnect();
    }

    //event network
    public override void OnResponse(OperationResponse response)
    {
        base.OnResponse(response);
        MessageBox.CloseDialog();
        OperationCode opCode = (OperationCode)response.OperationCode;
        ErrorCode errCode = (ErrorCode)response.ReturnCode;

        if (errCode != ErrorCode.Success)
        {
            Debug.Log(string.Format("ResponseReceived, OperationCode = {0}, ReturnCode = {1}, DebugMsg = {2}", opCode, errCode, response.DebugMessage));

            if (errCode == ErrorCode.DuplicateLogin)
            {
                _uiLoginManager.OnResponeDuplicate();
            } else if (errCode == ErrorCode.UserNotFound) {
				_uiLoginManager.OnButtonWebsiteReg_Click ();
			} else 
                this.HandleErrorCode(errCode);

            return;
        }
		Debug.Log(string.Format("OK ResponseReceived, OperationCode = {0}, ReturnCode = {1}, DebugMsg = {2}", opCode, errCode, response.DebugMessage));

        switch (opCode)
        {
            case OperationCode.Register:
                _uiLoginManager.OnRegisterSuccess();
                break;
            case OperationCode.SignIn:
                //Login succecss
                OnSignIn(response);
                break;

                //load zone list from master to login
            case OperationCode.ZonesList:
                OnZonesList(response);
                break;
        }
                
    }
    
    //Login succecss
    private void OnSignIn(OperationResponse response)
    {
        object gameUser;
        if (response.Parameters.TryGetValue((byte)ParameterCode.UserData, out gameUser))
        {
            byte[] userData = gameUser as byte[];
            if (userData != null)
            {
                byte[] objData = (byte[])response[(byte)ParameterCode.GameRoles];
                GameManager.GameHeroes = Serialization.Load<GameObjCollection>(objData);
                
                objData = (byte[])response[(byte)ParameterCode.GameItems];
                GameManager.GameItems = Serialization.Load<GameObjCollection>(objData);

                objData = (byte[])response[(byte)ParameterCode.GameSkills];
                GameManager.GameSkills = Serialization.Load<GameObjCollection>(objData);

                objData = (byte[])response[(byte)ParameterCode.ChargeShop];
                GameManager.ChargeShop = Serialization.Load<GameObjCollection>(objData);

                objData = (byte[])response[(byte)ParameterCode.UserData];
                GameManager.InitGameUser(objData);

				_uiLoginManager.OnPhotonLoginSuccess();
            }
        }
        else
            Debug.Log("ParameterCode.UserData => NULL");

        if (GameManager.GameUser == null)
        {
            //GameManager.Status = GameStatus.Error;
            Debug.Log("UNEXPTECED ERROR!");
        }
    }

    private void OnZonesList(OperationResponse response)
    {
        object zoneList;
        if (response.Parameters.TryGetValue((byte)ParameterCode.ZoneList, out zoneList))
        {
            //Debug.Log("zoneList");
            GameManager.GameZones.Clear();
            foreach (var item in zoneList as Dictionary<string, string>)
            {
                GameZone gameZone = new GameZone() { ServerId = new Guid(item.Key) };
                gameZone.InitData(item.Value);
                GameManager.GameZones.Add(gameZone);

               // Debug.Log(string.Format("{0} - {1}", item.Key, item.Value));

            }
            _uiLoginManager.OnResponseZoneList();
        }

        //GameManager.Status = GameStatus.ZoneSelect;
    }
}
