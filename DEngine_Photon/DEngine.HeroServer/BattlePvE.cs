using DEngine.Common;
using DEngine.Common.Config;
using DEngine.Common.GameLogic;
using DEngine.HeroServer.GameData;
using Photon.SocketServer;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DEngine.HeroServer
{
    public enum WaitState
    {
        None,
        Waiting,
        Running,
        Finished,
    }

    public class BattleWave
    {
        public WaitState WaveState { get; set; }
        public float WaitTime { get; set; }
        public List<UserRole> WaveMobs { get; set; }
    }

    public class BattlePvE : BattleService
    {
        #region Properties

        public override BattleMode Mode { get { return BattleMode.RandomPvE; } }

        public List<UserRole> Runes { get; set; }

        public List<BattleWave> Waves { get; set; }

        #endregion

        #region Methods

        public BattlePvE()
        {
            Runes = new List<UserRole>();
            Waves = new List<BattleWave>();
        }

        protected override void Initialize()
        {
            //class Initialize() in battleservice
            base.Initialize();

            if (GameUser1.GamePlay is MissionService)
            {
                float xPos = -1f;
                foreach (UserRole role in GameUser1.ActiveRoles)
                {
                    role.State.CurrentPos = new Vector3(xPos, 0, 0);
                    role.State.TargetPos = new Vector3(xPos, 0, 0);
                    xPos += 1f;
                }
            }

            // Add towers to roles in battle
            foreach (UserRole rune in Runes)
                BattleRoles.LockAdd(rune);
        }

        //check status of wave in mission and send new wave , call in OnBattleUpdate() of battleservice .cs
        protected override void OnBattleLive()
        {
            for (int i = 0; i < Waves.Count; i++)
            {
                BattleWave wave = Waves[i];

                switch (wave.WaveState)
                {
                    //if first wave 
                    case WaitState.None:
                        if (i == 0)
                        {
                            //TotalTime : time battle run since start , value increases
                            wave.WaitTime = TotalTime + 1;// 1 sec after perform work inside if (wave.WaitTime < TotalTime)
                            wave.WaveState = WaitState.Waiting;
                        }

                        break;

                    //create new wave in battle and send new wave event if this wave status is Waiting
                    case WaitState.Waiting:
                        if (wave.WaitTime < TotalTime)
                        {
                            foreach (UserRole role in wave.WaveMobs)
                                BattleRoles.LockAdd(role);

                            wave.WaveState = WaitState.Running;

                            float nextWaveTime = -1;
                            if (i < Waves.Count - 1)
                            {
                                BattleWave nextWave = Waves[i + 1];
                                nextWaveTime = nextWave.WaitTime;

                                //if next wave active in time ,change nextWave.WaitTime(time active wave) =nextWave.WaitTime+current time 
                                if (nextWaveTime > 0)
                                {

                                    nextWave.WaitTime += TotalTime;
                                    nextWave.WaveState = WaitState.Waiting;
                                }
                            }

                            //send new wave to player , list of mobs in wave
                            EventData eventData = new EventData((byte)EventCode.Battle, new object());
                            eventData[(byte)ParameterCode.SubCode] = SubCode.MobCreate;
                            eventData[(byte)ParameterCode.Param01] = i;
                            eventData[(byte)ParameterCode.Param02] = Waves.Count;
                            eventData[(byte)ParameterCode.BattleTime] = nextWaveTime;
                            eventData[(byte)ParameterCode.BattleMobs] = Serialization.SaveArray(wave.WaveMobs.ToArray(), true);
                            Zone.SendUserEvent(GameUser1, eventData);
                        }

                        break;

                    //Wave is running
                    case WaitState.Running:
                        bool allDeath = true;
                        foreach (UserRole mob in wave.WaveMobs)
                        {
                            if (mob.IsDeath)
                            {
                                if (BattleRoles.LockRemove(mob) >= 0)
                                    OnMobKilled(mob);//check item drop in mode service
                            }
                            else
                                allDeath = false;
                        }


                        //if all mobs in wave death , next wave
                        if (allDeath)
                        {
                            wave.WaveState = WaitState.Finished;

                            if (i < Waves.Count - 1)
                            {
                                BattleWave nextWave = Waves[i + 1];
                                if (nextWave.WaitTime <= 0 || nextWave.WaveState < WaitState.Running)
                                {
                                    nextWave.WaitTime = TotalTime + 1;
                                    nextWave.WaveState = WaitState.Waiting;
                                }
                            }
                        }

                        break;

                    //nothing if this wave Finished
                    case WaitState.Finished:
                        break;
                }
            }

            if (Waves[0].WaveState < WaitState.Running)
                return;

            if (Runes.Count > 0)
            {
                GameUser runeUser = Runes[0].GameUser;
                int liveRunes = Runes.Count(r => !r.IsDeath);
                if (liveRunes < 1)
                {
                    if (runeUser == GameUser1)
                        OnBattleEnd(1, BattleCode.EndRuneKill);
                    else
                        OnBattleEnd(-1, BattleCode.EndRuneKill);

                    //Dispose();
                    IsDisposing = true;
                    return;
                }
            }

            //count heroes of player
            int liveCount1 = GameUser1.ActiveRoles.Count(r => !r.IsDeath);

            // count waves unfinished
            int liveCount2 = Waves.Count(w => w.WaveState < WaitState.Finished);

            //  heroes of player are exist and waves unfinished , not end game!
            if (liveCount1 > 0 && liveCount2 > 0)
                return;

            OnBattleEnd(liveCount2 - liveCount1, BattleCode.EndUserKill);

            IsDisposing = true;
            //Dispose();
        }

        protected override void OnBattleEnd(int result, BattleCode code)
        {
            base.OnBattleEnd(result, code);

            if (GameUser1.GamePlay != null)
                GameUser1.GamePlay.OnBattleEnd(this, result);
        }

        //check item drop in mode 
        private void OnMobKilled(UserRole mob)
        {
            if (GameUser1.GamePlay != null)
                GameUser1.GamePlay.OnItemDrop(new float[] { mob.State.CurrentPos.x, mob.State.CurrentPos.y, mob.State.CurrentPos.z });
        }

        #endregion
    }
}
