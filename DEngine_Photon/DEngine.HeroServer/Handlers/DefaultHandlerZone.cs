using DEngine.Common;
using DEngine.Common.Config;
using DEngine.HeroServer.Properties;
using DEngine.PhotonFX.Common;
using DEngine.PhotonFX.Slave;
using Photon.SocketServer;
using System;

namespace DEngine.HeroServer.Handlers
{
    public class DefaultHandlerZone : DefaultHandlerSlave
    {
        public DefaultHandlerZone(PhotonApplication serverApp)
            : base(serverApp)
        {
        }

        protected override void OnHandleEvent(IEventData eventData, PeerBase peer)
        {
            EventCode eventCode = (EventCode)eventData.Code;
            ZoneService zoneService = (ServerApp as ZoneService);

            try
            {
                switch (eventCode)
                {
                    case DEngine.Common.EventCode.LoadData:
                        ReloadData(zoneService);
                        ReloadConfig(zoneService);
                        break;

                    case DEngine.Common.EventCode.UserKick:
                        {
                            int userId = (int)eventData[(byte)ParameterCode.TargetId];
                            Log.InfoFormat("UserKick {0}", userId);
                            zoneService.OnSignOut(userId, "");
                        }
                        break;

                    case DEngine.Common.EventCode.UserBlock:
                        {
                            int userId = (int)eventData[(byte)ParameterCode.TargetId];
                            Log.InfoFormat("UserKick {0}", userId);
                            zoneService.OnSignOut(userId, "");
                        }
                        break;

                    default:
                        Log.InfoFormat("OnHandleEvent {0}", eventCode);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.WarnFormat(ex.Message);
            }
        }

        private void ReloadData(ZoneService zoneService)
        {
            Log.InfoFormat("Reloading Data...");
            zoneService.HeroDatabase.LoadGlobalTables();
        }

        private void ReloadConfig(ZoneService zoneService)
        {
            Log.InfoFormat("Reloading Config...");
            BattleConfig.Reload();
            DungeonConfig.Reload();
            GameConfig.Reload();
            ItemConfig.Reload();
            MissionConfig.Reload();
            RoleConfig.Reload();
            SkillConfig.Reload();
            UserConfig.Reload();
        }
    }
}
