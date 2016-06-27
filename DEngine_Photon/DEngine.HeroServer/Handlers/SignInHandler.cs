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
    public class SignInHandler : DefaultHandlerSlave
    {
        public override HandlerType Type { get { return HandlerType.Request; } }


       
        //register network event SignIn for this handle 
        public override byte RequestCode { get { return (byte)OperationCode.SignIn; } }

        public SignInHandler(PhotonApplication serverApp)
            : base(serverApp)
        {
        }

        protected override void OnHandleRequest(OperationRequest request, PeerBase peer)
        {
            SignInOperation requestData = new SignInOperation(peer.Protocol, request);

            OperationResponse response = new OperationResponse(request.OperationCode, new object());
            response[(byte)ParameterCode.ClientId] = requestData.ClientId.ToByteArray();
            response[(byte)ParameterCode.UserName] = requestData.UserName;

            ZoneService zoneService = (ZoneService)ServerApp;

            if (!requestData.IsValid)
            {
                response.ReturnCode = (short)ErrorCode.InvalidParams;
                response.DebugMessage = requestData.GetErrorMessage();
            }
            else
            {
                GameUser gameUser;
                ErrorCode errCode = zoneService.OnSignIn(requestData.UserName, requestData.Password, requestData.RemoteIP, out gameUser);
                response.ReturnCode = (short)errCode;

                if (errCode == ErrorCode.Success)
                {
                    gameUser.ClientId = requestData.ClientId;
                    response[(byte)ParameterCode.UserId] = gameUser.Id;
                    response[(byte)ParameterCode.UserData] = Serialization.Save(gameUser, true);
                    response[(byte)ParameterCode.GameRoles] = Serialization.Save(Global.GameRoles);
                    response[(byte)ParameterCode.GameItems] = Serialization.Save(Global.GameItems);
                    response[(byte)ParameterCode.GameSkills] = Serialization.Save(Global.GameSkills);
                    response[(byte)ParameterCode.ChargeShop] = Serialization.Save(Global.ChargeShop);
                }

                response[(byte)ParameterCode.ZoneCCU] = zoneService.AllUsers.GetCount();
            }
            //response.OperationCode
            //send data den peer xu ly
            peer.SendOperationResponse(response, new SendParameters());
        }
    }
}
