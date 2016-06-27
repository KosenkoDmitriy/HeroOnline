using DEngine.PhotonFX.Common;
using Photon.SocketServer;

namespace DEngine.PhotonFX.Master
{
    public class MasterServer : PhotonApplication
    {
        #region Properties

        public SlavePeerCollection SlavePeers { get; private set; }
        public ClientPeerCollection ClientPeers { get; private set; }
        public UserDataCollection SignedInUsers { get; private set; }

        #endregion

        public MasterServer()
        {
            //peers Zone , when zone register to Master , Master add Zone to SlavePeers list 
            SlavePeers = new SlavePeerCollection();

            //Peer Client
            ClientPeers = new ClientPeerCollection();

            //list user loging .
            SignedInUsers = new UserDataCollection();
        }

        protected override IPhotonHandler CreateDefaultHandler()
        {
            return new DefaultHandlerMaster(this);
        }

        protected virtual bool IsServerPeer(InitRequest initRequest)
        {
            return (initRequest.LocalPort == MasterPort);
        }

        protected virtual MasterClientPeer CreateClientPeer(InitRequest initRequest)
        {
            return new MasterClientPeer(initRequest, this);
        }

        protected virtual MasterSlavePeer CreateSlavePeer(InitRequest initRequest)
        {
            return new MasterSlavePeer(initRequest, this);
        }

        //send event to all user of Zone 
        public void BroadCastSlaveEvent(EventData eventData)
        {
            Log.InfoFormat("BroadCastSlaveEvent Data...");
            SlavePeers.ForEach((MasterSlavePeer peer) => { peer.SendEvent(eventData, new SendParameters()); });
        }

        #region Overrides of ApplicationBase

        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            if (IsServerPeer(initRequest))
            {
                Log.InfoFormat("Received InitRequest from Slave {0}.", initRequest.ConnectionId);
                return CreateSlavePeer(initRequest);
            }

            Log.InfoFormat("Received InitRequest from Client {0}.", initRequest.ConnectionId);

            return CreateClientPeer(initRequest);
        }

        public override void HandleResponse(OperationResponse response, PeerBase peer)
        {
            DefaultHandler.HandleResponse(response, peer);
        }

        public override void HandleEvent(IEventData eventData, PeerBase peer)
        {
            DefaultHandler.HandleEvent(eventData, peer);
        }

        #endregion
    }
}
