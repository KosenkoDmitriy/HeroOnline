using DEngine.Common;
using DEngine.Common.Config;
using DEngine.Common.GameLogic;
using DEngine.HeroServer.GameData;
using Photon.SocketServer;
using System.Collections.Generic;
using System.Linq;

namespace DEngine.HeroServer
{
    public class MissionService : GamePlayService
    {
        private static int[] awardRank = new int[] { 0, 100, 120, 150 };

        private BattlePvE pveBattle;

        #region Properties

        public int itemRank { get; private set; }

        public int mobGrade { get; private set; }


        public override PlayMode Mode
        {
            get { return PlayMode.Mission; }
        }

        #endregion

        #region Methods

        public MissionService(ZoneService zone, GameUser gameUser, int mission, int difficulty = 0)
        {
            Id = mission;
            Zone = zone;
            switch (difficulty)
            {
                case 0:
                    itemRank = 1;
                    mobGrade = 1;
                    break;
                case 1:
                    itemRank = 2;
                    mobGrade = 3;
                    break;
                case 2:
                    itemRank = 3;
                    mobGrade = 5;
                    break;
            }

            GameUser = gameUser;
            GameUser.GamePlay = this;

            UseEnergy(gameUser, Id, difficulty);

            OnGamePlayBegin();
        }

        public override void OnBattleBegin(BattleService battle)
        {
        }

        public override void OnBattleEnd(BattleService battle, int result)
        {
            if (result <= 0)
                OnGamePlayEnd(result);
        }

        //send start battle
        public override void OnGamePlayBegin()
        {
            // Create PvEBattle;
            pveBattle = BattleService.CreatePvE(Zone, GameUser, false);

            //get index max of role in roles list , create ID list in battle for other (mod,rune ,boss,ellite) begin mobStartId , index ID of Mobs from mobStartId + 100
            int mobStartId = GameUser.ActiveRoles.Max(r => r.Id);
            pveBattle.Runes = GetRunes(pveBattle.GameUser1, pveBattle.GameUser2, Id, mobStartId + 100);
            pveBattle.Waves = GetWaves(pveBattle.GameUser2, Id, mobStartId + 200);
            pveBattle.Start();
        }

        //reborn to continue game 
        public override void OnGamePlayResume()
        {
            if (pveBattle != null)
            {
                pveBattle.IsDisposing = false;
                GameUser.BattleInit(0);

                foreach (UserRole rune in pveBattle.Runes)
                    rune.InitBattle(0, 0, 0);

                EventData eventData = new EventData((byte)EventCode.Battle, new object());
                eventData[(byte)ParameterCode.SubCode] = SubCode.MissionResume;
                Zone.SendUserEvent(GameUser, eventData);
            }
        }

        public override void OnGamePlayEnd(int result)
        {
            if (pveBattle != null)
                pveBattle.Dispose();

            if (result < 0)
            {
                if (GameUser.Base.MissionLevel <= Id)
                    GameUser.Base.MissionLevel = Id + 1;

                AddItemAdward(MissionConfig.MISSIONS[Id].GerRandItem(), 1);

                int userExp = MissionConfig.MISSIONS[Id].UserExp * awardRank[itemRank] / 100;
                int roleExp = MissionConfig.MISSIONS[Id].RoleExp * awardRank[itemRank] / 100;
                int silverAdd = MissionConfig.MISSIONS[Id].Silver * awardRank[itemRank] / 100;

                GameAward.UserExp = userExp;
                GameAward.RoleExp = roleExp;
                GameAward.Silver = silverAdd;

                if (GameUser.SlaveCount < 0)
                    GameAward.SilverEx = -GameAward.Silver * 10 / 100;
                else
                    GameAward.SilverEx = GameAward.Silver * GameUser.SlaveCount * 10 / 100;

                GameUser.AddCash(GameAward.Silver + GameAward.SilverEx, GameAward.Gold);
                GameUser.AddExp(GameAward.UserExp);
                foreach (UserRole role in GameUser.ActiveRoles)
                    role.AddExp(GameAward.RoleExp);

                // Add UserItems
                List<UserItem> addItems = new List<UserItem>();
                foreach (var item in GameAward.Items)
                {
                    UserItem userItem = GameUser.AddItem(item.Key, item.Value, itemRank);
                    if (userItem != null)
                        addItems.Add(userItem);
                }

                Zone.HeroDatabase.UserAddItems(GameUser, addItems, UserAction.None);

                string missionInfo = string.Format("{0}\n{1}", Id, itemRank);
                Zone.SendWorldMessage(GameUser, WorldMessage.Mission, missionInfo);
            }
            else
                GameAward.Clear();

            Zone.SendUserSync(GameUser, GameAward);

            EventData eventData = new EventData((byte)EventCode.Battle, new object());
            eventData[(byte)ParameterCode.SubCode] = SubCode.MissionEnd;
            eventData[(byte)ParameterCode.BattleRes] = result;
            Zone.SendUserEvent(GameUser, eventData);

            GameUser.GamePlay = null;
        }

        public override void OnItemDrop(float[] vecPos = null)
        {
            int dropIdx = GameUser.GamePlay.Id;
            SendItemDrop(MissionConfig.DROP_ITEMS[dropIdx], vecPos);
        }


        //check Energy for mission
        public static bool UseEnergy(GameUser gameUser, int mission, int difficult, bool justTest = false)
        {
            if (mission < 1 || mission >= MissionConfig.MISSIONS.Length)
                return false;

            int itemRank = difficult + 1;

            //require energy
            int reqEnegy = MissionConfig.MISSIONS[mission].Energy * awardRank[itemRank] / 100;

            foreach (UserRole userRole in gameUser.ActiveRoles)
            {
                if (userRole.Base.Energy < reqEnegy)
                    return false;
            }

            if (!justTest)
            {
                foreach (UserRole userRole in gameUser.ActiveRoles)
                    userRole.AddEnergy(-reqEnegy);
            }

            return true;
        }

        //get towers in config
        public List<UserRole> GetRunes(GameUser gameUser1, GameUser gameUser2, int mission, int runeUId)
        {
            List<UserRole> allRunes = new List<UserRole>();

            if (mission < 1 || mission >= MissionConfig.MISSIONS.Length)
                return allRunes;

            if (MissionConfig.MISSIONS[mission].Runes == null)
                return allRunes;

            foreach (RuneData rData in MissionConfig.MISSIONS[mission].Runes)
            {
                GameRole gameRole = (GameRole)Global.GameRoles[rData.Id];
                if (gameRole == null)
                    continue;

                UserRole userRole = new UserRole();
                userRole.GameUser = rData.Faction == 0 ? gameUser1 : gameUser2;
                userRole.CreateTemp(gameRole, runeUId++, mission, rData.Grade);
                userRole.InitBattle(rData.XPos, rData.YPos, rData.ZPos);
                allRunes.Add(userRole);
            }

            return allRunes;
        }

        //get waves in config by  mission id
        public List<BattleWave> GetWaves(GameUser gameUser, int mission, int mobUId)
        {
            List<BattleWave> allWaves = new List<BattleWave>();

            if (mission < 1 || mission >= MissionConfig.MISSIONS.Length)
                return allWaves;

            if (MissionConfig.MISSIONS[mission].Waves == null)
                return allWaves;

            foreach (WaveData wData in MissionConfig.MISSIONS[mission].Waves)
            {
                BattleWave wave = new BattleWave();
                wave.WaveState = WaitState.None;
                wave.WaitTime = wData.WaitTime;

                wave.WaveMobs = new List<UserRole>();

                List<int> mobList = wData.GetMobList();
                foreach (int rId in mobList)
                {
                    GameRole gameRole = (GameRole)Global.GameRoles[rId];
                    if (gameRole == null)
                        continue;

                    UserRole userRole = new UserRole();
                    userRole.GameUser = gameUser;

                    // Different with Dungeon, MobLevel will be read from config
                    userRole.CreateTemp(gameRole, mobUId++, wData.MobLevel, mobGrade);

                    while (true)
                    {
                        float posX = 9f * (float)Helpers.Random.NextDouble() - 4.5f;
                        float posZ = 9f * (float)Helpers.Random.NextDouble() - 4.5f;

                        if (posX * posX + posZ * posZ > 16f)
                        {
                            userRole.InitBattle(posX, 0, posZ);
                            break;
                        }
                    }

                    wave.WaveMobs.Add(userRole);
                }

                allWaves.Add(wave);
            }

            return allWaves;
        }

        #endregion
    }
}
