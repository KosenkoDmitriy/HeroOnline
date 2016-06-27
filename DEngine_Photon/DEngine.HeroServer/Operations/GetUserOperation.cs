using DEngine.Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;

namespace DEngine.HeroServer.Operations
{
    public class GetUserOperation : Operation
    {
        public GetUserOperation()
        {
        }

        public GetUserOperation(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request)
        {
        }

        [DataMember(Code = (byte)ParameterCode.ClientId, IsOptional = false)]
        public Guid ClientId { get; set; }

        [DataMember(Code = (byte)ParameterCode.UserId, IsOptional = false)]
        public int UserId { get; set; }

        [DataMember(Code = (byte)ParameterCode.TargetId, IsOptional = false)]
        public int TargetId { get; set; }
    }
}
