using DEngine.Common;
using DEngine.HeroServer.Operations;
using DEngine.PhotonFX.Common;
using DEngine.PhotonFX.Slave;
using Photon.SocketServer;

namespace DEngine.HeroServer.Handlers
{
    public class RoleUpdateHandler : DefaultHandlerSlave
    {
        public override HandlerType Type { get { return HandlerType.Request; } }

        public override byte RequestCode { get { return (byte)OperationCode.RoleUpdate; } }

        public RoleUpdateHandler(PhotonApplication serverApp)
            : base(serverApp)
        {
        }

        protected override void OnHandleRequest(OperationRequest request, PeerBase peer)
        {
            RoleUpdateOperation requestData = new RoleUpdateOperation(peer.Protocol, request);

            OperationResponse response = new OperationResponse(request.OperationCode, new object());
            response[(byte)ParameterCode.ClientId] = requestData.ClientId.ToByteArray();

            if (!requestData.IsValid)
            {
                response.ReturnCode = (short)ErrorCode.InvalidParams;
                response.DebugMessage = requestData.GetErrorMessage();
            }
            else
            {
                ErrorCode errCode = (ServerApp as ZoneService).OnRoleUpdate(requestData);
                response.ReturnCode = (short)errCode;
            }

            peer.SendOperationResponse(response, new SendParameters());
        }
    }
}
