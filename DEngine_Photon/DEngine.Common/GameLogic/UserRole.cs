using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using DEngine.Common.Config;

#if _SERVER
using DEngine.HeroServer;
using DEngine.HeroServer.GameData;
using ExitGames.Logging;
#endif

namespace DEngine.Common.GameLogic
{
    public class UserRoleHire : GameObj
    {
        public int UserId;
        public int Avatar;
        public string NickName;
        public int RoleId;
        public int RoleLevel;
        public int Class;
        public int Grade;
        public int ElemId;
        public string Skills;
        public int HireMode;
        public int HireGold;
        public int HireSilver;

#if _SERVER
        public void InitData(RoleHire role)
        {
            
            Id = role.Role_UID;//index of Role in dbo.Role table

            RoleId = role.RoleId;

            GameRole gameRole = (GameRole)Global.GameRoles[RoleId];
            Name = gameRole != null ? gameRole.Name : "NULL";
            Class = gameRole != null ? gameRole.Class : 0;

            RoleLevel = role.Level;
            Grade = role.Grade;
            ElemId = role.ElemId;

            // Add Skills
            string roleSkills = "";
            if (gameRole != null)
            {
                foreach (int skillId in gameRole.Skills[ElemId])
                    roleSkills += string.Format("{0};", skillId);
            }
            Skills = roleSkills;

            UserId = role.UserId;
            Avatar = role.UserEx.Avatar;
            NickName = role.UserEx.NickName;

            HireMode = role.HireMode;
            HireGold = role.HireGold;
            HireSilver = role.HireSilver;
        }
#endif

        public override void Serialize(BinSerializer serializer)
        {
            base.Serialize(serializer);

            serializer.Serialize(ref UserId);
            serializer.Serialize(ref Avatar);
            serializer.Serialize(ref NickName);
            serializer.Serialize(ref RoleId);
            serializer.Serialize(ref RoleLevel);
            serializer.Serialize(ref Class);
            serializer.Serialize(ref Grade);
            serializer.Serialize(ref ElemId);
            serializer.Serialize(ref Skills);
            serializer.Serialize(ref HireMode);
            serializer.Serialize(ref HireGold);
            serializer.Serialize(ref HireSilver);
        }
    }

    public class UserRole : GameObj
    {
        private const int MAGIC_MAX = 8;

        private static int[] MeleeRoles = new int[] { 2, 5, 9 };
        private static int[] RangerRoles = new int[] { 3, 6, 8 };
        private static int[] BuffRoles = new int[] { 1, 4, 7 };

        [StructLayout(LayoutKind.Sequential)]

        
        public struct RoleBase
        {
            public int UserId;
            public int RoleId;//roleid of dbo.rolebase table , Ex : character type
            public ElemType ElemId;//id element (ngũ hành)
            public RoleStatus Status;
            public RoleType Type;
            public RoleClass Class;
            public int Grade;
            public int Level;
            public int Exp;
            public int AIMode;
            public int Energy;
            public long UseTime;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RoleAttrib
        {
            public int MaxHP;
            public int HPRegen;
            public int MaxMP;
            public int MPRegen;
            public int MaxEng;
            public int EPRegen;

            public int MoveSpeed;
            public int AttackSpeed;
            public int AttackValue;
            public int DefenceValue;
            public float[] DefenceElems;
            public int FeedbackDamage;
            public int OutputDamageToHP;
            public int OutputDamageToMP;
            public int InputDamageToMP;
            public int IgnoreDefence;

            public float BlockRate;
            public float HitRate;
            public float EvasRate;
            public float CritRate;
            public float CritPower;
            public float HitRange;
        }

        //state additive effect impact on the role (in battle)
     
        public class MagicState : IDataSerializable
        {
            public AttribType AttribType;
            public EffectType EffectType;
            public float AffectValue;
            public float RemainTime;

            public void Reset()
            {
                AttribType = AttribType.None;
                EffectType = EffectType.None;
                AffectValue = 0;
                RemainTime = 0;
            }

            public void Serialize(BinSerializer serializer)
            {
                if (serializer.Mode == SerializerMode.Write)
                {
                    serializer.Writer.Write((int)AttribType);
                    serializer.Writer.Write((int)EffectType);
                    serializer.Writer.Write(AffectValue);
                    serializer.Writer.Write(RemainTime);
                }
                else
                {
                    AttribType = (AttribType)serializer.Reader.ReadInt32();
                    EffectType = (EffectType)serializer.Reader.ReadInt32();
                    AffectValue = serializer.Reader.ReadSingle();
                    RemainTime = serializer.Reader.ReadSingle();
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]

        //status of role in battle
        public struct RoleState
        {
            public int RoleUId;//index of role in dbo.role table
            public int TargetId;
            public RoleAction Action;
            public Vector3 CurrentPos;
            public Vector3 TargetPos;
            public bool SkillDisable;
            public bool ItemDisable;
            public int Damage;

            public int CurHP;
            public int MaxHP;
            public int HPReg;
            public int CurMP;
            public int MaxMP;
            public int MPReg;

            public float MoveSpeed;
            public float AtkSpeed;
            public float AtkDelay;

            public float AttackValue;
            public float DefencePercent;
            public float[] DefenceElems;
            public float FeedbackDamage;
            public float DecreaseDamage;
            public float ManaShield;
            public float OutputDamageToHP;
            public float OutputDamageToMP;
            public float InputDamageToMP;
            public float IgnoreDefence;

            public float BlockRate;
            public float HitRate;
            public float EvasRate;
            public float CritRate;
            public float CritPower;
            public float HitRange;

            public MagicState[] MagicStates;

#if _SERVER
            //init MagicStates list , call when init battle
            public void InitMagicStates()
            {
                
                //Init additive effects 
                MagicStates = new MagicState[MAGIC_MAX];
                for (int i = 0; i < MAGIC_MAX; i++)
                    MagicStates[i] = new MagicState();

                //defence list of 5 element
                DefenceElems = new float[(int)ElemType.Count];
            }

            
            //clear Buff Skills of Role
            public void ClearBuff()
            {
                foreach (MagicState mState in MagicStates)
                {
                    if (mState.AttribType == AttribType.None)
                        continue;

                    if (mState.AffectValue > 0)
                        mState.Reset();
                }
            }

            //clear Debuff Skills of Role
            public void ClearDebuff()
            {
                foreach (MagicState mState in MagicStates)
                {
                    if (mState.AttribType == AttribType.None)
                        continue;

                    if (mState.AffectValue < 0)
                        mState.Reset();
                }
            }


            //add new additive effects on hero
            public void AddMagicState(AttribType attribType, EffectType effectType, float affectValue, float affectDuration)
            {
                if (MagicStates == null)
                    InitMagicStates();

                foreach (MagicState mState in MagicStates)
                {
                    if (mState.AttribType == AttribType.None)
                    {
                        mState.AttribType = attribType;
                        mState.EffectType = effectType;
                        mState.AffectValue = affectValue;
                        mState.RemainTime = affectDuration;
                        break;
                    }
                }
            }

            // check time (duration) of additive effect            
            public void RefreshMagicStates()
            {
                foreach (MagicState mState in MagicStates)
                {
                    if (mState.AttribType == AttribType.None)
                        continue;

                    mState.RemainTime -= 1;

                    if (mState.RemainTime <= 0)
                        mState.Reset();
                }
            }

            //change HP value
            public bool AddHPValue(int addValue)
            {
                int newHP = Mathf.Clamp(CurHP + addValue, 0, MaxHP);

                if (newHP < CurHP)
                    Damage += CurHP - newHP;

                if (CurHP == newHP)
                    return false;

                CurHP = newHP;

                if (CurHP == 0)
                {
                    Log.InfoFormat("Role {0} is dead because of damage {1}!", RoleUId, addValue);
                    Action = RoleAction.Dead;
                }

                return true;
            }

            //change MP value
            public bool AddMPValue(int addValue)
            {
                int newMP = Mathf.Clamp(CurMP + addValue, -1, MaxMP);

                if (newMP < 0 || CurMP == newMP)
                    return false;

                CurMP = newMP;

                return true;
            }
#endif
        }

        #region Public Fields

        public RoleBase Base;//baseclass of role from gameRole

        public RoleAttrib Attrib;

        public RoleState State;

        // list of items equip to role (this character)
        //list item equiment
        public readonly List<UserItem> RoleItems = new List<UserItem>();

        //list of skills of role (this character))        
        public readonly List<RoleSkill> RoleSkills = new List<RoleSkill>();

        #endregion

        #region Private Fields

        private int HPBase;
        private int MPBase;
        private int AttackBase;
        private int DefenceBase;
        private int AtkSpeedBase;
        private float CritRateBase;
        private float CritPowerBase;

        #endregion

#if _SERVER
        protected static ILogger Log = LogManager.GetCurrentClassLogger();

        private int itemUseCount;

        private RoleSkill castSkill;

        private int activeTargetCount;

        private float activeRange;

        private float elapsedTime;
#endif

        #region Properties

        public GameUser GameUser { get; set; }

        public GameRole GameRole { get; set; }

        public UserRole TargetRole { get; set; }

        public bool StateChanged { get; set; }

        public bool IsDeath { get { return State.Action == RoleAction.Dead; } }

        #endregion

        #region Methods

        public void AddExp(int exp)
        {
            if (Base.Level >= RoleConfig.LEVEL_MAX)
                return;

            Base.Exp += exp;

            while (Base.Level < RoleConfig.LEVEL_MAX)
            {
                if (Base.Exp < RoleConfig.LEVELS_EXP[Base.Level])
                    break;

                Base.Exp -= RoleConfig.LEVELS_EXP[Base.Level];
                Base.Level += 1;

                Base.Energy = RoleConfig.ENERGY_MAX;
                Base.UseTime = DateTime.Now.ToFileTime();
            }

            if (Base.Level == RoleConfig.LEVEL_MAX)
                Base.Exp = 0;

            InitAttrib();
        }

        public void AddEnergy(int engValue)
        {
            Base.Energy += engValue;
            if (Base.Energy > Attrib.MaxEng)
                Base.Energy = Attrib.MaxEng;
        }

        //Up star of Role
        public bool Upgrade()
        {
            if (Base.Grade < 5 && Base.Level > 9)
            {
                Base.Grade += 1;
                Base.Level -= 9;
                Base.Exp = 0;
                Base.Energy = RoleConfig.ENERGY_MAX;
                Base.UseTime = DateTime.Now.ToFileTime();

                InitAttrib();
                return true;
            }

            return false;
        }


        //update attributes  of Role when attribute are changed
        // (by : add exp , upgrade , set item for role) 
        public void InitAttrib()
        {
            //InitBase in design
            float fRate = (float)RoleConfig.GRADES_ATTRIB[Base.Grade] / 100;
            HPBase = (int)(fRate * RoleConfig.ROLE_MAXDATA[Base.Level].MaxHP);
            MPBase = (int)(fRate * RoleConfig.ROLE_MAXDATA[Base.Level].MaxMP);
            AttackBase = (int)(fRate * RoleConfig.ROLE_MAXDATA[Base.Level].Attack);
            DefenceBase = (int)(fRate * RoleConfig.ROLE_MAXDATA[Base.Level].Defence);
            AtkSpeedBase = (int)(fRate * RoleConfig.ROLE_MAXDATA[Base.Level].AtkSpeed);
            CritRateBase = RoleConfig.ROLE_MAXDATA[Base.Level].CritRate;
            CritPowerBase = RoleConfig.ROLE_MAXDATA[Base.Level].CritPower;

            // Stregth Attribs
            Attrib.MaxHP = HPBase * GameRole.Strength / 100;
            Attrib.CritPower = CritPowerBase * GameRole.Strength / 100 + RoleConfig.CRIT_POWER_DEF;

            // Agility Attribs
            Attrib.AttackValue = AttackBase * GameRole.Agility / 100;
            Attrib.CritRate = CritRateBase * GameRole.Agility / 100 + RoleConfig.CRIT_RATE_DEF;

            // Intelligent Attribs
            Attrib.MaxMP = MPBase * GameRole.Intelligent / 100;
            Attrib.DefenceValue = DefenceBase * GameRole.Intelligent / 100;

            ElemType genElem = GameConfig.ELEMS_OVER_FORWARD[(int)Base.ElemId];
            ElemType overElem = GameConfig.ELEMS_OVER_RESVERSE[(int)Base.ElemId];

            int defenceGen = (5 + 10 * Base.Level / RoleConfig.LEVEL_MAX);
            int defenceOver = -(10 + 20 * Base.Level / RoleConfig.LEVEL_MAX);

            Attrib.DefenceElems = new float[(int)ElemType.Count];
            for (ElemType i = ElemType.None; i < ElemType.Count; i++)
            {
                if (i == genElem)
                    Attrib.DefenceElems[(int)i] = defenceGen;
                if (i == overElem)
                    Attrib.DefenceElems[(int)i] = defenceOver;
            }

            // Common Attribs
            Attrib.MaxEng = RoleConfig.ENERGY_MAX;
            Attrib.EvasRate = RoleConfig.EVAS_RATE;
            Attrib.HitRate = RoleConfig.HIT_RATE;
            Attrib.HPRegen = RoleConfig.HP_REG;
            Attrib.MPRegen = RoleConfig.MP_REG;
            Attrib.EPRegen = RoleConfig.EP_REG;
            Attrib.FeedbackDamage = 0;
            Attrib.OutputDamageToHP = 0;
            Attrib.OutputDamageToMP = 0;
            Attrib.InputDamageToMP = 0;
            Attrib.IgnoreDefence = 0;
            Attrib.BlockRate = 0;
            Attrib.HitRange = 0;

            Attrib.MoveSpeed = GameRole.MoveSpeed;
            Attrib.AttackSpeed = GameRole.AttackRate;

            if (RoleSkills.Count > 0 && RoleSkills[0].GameSkill != null)
                Attrib.HitRange = RoleSkills[0].GameSkill.CastRange;

            
          // Updates the attributes of heroes when equipped item
            ProcessItemData();
        }

        public override void Serialize(BinSerializer serializer)
        {
            base.Serialize(serializer);

            serializer.SerializeEx(ref Base);
            serializer.SerializeEx(ref Attrib);
            serializer.SerializeEx(ref State);

            serializer.Serialize(RoleSkills);
        }

        // Updates the attributes of heroes when equipped item
        private void ProcessItemData()
        {
            float[] attribValue = new float[(int)AttribType.AttribCount];

          
            foreach (UserItem item in RoleItems)
            {
                foreach (ItemAttrib att in item.Attribs)
                {
                    attribValue[(int)att.Attrib] += att.Value;
                    //attribValue[FeedbackDamage] 
                }
            }

            Attrib.MaxHP += Attrib.MaxHP * (int)attribValue[(int)AttribType.HPMaxPercent] / 100;
            Attrib.MaxHP += (int)attribValue[(int)AttribType.HPMaxValue];

            Attrib.MaxMP += Attrib.MaxMP * (int)attribValue[(int)AttribType.MPMaxPercent] / 100;
            Attrib.MaxMP += (int)attribValue[(int)AttribType.MPMaxValue];

            Attrib.HPRegen += (int)attribValue[(int)AttribType.HPRegenValue];
            Attrib.MPRegen += (int)attribValue[(int)AttribType.MPRegenValue];
            Attrib.MaxEng += (int)attribValue[(int)AttribType.EnergyMax];
            Attrib.EPRegen += (int)attribValue[(int)AttribType.EnergyRegen];

            Attrib.MoveSpeed += (int)attribValue[(int)AttribType.MoveSpeedPercent] * Attrib.MoveSpeed / 100;
            Attrib.AttackSpeed += (int)attribValue[(int)AttribType.AttackSpeedPercent] * Attrib.AttackSpeed / 100;

            Attrib.DefenceValue += (int)attribValue[(int)AttribType.DefenceValue];
            Attrib.FeedbackDamage += (int)attribValue[(int)AttribType.FeedbackDamage];
            Attrib.AttackValue += Attrib.AttackValue * (int)attribValue[(int)AttribType.AttackPercent] / 100;
            Attrib.AttackValue += (int)attribValue[(int)AttribType.AttackValue];

            Attrib.DefenceElems[(int)ElemType.Metal] += attribValue[(int)AttribType.DefenceMetal];
            Attrib.DefenceElems[(int)ElemType.Wood] += attribValue[(int)AttribType.DefenceWood];
            Attrib.DefenceElems[(int)ElemType.Water] += attribValue[(int)AttribType.DefenceWater];
            Attrib.DefenceElems[(int)ElemType.Fire] += attribValue[(int)AttribType.DefenceFire];
            Attrib.DefenceElems[(int)ElemType.Earth] += attribValue[(int)AttribType.DefenceEarth];
            for (int i = 0; i < (int)ElemType.Count; i++)
                Attrib.DefenceElems[i] += attribValue[(int)AttribType.DefencePercent];

            Attrib.HitRate += attribValue[(int)AttribType.HitRate];
            Attrib.CritRate += attribValue[(int)AttribType.CriticalRate];
            Attrib.CritPower += attribValue[(int)AttribType.CriticalPower];
            Attrib.BlockRate += attribValue[(int)AttribType.BlockRate];
            Attrib.EvasRate += attribValue[(int)AttribType.EvasionRate];

            Attrib.OutputDamageToHP += (int)attribValue[(int)AttribType.ConvertOutputDamageToHP];
            Attrib.OutputDamageToMP += (int)attribValue[(int)AttribType.ConvertOutputDamageToMP];
            Attrib.InputDamageToMP += (int)attribValue[(int)AttribType.ConvertInputDamageToMP];
            Attrib.IgnoreDefence += (int)attribValue[(int)AttribType.IgnoreDefense];
        }

        #endregion

#if _SERVER
        public bool IsReady { get { return Base.Status == RoleStatus.Active && Base.Energy >= RoleConfig.ENERGY_MIN; } }


        //sinh ngẫu nhiên hero có số sao ngẫu nhiên từ 1->heroRank (VN language)

        //Create Hero Random (summon , buy heroes pack)
        public void CreateRandom(int userId, int heroRank, int roleCount = -1)
        {
            
            int roleId = 0;//character type =roleid in table
            int roleIdx = Helpers.Random.Next(3);

            // Random Hero
            switch (roleCount)
            {
                //0 : summon in tutorial, first hero 
                case 0:
                    roleId = MeleeRoles[roleIdx];
                    

                    GameRole = (GameRole)Global.GameRoles[roleId];
                    break;
                //1 : summon in tutorial, second hero
                case 1:
                    roleId = RangerRoles[roleIdx];
                    GameRole = (GameRole)Global.GameRoles[roleId];
                    break;

                //2 : summon in tutorial, 3rd hero
                case 2:                    
                    roleId = BuffRoles[roleIdx];
                    GameRole = (GameRole)Global.GameRoles[roleId];
                    break;

                default:
                    while (true)
                    {
                        roleIdx = Helpers.Random.Next(Global.GameHeroes.Count);
                        GameRole = (GameRole)Global.GameHeroes[roleIdx];
                        if (GameRole.Type == (int)RoleType.Hero)
                            break;
                    }
                    roleId = GameRole.Id;
                    break;
            }

            // Random Grade
            RoleGrade gradeData = RoleConfig.GetGradeData(heroRank);
            int idx = Helpers.GetRandomIndex(gradeData.Rates);

            Name = GameRole.Name;

            Base.UserId = userId;
            Base.RoleId = roleId;
            Base.ElemId = (ElemType)(1 + Helpers.Random.Next((int)ElemType.Count - 1));
            Base.Grade = gradeData.Grades[idx];
            Base.Level = RoleConfig.LEVEL_MIN;
            Base.Energy = RoleConfig.ENERGY_MAX;
            Base.UseTime = DateTime.Now.ToFileTime();
        }


        //create attributes for userrole of mob (monster , boss , rune) 
        public void CreateTemp(GameRole gameRole, int uId, int roleLevel, int roleGrade = 1)
        {
            //index of object
            Id = uId;
            Name = gameRole.Name;

            GameRole = gameRole;

            Base.UserId = GameUser.Id;
            Base.RoleId = gameRole.Id;//character type
            Base.ElemId = (ElemType)(Helpers.Random.Next((int)ElemType.Count));
            Base.Type = (RoleType)gameRole.Type;
            Base.Status = RoleStatus.Active;
            Base.Level = roleLevel;
            Base.Grade = roleGrade;

            //copy skills from type of this Role in game 
            foreach (int skillId in gameRole.Skills[(int)Base.ElemId])
            {
                GameSkill gameSkill = (GameSkill)Global.GameSkills[skillId];
                if (gameSkill == null)
                    continue;

                RoleSkill roleSkill = new RoleSkill()
                {
                    Id = 0,
                    Name = gameSkill.Name,
                    RoleUId = uId,
                    SkillId = skillId,
                    UserRole = this,
                    Level = Base.Level,
                    GameSkill = gameSkill,
                };

                RoleSkills.Add(roleSkill);
            }

            InitAttrib();
        }

        //init base attributes , skills  
        public void InitData(Role role)
        {

            //index in dbo.Role table , not duplicate
            Id = role.Role_UID;
            Name = role.Name;

            Base.UserId = role.UserId;
            Base.RoleId = role.RoleId;//type character
            Base.ElemId = (ElemType)role.ElemId;
            Base.Type = (RoleType)GameRole.Type;
            Base.Status = (RoleStatus)role.Status;
            Base.Class = (RoleClass)GameRole.Class;
            Base.Grade = role.Grade;
            Base.Level = role.Level;
            Base.Exp = role.Exp;
            Base.AIMode = role.AIMode;
            Base.Energy = role.Energy;
            Base.UseTime = role.UseTime.ToFileTime();

            //copy skills from base role 
            foreach (int skillId in GameRole.Skills[(int)Base.ElemId])
            {
                GameSkill gameSkill = (GameSkill)Global.GameSkills[skillId];
                if (gameSkill == null)
                    continue;

                RoleSkill roleSkill = new RoleSkill()
                {
                    Id = 0,
                    Name = gameSkill.Name,
                    RoleUId = Id,
                    SkillId = skillId,
                    UserRole = this,
                    Level = Base.Level,
                    GameSkill = gameSkill,
                };

                RoleSkills.Add(roleSkill);
            }


            InitAttrib();
        }

        //copy base attributes from a Role
        public void UpdateData(Role role)
        {
            role.ElemId = (int)Base.ElemId;
            role.Status = (int)Base.Status;
            role.Grade = Base.Grade;
            role.Level = Base.Level;
            role.Exp = Base.Exp;
            role.AIMode = Base.AIMode;
            role.Energy = Base.Energy;
            role.UseTime = DateTime.FromFileTime(Base.UseTime);
        }

        //create new role from this UserRole 
        public Role GetNewRole()
        {
            Role newRole = new Role()
            {
                UserId = this.Base.UserId,
                RoleId = this.Base.RoleId,
                Name = this.Name,
                CreateTime = DateTime.Now,
            };

            UpdateData(newRole);
            return newRole;
        }

        //Init data when battle start
        public void InitBattle(float xPos, float yPos, float zPos)
        {
            State.RoleUId = Id;
            State.Action = RoleAction.Idle;
            State.CurrentPos = new Vector3(xPos, yPos, zPos);
            State.TargetPos = new Vector3(xPos, yPos, zPos);
            State.Damage = 0;

            State.InitMagicStates();

            State.CurHP = Attrib.MaxHP;
            State.CurMP = Attrib.MaxMP;

            itemUseCount = 0;

            foreach (UserItem userItem in RoleItems)
                userItem.NextUseTime = 0;

            InitStates();
        }


        //Init states  when battle start
        public void InitStates()
        {
            State.SkillDisable = false;
            State.ItemDisable = false;

            State.MaxHP = Attrib.MaxHP;
            State.MaxMP = Attrib.MaxMP;
            State.HPReg = Attrib.HPRegen;
            State.MPReg = Attrib.MPRegen;

            State.MoveSpeed = 0.1f * Attrib.MoveSpeed;
            State.AtkSpeed = Attrib.AttackSpeed;
            State.AtkDelay = 2f / (1 + State.AtkSpeed / 100);

            State.AttackValue = Attrib.AttackValue;
            State.DefencePercent = Attrib.DefenceValue * 50 / RoleConfig.DEFENCE_MAX;
            State.FeedbackDamage = Attrib.FeedbackDamage;
            State.DecreaseDamage = 0;
            for (int i = 0; i < (int)ElemType.Count; i++)
                State.DefenceElems[i] = Attrib.DefenceElems[i];

            State.BlockRate = Attrib.BlockRate;
            State.HitRate = Attrib.HitRate;
            State.EvasRate = Attrib.EvasRate;
            State.CritRate = Attrib.CritRate;
            State.CritPower = Attrib.CritPower;
            State.HitRange = Attrib.HitRange;

            State.OutputDamageToHP = Attrib.OutputDamageToHP;
            State.OutputDamageToMP = Attrib.OutputDamageToMP;
            State.InputDamageToMP = Attrib.InputDamageToMP;
            State.IgnoreDefence = Attrib.IgnoreDefence;

            ProcessHiddenSkills();
        }

        //update Energy of Role  ,increases with time , call in RefreshRoles of GameUser
        public void Refresh()
        {
            long currentTime = DateTime.Now.ToFileTime();

            if (Base.Energy < Attrib.MaxEng)
            {
                int restTime = (int)((currentTime - Base.UseTime) / 10000000);

                Base.Energy += restTime * Attrib.EPRegen;
                if (Base.Energy > Attrib.MaxEng)
                    Base.Energy = Attrib.MaxEng;

                Base.UseTime = currentTime;
            }
            else
                Base.UseTime = currentTime;
        }

        //Move event to target
        public void OnMove(float[] tgPos)
        {
            State.TargetId = 0;
            State.TargetPos = new Vector3(tgPos[0], tgPos[1], tgPos[2]);
            State.Action = RoleAction.Move;
        }

        //event attach to target
        public void OnAction(UserRole targetRole)
        {
            TargetRole = targetRole;
            activeRange = State.HitRange;

            State.TargetId = targetRole.Id;
            State.TargetPos = TargetRole.State.CurrentPos;
            State.Action = RoleAction.Action;
        }

        //active a skill
        public ErrorCode OnSkillCast(int skillId)
        {
            BattleService battleService = (BattleService)GameUser.Tag;
            castSkill = RoleSkills.FirstOrDefault(s => s.SkillId == skillId);
            if (castSkill == null)
            {
                Log.InfoFormat("Role {0}, Skill {0} is not avaiable", Name, skillId);
                return ErrorCode.SkillNotAvaiable;
            }

            // Check skill cost
            int hpCost = 0;
            int mpCost = 0;

            switch (castSkill.CostType)
            {
                case CostType.None:
                    break;

                case CostType.HPValue:
                    hpCost = castSkill.CostValue;
                    break;

                case CostType.MPValue:
                    mpCost = castSkill.CostValue;
                    break;

                case CostType.HPPercent:
                    if (castSkill.CostValue == 100)
                        hpCost = State.CurHP - 1;
                    else
                        hpCost = castSkill.CostValue * State.CurHP / 100;
                    break;

                case CostType.MPPercent:
                    mpCost = castSkill.CostValue * State.CurMP / 100;
                    break;

                case CostType.HPMaxPercent:
                    hpCost = castSkill.CostValue * State.MaxHP / 100;
                    break;

                case CostType.MPMaxPercent:
                    mpCost = castSkill.CostValue * State.MaxMP / 100;
                    break;

                default:
                    return ErrorCode.SkillInvalidCost;
            }

            if (hpCost > 0)
            {
                if (hpCost >= State.CurHP)
                    return ErrorCode.SkillHPInsufficient;
                StateChanged = State.AddHPValue(-hpCost);
            }

            if (mpCost > 0)
            {
                if (mpCost > State.CurMP)
                    return ErrorCode.SkillMPInsufficient;

                StateChanged = State.AddMPValue(-mpCost);
            }

            // Check  target type of skill
            if (TargetRole == null && castSkill.TargetOne)
            {
                Log.WarnFormat("TargetType.TargetOne with SkillId = {0}", castSkill.SkillId);
                return ErrorCode.SkillInvalidTarget;
            }

            if (TargetRole != null)
            {
                if (castSkill.TargetType == TargetType.AllyOne && TargetRole.Base.UserId != Base.UserId)
                {
                    Log.WarnFormat("Skill {0} Error: TargetType.AllyOne with UserId = {1} / {2}", castSkill.SkillId, Base.UserId, TargetRole.Base.UserId);
                    return ErrorCode.SkillInvalidTarget;
                }

                if (castSkill.TargetType == TargetType.EnemyOne && TargetRole.Base.UserId == Base.UserId)
                {
                    Log.WarnFormat("Skill {0} Error: TargetType.EnemyOne with same UserId = {1}", castSkill.SkillId, Base.UserId);
                    return ErrorCode.SkillInvalidTarget;
                }
            }

            activeRange = castSkill.GameSkill.CastRange;
            if (activeRange == 0)
                activeRange = float.MaxValue;
            activeTargetCount = castSkill.TargetOne ? 1 : 3;

            if (castSkill.TargetOne)
                State.Action = RoleAction.Skill;
            else
            {
                float coolTime = castSkill.GameSkill.CoolTime;
                if (coolTime == 0)
                    coolTime = State.AtkDelay;

                castSkill.NextCastTime = battleService.TotalTime + coolTime * 0.8f;

                State.Action = (TargetRole != null) ? RoleAction.Action : RoleAction.Idle;
            }

            return ErrorCode.Success;
        }

        //Hit the target
        public SubCode OnSkillHit(int skillId, UserRole targetRole)
        {
            if (skillId < 0)
            {
                targetRole.State.AddHPValue(-targetRole.State.CurHP);
                return SubCode.SkillCrit;
            }

            RoleSkill hitSkill = RoleSkills.FirstOrDefault(s => s.SkillId == skillId);
            if (hitSkill == null)
                return SubCode.Default;

            EffectMask skillEffectMask = (EffectMask)hitSkill.GameSkill.EffectMask;


            //check the results if rate of evasion of the target is higher, the target successfully evade

            int randRank = Helpers.Random.Next(100);
            if (((skillEffectMask & EffectMask.Evasion) == EffectMask.Evasion) && (randRank < targetRole.State.EvasRate))
                return SubCode.SkillEvas;

            //check the results if rate of block of the target is higher, skill has no effect on the target
            randRank = Helpers.Random.Next(100);
            if (((skillEffectMask & EffectMask.Block) == EffectMask.Block) && (randRank < targetRole.State.BlockRate))
                return SubCode.SkillBlock;

            //check the results if rate of hit of selft is lower, skill will miss the target
            randRank = Helpers.Random.Next(100);
            if (((skillEffectMask & EffectMask.HitRate) == EffectMask.HitRate) && (randRank >= State.HitRate))
                return SubCode.SkillMiss;

            //check the results if rate of critical of selft is higher , active critical
            randRank = Helpers.Random.Next(100);
            bool skillCrit = (((skillEffectMask & EffectMask.Critical) == EffectMask.Critical) && (randRank < State.CritRate));

            
            randRank = Helpers.Random.Next(100);
            bool ignoreDef = (randRank < State.CritRate);

            //calculate for skill input with level of the affect
            ProcessSkillInput(targetRole, hitSkill, skillCrit, ignoreDef);

            if (--activeTargetCount <= 0)
                hitSkill = null;

            return skillCrit ? SubCode.SkillCrit : SubCode.SkillHit;
        }

        public ErrorCode OnItemEat(int itemId)
        {
            UserItem userItem = RoleItems.FirstOrDefault(i => i.Id == itemId);
            if (userItem == null || userItem.Count == 0)
                return ErrorCode.ItemNotAvailable;

            BattleService battleService = (BattleService)GameUser.Tag;
            if (itemUseCount >= BattleConfig.ITEM_TAKEWITH)
            {
                Log.WarnFormat("UserItem {0} is over used", itemId);
                return ErrorCode.OperationDedined;
            }

            if (userItem.NextUseTime > battleService.TotalTime)
            {
                Log.WarnFormat("UserItem {0} is in cooling time", itemId);
                return ErrorCode.ItemNotAvailable;
            }

            itemUseCount += 1;
            userItem.NextUseTime = battleService.TotalTime + 0.8f * BattleConfig.ITEM_USEDELAY;

            // Instant affect item
            float[] stateValues = new float[(int)AttribType.AttribCount];
            foreach (var attr in userItem.Attribs)
                stateValues[(int)attr.Attrib] = attr.Value;

            //call calculate the impact of skill into itself
            ProcessSkillOutput(stateValues);

            if (--userItem.Count <= 0)
            {
                RoleItems.Remove(userItem);
                GameUser.DelItems.Add(userItem);
                GameUser.UserItems.Remove(userItem);
            }

            return ErrorCode.Success;
        }

        public void Update(float deltaTime)
        {
            elapsedTime += deltaTime;
            if (elapsedTime >= 1.0f)
            {
                FixedUpdate();
                elapsedTime = 0;
            }

            if (TargetRole != null && TargetRole.State.Action == RoleAction.Dead)
                TargetRole = null;

            switch (State.Action)
            {
                case RoleAction.Move:
                    {
                        float targetDis = Vector3.Distance(State.CurrentPos, State.TargetPos);

                        if (targetDis > 0.5f)
                        {
                            float moveStep = State.MoveSpeed * deltaTime;
                            State.CurrentPos = Vector3.MoveTowards(State.CurrentPos, State.TargetPos, moveStep);
                        }
                        else
                            State.Action = RoleAction.Idle;
                    }

                    break;

                case RoleAction.Action:
                    if (TargetRole == null)
                        State.Action = RoleAction.Idle;
                    else
                    {
                        float targetDis = Vector3.Distance(State.CurrentPos, TargetRole.State.CurrentPos);

                        if (targetDis > activeRange)
                        {
                            float moveStep = State.MoveSpeed * deltaTime;
                            State.CurrentPos = Vector3.MoveTowards(State.CurrentPos, TargetRole.State.CurrentPos, moveStep);
                        }
                    }

                    break;

                case RoleAction.Skill:
                    if (TargetRole == null || castSkill == null)
                        State.Action = RoleAction.Idle;
                    else
                    {
                        float targetDis = Vector3.Distance(State.CurrentPos, TargetRole.State.CurrentPos);

                        if (targetDis > activeRange)
                        {
                            float moveStep = State.MoveSpeed * deltaTime;
                            State.CurrentPos = Vector3.MoveTowards(State.CurrentPos, TargetRole.State.CurrentPos, moveStep);
                        }
                        else
                        {
                            float coolTime = castSkill.GameSkill.CoolTime;
                            if (coolTime == 0)
                                coolTime = State.AtkDelay;

                            BattleService battleService = (BattleService)GameUser.Tag;
                            castSkill.NextCastTime = battleService.TotalTime + coolTime * 0.8f;
                            State.Action = RoleAction.Action;
                        }
                    }

                    break;
            }
        }

        private void FixedUpdate()
        {
            InitStates();

            List<MagicState> magicStates = State.MagicStates.Where(r => r.AttribType != AttribType.None).ToList();

            if (magicStates.Count > 0)
            {
                float[] stateValues = new float[(int)AttribType.AttribCount];

                foreach (MagicState magic in magicStates)
                    stateValues[(int)magic.AttribType] += magic.AffectValue;

                //calculate for skill input with level of the affect
                ProcessSkillOutput(stateValues);

                State.RefreshMagicStates();
            }

            if (State.AddHPValue(State.HPReg))
                StateChanged = true;

            if (State.AddMPValue(State.MPReg))
                StateChanged = true;
        }

        //Aura Skill
        private void ProcessHiddenSkills()
        {
            foreach (RoleSkill roleSkill in RoleSkills)
            {
                if (roleSkill.GameSkill.SkillType != (int)SkillType.Aura)
                    continue;

                if (roleSkill.TargetType == TargetType.Self)
                    ProcessSkillInput(this, roleSkill);
                else if (roleSkill.TargetType == TargetType.AllyGroup || roleSkill.TargetType == TargetType.EnemyGroup)
                {
                    BattleService battle = GameUser.Tag as BattleService;
                    if (battle != null)
                    {
                        battle.BattleRoles.ForEach((GameObj obj) =>
                        {
                            UserRole role = (UserRole)obj;
                            if (roleSkill.TargetType == TargetType.AllyGroup)
                            {
                                if (role.GameUser == GameUser)
                                    ProcessSkillInput(role, roleSkill);
                            }
                            else
                            {
                                if (role.GameUser != GameUser)
                                    ProcessSkillInput(role, roleSkill);
                            }
                        });
                    }
                }
            }
        }

        //calculate for skill input with level of the affect to target
        private void ProcessSkillInput(UserRole targetRole, RoleSkill roleSkill, bool skillCrit = false, bool ingoreDef = false)
        {
            foreach (var attrib in roleSkill.GameSkill.Attribs)
            {
                if (attrib.DstAttrib == AttribType.None)
                    break;

                float wizardPlus = (float)GameUser.WizardCounts[(int)Base.Class] / 10;
                float affectValue = attrib.Value + attrib.Value * wizardPlus;

                switch (attrib.SrcAttrib)
                {
                    case AttribType.AttackValue:
                        affectValue = State.AttackValue * affectValue / 100;
                        if (affectValue < 0) // Positive Damage
                        {
                            if (!ingoreDef)
                            {
                                float targetDef = targetRole.State.DefencePercent + targetRole.State.DefenceElems[(int)Base.ElemId];
                                affectValue -= affectValue * targetDef / 100;
                            }

                            affectValue -= affectValue * targetRole.State.DecreaseDamage / 100;
                        }
                        break;

                    /*case AttribType.Strength:
                        affectValue = GameRole.Strength * affectValue / 100;
                        break;

                    case AttribType.Agility:
                        affectValue = GameRole.Agility * affectValue / 100;
                        break;

                    case AttribType.Intelligent:
                        affectValue = GameRole.Intelligent * affectValue / 100;
                        break;*/
                }

                // Gradual affect skill
                if (roleSkill.GameSkill.Duration > 0 && attrib.DstAttrib > AttribType.MPValue)
                {
                    EffectType effectType = (EffectType)roleSkill.GameSkill.EffectType;
                    float affectDuration = roleSkill.GameSkill.Duration;

                    targetRole.State.AddMagicState(attrib.DstAttrib, effectType, affectValue, affectDuration);

                    continue;
                }

                if (skillCrit)
                    affectValue = affectValue * State.CritPower / 100;

                // Instant affect skill
                float[] stateValues = new float[(int)AttribType.AttribCount];
                stateValues[(int)attrib.DstAttrib] = affectValue;

                //call calculate the impact of skill into itself
                targetRole.ProcessSkillOutput(stateValues, this);
            }
        }

        //calculate the impact of skill into itself
        private void ProcessSkillOutput(float[] stateValues, UserRole srcRole = null)
        {
            State.AttackValue += stateValues[(int)AttribType.AttackValue];
            State.AttackValue += stateValues[(int)AttribType.AttackPercent] * Attrib.AttackValue / 100;

            State.DefencePercent += stateValues[(int)AttribType.DefencePercent];

            State.MoveSpeed += stateValues[(int)AttribType.MoveSpeedPercent] * State.MoveSpeed / 100;
            State.AtkSpeed += stateValues[(int)AttribType.AttackSpeedPercent] * Attrib.AttackSpeed / 100;
            State.AtkDelay = 2f / (1 + State.AtkSpeed / 100);

            State.FeedbackDamage += stateValues[(int)AttribType.FeedbackDamage];
            State.DecreaseDamage += stateValues[(int)AttribType.DecreaseDamage];
            State.ManaShield += stateValues[(int)AttribType.ManaShield];

            State.OutputDamageToHP += stateValues[(int)AttribType.ConvertOutputDamageToHP];
            State.OutputDamageToMP += stateValues[(int)AttribType.ConvertOutputDamageToMP];
            State.InputDamageToMP += stateValues[(int)AttribType.ConvertInputDamageToMP];
            State.IgnoreDefence += stateValues[(int)AttribType.IgnoreDefense];

            State.HitRate += stateValues[(int)AttribType.HitRate];
            State.EvasRate += stateValues[(int)AttribType.EvasionRate];
            State.BlockRate += stateValues[(int)AttribType.BlockRate];
            State.CritRate += stateValues[(int)AttribType.CriticalRate];
            State.CritPower += stateValues[(int)AttribType.CriticalPower];

            State.SkillDisable = stateValues[(int)AttribType.SkillDisable] > 0.5;
            State.ItemDisable = stateValues[(int)AttribType.ItemDisable] > 0.5;

            if (stateValues[(int)AttribType.BuffClear] > 0.5)
                State.ClearBuff();

            if (stateValues[(int)AttribType.DebuffClear] > 0.5)
                State.ClearDebuff();

            // HP & MP Calculation
            State.MaxHP += (int)stateValues[(int)AttribType.HPMaxPercent] * Attrib.MaxHP / 100;
            State.MaxMP += (int)stateValues[(int)AttribType.MPMaxPercent] * Attrib.MaxMP / 100;

            State.HPReg += (int)stateValues[(int)AttribType.HPRegenValue];
            State.HPReg += (int)stateValues[(int)AttribType.HPRegenPercent] * Attrib.MaxHP / 100;

            State.MPReg += (int)stateValues[(int)AttribType.MPRegenValue];
            State.MPReg += (int)stateValues[(int)AttribType.MPRegenPercent] * Attrib.MaxMP / 100;

            StateChanged = true;

            int hpDamage = (int)(stateValues[(int)AttribType.HPValue]);
            hpDamage += (int)(stateValues[(int)AttribType.HPPercent] * State.MaxHP / 100);

            int mpDamage = (int)(stateValues[(int)AttribType.MPValue]);
            mpDamage += (int)(stateValues[(int)AttribType.MPPercent] * State.MaxMP / 100);

            if (hpDamage < 0)
            {
                // Calculate ManaShield
                if (State.ManaShield > 0)
                {
                    int mpDamageShield = (int)(hpDamage * State.ManaShield / 100);
                    int hpDamageShield = hpDamage - mpDamageShield;
                    if (State.CurMP + mpDamage + mpDamageShield < 0)
                    {
                        hpDamage = hpDamageShield + State.CurMP + mpDamage + mpDamageShield;
                        mpDamage = -State.CurMP;
                    }
                    else
                    {
                        hpDamage = hpDamageShield;
                        mpDamage += mpDamageShield;
                    }
                }

                if (State.InputDamageToMP > 0)
                    mpDamage -= (int)(hpDamage * State.InputDamageToMP / 100);

                // Calculate FeedbackDamage
                if (srcRole != null)
                {
                    if (State.FeedbackDamage > 0)
                    {
                        int feedbackDamage = (int)(hpDamage * State.FeedbackDamage / 100);
                        srcRole.State.AddHPValue(feedbackDamage);
                        srcRole.StateChanged = true;
                    }

                    if (srcRole.State.OutputDamageToHP > 0)
                    {
                        int convertHP = -(int)(hpDamage * srcRole.State.OutputDamageToHP / 100);
                        srcRole.State.AddHPValue(convertHP);
                        srcRole.StateChanged = true;
                    }

                    if (srcRole.State.OutputDamageToMP > 0)
                    {
                        int convertMP = -(int)(hpDamage * srcRole.State.OutputDamageToMP / 100);
                        srcRole.State.AddMPValue(convertMP);
                        srcRole.StateChanged = true;
                    }
                }
            }

            State.AddHPValue(hpDamage);
            State.AddMPValue(mpDamage);
        }
#endif
    }
}
