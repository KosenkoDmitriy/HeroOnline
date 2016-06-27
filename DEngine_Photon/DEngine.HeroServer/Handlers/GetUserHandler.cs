using DEngine.Common;
using DEngine.Common.GameLogic;
using DEngine.HeroServer.Operations;
using DEngine.PhotonFX.Common;
using DEngine.PhotonFX.Slave;
using Photon.SocketServer;
using System.Collections.Generic;

namespace DEngine.HeroServer.Handlers
{
    public class GetUserHandler : DefaultHandlerSlave
    {
        public override HandlerType Type { get { return HandlerType.Request; } }

        public override byte RequestCode { get { return (byte)OperationCode.GetUser; } }

        public GetUserHandler(PhotonApplication serverApp)
            : base(serverApp)
        {
        }

        protected override void OnHandleRequest(OperationRequest request, PeerBase peer)
        {
            GetUserOperation requestData = new GetUserOperation(peer.Protocol, request);

            Dictionary<byte, object> parameters = new Dictionary<byte, object>();
            parameters.Add((byte)ParameterCode.ClientId, requestData.ClientId.ToByteArray());

            OperationResponse response = new OperationResponse(request.OperationCode, parameters);

            if (!requestData.IsValid)
            {
                response.ReturnCode = (short)ErrorCode.InvalidParams;
                response.DebugMessage = requestData.GetErrorMessage();
            }
            else
            {
                GameUser targetUser = (ServerApp as ZoneService).HeroDatabase.UserGet(requestData.TargetId);

                if (targetUser == null)
                    response.ReturnCode = (short)ErrorCode.TargetNotFound;
                else
                {
                    response.Parameters.Add((byte)ParameterCode.TargetId, targetUser.Id);
                    response.Parameters.Add((byte)ParameterCode.UserData, GameObj.Serilize(targetUser));
                }
            }

            peer.SendOperationResponse(response, new SendParameters());
        }
    }
}
