using ExitGames.Logging;
using Photon.SocketServer;
using System;

namespace DEngine.PhotonFX.Common
{
    public abstract class PhotonHandler : IPhotonHandler
    {
        protected readonly ILogger Log = LogManager.GetCurrentClassLogger();
        protected readonly PhotonApplication ServerApp;

        public abstract HandlerType Type { get; }
        public abstract byte RequestCode { get; }
        public abstract byte ResponseCode { get; }
        public abstract byte EventCode { get; }

        public PhotonHandler(PhotonApplication serverApp)
        {
            ServerApp = serverApp;
        }

        public void HandleRequest(OperationRequest request, PeerBase peer)
        {
            OnHandleRequest(request, peer);
        }

        public void HandleResponse(OperationResponse response, PeerBase peer)
        {
            OnHandleResponse(response, peer);
        }

        public void HandleEvent(IEventData eventData, PeerBase peer)
        {
            OnHandleEvent(eventData, peer);
        }

        protected abstract void OnHandleRequest(OperationRequest request, PeerBase peer);

        protected abstract void OnHandleResponse(OperationResponse response, PeerBase peer);

        protected abstract void OnHandleEvent(IEventData eventData, PeerBase peer);
    }
}
