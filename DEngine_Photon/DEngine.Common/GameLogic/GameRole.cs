using DEngine.Common;
using System.Collections.Generic;

#if _SERVER
using DEngine.HeroServer.GameData;
#endif

namespace DEngine.Common.GameLogic
{
    public class GameRole : GameObj
    {
        #region Fields

        public string Description;

        public int Type;//RoleType( group of role)
        //public enum RoleType
        //{
        //    Default = 0,
        //    Hero = 1,
        //    Mob = 2,
        //    Elite = 3,
        //    Boss = 4,
        //    Hostage = 5,
        //    HiredHero = 6,
        //}

        public int Class;

        public string AssetPath;

        public int Strength;

        public int Agility;

        public int Intelligent;

        public int MoveSpeed;

        public int AttackRate;

        #endregion

        #region Properties

#if _SERVER
      //not use
        public List<int> Items { get; protected set; }

        public List<int>[] Skills { get; protected set; }
#endif

        #endregion

        #region Methods

        public override void Serialize(BinSerializer serializer)
        {
            base.Serialize(serializer);

            serializer.Serialize(ref Description);
            serializer.Serialize(ref Type);
            serializer.Serialize(ref Class);
            serializer.Serialize(ref AssetPath);
            serializer.Serialize(ref Strength);
            serializer.Serialize(ref Agility);
            serializer.Serialize(ref Intelligent);
            serializer.Serialize(ref MoveSpeed);
            serializer.Serialize(ref AttackRate);
        }

#if _SERVER
        public void InitData(RoleBase role)
        {
            Type = role.Type;
            Class = role.Class;
            AssetPath = role.AssetPath;
            Description = role.Desc;
            Strength = role.Strength;
            Agility = role.Agility;
            Intelligent = role.Intelligent;
            MoveSpeed = role.MoveSpeed;
            AttackRate = role.AttackRate;

            try
            {
                Items = new List<int>();

                Skills = new List<int>[(int)ElemType.Count];
                for (int i = 0; i < (int)ElemType.Count; i++)
                    Skills[i] = new List<int>();

                // Add BaseItems
                if (role.Items != null)
                {
                    string[] allFields = role.Items.Split(',');
                    foreach (string field in allFields)
                    {
                        int ItemId = int.Parse(field);
                        Items.Add(ItemId);
                    }
                }

                // Add BaseSkills
                if (role.Skills != null)
                {
                    string[] allFields = role.Skills.Split(',');
                    foreach (string field in allFields)
                    {
                        int skillId = int.Parse(field);
                        for (int i = 0; i < (int)ElemType.Count; i++)
                            Skills[i].Add(skillId);
                    }
                }

                // Add ExtraSkills
                foreach (RoleExtra roleExt in role.RoleExtras)
                {
                    string[] allFields = roleExt.Skills.Split(',');
                    foreach (string field in allFields)
                    {
                        int skillId = int.Parse(field);
                        Skills[roleExt.ElemId].Add(skillId);
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
