using DEngine.Common;
using DEngine.Common.Config;
using DEngine.Common.GameLogic;
using DEngine.HeroServer.Operations;
using DEngine.PhotonFX.Common;
using DEngine.PhotonFX.Slave;
using Photon.SocketServer;
using System.Collections.Generic;

namespace DEngine.HeroServer.Handlers
{
    public class ChargeCashHandler : DefaultHandlerSlave
    {
        public override HandlerType Type { get { return HandlerType.Request; } }
       
        //register OperationCode.ChargeCash event hanlde
        public override byte RequestCode { get { return (byte)OperationCode.ChargeCash; } }

        public ChargeCashHandler(PhotonApplication serverApp)
            : base(serverApp)
        {
        }

        protected override void OnHandleRequest(OperationRequest request, PeerBase peer)
        {
            ChargeCashOperation requestData = new ChargeCashOperation(peer.Protocol, request);

            OperationResponse response = new OperationResponse(request.OperationCode, new object());
            response[(byte)ParameterCode.ClientId] = requestData.ClientId.ToByteArray();

            if (!requestData.IsValid)
            {
                response.ReturnCode = (short)ErrorCode.InvalidParams;
                response.DebugMessage = requestData.GetErrorMessage();
            }
            else
            {
                response[(byte)ParameterCode.CardAmount] = 0;
                response[(byte)ParameterCode.ChargeGold] = 0;
                response[(byte)ParameterCode.ChargeSilver] = 0;

                Log.InfoFormat("ChargeCashOperation ({0} => {1} => {2})", requestData.Param01, requestData.Param02, requestData.Param03);

                switch ((ChargeType)requestData.Param01)
                {
                    case ChargeType.MobileCard:
                        OnChargeMobileCard(response, (CardType)requestData.Param02, requestData.UserId, requestData.Param03, requestData.Param04);
                        break;

                    case ChargeType.GoogleStore:
                        OnChargeGoogleStore(response, requestData.UserId, requestData.Param03, requestData.Param04);
                        break;

                    case ChargeType.AppleStore:
                        OnChargeAppleStore(response, requestData.UserId, requestData.Param03, requestData.Param04);
                        break;
                }
            }

            peer.SendOperationResponse(response, new SendParameters());
        }

        private void OnChargeMobileCard(OperationResponse response, CardType cardType, int userId, string cardSeri, string cardCode)
        {
            response.ReturnCode = (short)ErrorCode.InvalidParams;

            string serviceUrl = string.Format("{0}/Account/ChargeCard?worldid={1}&userid={2}&cardtype={3}&cardseri={4}&cardcode={5}", ServerConfig.SERVICE_URL, ServerConfig.WORLD_ID, userId, (int)cardType, cardSeri, cardCode);
            int cardAmount = HttpService.GetResponse(serviceUrl).Code;
            if (cardAmount <= 0)
            {
                response.ReturnCode = (short)ErrorCode.MobileCardChargeFailed;
                return;
            }

            int goldAdd = cardAmount / 1000;
            switch (goldAdd)
            {
                case 30:
                    goldAdd += 5;
                    break;
                case 50:
                    goldAdd += 10;
                    break;
                case 100:
                    goldAdd += 20;
                    break;
                case 200:
                    goldAdd += 50;
                    break;
                case 300:
                    goldAdd += 90;
                    break;
                case 500:
                    goldAdd += 188;
                    break;
            }

            GameUser gameUser = null;
            ErrorCode errCode = ((ZoneService)ServerApp).OnCardCharge(userId, goldAdd, 0, ref gameUser);

            if (errCode == ErrorCode.Success)
            {
                if (gameUser != null)
                    response[(byte)ParameterCode.UserBase] = Serialization.SaveStruct(gameUser.Base);

                response[(byte)ParameterCode.CardAmount] = cardAmount;
                response[(byte)ParameterCode.ChargeGold] = goldAdd;
            }

            response.ReturnCode = (short)errCode;
        }

        private void OnChargeGoogleStore(OperationResponse response, int userId, string product, string token)
        {
            response.ReturnCode = (short)ErrorCode.InvalidParams;

            string serviceUrl = string.Format("{0}/Account/GoogleStore?worldid={1}&userid={2}&product={3}&token={4}", ServerConfig.SERVICE_URL, ServerConfig.WORLD_ID, userId, product, token);
            int cardAmount = HttpService.GetResponse(serviceUrl).Code;
            if (cardAmount <= 0)
            {
                response.ReturnCode = (short)ErrorCode.GoogleStoreChargeFailed;
                return;
            }

            int silverAdd = 0;
            int goldAdd = cardAmount;
            if (cardAmount > 1000)
            {
                goldAdd = 0;
                silverAdd = cardAmount;
            }

            GameUser gameUser = null;
            ErrorCode errCode = ((ZoneService)ServerApp).OnCardCharge(userId, goldAdd, silverAdd, ref gameUser);

            if (errCode == ErrorCode.Success)
            {
                if (gameUser != null)
                    response[(byte)ParameterCode.UserBase] = Serialization.SaveStruct(gameUser.Base);

                response[(byte)ParameterCode.CardAmount] = 0;
                response[(byte)ParameterCode.ChargeGold] = goldAdd;
                response[(byte)ParameterCode.ChargeSilver] = silverAdd;
            }

            response.ReturnCode = (short)errCode;
        }

        private void OnChargeAppleStore(OperationResponse response, int userId, string product, string receipt)
        {
            response.ReturnCode = (short)ErrorCode.InvalidParams;

            Dictionary<string, string> postData = new Dictionary<string, string>();
            postData.Add("worldid", ServerConfig.WORLD_ID.ToString());
            postData.Add("userid", userId.ToString());
            postData.Add("product", product);
            postData.Add("receipt", receipt);

            string serviceUrl = string.Format("{0}/Account/AppleStore", ServerConfig.SERVICE_URL);
            int cardAmount = HttpService.GetResponse(serviceUrl, postData).Code;
            if (cardAmount <= 0)
            {
                response.ReturnCode = (short)ErrorCode.AppleStoreChargeFailed;
                return;
            }

            int silverAdd = 0;
            int goldAdd = cardAmount;
            if (cardAmount > 1000)
            {
                goldAdd = 0;
                silverAdd = cardAmount;
            }

            GameUser gameUser = null;
            ErrorCode errCode = ((ZoneService)ServerApp).OnCardCharge(userId, goldAdd, silverAdd, ref gameUser);

            if (errCode == ErrorCode.Success)
            {
                if (gameUser != null)
                    response[(byte)ParameterCode.UserBase] = Serialization.SaveStruct(gameUser.Base);

                response[(byte)ParameterCode.CardAmount] = 0;
                response[(byte)ParameterCode.ChargeGold] = goldAdd;
                response[(byte)ParameterCode.ChargeSilver] = silverAdd;
            }

            response.ReturnCode = (short)errCode;
        }
    }
}
