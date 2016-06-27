using DEngine.Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;

namespace DEngine.HeroServer.Operations
{
    public class RolesListOperation : Operation
    {
        public RolesListOperation()
        {
        }

        public RolesListOperation(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request)
        {
        }

        [DataMember(Code = (byte)ParameterCode.ClientId, IsOptional = false)]
        public Guid ClientId { get; set; }

        [DataMember(Code = (byte)ParameterCode.UserId, IsOptional = false)]
        public int UserId { get; set; }
    }
}
