using DEngine.Common;
using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DEngine.Unity.Photon
{
    public class PhotonController
    {
        #region Events

        public event EventHandler Connected;

        public event EventHandler<PhotonEventArgs> Disconnected;

        public event EventHandler<PhotonEventArgs> StatusChanged;

        public event EventHandler<PhotonEventArgs> EventReceived;

        public event EventHandler<PhotonEventArgs> ResponseReceived;

        #endregion

        #region Properties

        public PhotonManager PhotonManager { get; private set; }

        public NetworkState State { get { return PhotonManager.State; } }

        public bool IsConnected { get { return PhotonManager.State is Connected; } }

        #endregion

        #region Common Methods

        public PhotonController()
        {
            PhotonManager = PhotonManager.Initialize(this);
        }

        public void Connect(string serverAdddress, string applicationName)
        {
            PhotonManager.Connect(serverAdddress, applicationName);
        }

        public void Disconnect()
        {
            PhotonManager.Disconnect();
        }

        #endregion

        #region Event Methods

        public virtual void OnDebugReturn(DebugLevel level, string message)
        {
            Debug.Log(string.Format("DebugReturn, Level = {0}, Message = {1}", level, message));
        }

        public virtual void OnEvent(EventData eventData)
        {
            /*Debug.Log(string.Format("OnEvent Code = {0}", (EventCode)eventData.Code));
            if (eventData.Code == (byte)EventCode.Battle)
            {
                SubCode subCode = (SubCode)eventData[(byte)ParameterCode.SubCode];
                Debug.Log(string.Format("SubCode = {0}", subCode));
            }*/

            if (EventReceived != null)
                EventReceived(this, new PhotonEventArgs() { EventData = eventData });
        }

        public virtual void OnResponse(OperationResponse response)
        {
            //Debug.Log(string.Format("OnResponse OperationCode = {0}, ReturnCode = {1}", (OperationCode)response.OperationCode, (ErrorCode)response.ReturnCode));

            if (ResponseReceived != null)
                ResponseReceived(this, new PhotonEventArgs() { Response = response });
        }

        public virtual void OnConnect()
        {
            if (Connected != null)
                Connected(this, EventArgs.Empty);
        }

        public virtual void OnDisconnect(StatusCode statusCode)
        {
            if (Disconnected != null)
                Disconnected(this, new PhotonEventArgs() { StatusCode = statusCode });
        }

        public virtual void OnStatusChanged(StatusCode statusCode)
        {
            Debug.LogWarning(string.Format("OnStatusChanged, StatusCode = {0}", statusCode));

            if (StatusChanged != null)
                StatusChanged(this, new PhotonEventArgs() { StatusCode = statusCode });
        }

        #endregion

        #region Send Methods

        public void SendOperation(byte opCode, bool sendReliable = true)
        {
            OperationRequest request = new OperationRequest() { OperationCode = opCode };
            PhotonManager.SendOperation(request, sendReliable, 0, true);
        }

        public void SendOperation(byte opCode, Dictionary<byte, object> parameters, bool sendReliable = true)
        {
            OperationRequest request = new OperationRequest() { OperationCode = opCode, Parameters = parameters };
            PhotonManager.SendOperation(request, sendReliable, 0, true);
        }

        public void SendOperation(OperationRequest request, bool sendReliable = true)
        {
            PhotonManager.SendOperation(request, sendReliable, 0, true);
        }

        public void SendOperation(OperationRequest request, bool sendReliable, byte channelId, bool encrypt)
        {
            PhotonManager.SendOperation(request, sendReliable, channelId, encrypt);
        }

        #endregion
    }
}
