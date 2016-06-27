using ExitGames.Client.Photon;

namespace DEngine.Unity.Photon
{
    public class Connecting : NetworkState
    {
        public Connecting(PhotonPeer peer)
            : base(peer)
        {
        }

        public override void OnUpdate()
        {
            photonPeer.Service();
        }

        public override void SendOperation(OperationRequest request, bool sendReliable, byte channelId, bool encrypt)
        {
            photonPeer.OpCustom(request, sendReliable, channelId, encrypt);
        }
    }
}
