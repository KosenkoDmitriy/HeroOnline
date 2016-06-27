using System.Collections.Generic;
using System.Runtime.InteropServices;

#if _SERVER
using DEngine.HeroServer.GameData;
#endif

namespace DEngine.Common.GameLogic
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SkillAttrib
    {
        public AttribType SrcAttrib;
        public AttribType DstAttrib;
        public float Value;
    }

    public class GameSkill : GameObj
    {
        #region Fields

        public string Description;

        public int SkillType;

        public int TargetType;

        public int CostType;

        public int CostValue;

        public float CoolTime;

        public float CastRange;

        public float EffectRange;

        public float Duration;

        public int EffectType;

        public int EffectMask;

        public readonly List<SkillAttrib> Attribs = new List<SkillAttrib>();

        #endregion

        #region Methods

        public override void Serialize(BinSerializer serializer)
        {
            base.Serialize(serializer);

            serializer.Serialize(ref Description);
            serializer.Serialize(ref SkillType);
            serializer.Serialize(ref TargetType);
            serializer.Serialize(ref CostType);
            serializer.Serialize(ref CostValue);
            serializer.Serialize(ref CoolTime);
            serializer.Serialize(ref CastRange);
            serializer.Serialize(ref EffectRange);
            serializer.Serialize(ref Duration);
            serializer.Serialize(ref EffectType);
            serializer.Serialize(ref EffectMask);

            int count = Attribs.Count;
            serializer.Serialize(ref count);

            if (serializer.Mode == SerializerMode.Read)
            {
                Attribs.Clear();

                for (int i = 0; i < count; i++)
                {
                    SkillAttrib newAttrib = new SkillAttrib();
                    serializer.SerializeEx(ref newAttrib);
                    Attribs.Add(newAttrib);
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    SkillAttrib newAttrib = Attribs[i];
                    serializer.SerializeEx(ref newAttrib);
                }
            }
        }

#if _SERVER
        public void InitData(SkillBase skill)
        {
            Description = skill.Desc;
            SkillType = skill.SkillType;
            TargetType = skill.TargetType;
            CostType = skill.CostType;
            CostValue = skill.CostValue;
            CoolTime = skill.CoolTime;
            CastRange = skill.CastRange;
            EffectRange = skill.EffectRange;
            Duration = skill.Duration;
            EffectType = skill.EffectType;
            EffectMask = skill.EffectMask;

            Attribs.Clear();
            AddAttrib(skill.SrcAtt01, skill.DstAtt01, skill.Value01);
            AddAttrib(skill.SrcAtt02, skill.DstAtt02, skill.Value02);
            AddAttrib(skill.SrcAtt03, skill.DstAtt03, skill.Value03);
            AddAttrib(skill.SrcAtt04, skill.DstAtt04, skill.Value04);
            AddAttrib(skill.SrcAtt05, skill.DstAtt05, skill.Value05);
        }

        private void AddAttrib(int srcAttrib, int dstAttrib, float affectValue)
        {
            SkillAttrib newAttrib = new SkillAttrib()
            {
                SrcAttrib = (AttribType)srcAttrib,
                DstAttrib = (AttribType)dstAttrib,
                Value = affectValue,
            };

            Attribs.Add(newAttrib);
        }
#endif
        #endregion
    }
}
