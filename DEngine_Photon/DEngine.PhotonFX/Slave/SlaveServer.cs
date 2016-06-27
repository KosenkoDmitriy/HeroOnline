using DEngine.PhotonFX.Common;
using Photon.SocketServer;
using Photon.SocketServer.ServerToServer;
using System;
using System.Net;
using System.Threading;

namespace DEngine.PhotonFX.Slave
{
    public abstract class SlaveServer : PhotonApplication
    {
        #region Constants and Fields

        public static readonly Guid ServerId = Guid.NewGuid();

        private SlaveMasterPeer _masterPeer;
        private byte _isReconnecting;
        private Timer _retry;

        #endregion

        #region Properties

        protected SlaveMasterPeer MasterPeer { get { return _masterPeer; } }

        protected IPAddress IpAddress { get; set; }

        protected int? TcpPort { get; set; }

        protected int? UdpPort { get; set; }

        protected IPEndPoint MasterEndPoint { get; set; }

        protected int ConnectRetryInterval { get; set; }

        #endregion

        public SlaveServer()
        {
            IpAddress = IPAddress.Parse("127.0.0.1");
            TcpPort = 4530;
            UdpPort = 5055;

            IPAddress address = IPAddress.Parse(MasterIP);
            MasterEndPoint = new IPEndPoint(address, MasterPort);
            ConnectRetryInterval = 15;
        }

        protected override IPhotonHandler CreateDefaultHandler()
        {
            return new DefaultHandlerSlave(this);
        }

        internal protected virtual void OnMasterConnected()
        {
            RegisterWithMaster();
        }

        internal protected virtual void OnMasterDisconnected()
        {
            ReconnectToMaster();
        }

        internal protected virtual void OnMasterRegistered()
        {
        }

        private void ConnectToMaster()
        {
            if (ConnectToServerTcp(MasterEndPoint, "Master", "MasterConnect") == false)
            {
                Log.ErrorFormat("Master connection failed.");
                return;
            }

            Log.InfoFormat(_isReconnecting == 0 ? "Connecting to Master at {0}." : "Reconnecting to Master at {0}.", MasterEndPoint);
        }

        private void ReconnectToMaster()
        {
            Thread.VolatileWrite(ref _isReconnecting, 1);
            _retry = new Timer(o => ConnectToMaster(), null, ConnectRetryInterval * 1000, 0);
        }

        private void RegisterWithMaster()
        {
            OperationRequest request = new OperationRequest((byte)ServerOperationCode.SlaveRegister,
                new SlaveRegOperation()
                {
                    ServerId = ServerId.ToString(),
                    ServerName = ApplicationName,
                    ServerAddress = IpAddress.ToString(),
                    TcpPort = TcpPort,
                    UdpPort = UdpPort,
                    ZoneMaxCCU = ZoneMaxCCU,
                });

            _masterPeer.SendOperationRequest(request, new SendParameters());
        }

        #region Overrides of ApplicationBase

        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            Log.WarnFormat("Connection rejected from {0}:{1}", initRequest.RemoteIP, initRequest.RemotePort);
            return null;
        }

        protected override ServerPeerBase CreateServerPeer(InitResponse initResponse, object state)
        {
            Thread.VolatileWrite(ref _isReconnecting, 0);
            _masterPeer = new SlaveMasterPeer(initResponse, this);
            return _masterPeer;
        }

        protected override void OnServerConnectionFailed(int errorCode, string errorMessage, object state)
        {
            if (_isReconnecting == 0)
                Log.ErrorFormat("Master connection failed with error {0}:{1}", errorCode, errorMessage);

            string stateString = (string)state;
            if (stateString != null && stateString.Equals("MasterConnect"))
                ReconnectToMaster();
        }

        protected override void Setup()
        {
            base.Setup();

            ConnectToMaster();
        }

        #endregion
    }
}
