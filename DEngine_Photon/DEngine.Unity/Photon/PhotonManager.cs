using ExitGames.Client.Photon;
using UnityEngine;

namespace DEngine.Unity.Photon
{
    public class PhotonManager : MonoBehaviour, IPhotonPeerListener
    {
        public PhotonPeer Peer { get; private set; }
        public NetworkState State { get; private set; }
        public PhotonController Controller { get; private set; }

        #region Common Methods

        public static PhotonManager Initialize(PhotonController controller)
        {
            GameObject photonObj = GameObject.Find("PhotonManager");
            if (photonObj == null)
            {
                photonObj = new GameObject("PhotonManager");
                photonObj.AddComponent<PhotonManager>();
            }

            PhotonManager photonManager = photonObj.GetComponent<PhotonManager>();
            photonManager.Controller = controller;
            return photonManager;
        }

        public void Connect(string serverAdddress, string applicationName)
        {
            Disconnect();

            State = new Connecting(Peer);
            Peer.Connect(serverAdddress, applicationName);
        }

        public void Disconnect()
        {
            State = new Disconnected(Peer);
            Peer.Disconnect();
        }

        public void SendOperation(OperationRequest request, bool sendReliable, byte channelId, bool encrypt)
        {
            State.SendOperation(request, sendReliable, channelId, encrypt);
        }

        #endregion

        #region MonoBehaviour Methods

        void Awake()
        {
            Peer = new PhotonPeer(this, ConnectionProtocol.Tcp);
            State = new Disconnected(Peer);
        }

        void Start()
        {
            DontDestroyOnLoad(this);
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.runInBackground = true;
        }

        void Update()
        {
            State.OnUpdate();
        }

        void OnApplicationQuit()
        {
            Disconnect();
        }

        #endregion

        #region IPhotonPeerListener Implementation

        public void DebugReturn(DebugLevel level, string message)
        {
            Controller.OnDebugReturn(level, message);
        }

        public void OnEvent(EventData eventData)
        {
            Controller.OnEvent(eventData);
        }

        public void OnOperationResponse(OperationResponse operationResponse)
        {
            Controller.OnResponse(operationResponse);
        }

        public void OnStatusChanged(StatusCode statusCode)
        {
            switch (statusCode)
            {
                case StatusCode.Connect:
                    Peer.EstablishEncryption();
                    break;

                case StatusCode.Disconnect:
                case StatusCode.DisconnectByServer:
                case StatusCode.DisconnectByServerLogic:
                case StatusCode.DisconnectByServerUserLimit:
                case StatusCode.Exception:
                case StatusCode.ExceptionOnConnect:
                case StatusCode.TimeoutDisconnect:
                    State = new Disconnected(Peer);
                    Controller.OnDisconnect(statusCode);
                    break;

                case StatusCode.EncryptionEstablished:
                    State = new Connected(Peer);
                    Controller.OnConnect();
                    break;

                default:
                    Controller.OnStatusChanged(statusCode);
                    break;
            }
        }

        #endregion
    }
}
