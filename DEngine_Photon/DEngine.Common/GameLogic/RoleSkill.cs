using DEngine.Common;

#if _SERVER
using DEngine.HeroServer.GameData;
#endif

namespace DEngine.Common.GameLogic
{
    public class RoleSkill : GameObj
    {
        #region Fields

        public int RoleUId;

        public int SkillId;

        public int Level;

        #endregion

        #region Properties

        public UserRole UserRole { get; set; }

        public GameSkill GameSkill { get; set; }

        public CostType CostType
        {
            get
            {
                return (CostType)GameSkill.CostType;
            }
        }

        public int CostValue
        {
            get
            {
                if (CostType != CostType.MPValue)
                    return GameSkill.CostValue;

                float costValue = GameSkill.CostValue;
                for (int i = 0; i < Level - 1; i++)
                    costValue += costValue * 6 / 100;

                return (int)costValue;
            }
        }

        public float NextCastTime { get; set; }

        public TargetType TargetType
        {
            get { return GameSkill != null ? (TargetType)GameSkill.TargetType : TargetType.None; }
        }

        public bool TargetOne
        {
            get
            {
                return (TargetType == TargetType.AllyOne || TargetType == TargetType.EnemyOne);
            }
        }

        public bool TargetGroup
        {
            get
            {
                return (TargetType == TargetType.AllyGroup || TargetType == TargetType.AllyGroup || TargetType == TargetType.AreaEffect);
            }
        }

        public bool TargetAlly
        {
            get
            {
                return (TargetType == TargetType.AllyOne || TargetType == TargetType.AllyGroup);
            }
        }

        public bool TargetEnemy
        {
            get
            {
                return (TargetType == TargetType.EnemyOne || TargetType == TargetType.EnemyGroup);
            }
        }

        #endregion

        #region Methods

        public override void Serialize(BinSerializer serializer)
        {
            base.Serialize(serializer);

            serializer.Serialize(ref RoleUId);
            serializer.Serialize(ref SkillId);
            serializer.Serialize(ref Level);
        }

        #endregion
    }
}
