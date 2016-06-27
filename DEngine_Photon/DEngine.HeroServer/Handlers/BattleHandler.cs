using DEngine.Common;
using DEngine.Common.GameLogic;
using DEngine.HeroServer.Operations;
using DEngine.PhotonFX.Common;
using DEngine.PhotonFX.Slave;
using Photon.SocketServer;
using System.Collections.Generic;

namespace DEngine.HeroServer.Handlers
{
    public class BattleHandler : DefaultHandlerSlave
    {
        public override HandlerType Type { get { return HandlerType.Request; } }

        //register OperationCode.Battle event hanlde
        public override byte RequestCode { get { return (byte)OperationCode.Battle; } }

        public BattleHandler(PhotonApplication serverApp)
            : base(serverApp)
        {
        }

        protected override void OnHandleRequest(OperationRequest request, PeerBase peer)
        {
            BattleOperation requestData = new BattleOperation(peer.Protocol, request);

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
                // call OnBattle  
                ErrorCode errCode = (ServerApp as ZoneService).OnBattle(requestData);

                if (errCode != ErrorCode.Success)
                {
                    response.ReturnCode = (short)errCode;
                    peer.SendOperationResponse(response, new SendParameters());

                    Log.WarnFormat("BattleRequest returned! SubCode={0}, RoleId={1}, TargetId={2}, ErrCode={3}", (SubCode)requestData.SubCode, requestData.RoleId, requestData.TargetId, errCode);
                }
            }
        }
    }
}
