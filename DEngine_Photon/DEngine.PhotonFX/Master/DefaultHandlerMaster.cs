using DEngine.Common;
using DEngine.PhotonFX.Common;
using Photon.SocketServer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DEngine.PhotonFX.Master
{
    public class DefaultHandlerMaster : PhotonHandler
    {

        #region Properties

        public override HandlerType Type
        {
            get { return HandlerType.All; }
        }

        public override byte RequestCode
        {
            get { return 0; }
        }

        public override byte ResponseCode
        {
            get { return 0; }
        }

        public override byte EventCode
        {
            get { return 0; }
        }

        #endregion

        #region Overrides of PhotonHandler

        public DefaultHandlerMaster(PhotonApplication serverApp)
            : base(serverApp)
        {
        }

        protected override void OnHandleRequest(OperationRequest request, PeerBase peer)
        {
            OperationResponse response = new OperationResponse()
            {
                OperationCode = request.OperationCode,
                ReturnCode = (short)ErrorCode.UnknownRequest,
            };

            peer.SendOperationResponse(response, new SendParameters() { Encrypted = true });
        }

        protected override void OnHandleResponse(OperationResponse response, PeerBase peer)
        {
            if (response.ReturnCode == (short)ErrorCode.Success)
            {
                //update CCU of Zone when Zone send event SignIn,SignOut
                if (response.OperationCode == (byte)OperationCode.SignIn || response.OperationCode == (byte)OperationCode.SignOut)
                    ((MasterSlavePeer)peer).ZoneCurCCU = (int)response[(byte)ParameterCode.ZoneCCU];
            }

            //get clientId from packet 
            Guid clientId = new Guid((byte[])response[(byte)ParameterCode.ClientId]);

            MasterClientPeer clientPeer;

            //get clientPeer by clientId from list , if exist , it is packet from zone then forward it to client required
            if (!((MasterServer)ServerApp).ClientPeers.TryGetValue(clientId, out clientPeer))
                return;

            HandleSlaveResponse(response, (MasterSlavePeer)peer, clientPeer);

            // Forward SlaveResponse to clientPeer
            response.Parameters.Remove((byte)ParameterCode.ClientId);
            clientPeer.SendOperationResponse(response, new SendParameters() { Encrypted = true });
        }

        protected override void OnHandleEvent(IEventData eventData, PeerBase peer)
        {
            //get clientId from packet
            Guid clientId = new Guid((byte[])eventData[(byte)ParameterCode.ClientId]);

            MasterClientPeer clientPeer;
            if (!((MasterServer)ServerApp).ClientPeers.TryGetValue(clientId, out clientPeer))
                return;

            // Forward SlaveEvent to clientPeer
            eventData.Parameters.Remove((byte)ParameterCode.ClientId);
            clientPeer.SendEvent(eventData, new SendParameters() { Encrypted = true });
        }

        #endregion

        #region Private Handlers

        private void HandleSlaveResponse(OperationResponse response, MasterSlavePeer serverPeer, MasterClientPeer clientPeer)
        {
            MasterServer masterServer = (MasterServer)ServerApp;
            OperationCode responseCode = (OperationCode)response.OperationCode;

            if (response.ReturnCode == (short)ErrorCode.Success)
            {
                switch (responseCode)
                {
                    case OperationCode.SignIn:
                        clientPeer.UserId = (int)response[(byte)ParameterCode.UserId];
                        clientPeer.CurrentZone = serverPeer;
                        break;
                }
            }
            else
            {
                switch (responseCode)
                {
                    case OperationCode.SignIn:
                        masterServer.SignedInUsers.OnDisconnect(clientPeer.UserName);
                        break;
                }
            }
        }

        #endregion
    }
}
