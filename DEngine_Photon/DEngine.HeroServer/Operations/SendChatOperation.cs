using DEngine.Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;

namespace DEngine.HeroServer.Operations
{
    public class SendChatOperation : Operation
    {
        public SendChatOperation()
        {
        }

        public SendChatOperation(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request)
        {
        }

        [DataMember(Code = (byte)ParameterCode.ClientId, IsOptional = false)]
        public Guid ClientId { get; set; }

        [DataMember(Code = (byte)ParameterCode.UserId, IsOptional = false)]
        public int SenderId { get; set; }

        [DataMember(Code = (byte)ParameterCode.TargetId, IsOptional = false)]
        public int TargetId { get; set; }

        [DataMember(Code = (byte)ParameterCode.Message, IsOptional = false)]
        public string Message { get; set; }
    }
}
