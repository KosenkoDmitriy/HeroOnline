using DEngine.Common;
using System.Runtime.InteropServices;

#if _SERVER
using DEngine.HeroServer.GameData;
using System;
#endif

namespace DEngine.Common.GameLogic
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ItemMainOpt
    {
        public AttribType Attrib;
        public float Step;
        public float MinVal;
        public float MaxVal;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ItemAttrib
    {
        public short Grade;
        public AttribType Attrib;
        public float Value;

        public void SetGradeAttrib(int value)
        {
            Grade = (short)((value & 0xffff0000) >> 16);
            Attrib = (AttribType)(value & 0x0000ffff);
        }

        public int GetGradeAttrib()
        {
            return (Grade << 16 | (int)Attrib);
        }
    }

    public class GameItem : GameObj
    {
        #region Fields

        public string Description;

        public int Kind;

        public int SubKind;

        public int Level;

        public int Stack;

        public int SellPrice;

        public ItemMainOpt[] MainOptions = new ItemMainOpt[2];

        #endregion

        #region Methods

        public override void Serialize(BinSerializer serializer)
        {
            base.Serialize(serializer);

            serializer.Serialize(ref Description);
            serializer.Serialize(ref Kind);
            serializer.Serialize(ref SubKind);
            serializer.Serialize(ref Level);
            serializer.Serialize(ref Stack);
            serializer.Serialize(ref SellPrice);
        }

#if _SERVER
        public void InitData(ItemBase item)
        {
            Description = item.Desc;
            Kind = item.Kind;
            SubKind = item.SubKind;
            Level = item.Level;
            Stack = item.Stack;
            SellPrice = item.SellPrice;

            MainOptions[0].Attrib = (AttribType)item.Attrib01;
            MainOptions[0].Step = item.Step01;
            MainOptions[0].MinVal = item.MinVal01;
            MainOptions[0].MaxVal = item.MaxVal01;

            MainOptions[1].Attrib = (AttribType)item.Attrib02;
            MainOptions[1].Step = item.Step02;
            MainOptions[1].MinVal = item.MinVal02;
            MainOptions[1].MaxVal = item.MaxVal02;
        }

        public float GenAttribValue(float step, float min, float max, bool mainOpt = false)
        {
            if (step < 0.0001)
                return min;

            float retValue = min;
            float levelRank = mainOpt ? ((float)Level - 1) / 9 : 1f;

            float range = max - min;
            min = min + 0.7f * range * levelRank;
            max = min + 0.3f * range;

            float randRank = (float)Helpers.Random.NextDouble();
            randRank *= randRank;

            randRank = 0.7f * randRank + 0.3f * levelRank;
            float genValue = min + (max - min) * randRank;

            while ((retValue + step / 2) < genValue)
                retValue += step;

            return retValue;
        }
#endif
        #endregion
    }
}
