using DEngine.Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;

namespace DEngine.HeroServer.Operations
{
    public class ShopBuyOperation : Operation
    {
        public ShopBuyOperation()
        {
        }

        public ShopBuyOperation(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request)
        {
        }

        [DataMember(Code = (byte)ParameterCode.ClientId, IsOptional = false)]
        public Guid ClientId { get; set; }

        [DataMember(Code = (byte)ParameterCode.UserId, IsOptional = false)]
        public int UserId { get; set; }

        [DataMember(Code = (byte)ParameterCode.TargetId, IsOptional = false)]
        public int ShopId { get; set; }

        [DataMember(Code = (byte)ParameterCode.ItemId, IsOptional = true)]
        public int ItemId { get; set; }

        [DataMember(Code = (byte)ParameterCode.ItemCount, IsOptional = true)]
        public int ItemCount { get; set; }
    }
}
