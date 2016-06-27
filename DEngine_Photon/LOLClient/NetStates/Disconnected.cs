using ExitGames.Client.Photon;

namespace LOLClient
{
    public class Disconnected : NetworkState
    {
        public Disconnected(PhotonPeer peer)
            : base(peer)
        {
        }

        public override void OnUpdate()
        {
        }

        public override void SendOperation(OperationRequest request, bool sendReliable, byte channelId, bool encrypt)
        {
        }
    }
}
