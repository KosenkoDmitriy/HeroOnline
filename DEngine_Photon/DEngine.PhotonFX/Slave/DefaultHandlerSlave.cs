using DEngine.Common;
using Photon.SocketServer;
using DEngine.PhotonFX.Common;
using System;
using System.Collections.Generic;

namespace DEngine.PhotonFX.Slave
{
    public class DefaultHandlerSlave : PhotonHandler
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

        public DefaultHandlerSlave(PhotonApplication serverApp)
            : base(serverApp)
        {
        }

        protected override void OnHandleRequest(OperationRequest request, PeerBase peer)
        {
            Log.WarnFormat("OnHandleRequest {0} with DefaultHandler.", request.OperationCode);

            OperationResponse response = new OperationResponse(request.OperationCode, new object());
            response.ReturnCode = (short)ErrorCode.UnknownRequest;

            response[(byte)ParameterCode.ClientId] = (byte[])request[(byte)ParameterCode.ClientId];

            peer.SendOperationResponse(response, new SendParameters());
        }

        protected override void OnHandleResponse(OperationResponse response, PeerBase peer)
        {
            Log.WarnFormat("OnHandleResponse {0} with DefaultHandler.", response.OperationCode);
        }

        protected override void OnHandleEvent(IEventData eventData, PeerBase peer)
        {
            Log.WarnFormat("OnHandleEvent {0} with DefaultHandler.", eventData.Code);
        }
    }
}
