using DEngine.Common;
using DEngine.Common.Config;
using DEngine.Common.GameLogic;
using DEngine.HeroServer.Operations;
using ExitGames.Logging;
using Photon.SocketServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DEngine.HeroServer
{
    public class BattleService : IDisposable
    {
        public enum BattleCode
        {
            Start,
            EndUserQuit,
            EndUserKill,
            EndRuneKill,
            EndTimeOver,
        }

        #region Fields

        protected static ILogger Log = LogManager.GetCurrentClassLogger();

        //list of role character in battle (heroes of player , mob (monster) , rune,Heroes boss)
        public readonly GameObjCollection BattleRoles;

        private static GameClock clock = new GameClock();
        private Timer scheduleTimer;
        private float startTime;
        private float totalTime;
        private float deltaTime;
        private int readyCount;

        #endregion

        #region Properties

        public virtual bool IsDisposed { get; set; }

        public virtual bool IsDisposing { get; set; }

        public virtual BattleMode Mode { get { return BattleMode.Challenge; } }

        public virtual ZoneService Zone { get; protected set; }

        public virtual GameUser GameUser1 { get; protected set; }

        public virtual GameUser GameUser2 { get; protected set; }

        public virtual float TotalTime { get { return totalTime; } }

        public virtual float DeltaTime { get { return deltaTime; } }

        //count ready players ,  schedule call OnBattleUpdate() in 10 times per second
        public virtual int ReadyCount
        {
            get { return readyCount; }
            set
            {
                if (readyCount < 2)
                {
                    lock (this)
                    {
                        readyCount = value;

                        if (readyCount == 2)
                        {
                            startTime = clock.TotalTime;


                            scheduleTimer = new Timer(fn => { OnBattleUpdate(); }, null, 10, 20);

                            if (Mode == BattleMode.RandomPvP)
                            {
                                foreach (UserRole userRole in GameUser1.ActiveRoles)
                                    userRole.Base.Energy -= RoleConfig.ENERGY_MIN;

                                foreach (UserRole userRole in GameUser2.ActiveRoles)
                                    userRole.Base.Energy -= RoleConfig.ENERGY_MIN;
                            }

                            EventData beginEvent = new EventData((byte)EventCode.Battle, new object());
                            beginEvent[(byte)ParameterCode.SubCode] = SubCode.Ready;
                            SendBattleEvent(beginEvent);
                        }
                    }
                }
            }
        }

        #endregion

        #region Public Methods


        public static BattleService Create(ZoneService zone, BattleMode mode, GameUser user1, GameUser user2, bool autoStart = true)
        {
            BattleService newBattle = null;

            switch (mode)
            {
                case BattleMode.Challenge:
                    newBattle = new BattleService();
                    break;

                //player vs player mode
                case BattleMode.RandomPvP:
                    newBattle = new BattlePvP();
                    break;

                //player vs AI player
                case BattleMode.RandomPvA:
                    newBattle = new BattlePvA();
                    break;

                //Player vs monsters (wave)
                case BattleMode.RandomPvE:
                    newBattle = new BattlePvE();
                    break;
            }

            if (newBattle != null)
            {
                newBattle.Zone = zone;
                newBattle.GameUser1 = user1;
                newBattle.GameUser2 = user2;
                if (autoStart)
                    newBattle.Start();
            }

            return newBattle;
        }

        //Create Player vs monsters (wave) Mode
        public static BattlePvE CreatePvE(ZoneService zone, GameUser gameUser1, bool autoStart = true)
        {
            GameUser gameUser2 = new GameUser() { Id = 0, Name = "User02" };
            gameUser2.Base.NickName = "Mission";
            gameUser2.Base.Level = gameUser1.Base.Level;

            return (BattlePvE)Create(zone, BattleMode.RandomPvE, gameUser1, gameUser2, autoStart);
        }

        protected BattleService()
        {
            BattleRoles = new GameObjCollection();
        }

        public void Start()
        {
            //init players , add roles of Players to list of roles in battle
            Initialize();

            //start time of battle and send start play
            OnBattleBegin();
        }

        public void Dispose()
        {
            lock (this)
            {
                IsDisposing = true;

                if (!IsDisposed)
                {
                    GameUser1.Tag = null;
                    GameUser1.RefreshRoles();
                    GameUser1.Status = UserStatus.Default;

                    if (Mode <= BattleMode.RandomPvP)
                    {
                        GameUser2.Tag = null;
                        GameUser2.RefreshRoles();
                        GameUser2.Status = UserStatus.Default;
                    }

                    if (scheduleTimer != null)
                        scheduleTimer.Dispose();
                    BattleRoles.Clear();
                    IsDisposed = true;
                }
            }
        }


        //network event
        public ErrorCode OnRequest(BattleOperation requestData, GameUser gameUser)
        {
            lock (this)
            {
                if (IsDisposed || ReadyCount < 2)
                {
                    Log.WarnFormat("BattleRequest OperationDedined! ReadyCount = {0}", ReadyCount);
                    return ErrorCode.OperationDedined;
                }

                switch ((SubCode)requestData.SubCode)
                {
                    // di chuyển 
                    case SubCode.Move:
                        return OnUserMove(requestData, gameUser);

                    //command Attack to Target , di chuyển đến phạm vi tấn công để ra skill tấn công
                    case SubCode.Action:
                        return OnUserAction(requestData, gameUser);

                    // active a skill of a Role for User
                    case SubCode.SkillCast:
                        return OnUserSkillCast(requestData, gameUser);

                    //user say Hit target from user attack
                    case SubCode.SkillHit:
                        return OnUserSkillHit(requestData, gameUser);

                    //User get Item
                    case SubCode.ItemEat:
                        return OnUserItemEat(requestData, gameUser);

                    case SubCode.Quit:
                        return OnUserLeave(gameUser);
                }

                return ErrorCode.Success;
            }
        }

        //quit battle
        public ErrorCode OnUserLeave(GameUser gameUser)
        {
            lock (this)
            {
                if (gameUser == GameUser1)
                {
                    OnBattleEnd(1, BattleCode.EndUserQuit);
                    Dispose();

                    if (gameUser.GamePlay != null)
                        gameUser.GamePlay.OnGamePlayEnd(1);

                    return ErrorCode.Success;
                }
                else if (gameUser == GameUser2)
                {
                    OnBattleEnd(-1, BattleCode.EndUserQuit);
                    Dispose();

                    if (gameUser.GamePlay != null)
                        gameUser.GamePlay.OnGamePlayEnd(1);

                    return ErrorCode.Success;
                }

                return ErrorCode.InvalidParams;
            }
        }

        #endregion

        #region Protected Methods

        //init players
        protected virtual void Initialize()
        {
            GameUser1.EnemyUser = GameUser2;
            GameUser2.EnemyUser = GameUser1;

            //save battle pointer
            GameUser1.Tag = this;

            GameUser1.BattleInit(0);

            //add roles of user1 in list of Roles in battle
            foreach (UserRole role in GameUser1.ActiveRoles)
                BattleRoles.LockAdd(role);

            GameUser2.Tag = this;
            GameUser2.BattleInit(1);
            foreach (UserRole role in GameUser2.ActiveRoles)
                BattleRoles.LockAdd(role);
        }


        //start time battle and send start play
        protected virtual void OnBattleBegin()
        {
            lock (this)
            {
                Log.InfoFormat("OnBattleBegin Mode={0}. Users: {1} vs. {2}", Mode, GameUser1.Name, GameUser2.Name);

                GameBattle battleData = new GameBattle()
                {
                    Mode = this.Mode,
                    User01 = GameUser1,
                    User02 = GameUser2,
                };

                EventData beginEvent = new EventData((byte)EventCode.Battle, new object());
                beginEvent[(byte)ParameterCode.SubCode] = SubCode.Begin;
                beginEvent[(byte)ParameterCode.BattleTime] = BattleConfig.BATTLE_DURATION;
                beginEvent[(byte)ParameterCode.BattleData] = Serialization.Save(battleData);
                beginEvent[(byte)ParameterCode.BattleRoles] = Serialization.Save(BattleRoles, true);
                SendBattleEvent(beginEvent);
            }
        }

        //  scheduleTimer call 10 times in a second 
        protected virtual void OnBattleUpdate()
        {
            lock (this)
            {
                if (IsDisposed || IsDisposing)
                    return;

                float curTime = clock.TotalTime - startTime;
                deltaTime = curTime - totalTime;
                totalTime = curTime;

                // Update all roles
                foreach (UserRole role in BattleRoles)
                {
                    if (role.IsDeath)
                        continue;

                    role.Update(deltaTime);
                }

                // Sync changed roles
                OnBattleSync();

                // Check live roles
                OnBattleLive();
            }
        }

        //send battle sync ,send changes  of roles(hero,mob,boss..) in battle to player for update
        protected virtual void OnBattleSync()
        {
            List<UserRole.RoleState> roleStates = new List<UserRole.RoleState>();

            foreach (UserRole role in BattleRoles)
            {
                if (!role.StateChanged)
                    continue;

                roleStates.Add(role.State);
                role.StateChanged = false;
            }

            // BattleSync
            if (roleStates.Count > 0)
            {
                float remainTime = BattleConfig.BATTLE_DURATION - TotalTime;
                EventData eventData = new EventData((byte)EventCode.BattleSync, new object());
                eventData[(byte)ParameterCode.BattleTime] = remainTime > 0 ? remainTime : 0;
                eventData[(byte)ParameterCode.RoleState] = Serialization.SaveStructArray(roleStates.ToArray());
                SendBattleEvent(eventData);
            }
        }


        //check if no longer exists opponent to fight then end game
        protected virtual void OnBattleLive()
        {
            if (totalTime > BattleConfig.BATTLE_DURATION)
            {
                int totalDamage1 = GameUser1.ActiveRoles.Sum(r => r.State.Damage);
                int totalDamage2 = GameUser2.ActiveRoles.Sum(r => r.State.Damage);

                OnBattleEnd(totalDamage1 - totalDamage2, BattleCode.EndTimeOver);
                Dispose();
            }
            else
            {
                int liveCount1 = GameUser1.ActiveRoles.Count(r => !r.IsDeath);
                int liveCount2 = GameUser2.ActiveRoles.Count(r => !r.IsDeath);

                if (liveCount1 > 0 && liveCount2 > 0)
                    return;

                Log.InfoFormat("{0}={1}, {2}={3}", GameUser1.Name, liveCount1, GameUser2.Name, liveCount2);

                OnBattleEnd(liveCount2 - liveCount1, BattleCode.EndUserKill);
                Dispose();
            }
        }


        //send battle end
        protected virtual void OnBattleEnd(int result, BattleCode code)
        {
            EventData eventData = new EventData((byte)EventCode.Battle, new object());
            eventData[(byte)ParameterCode.SubCode] = SubCode.End;
            eventData[(byte)ParameterCode.Param01] = GameUser1.Id;
            eventData[(byte)ParameterCode.Param02] = (Mode <= BattleMode.RandomPvP) ? GameUser2.Id : 0;
            eventData[(byte)ParameterCode.BattleRes] = result;
            SendBattleEvent(eventData);
        }

        //send event to players in battle
        protected virtual void SendBattleEvent(EventData eventData)
        {
            Zone.SendUserEvent(GameUser1, eventData);

            //Challenge mode
            if (Mode <= BattleMode.RandomPvP)
                Zone.SendUserEvent(GameUser2, eventData);
        }

        //event Move of Player
        private ErrorCode OnUserMove(BattleOperation operation, GameUser gameUser)
        {
            if (operation.TargetPos == null)
                return ErrorCode.InvalidParams;

            //get userrole by RoleId in Battle , RoleId of Heroes from Role_uid from dbo.role table  (roles of all user) , RoleId of Mob(monster, boss , rune) = Max ID of Heroes in Battle +n
            UserRole userRole = (UserRole)BattleRoles[operation.RoleId];

            if (userRole == null || userRole.IsDeath)
                return ErrorCode.RoleNotFound;

            userRole.OnMove(operation.TargetPos);

            EventData eventData = new EventData((byte)EventCode.Battle, new object());
            eventData[(byte)ParameterCode.SubCode] = SubCode.Move;
            eventData[(byte)ParameterCode.UserId] = gameUser.Id;
            eventData[(byte)ParameterCode.RoleId] = operation.RoleId;
            eventData[(byte)ParameterCode.TargetPos] = operation.TargetPos;

            SendBattleEvent(eventData);

            return ErrorCode.Success;
        }

        //event command attack on target
        private ErrorCode OnUserAction(BattleOperation operation, GameUser gameUser)
        {
            UserRole userRole = (UserRole)BattleRoles[operation.RoleId];
            if (userRole == null || userRole.IsDeath)
                return ErrorCode.RoleNotFound;

            UserRole targetRole = (UserRole)BattleRoles[operation.TargetId];
            if (targetRole == null || targetRole.IsDeath)
                return ErrorCode.TargetNotFound;

            userRole.OnAction(targetRole);

            EventData eventData = new EventData((byte)EventCode.Battle, new object());
            eventData[(byte)ParameterCode.SubCode] = SubCode.Action;
            eventData[(byte)ParameterCode.UserId] = gameUser.Id;
            eventData[(byte)ParameterCode.RoleId] = operation.RoleId;
            eventData[(byte)ParameterCode.TargetId] = operation.TargetId;

            SendBattleEvent(eventData);

            return ErrorCode.Success;
        }

        //active a skill of a Role from user
        private ErrorCode OnUserSkillCast(BattleOperation operation, GameUser gameUser)
        {
            UserRole userRole = (UserRole)BattleRoles[operation.RoleId];
            if (userRole == null || userRole.IsDeath)
                return ErrorCode.RoleNotFound;

            ErrorCode errorCode = userRole.OnSkillCast(operation.SkillId);
            if (errorCode != ErrorCode.Success)
            {
                if (userRole.TargetRole != null)
                    userRole.State.Action = RoleAction.Action;
                else
                    userRole.State.Action = RoleAction.Idle;

                return errorCode;
            }

            EventData eventData = new EventData((byte)EventCode.Battle, new object());
            eventData[(byte)ParameterCode.SubCode] = SubCode.SkillCast;
            eventData[(byte)ParameterCode.UserId] = gameUser.Id;
            eventData[(byte)ParameterCode.RoleId] = operation.RoleId;
            eventData[(byte)ParameterCode.SkillId] = operation.SkillId;

            SendBattleEvent(eventData);

            return ErrorCode.Success;
        }

        //a role hits the target 
        private ErrorCode OnUserSkillHit(BattleOperation operation, GameUser gameUser)
        {
            UserRole userRole = (UserRole)BattleRoles[operation.RoleId];
            if (userRole == null)
                return ErrorCode.RoleNotFound;

            UserRole targetRole = (UserRole)BattleRoles[operation.TargetId];
            if (targetRole == null || targetRole.IsDeath)
                return ErrorCode.TargetNotFound;

            //userRole Attack will check result
            SubCode subCode = userRole.OnSkillHit(operation.SkillId, targetRole);
            if (subCode == SubCode.Default)
            {
                Log.WarnFormat("OnUserSkillHit OperationDedined");
                return ErrorCode.OperationDedined;
            }

            if (subCode != SubCode.SkillHit)
            {
                EventData eventData = new EventData((byte)EventCode.Battle, new object());
                eventData[(byte)ParameterCode.SubCode] = subCode;
                eventData[(byte)ParameterCode.TargetId] = operation.TargetId;
                eventData[(byte)ParameterCode.SkillId] = operation.SkillId;

                SendBattleEvent(eventData);
            }

            return ErrorCode.Success;
        }

        //user get a item
        private ErrorCode OnUserItemEat(BattleOperation operation, GameUser gameUser)
        {
            UserRole userRole = (UserRole)BattleRoles[operation.RoleId];
            if (userRole == null || userRole.IsDeath)
                return ErrorCode.RoleNotFound;

            userRole.OnItemEat(operation.TargetId);

            EventData eventData = new EventData((byte)EventCode.Battle, new object());
            eventData[(byte)ParameterCode.SubCode] = SubCode.ItemEat;
            eventData[(byte)ParameterCode.UserId] = gameUser.Id;
            eventData[(byte)ParameterCode.RoleId] = operation.RoleId;
            eventData[(byte)ParameterCode.TargetId] = operation.TargetId;

            SendBattleEvent(eventData);

            return ErrorCode.Success;
        }

        #endregion
    }
}
