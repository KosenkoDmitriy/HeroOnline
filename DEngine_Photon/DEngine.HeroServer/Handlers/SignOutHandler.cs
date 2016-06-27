using DEngine.Common;
using DEngine.Common.GameLogic;
using DEngine.HeroServer.GameData;
using DEngine.HeroServer.Operations;
using DEngine.PhotonFX.Common;
using DEngine.PhotonFX.Slave;
using Photon.SocketServer;
using System.Collections.Generic;

namespace DEngine.HeroServer.Handlers
{
    public class SignOutHandler : DefaultHandlerSlave
    {
        public override HandlerType Type { get { return HandlerType.Request; } }

        //register network event SignOut for this handle 
        public override byte RequestCode { get { return (byte)OperationCode.SignOut; } }

        public SignOutHandler(PhotonApplication serverApp)
            : base(serverApp)
        {
        }

        protected override void OnHandleRequest(OperationRequest request, PeerBase peer)
        {
            byte[] clientId = (byte[])request[(byte)ParameterCode.ClientId];
            int userId = (int)request[(byte)ParameterCode.UserId];
            string userName = (string)request[(byte)ParameterCode.UserName];

            ZoneService zoneService = (ZoneService)ServerApp;

            ErrorCode errCode = zoneService.OnSignOut(userId, userName);

            OperationResponse response = new OperationResponse(request.OperationCode, new object());
            response.ReturnCode = (short)errCode;
            response[(byte)ParameterCode.ClientId] = clientId;
            response[(byte)ParameterCode.UserId] = userId;
            response[(byte)ParameterCode.UserName] = userName;
            response[(byte)ParameterCode.ZoneCCU] = zoneService.AllUsers.GetCount();

            peer.SendOperationResponse(response, new SendParameters());
        }
    }
}
