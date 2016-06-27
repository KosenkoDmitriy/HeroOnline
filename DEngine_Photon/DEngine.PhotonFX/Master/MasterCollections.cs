using DEngine.PhotonFX.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DEngine.PhotonFX.Master
{
    public class SlavePeerCollection : Dictionary<Guid, MasterSlavePeer>
    {
        #region Public Methods

        public Dictionary<string, string> GetZonesList()
        {
            Dictionary<string, string> zoneList = this.ToDictionary(server => server.Key.ToString(), server => server.Value.ToString());
            return zoneList;
        }

        public new MasterSlavePeer this[Guid key]
        {
            get
            {
                lock (this)
                {
                    try { return base[key]; }
                    catch { return null; }
                }
            }
        }

        public void OnConnect(MasterSlavePeer slavePeer)
        {
            Guid serverId = slavePeer.ServerId;

            lock (this)
            {
                MasterSlavePeer oldPeer;
                if (TryGetValue(serverId, out oldPeer))
                {
                    oldPeer.Disconnect();
                    Remove(serverId);
                }

                Add(serverId, slavePeer);
            }
        }

        public void OnDisconnect(MasterSlavePeer slavePeer)
        {
            Guid serverId = slavePeer.ServerId;

            lock (this)
            {
                MasterSlavePeer oldPeer;
                if (TryGetValue(serverId, out oldPeer))
                    Remove(serverId);
            }
        }

        public void ForEach(Action<MasterSlavePeer> action)
        {
            lock (this)
            {
                foreach (MasterSlavePeer peer in Values)
                    action(peer);
            }
        }

        #endregion
    }

    public class ClientPeerCollection : Dictionary<Guid, MasterClientPeer>
    {
        #region Public Methods

        public new MasterClientPeer this[Guid key]
        {
            get
            {
                lock (this)
                {
                    try { return base[key]; }
                    catch { return null; }
                }
            }
        }
        
        public int OnConnect(Guid clientId, MasterClientPeer clientPeer)
        {
            lock (this)
            {
                Add(clientId, clientPeer);
                return Count;
            }
        }

        public int OnDisconnect(Guid clientId)
        {
            lock (this)
            {
                Remove(clientId);
                return Count;
            }
        }

        public void ForEach(Action<MasterClientPeer> action)
        {
            lock (this)
            {
                foreach (MasterClientPeer peer in Values)
                {
                    if (peer.IsInitialized)
                        action(peer);
                }
            }
        }

        #endregion
    }

    public class UserDataCollection : Dictionary<string, MasterClientPeer>
    {
        #region Public Methods

        public new MasterClientPeer this[string key]
        {
            get
            {
                lock (this)
                {
                    try { return base[key]; }
                    catch { return null; }
                }
            }
        }
        
        public void OnConnect(string userName, MasterClientPeer clientPeer)
        {
            lock (this)
            {
                if (userName != null)
                    base[userName] = clientPeer;
            }
        }

        public void OnDisconnect(string userName)
        {
            lock (this)
            {
                if (userName != null)
                    Remove(userName);
            }
        }

        #endregion
    }
}
