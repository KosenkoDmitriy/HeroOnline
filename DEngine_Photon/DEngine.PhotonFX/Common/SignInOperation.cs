using DEngine.Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;

namespace DEngine.PhotonFX.Common
{
    public class SignInOperation : Operation
    {
        public SignInOperation()
        {
        }

        public SignInOperation(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request)
        {
        }

        [DataMember(Code = (byte)ParameterCode.ClientId, IsOptional = false)]
        public Guid ClientId { get; set; }

        [DataMember(Code = (byte)ParameterCode.ZoneId, IsOptional = false)]
        public Guid ZoneId { get; set; }

        [DataMember(Code = (byte)ParameterCode.RemoteIP, IsOptional = false)]
        public string RemoteIP { get; set; }

        [DataMember(Code = (byte)ParameterCode.UserName, IsOptional = false)]
        public string UserName { get; set; }

        [DataMember(Code = (byte)ParameterCode.Password, IsOptional = false)]
        public string Password { get; set; }
    }
}
