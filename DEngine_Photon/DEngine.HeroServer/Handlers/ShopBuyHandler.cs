using DEngine.Common;
using DEngine.Common.GameLogic;
using DEngine.HeroServer.Operations;
using DEngine.PhotonFX.Common;
using DEngine.PhotonFX.Slave;
using Photon.SocketServer;
using System.Collections.Generic;

namespace DEngine.HeroServer.Handlers
{
    public class ShopBuyHandler : DefaultHandlerSlave
    {
        public override HandlerType Type { get { return HandlerType.Request; } }

        public override byte RequestCode { get { return (byte)OperationCode.ShopBuy; } }

        public ShopBuyHandler(PhotonApplication serverApp)
            : base(serverApp)
        {
        }

        protected override void OnHandleRequest(OperationRequest request, PeerBase peer)
        {
            ShopBuyOperation requestData = new ShopBuyOperation(peer.Protocol, request);

            OperationResponse response = new OperationResponse(request.OperationCode, new object());
            response[(byte)ParameterCode.ClientId] = requestData.ClientId.ToByteArray();

            Log.InfoFormat("shop buy ({0})", requestData.UserId);
            //Log.InfoFormat("shop buy ({0} => {1} => {2})", requestData.UserId, requestData.Param02, requestData.Param03);

            if (!requestData.IsValid)
            {
                response.ReturnCode = (short)ErrorCode.InvalidParams;
                response.DebugMessage = requestData.GetErrorMessage();
            }
            else
            {
                GameObj shopItem;
                GameUser gameUser = null;

                ErrorCode errCode = (ServerApp as ZoneService).OnShopBuy(requestData, ref gameUser, out shopItem);
                if (errCode == ErrorCode.Success)
                {
                    response[(byte)ParameterCode.UserBase] = Serialization.SaveStruct(gameUser.Base);

                    if (shopItem != null)
                    {
                        if (shopItem is UserItem)
                        {
                            UserItem userItem = (UserItem)shopItem;
                            response[(byte)ParameterCode.ItemKind] = (int)ItemKind.None;
                            response[(byte)ParameterCode.ShopItem] = Serialization.Save(userItem);
                        }
                        else if (shopItem is UserRole)
                        {
                            UserRole userRole = (UserRole)shopItem;
                            response[(byte)ParameterCode.ItemKind] = (int)ItemKind.Hero;
                            response[(byte)ParameterCode.ShopItem] = Serialization.Save(userRole);
                        }
                    }
                    else
                    {
                        response[(byte)ParameterCode.UserItems] = Serialization.SaveArray(gameUser.UserItems.ToArray(), true);
                        response[(byte)ParameterCode.UserRoles] = Serialization.SaveArray(gameUser.UserRoles.ToArray(), true);
                    }

                }

                response.ReturnCode = (short)errCode;
            }

            peer.SendOperationResponse(response, new SendParameters());
        }
    }
}
