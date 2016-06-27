using DEngine.Common;
using System.Collections.Generic;

#if _SERVER
using DEngine.HeroServer.GameData;
#endif

namespace DEngine.Common.GameLogic
{
    public class GameMob : GameObj
    {
        #region Fields

        public string Description;

        public int Gender;

        public int Class;

        public int Points;

        public int Strength;

        public int Agility;

        public int Intelligent;

        public int Evasion;

        public int HitRate;

        public int HPRegen;

        public int MPRegen;

        public float Speed;

        #endregion

        #region Properties

#if _SERVER
        public List<int> Items { get; protected set; }

        public List<int> Skills { get; protected set; }
#endif

        #endregion

        #region Methods

        public override void Serialize(BinSerializer serializer)
        {
            base.Serialize(serializer);

            serializer.Serialize(ref Description);
            serializer.Serialize(ref Gender);
            serializer.Serialize(ref Class);
            serializer.Serialize(ref Points);
            serializer.Serialize(ref Strength);
            serializer.Serialize(ref Agility);
            serializer.Serialize(ref Intelligent);
            serializer.Serialize(ref Evasion);
            serializer.Serialize(ref HitRate);
            serializer.Serialize(ref HPRegen);
            serializer.Serialize(ref MPRegen);
            serializer.Serialize(ref Speed);
        }

#if _SERVER
        public void InitData(MobBase mob)
        {
            Gender = mob.Gender;
            Class = mob.Class;
            Description = mob.Desc;
            Points = mob.Points;
            Strength = mob.Strength;
            Agility = mob.Agility;
            Intelligent = mob.Intelligent;
            Evasion = mob.Evasion;
            HitRate = mob.HitRate;
            HPRegen = mob.HPReg;
            MPRegen = mob.MPReg;
            Speed = mob.Speed;

            try
            {
                Items = new List<int>();
                Skills = new List<int>();

                // Add BaseItems
                if (mob.Items != null)
                {
                    string[] allFields = mob.Items.Split(',');
                    foreach (string field in allFields)
                    {
                        int ItemId = int.Parse(field);
                        Items.Add(ItemId);
                    }
                }

                // Add BaseSkills
                if (mob.Skills != null)
                {
                    string[] allFields = mob.Skills.Split(',');
                    foreach (string field in allFields)
                    {
                        int skillId = int.Parse(field);
                        Skills.Add(skillId);
                    }
                }
            }
            catch
            {
            }
        }
#endif
        #endregion
    }
}
