using DEngine.Common;
using ExitGames.Logging;
using Photon.SocketServer;
using Photon.SocketServer.ServerToServer;
using PhotonHostRuntimeInterfaces;
using DEngine.PhotonFX.Common;
using System;
using System.Collections.Generic;

namespace DEngine.PhotonFX.Master
{
    public class MasterSlavePeer : ServerPeerBase
    {
        #region Constants and Fields

        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
        private readonly MasterServer _masterServer;

        #endregion

        #region Properties

        public Guid ServerId { get; set; }

        public string ServerName { get; set; }

        public string TcpAddress { get; set; }

        public string UdpAddress { get; set; }

        public int ZoneMaxCCU { get; set; }

        public int ZoneCurCCU { get; set; }

        #endregion

        #region Constructor

        public MasterSlavePeer(InitRequest initRequest, MasterServer masterServer)
            : base(initRequest.Protocol, initRequest.PhotonPeer)
        {
            _masterServer = masterServer;
        }

        #endregion

        #region Other Methods

        public override string ToString()
        {
            return string.Format("{0}\n{1}\n{2}\n{3}\n{4}", ServerName, TcpAddress, UdpAddress, ZoneMaxCCU, ZoneCurCCU);
        }

        private void HandleSlaveRegisterRequest(OperationRequest request)
        {
            OperationResponse response = new OperationResponse(request.OperationCode);

            SlaveRegOperation slaveRegData = new SlaveRegOperation(Protocol, request);
            if (!slaveRegData.IsValid)
            {
                response.ReturnCode = (short)ServerErrorCode.OperationInvalid;
                response.DebugMessage = slaveRegData.GetErrorMessage();
            }
            else
            {
                ServerId = new Guid(slaveRegData.ServerId);
                ServerName = slaveRegData.ServerName;

                if (slaveRegData.TcpPort != null)
                    TcpAddress = string.Format("{0}:{1}", slaveRegData.ServerAddress, slaveRegData.TcpPort);

                if (slaveRegData.UdpPort != null)
                    UdpAddress = string.Format("{0}:{1}", slaveRegData.ServerAddress, slaveRegData.UdpPort);

                if (slaveRegData.ZoneMaxCCU != null)
                    ZoneMaxCCU = slaveRegData.ZoneMaxCCU.Value;

                _masterServer.SlavePeers.OnConnect(this);

                Log.InfoFormat("Slave registered: Name={0}, Address={1}, TcpPort={2}, UdpPort={3}.",
                    slaveRegData.ServerName, slaveRegData.ServerAddress, slaveRegData.TcpPort, slaveRegData.UdpPort);
            }

            SendOperationResponse(response, new SendParameters());
        }

        private void HandleSlaveUnknownRequest(OperationRequest request)
        {
            OperationResponse response = new OperationResponse(request.OperationCode)
            {
                ReturnCode = (short)ServerErrorCode.OperationInvalid,
                DebugMessage = "Invalid OperationRequest",
            };

            SendOperationResponse(response, new SendParameters());
        }

        #endregion

        #region Overrides of PeerBase

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            _masterServer.SlavePeers.OnDisconnect(this);
            Log.WarnFormat("Slave {0} disconneted! Resion = {1}.", ConnectionId, reasonCode);
        }

        protected override void OnOperationRequest(OperationRequest request, SendParameters sendParameters)
        {
            switch (request.OperationCode)
            {
                case (byte)ServerOperationCode.SlaveRegister:
                    HandleSlaveRegisterRequest(request);
                    break;

                default:
                    HandleSlaveUnknownRequest(request);
                    break;
            }
        }

        #endregion

        #region Overrides of ServerPeerBase

        protected override void OnOperationResponse(OperationResponse response, SendParameters sendParameters)
        {
            _masterServer.HandleResponse(response, this);
        }

        protected override void OnEvent(IEventData eventData, SendParameters sendParameters)
        {
            if (eventData.Parameters.ContainsKey((byte)ParameterCode.Channel))
            {
                ServerEventCode eventScope = (ServerEventCode)eventData[(byte)ParameterCode.Channel];

                switch (eventScope)
                {
                    case ServerEventCode.ZoneEvent:
                        _masterServer.ClientPeers.ForEach((MasterClientPeer peer) => { SendZoneEvent(peer, eventData); });
                        return;

                    case ServerEventCode.WorldEvent:
                        _masterServer.ClientPeers.ForEach((MasterClientPeer peer) => { SendWorldEvent(peer, eventData); });
                        return;
                }
            }

            _masterServer.HandleEvent(eventData, this);
        }

        private void SendZoneEvent(MasterClientPeer clientPeer, IEventData eventData)
        {
            if (clientPeer.CurrentZone == this)
            {
                SendParameters sendParams = new SendParameters() { Encrypted = true };
                clientPeer.SendEvent(eventData, sendParams);
            }
        }

        private void SendWorldEvent(MasterClientPeer clientPeer, IEventData eventData)
        {
            SendParameters sendParams = new SendParameters() { Encrypted = true };
            clientPeer.SendEvent(eventData, sendParams);
        }

        #endregion
    }
}
