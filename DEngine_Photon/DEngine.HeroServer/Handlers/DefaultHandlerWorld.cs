using DEngine.Common;
using DEngine.Common.Config;
using DEngine.HeroServer.Properties;
using DEngine.PhotonFX.Common;
using DEngine.PhotonFX.Master;
using Photon.SocketServer;
using System;
using System.Collections.Generic;

namespace DEngine.HeroServer.Handlers
{
    public class DefaultHandlerWorld : DefaultHandlerMaster
    {
        static DefaultHandlerWorld()
        {
        }

        public DefaultHandlerWorld(PhotonApplication serverApp)
            : base(serverApp)
        {
        }

        protected override void OnHandleRequest(OperationRequest request, PeerBase peer)
        {
            OperationCode opCode = (OperationCode)request.OperationCode;
            OperationResponse response = new OperationResponse() { OperationCode = request.OperationCode };

            try
            {
                switch (opCode)
                {
                    case OperationCode.Register:
                        {
                            string userName = (string)request[(byte)ParameterCode.UserName];
                            string password = (string)request[(byte)ParameterCode.Password];
                            string email = (string)request[(byte)ParameterCode.Email];
                            string nickname = (string)request[(byte)ParameterCode.NickName];
                            response.ReturnCode = (short)OnRegister(userName, password, email, nickname);
                        }
                        break;

                    case OperationCode.ChangePass:
                        {
                            string userName = (string)request[(byte)ParameterCode.UserName];
                            string password = (string)request[(byte)ParameterCode.Password];
                            string newpassword = (string)request[(byte)ParameterCode.NewPassword];
                            response.ReturnCode = (short)OnChangePass(userName, password, newpassword);
                        }
                        break;

                    default:
                        response.ReturnCode = (short)ErrorCode.UnknownRequest;
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Warn(ex.Message);
                Log.Warn(ex.StackTrace);
                response.ReturnCode = (short)ErrorCode.InvalidParams;
            }

            //send result to sender client 
            peer.SendOperationResponse(response, new SendParameters() { Encrypted = true });
        }

        private ErrorCode OnRegister(string userName, string password, string email, string nickname)
        {
            string serviceUrl = string.Format("{0}/Account/Register?username={1}&password={2}&email={3}&nickname={4}", ServerConfig.SERVICE_URL, userName, password, email, nickname);
            int userId = HttpService.GetResponse(serviceUrl).Code;

            if (userId == -2)
                return ErrorCode.DuplicateUserName;

            if (userId == -3)
                return ErrorCode.DuplicateNickName;

            if (userId < 0)
                return ErrorCode.InvalidParams;

            return ErrorCode.Success;
        }

        private ErrorCode OnChangePass(string userName, string password, string newpassword)
        {
            string serviceUrl = string.Format("{0}/Account/ChangePass?username={1}&oldpass={2}&newpass={3}", ServerConfig.SERVICE_URL, userName, password, newpassword);
            int userId = HttpService.GetResponse(serviceUrl).Code;

            if (userId == -2)
                return ErrorCode.UserNotFound;

            if (userId == -3)
                return ErrorCode.InvalidPassword;

            if (userId < 0)
                return ErrorCode.InvalidParams;

            return ErrorCode.Success;
        }
    }
}
