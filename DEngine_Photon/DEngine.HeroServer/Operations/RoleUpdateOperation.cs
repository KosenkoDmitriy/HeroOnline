using DEngine.Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;

namespace DEngine.HeroServer.Operations
{
    public class RoleUpdateOperation : Operation
    {
        public RoleUpdateOperation()
        {
        }

        public RoleUpdateOperation(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request)
        {
        }

        [DataMember(Code = (byte)ParameterCode.ClientId, IsOptional = false)]
        public Guid ClientId { get; set; }

        [DataMember(Code = (byte)ParameterCode.UserId, IsOptional = false)]
        public int UserId { get; set; }

        [DataMember(Code = (byte)ParameterCode.RoleId, IsOptional = false)]
        public int RoleUId { get; set; }

        [DataMember(Code = (byte)ParameterCode.Param01, IsOptional = false)]
        public int Status { get; set; }

        [DataMember(Code = (byte)ParameterCode.Param02, IsOptional = false)]
        public int AIMode { get; set; }
    }
}
