using UnityEngine;
using System.Collections;
using DEngine.Unity.Photon;
using ExitGames.Client.Photon;
using DEngine.Common;
using System.Collections.Generic;
using DEngine.Common.GameLogic;

public class EmpireController : PhotonController
{
    private UIEmpireManager _manager;
    public bool islock;

    public EmpireController(UIEmpireManager manager)
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

    public void SendExpandLand()
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.ExpandLand);
        SendOperation((byte)OperationCode.UserUpdate, parameters);
    }

    public void SendOpenLandCell(Vector2 pos)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.OpenLandCell);
        parameters.Add((byte)ParameterCode.Param01, (int)pos.x);
        parameters.Add((byte)ParameterCode.Param02, (int)pos.y);
        SendOperation((byte)OperationCode.UserUpdate, parameters);
    }

    public void SendBuildHouse(int houseID, Vector2 pos)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.BuildHouse);
        parameters.Add((byte)ParameterCode.TargetId, houseID);
        parameters.Add((byte)ParameterCode.Param01, (int)pos.x);
        parameters.Add((byte)ParameterCode.Param02, (int)pos.y);
        SendOperation((byte)OperationCode.UserUpdate, parameters);
    }


    public void SendDestroyHouse(int houseID, Vector2 pos)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.DestroyHouse);
        parameters.Add((byte)ParameterCode.TargetId, houseID);
        parameters.Add((byte)ParameterCode.Param01, (int)pos.x);
        parameters.Add((byte)ParameterCode.Param02, (int)pos.y);
        SendOperation((byte)OperationCode.UserUpdate, parameters);
    }

    public void SendCheckBank(bool recive)
    {
        int param01 = recive ? 1 : 0;
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters.Add((byte)ParameterCode.SubCode, SubCode.CheckBank);
        parameters.Add((byte)ParameterCode.Param01, param01);
        SendOperation((byte)OperationCode.UserUpdate, parameters);
    }

    public override void OnEvent(EventData eventData)
    {
        EventCode evCode = (EventCode)eventData.Code;


    }

    public override void OnResponse(OperationResponse response)
    {
        base.OnResponse(response);

        OperationCode opCode = (OperationCode)response.OperationCode;
        ErrorCode errCode = (ErrorCode)response.ReturnCode;

        if (errCode != ErrorCode.Success)
        {
            _manager.ClearDrag();
            Debug.Log(string.Format("ResponseReceived, OperationCode = {0}, ReturnCode = {1}, DebugMsg = {2}", opCode, errCode, response.DebugMessage));
            this.HandleErrorCode(errCode);
            return;
        }

        switch (opCode)
        {
            case OperationCode.UserUpdate:
                {
                    SubCode subCode = (SubCode)response.Parameters[(byte)ParameterCode.SubCode];
                    switch (subCode)
                    {
                        case SubCode.ExpandLand:
                            OnHandleExpandLand(response);
                            break;
                        case SubCode.OpenLandCell:
                            OnHandleOpenLandCell(response);
                            break;
                        case SubCode.BuildHouse:
                            OnHandleBuildHouse(response);
                            break;
                        case SubCode.DestroyHouse:
                            OnHandleDestroyHouse(response);
                            break;
                        case SubCode.CheckBank:
                            OnHandleCheckBank(response);
                            break;
                    }
                }
                break;
        }
    }


    private void OnHandleExpandLand(OperationResponse response)
    {
        OnReciveLand(response);
        _manager.StartCoroutine(_manager.OnResponseExpandServer());
    }

    private void OnHandleOpenLandCell(OperationResponse response)
    {
        OnReciveLand(response);
        _manager.OnResponseOpenCell();
    }

    private void OnHandleBuildHouse(OperationResponse response)
    {
        OnReciveLand(response);
        _manager.OnBuilHouseSuccess();
    }

    private void OnHandleDestroyHouse(OperationResponse response)
    {
        OnReciveLand(response);
        _manager.OnResponseDestroyHouse();
    }

    private void OnHandleCheckBank(OperationResponse response)
    {
        byte[] landData = (byte[])response.Parameters[(byte)ParameterCode.UserLand];
        byte[] userData = (byte[])response[(byte)ParameterCode.UserBase];

        float oldSilver = GameManager.GameUser.Base.Silver;
        float newSilver = GameManager.GameUser.Base.Silver;

        if (userData != null)
        {
            GameUser.UserBase miniUser = Serialization.LoadStruct<GameUser.UserBase>(userData);
            GameManager.GameUser.Base = miniUser;
            newSilver = GameManager.GameUser.Base.Silver;
        }

        UserLand userLand = Serialization.Load<UserLand>(landData);

        GameManager.GameUser.Land = userLand;


        _manager.OnResponseCheckBank(newSilver - oldSilver);
    }

    private void OnReciveLand(OperationResponse response)
    {
        byte[] landData = (byte[])response.Parameters[(byte)ParameterCode.UserLand];
        byte[] userData = (byte[])response[(byte)ParameterCode.UserBase];

        UserLand userLand = Serialization.Load<UserLand>(landData);

        GameManager.GameUser.Land = userLand;
        _manager.curLand = userLand;

        if (userData != null)
        {
            GameUser.UserBase miniUser = Serialization.LoadStruct<GameUser.UserBase>(userData);
            GameManager.GameUser.Base = miniUser;
        }
    }
}
