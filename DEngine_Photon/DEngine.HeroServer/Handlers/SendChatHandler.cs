using DEngine.Common;
using DEngine.PhotonFX.Common;
using DEngine.PhotonFX.Slave;
using DEngine.HeroServer.Operations;
using Photon.SocketServer;
using System.Collections.Generic;

namespace DEngine.HeroServer.Handlers
{
    public class SendChatHandler : DefaultHandlerSlave
    {
        public override HandlerType Type { get { return HandlerType.Request; } }

        public override byte RequestCode { get { return (byte)OperationCode.SendChat; } }

        public SendChatHandler(PhotonApplication serverApp)
            : base(serverApp)
        {
        }

        protected override void OnHandleRequest(OperationRequest request, PeerBase peer)
        {
            SendChatOperation requestData = new SendChatOperation(peer.Protocol, request);

            OperationResponse response = new OperationResponse(request.OperationCode, new object());
            response[(byte)ParameterCode.ClientId] = requestData.ClientId.ToByteArray();

            if (!requestData.IsValid)
            {
                response.ReturnCode = (short)ErrorCode.InvalidParams;
                response.DebugMessage = requestData.GetErrorMessage();
                peer.SendOperationResponse(response, new SendParameters());
            }
            else
            {
                (ServerApp as ZoneService).OnSendChat(requestData.SenderId, requestData.TargetId, requestData.Message);
            }
        }
    }
}
