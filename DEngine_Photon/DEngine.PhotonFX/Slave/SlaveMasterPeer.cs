using DEngine.Common;
using ExitGames.Logging;
using Photon.SocketServer;
using Photon.SocketServer.ServerToServer;
using PhotonHostRuntimeInterfaces;
using DEngine.PhotonFX.Common;

namespace DEngine.PhotonFX.Slave
{
    public class SlaveMasterPeer : ServerPeerBase
    {
        #region Constants and Fields

        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private readonly SlaveServer _slaveServer;

        #endregion

        public SlaveMasterPeer(InitResponse initResponse, SlaveServer slaveServer)
            : base(initResponse.Protocol, initResponse.PhotonPeer)
        {
            _slaveServer = slaveServer;
            RequestFiber.Enqueue(_slaveServer.OnMasterConnected);
        }

        #region Overrides of PeerBase

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            Log.WarnFormat("Connection to Master closed (id = {0}).", this.ConnectionId);
            _slaveServer.OnMasterDisconnected();
        }

        protected override void OnOperationRequest(OperationRequest request, SendParameters sendParameters)
        {
            _slaveServer.HandleRequest(request, this);
        }

        #endregion

        #region Overrides of ServerPeerBase

        protected override void OnEvent(IEventData eventData, SendParameters sendParameters)
        {
            _slaveServer.HandleEvent(eventData, this);
        }

        protected override void OnOperationResponse(OperationResponse response, SendParameters sendParameters)
        {
            switch (response.OperationCode)
            {
                case (byte)ServerOperationCode.SlaveRegister:
                    if (response.ReturnCode != 0)
                    {
                        Log.WarnFormat("Failed to register at Master: err={0}, msg={1}.", response.ReturnCode, response.DebugMessage);
                        this.Disconnect();
                        return;
                    }

                    Log.InfoFormat("Successfully registered at MasterServer.");
                    _slaveServer.OnMasterRegistered();

                    break;

                default:
                    Log.WarnFormat("Unknown OperationResponse {0} from MasterServer.", response.OperationCode);
                    break;
            }
        }

        #endregion
    }
}
