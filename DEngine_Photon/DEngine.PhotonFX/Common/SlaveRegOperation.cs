using System;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace DEngine.PhotonFX.Common
{
    public class SlaveRegOperation : Operation
    {
        public SlaveRegOperation()
        {
        }

        public SlaveRegOperation(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request)
        {
        }

        #region Properties

        [DataMember(Code = 1, IsOptional = false)]
        public string ServerId { get; set; }

        [DataMember(Code = 2, IsOptional = false)]
        public string ServerName { get; set; }

        [DataMember(Code = 3, IsOptional = false)]
        public string ServerAddress { get; set; }

        [DataMember(Code = 4, IsOptional = true)]
        public int? TcpPort { get; set; }

        [DataMember(Code = 5, IsOptional = true)]
        public int? UdpPort { get; set; }

        [DataMember(Code = 6, IsOptional = true)]
        public int? ZoneMaxCCU { get; set; }

        #endregion
    }
}
