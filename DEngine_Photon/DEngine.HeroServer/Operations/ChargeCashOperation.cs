using DEngine.Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;

namespace DEngine.HeroServer.Operations
{
    public class ChargeCashOperation : Operation
    {
        public ChargeCashOperation()
        {
        }

        public ChargeCashOperation(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request)
        {
        }

       
        [DataMember(Code = (byte)ParameterCode.ClientId, IsOptional = false)]
        public Guid ClientId { get; set; }

       
        [DataMember(Code = (byte)ParameterCode.UserId, IsOptional = false)]
        public int UserId { get; set; }

        [DataMember(Code = (byte)ParameterCode.Param01, IsOptional = false)]
        public int Param01 { get; set; }

        [DataMember(Code = (byte)ParameterCode.Param02, IsOptional = false)]
        public int Param02 { get; set; }

        [DataMember(Code = (byte)ParameterCode.Param03, IsOptional = false)]
        public string Param03 { get; set; }

        [DataMember(Code = (byte)ParameterCode.Param04, IsOptional = true)]
        public string Param04 { get; set; }
    }
}
