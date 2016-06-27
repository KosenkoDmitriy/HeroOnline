using DEngine.Common;
using DEngine.PhotonFX.Common;
using ExitGames.Logging;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using System;
using System.Collections.Generic;

namespace DEngine.PhotonFX.Master
{
    public class MasterClientPeer : PeerBase
    {
        #region Constants and Fields

        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private readonly MasterServer _masterServer;

        #endregion

        #region Properties

        public bool IsInitialized { get { return (CryptoProvider != null && CryptoProvider.IsInitialized); } }

        public Guid ClientId { get; private set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public MasterSlavePeer CurrentZone { get; set; }

        #endregion

        #region Constructors & Overrides

        public MasterClientPeer(InitRequest initRequest, MasterServer masterServer)
            : base(initRequest.Protocol, initRequest.PhotonPeer)
        {
            ClientId = Guid.NewGuid();
            _masterServer = masterServer;
            int clietCount = _masterServer.ClientPeers.OnConnect(ClientId, this);
            Log.InfoFormat("Client {0} connected. IP = {1}, MasterCCU = {2}", ClientId, initRequest.RemoteIP, clietCount);
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            SignOutCurrentUser(false);
            int clientCount = _masterServer.ClientPeers.OnDisconnect(ClientId);
            Log.InfoFormat("Client {0} disconnected with reason: {1}. MasterCCU = {2}", ClientId, reasonCode, clientCount);
        }

        protected override void OnOperationRequest(OperationRequest request, SendParameters sendParameters)
        {
            OnHandleRequests(request);
        }

        #endregion

        #region Other Methods

        public void SignOutCurrentUser(bool disConnect = true)
        {
            if (UserName != null)
                OnHandleRequests(new OperationRequest((byte)OperationCode.SignOut, new object()));

            if (disConnect)
                Disconnect();
        }

        private void OnHandleRequests(OperationRequest request)
        {
            if (!IsInitialized)
            {
                OnUnauthorizedRequest(request);
                return;
            }

            OperationCode requestCode = (OperationCode)request.OperationCode;

            request[(byte)ParameterCode.ClientId] = ClientId.ToByteArray();
            request[(byte)ParameterCode.RemoteIP] = RemoteIP;
            request[(byte)ParameterCode.UserId] = UserId;

            // Handle WorldRequest
            if (requestCode > OperationCode.World_Begin && requestCode < OperationCode.World_End)
            {
                HandleWorldRequests(request);
                return;
            }

            // Handle ZoneRequest
            if (requestCode > OperationCode.Zone_Begin && requestCode < OperationCode.Zone_End)
            {
                HandleZoneRequests(request);
                return;
            }
        }

        private void HandleWorldRequests(OperationRequest request)
        {
            OperationCode requestCode = (OperationCode)request.OperationCode;

            switch (requestCode)
            {
                case OperationCode.ZonesList:
                    ProcessZonesList(request);
                    break;

                case OperationCode.SignIn:
                    ProcsessSignIn(request);
                    break;

                case OperationCode.SignOut:
                    ProcessSignOut(request);
                    break;

                default:
                    _masterServer.HandleRequest(request, this);
                    break;
            }
        }

        private void ProcessZonesList(OperationRequest request)
        {
            OperationResponse response = new OperationResponse(request.OperationCode, new object());
            response[(byte)ParameterCode.ZoneList] = _masterServer.SlavePeers.GetZonesList();

            SendOperationResponse(response, new SendParameters() { Encrypted = true });
        }

        private void ProcsessSignIn(OperationRequest request)
        {
            SignInOperation requestData = new SignInOperation(this.Protocol, request);
            if (!requestData.IsValid)
            {
                OnUnauthorizedRequest(request, ErrorCode.InvalidParams);
                return;
            }

            // Check DuplicateLogin
            MasterClientPeer otherPeer = _masterServer.SignedInUsers[requestData.UserName];
            if (otherPeer != null)
            {
                otherPeer.SignOutCurrentUser();
                OnUnauthorizedRequest(request, ErrorCode.DuplicateLogin);
                return;
            }

            // Forward SignIn request
            MasterSlavePeer signInZone;
            if (!_masterServer.SlavePeers.TryGetValue(requestData.ZoneId, out signInZone))
                OnUnauthorizedRequest(request, ErrorCode.InvalidParams);
            else
            {
                UserName = requestData.UserName;
                _masterServer.SignedInUsers.OnConnect(UserName, this);

                // Send SignIn request
                signInZone.SendOperationRequest(request, new SendParameters());
            }
        }

        private bool ProcessSignOut(OperationRequest request)
        {
            if (CurrentZone != null)
            {
                request[(byte)ParameterCode.UserName] = UserName;
                CurrentZone.SendOperationRequest(request, new SendParameters());
            }

            _masterServer.SignedInUsers.OnDisconnect(UserName);

            UserId = 0;
            UserName = null;
            return false;
        }

        private void HandleZoneRequests(OperationRequest request)
        {
            if (UserId == 0 || CurrentZone == null)
            {
                OnUnauthorizedRequest(request);
                return;
            }

            // Forward ClientRequest to CurrentZone
            CurrentZone.SendOperationRequest(request, new SendParameters());
        }

        private void OnUnauthorizedRequest(OperationRequest request, ErrorCode returnCode = ErrorCode.OperationDedined)
        {
            OperationResponse response = new OperationResponse(request.OperationCode);
            response.ReturnCode = (short)returnCode;
            SendOperationResponse(response, new SendParameters() { Encrypted = true });
        }

        #endregion
    }
}
