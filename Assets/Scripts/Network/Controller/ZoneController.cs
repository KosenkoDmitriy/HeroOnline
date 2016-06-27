using DEngine.Common;
using DEngine.Common.GameLogic;
using ExitGames.Client.Photon;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using DEngine.Unity.Photon;


public class ZoneController : PhotonController
{
    private UIZoneMenuManager _UIZoneManager;

    public ZoneController(UIZoneMenuManager uiUIZoneManager)
        :base()
    {
        _UIZoneManager = uiUIZoneManager;
    }


 
    public void SendRequestZones()
    {
        SendOperation((byte)OperationCode.ZonesList);
    }

  


    public override void OnConnect()
    {
        //Debug.Log(" OnConnect()");
        base.OnConnect();
     //   _uiMainMenuManager.OnPhotonConnected();
    }

    public override void OnDisconnect(StatusCode statusCode)
    {
        base.OnDisconnect(statusCode);
        this.HandleDisConnect();
    }
    
    public override void OnResponse(OperationResponse response)
    {
        base.OnResponse(response);

        OperationCode opCode = (OperationCode)response.OperationCode;
        ErrorCode errCode = (ErrorCode)response.ReturnCode;

        if (errCode != ErrorCode.Success)
        {
            Debug.Log(string.Format("ResponseReceived, OperationCode = {0}, ReturnCode = {1}, DebugMsg = {2}", opCode, errCode, response.DebugMessage));
            this.HandleErrorCode(errCode);
            return;
        }

        if (errCode == ErrorCode.Success)
        {
            switch (opCode)
            {
                case OperationCode.ZonesList:
                    Debug.Log("ZonesList");
                    OnZonesList(response);
                    break;
            }
        }
        else
        {
            switch (opCode)
            {
                case OperationCode.SignIn:
                    MessageBox.ShowDialog(errCode.ToString(), UINoticeManager.NoticeType.Message);
                    break;
            }

        }
    }

    
    private void OnZonesList(OperationResponse response)
    {
        object zoneList;
        if (response.Parameters.TryGetValue((byte)ParameterCode.ZoneList, out zoneList))
        {
            GameManager.GameZones.Clear();
            foreach (var item in zoneList as Dictionary<string, string>)
            {
                GameZone gameZone = new GameZone() { ServerId = new Guid(item.Key) };
                gameZone.InitData(item.Value);
                GameManager.GameZones.Add(gameZone);

                Debug.Log(string.Format("{0} - {1}", item.Key, item.Value));

            }
       //     _UIZoneManager.OnPhotonResponseZones();
        }

        //GameManager.Status = GameStatus.ZoneSelect;
    }

 

    public void SendZonesList()
    {
        SendOperation((byte)OperationCode.ZonesList);
    }
}
