using DEngine.Common;
using DEngine.Common.GameLogic;
using DEngine.HeroServer.Operations;
using DEngine.PhotonFX.Common;
using DEngine.PhotonFX.Slave;
using Photon.SocketServer;
using System.Collections.Generic;

namespace DEngine.HeroServer.Handlers
{
    public class UserUpdateHandler : DefaultHandlerSlave
    {
        public override HandlerType Type { get { return HandlerType.Request; } }

        //register network event OperationCode.UserUpdate for this handle 
        public override byte RequestCode { get { return (byte)OperationCode.UserUpdate; } }

        public UserUpdateHandler(PhotonApplication serverApp)
            : base(serverApp)
        {
        }

        protected override void OnHandleRequest(OperationRequest request, PeerBase peer)
        {
            UserUpdateOperation requestData = new UserUpdateOperation(peer.Protocol, request);

            OperationResponse response = new OperationResponse(request.OperationCode, new object());
            response[(byte)ParameterCode.ClientId] = requestData.ClientId.ToByteArray();

            if (!requestData.IsValid)
            {
                response.ReturnCode = (short)ErrorCode.InvalidParams;
                response.DebugMessage = requestData.GetErrorMessage();
            }
            else
            {
                response[(byte)ParameterCode.SubCode] = requestData.SubCode;

                GameUser gameUser = null;
                ErrorCode errCode = (ServerApp as ZoneService).OnUserUpdate(requestData, ref gameUser);

                response[(byte)ParameterCode.UserBase] = Serialization.SaveStruct(gameUser.Base);
                response[(byte)ParameterCode.UserLand] = Serialization.Save(gameUser.Land);

                if (errCode == ErrorCode.Success)
                {
                    switch ((SubCode)requestData.SubCode)
                    {
                        case SubCode.ItemSell:
                        case SubCode.ItemUpgrade:
                        case SubCode.TutorStep:
                            response[(byte)ParameterCode.UserItems] = Serialization.SaveArray(gameUser.UserItems.ToArray(), true);
                            break;
                        case SubCode.RoleUpExp:
                        case SubCode.RoleUpStar:
                            response[(byte)ParameterCode.UserRoles] = Serialization.SaveArray(gameUser.UserRoles.ToArray(), true);
                            break;
                        case SubCode.GetFriend:
                        case SubCode.AddFriend:
                            response[(byte)ParameterCode.Friends] = Serialization.SaveArray(gameUser.UserFriends.ToArray(), true);
                            break;
                        case SubCode.CheckEmail:
                            response[(byte)ParameterCode.UserMails] = Serialization.SaveArray(gameUser.UserMails.ToArray(), true);
                            break;
                        case SubCode.ReadEmail:
                            response[(byte)ParameterCode.UserMails] = Serialization.SaveArray(gameUser.UserMails.ToArray(), true);
                            response[(byte)ParameterCode.UserItems] = Serialization.SaveArray(gameUser.UserItems.ToArray(), true);
                            break;
                        case SubCode.CheckReport:
                            response[(byte)ParameterCode.UserPvPLog] = Serialization.SaveArray(gameUser.PvPLogs.ToArray(), true);
                            response[(byte)ParameterCode.UserPvALog] = Serialization.SaveArray(gameUser.PvALogs.ToArray(), true);
                            break;
                        case SubCode.OnlineAward:
                            if (gameUser.OnlineAward != null)
                            {
                                response[(byte)ParameterCode.GameAward] = Serialization.SaveStruct(gameUser.OnlineAward);
                                response[(byte)ParameterCode.UserItems] = Serialization.SaveArray(gameUser.UserItems.ToArray(), true);
                            }
                            break;

                        case SubCode.ExpandLand:
                        case SubCode.OpenLandCell:
                        case SubCode.BuildHouse:
                        case SubCode.DestroyHouse:
                            response[(byte)ParameterCode.UserLand] = Serialization.Save(gameUser.Land);
                            break;

                        case SubCode.SetHire:
                            response[(byte)ParameterCode.HireRoles] = Serialization.SaveArray(gameUser.HireRoles.ToArray(), true);
                            break;

                        case SubCode.GetHires:
                            {
                                List<UserRoleHire> hireRoles = (ServerApp as ZoneService).HeroDatabase.GetHireRoles(gameUser.Base.Level);
                                response[(byte)ParameterCode.HireRoles] = Serialization.SaveArray(hireRoles.ToArray(), true);
                                break;
                            }
                    }
                }
                else if (errCode == ErrorCode.ItemsUpgradeFailed)
                    response[(byte)ParameterCode.UserItems] = Serialization.SaveArray(gameUser.UserItems.ToArray(), true);

                response.ReturnCode = (short)errCode;
            }

            //send info of require to client
            peer.SendOperationResponse(response, new SendParameters());
        }
    }
}
