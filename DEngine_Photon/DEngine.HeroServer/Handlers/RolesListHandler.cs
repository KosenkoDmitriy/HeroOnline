using DEngine.Common;
using DEngine.HeroServer.Operations;
using DEngine.PhotonFX.Common;
using DEngine.PhotonFX.Slave;
using Photon.SocketServer;

namespace DEngine.HeroServer.Handlers
{
    public class RolesListHandler : DefaultHandlerSlave
    {
        public override HandlerType Type { get { return HandlerType.Request; } }

        public override byte RequestCode { get { return (byte)OperationCode.RolesList; } }

        public RolesListHandler(PhotonApplication serverApp)
            : base(serverApp)
        {
        }

        protected override void OnHandleRequest(OperationRequest request, PeerBase peer)
        {
            RolesListOperation requestData = new RolesListOperation(peer.Protocol, request);

            OperationResponse response = new OperationResponse(request.OperationCode, new object());
            response[(byte)ParameterCode.ClientId] = (byte[])request[(byte)ParameterCode.ClientId];

            if (!requestData.IsValid)
            {
                response.ReturnCode = (short)ErrorCode.InvalidParams;
                response.DebugMessage = requestData.GetErrorMessage();
            }
            else
            {
                byte[] rolesList = null;
                byte[] itemsList = null;

                ErrorCode errCode = (ServerApp as ZoneService).OnRolesList(requestData.UserId, ref rolesList, ref itemsList);
                if (errCode == ErrorCode.Success)
                {
                    response[(byte)ParameterCode.UserRoles] = rolesList;
                    response[(byte)ParameterCode.UserItems] = itemsList;
                }

                response.ReturnCode = (short)errCode;
            }

            peer.SendOperationResponse(response, new SendParameters());
        }
    }
}
