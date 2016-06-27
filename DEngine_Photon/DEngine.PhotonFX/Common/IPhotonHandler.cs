using Photon.SocketServer;
using System;

namespace DEngine.PhotonFX.Common
{
    [Flags]
    public enum HandlerType
    {
        Request = 0x01,
        Response = 0x02,
        Event = 0x04,
        All = 0x07,
    }

    public interface IPhotonHandler
    {
        HandlerType Type { get; }

        byte RequestCode { get; }
        byte ResponseCode { get; }
        byte EventCode { get; }

        void HandleRequest(OperationRequest request, PeerBase peer);
        void HandleResponse(OperationResponse response, PeerBase peer);
        void HandleEvent(IEventData eventData, PeerBase peer);
    }
}
