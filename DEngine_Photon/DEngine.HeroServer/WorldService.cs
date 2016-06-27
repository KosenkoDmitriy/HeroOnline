using DEngine.Common;
using DEngine.Common.Config;
using DEngine.Common.GameLogic;
using DEngine.HeroServer.GameData;
using DEngine.HeroServer.Handlers;
using DEngine.PhotonFX.Common;
using DEngine.PhotonFX.Master;
using LOLServices.Models;
using NCrontab;
using Photon.SocketServer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DEngine.HeroServer
{
    public class WorldService : MasterServer
    {
        private static char[] cmdSplit = new char[] { ' ' }; 
        
        private DateTime curUpdateTime;

        protected override string MasterIP { get { return ServerConfig.MASTER_IP; } }
        protected override int MasterPort { get { return ServerConfig.MASTER_PORT; } }
        protected override int ZoneMaxCCU { get { return ServerConfig.ZONE_MAXCCU; } }

        public HeroDB HeroDatabase { get; private set; }

        public WorldService()
        {
            HeroDatabase = new HeroDB();
        }

        protected override IPhotonHandler CreateDefaultHandler()
        {
            return new DefaultHandlerWorld(this);
        }

        protected override void Update()
        {
            curUpdateTime = DateTime.Now;
            curUpdateTime.AddSeconds(-curUpdateTime.Second);

            LoadManagerCmds();

            UpdateDBLogs();

            foreach (var ge in GameConfig.GAME_EVENTS)
            {
                int eventId = ge.Key;
                GameEvent gameEvent = ge.Value;

                if (curUpdateTime < gameEvent.StartTime || curUpdateTime > gameEvent.EndTime)
                    continue;

                try
                {
                    CrontabSchedule schedule = CrontabSchedule.Parse(gameEvent.Schedule);
                    DateTime nextTime = schedule.GetNextOccurrence(gameEvent.LastRunTime);
                    if (nextTime < DateTime.Now)
                    {
                        DoEvent(gameEvent);
                        gameEvent.LastRunTime = DateTime.Now;
                    }
                }
                catch (Exception ex)
                {
                    Log.WarnFormat(ex.Message);
                    Log.WarnFormat(ex.StackTrace);
                }
            }
        }

        private void LoadManagerCmds()
        {
            try
            {
                string serviceUrl = string.Format("{0}/Manager/GetCommands?worldid={1}", ServerConfig.SERVICE_URL, ServerConfig.WORLD_ID);
                string cmdXml = HttpService.GetResponseXml(serviceUrl);
                CommandInfoList cmdList = CommandInfoList.FromXML(cmdXml);

                foreach (CommandInfo cmd in cmdList.Commands)
                    DoManagerCmd(cmd);
            }
            catch (Exception ex)
            {
                Log.WarnFormat("LoadManagerCmds Failed!");
                Log.WarnFormat(ex.Message);
            }
        }

        private void DoManagerCmd(CommandInfo command)
        {
            EventData eventData = new EventData() { Parameters = new Dictionary<byte, object>() };

            try
            {
                switch ((CommandCode)command.Code)
                {
                    case CommandCode.ZoneReload:
                        eventData.Code = (byte)EventCode.LoadData;
                        BroadCastSlaveEvent(eventData);
                        break;

                    case CommandCode.ZoneRestart:
                        break;

                    case CommandCode.ZoneMessage:
                        eventData.Code = (byte)EventCode.SendChat;
                        eventData[(byte)ParameterCode.UserId] = 0;
                        eventData[(byte)ParameterCode.Message] = command.Params;
                        ClientPeers.ForEach((MasterClientPeer clientPeer) =>
                        {
                            SendParameters sendParams = new SendParameters() { Encrypted = true };
                            clientPeer.SendEvent(eventData, sendParams);
                        });
                        break;

                    case CommandCode.UserMessage:
                        eventData.Code = (byte)EventCode.SendChat;
                        eventData[(byte)ParameterCode.UserId] = 0;
                        eventData[(byte)ParameterCode.TargetId] = command.UserId;
                        eventData[(byte)ParameterCode.Message] = command.Params;
                        BroadCastSlaveEvent(eventData);
                        break;

                    case CommandCode.UserKick:
                        {
                            MasterClientPeer clientPeer = SignedInUsers[command.UserName];
                            if (clientPeer != null)
                                clientPeer.SignOutCurrentUser();
                        }
                        break;

                    case CommandCode.UserBlock:
                        break;

                    case CommandCode.UserAddCash:
                        {
                            string[] cmdParams = command.Params.Split(cmdSplit, StringSplitOptions.RemoveEmptyEntries);
                            HeroDatabase.UserAddCashOnline(command.UserId, Int32.Parse(cmdParams[0]), Int32.Parse(cmdParams[1]));
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.WarnFormat("DoManagerCmd({0}) Failed!", command);
                Log.WarnFormat(ex.Message);
            }
        }

        private void UpdateDBLogs()
        {
            if (curUpdateTime.Minute % 10 == 0) // Update OnlineLog each 10 minutes
            {
                if (SlavePeers.Count < 1)
                    return;

                using (HeroDBDataContext dbContext = new HeroDBDataContext())
                {
                    OnlineLog newLog = new OnlineLog()
                    {
                        LogTime = curUpdateTime,
                        TotalCCU = SlavePeers.Sum(sl => sl.Value.ZoneCurCCU),
                    };

                    dbContext.OnlineLogs.InsertOnSubmit(newLog);
                    dbContext.SubmitChanges();
                }
            }
        }

        private void DoEvent(GameEvent gameEvent)
        {
            switch (gameEvent.Name)
            {
                case "WeeklyArenaAward":
                    WeeklyArenaAward(gameEvent);
                    break;
            }
        }

        private void WeeklyArenaAward(GameEvent gameEvent)
        {
            Log.InfoFormat("WeeklyArenaAward at {0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);

            HeroDatabase.UpdateUsersRank();
            GameObjList topUsers = HeroDatabase.GetTopUsers((int)UserListType.TopHonor, 1000);
            if (topUsers.Count < 1)
                return;
            
            GameUser gameUser1 = (GameUser)topUsers[0];
            UserItem userItem = new UserItem();
            userItem.CreateRandomeEquip(0, gameUser1.Base.Level, 4);

            GameObjList emailAttachments = new GameObjList();
            emailAttachments.Add(userItem);
            HeroDatabase.UserSendEmail(0, gameUser1.Id, "Arena Weekly Award", "Arena award for rank: 1", 0, 0, emailAttachments);

            int[] goldValues = Utilities.GetArrayInt(gameEvent.EventData);
            for (int i = 1; i < 1000 && i < topUsers.Count; i++)
            {
                int goldValue = 0;
                if (i <= 10)
                    goldValue = goldValues[i];
                else if (i <= 100)
                    goldValue = goldValues[11];
                else if (i <= 1000)
                    goldValue = goldValues[12];

                string mailContent = string.Format("Arena award for rank: {0}.", i, goldValue);

                if (goldValue > 0)
                    mailContent = string.Format("Arena award for rank: {0}. Gold = {1}", i, goldValue);

                HeroDatabase.UserSendEmail(0, topUsers[i].Id, "Arena Weekly Award", mailContent, 0, goldValue);
            }
        }
    }
}
