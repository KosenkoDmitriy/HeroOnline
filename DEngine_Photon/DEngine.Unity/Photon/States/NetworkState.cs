using ExitGames.Client.Photon;

namespace DEngine.Unity.Photon
{
    public class NetworkState
    {
        protected PhotonPeer photonPeer;

        protected NetworkState(PhotonPeer peer)
        {
            photonPeer = peer;
        }

        public virtual void OnUpdate()
        {
        }

        public virtual void SendOperation(OperationRequest request, bool sendReliable, byte channelId, bool encrypt)
        {
        }
    }
}
