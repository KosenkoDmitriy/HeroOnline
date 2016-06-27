using ExitGames.Client.Photon;
using System;

namespace DEngine.Unity.Photon
{
    public class PhotonEventArgs : EventArgs
    {
        public StatusCode StatusCode { get; set; }
        public EventData EventData { get; set; }
        public OperationResponse Response { get; set; }
    }
}
