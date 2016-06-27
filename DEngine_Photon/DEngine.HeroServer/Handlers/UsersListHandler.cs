using DEngine.Common;
using DEngine.PhotonFX.Common;
using DEngine.PhotonFX.Slave;
using DEngine.HeroServer.Operations;
using Photon.SocketServer;
using System.Collections.Generic;
using DEngine.Common.GameLogic;

namespace DEngine.HeroServer.Handlers
{
    public class UsersListHandler : DefaultHandlerSlave
    {
        public override HandlerType Type { get { return HandlerType.Request; } }

        //register network event UsersList for this handle 
        public override byte RequestCode { get { return (byte)OperationCode.UsersList; } }

        public UsersListHandler(PhotonApplication serverApp)
            : base(serverApp)
        {
        }

        protected override void OnHandleRequest(OperationRequest request, PeerBase peer)
        {
            UsersListOperation requestData = new UsersListOperation(peer.Protocol, request);

            OperationResponse response = new OperationResponse(request.OperationCode, new object());
            response[(byte)ParameterCode.ClientId] = (byte[])request[(byte)ParameterCode.ClientId];

            if (!requestData.IsValid)
            {
                response.ReturnCode = (short)ErrorCode.InvalidParams;
                response.DebugMessage = requestData.GetErrorMessage();
            }
            else
            {
                GameObjList allUsers = null;

                ErrorCode errCode = (ServerApp as ZoneService).GetUserList(requestData.UserId, requestData.ListType, ref allUsers);
                response.ReturnCode = (short)errCode;

                if (errCode == ErrorCode.Success)
                    response[(byte)ParameterCode.ZoneUsers] = Serialization.Save(allUsers, true);
            }

            peer.SendOperationResponse(response, new SendParameters());
        }
    }
}
