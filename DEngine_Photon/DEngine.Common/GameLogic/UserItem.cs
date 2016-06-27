using System;
using System.Linq;
using DEngine.Common;
using DEngine.Common.Config;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#if _SERVER
using DEngine.HeroServer;
using DEngine.HeroServer.GameData;
#endif

namespace DEngine.Common.GameLogic
{
    public class UserItem : GameObj
    {
        #region Fields

        public int RoleUId;//index role in dbo.role table

        public int UserId;

        public int ItemId;

        public int Grade;

        public int[] Ranks;

        public int Count;

        public int Store;

        public int Position;

        public readonly List<ItemAttrib> Attribs = new List<ItemAttrib>();

        #endregion

        #region Properties

        public GameUser GameUser { get; set; }

        public GameItem GameItem { get; set; }

        public int MinRoleLevel
        {
            get { return GameItem != null ? GameItem.Level * 5 - 4 : 50; }
        }

        public float NextUseTime { get; set; }

        #endregion

        #region Methods

        public UserItem()
        {
            Ranks = new int[(int)ItemGrade.Count];
        }

        public override void Serialize(BinSerializer serializer)
        {
            base.Serialize(serializer);

            serializer.Serialize(ref RoleUId);
            serializer.Serialize(ref ItemId);
            serializer.Serialize(ref Grade);
            for (int i = 0; i < Ranks.Length; i++)
                serializer.Serialize(ref Ranks[i]);
            serializer.Serialize(ref Count);
            serializer.Serialize(ref Store);
            serializer.Serialize(ref Position);

            int count = Attribs.Count;
            serializer.Serialize(ref count);

            if (serializer.Mode == SerializerMode.Read)
            {
                Attribs.Clear();

                for (int i = 0; i < count; i++)
                {
                    ItemAttrib newAttrib = new ItemAttrib();
                    serializer.SerializeEx(ref newAttrib);
                    Attribs.Add(newAttrib);
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    ItemAttrib newAttrib = Attribs[i];
                    serializer.SerializeEx(ref newAttrib);
                }
            }
        }

        public UserItem GetNextRankItem(int attGrade)
        {
            UserItem newItem = new UserItem()
            {
                ItemId = this.ItemId,
                Grade = this.Grade,
                GameItem = this.GameItem,
            };
            for (int i = 0; i < newItem.Ranks.Length; i++)
                newItem.Ranks[i] = this.Ranks[i];

            newItem.Attribs.AddRange(this.Attribs);
            newItem.UpgradeRank(attGrade);

            return newItem;
        }

        public void UpgradeRank(int attGrade)
        {
            int nextRank = Ranks[attGrade] + 1;

            for (int i = 0; i < Attribs.Count; i++)
            {
                ItemAttrib curAttrib = Attribs[i];
                if (curAttrib.Grade == attGrade)
                {
                    curAttrib.Value = curAttrib.Value * (5 + nextRank) / (5 + Ranks[attGrade]);
                    Attribs[i] = curAttrib;
                }
            }

            Ranks[attGrade] = nextRank;
        }

#if _SERVER

        public ItemScript ItemScript { get; set; }

        public void InitData(Item item)
        {
            Id = item.Item_UID;
            Name = GameItem.Name;

            RoleUId = item.Role_UID;
            UserId = item.UserId;
            ItemId = item.ItemId;
            Grade = item.Grade;
            Ranks[0] = item.Rank0;
            Ranks[1] = item.Rank1;
            Ranks[2] = item.Rank2;
            Ranks[3] = item.Rank3;
            Ranks[4] = item.Rank4;
            Count = item.Count;
            Store = item.Store;
            Position = item.Position;

            Attribs.Clear();

            if (item.Attribs != null)
            {
                byte[] buffer = item.Attribs.ToArray();
                float[] attribs = new float[buffer.Length / 4];
                Buffer.BlockCopy(buffer, 0, attribs, 0, buffer.Length);
                for (int i = 0; i < attribs.Length /2 ; i++)
                {
                    ItemAttrib newAttrib = new ItemAttrib();
                    newAttrib.SetGradeAttrib((int)attribs[2 * i]);
                    newAttrib.Value = attribs[2 * i + 1];

                    Attribs.Add(newAttrib);
                }
            }
        }

        public void UpdateData(Item item)
        {
            item.Role_UID = RoleUId;
            item.Grade = Grade;
            item.Rank0 = Ranks[0];
            item.Rank1 = Ranks[1];
            item.Rank2 = Ranks[2];
            item.Rank3 = Ranks[3];
            item.Rank4 = Ranks[4];
            item.Count = Count;
            item.Store = Store;
            item.Position = Position;

            if (Attribs.Count > 0)
            {
                float[] attribs = new float[Attribs.Count * 2];
                for (int i = 0; i < attribs.Length; i += 2)
                {
                    attribs[i] = (float)Attribs[i / 2].GetGradeAttrib();
                    attribs[i + 1] = Attribs[i / 2].Value;
                }

                byte[] buffer = new byte[attribs.Length * 4];
                Buffer.BlockCopy(attribs, 0, buffer, 0, buffer.Length);
                item.Attribs = buffer;
            }
        }

        public Item GetNewItem()
        {
            Item newItem = new Item()
            {
                UserId = this.UserId,
                ItemId = this.ItemId,
                CreateTime = DateTime.Now,
            };

            UpdateData(newItem);
            return newItem;
        }

        public void CreateRandomeEquip(int userId, int userLevel, int itemGrade)
        {
            List<GameItem> equipItems = new List<GameItem>();
            int itemLevel = (userLevel + 4) / 5;
            foreach (GameItem item in Global.GameItems)
            {
                if (item.Kind < 2 || item.Kind > 4)
                    continue;

                if (item.Level == itemLevel)
                    equipItems.Add(item);
            }

            Grade = itemGrade;

            if (equipItems.Count > 0)
            {
                int idx = Helpers.Random.Next(equipItems.Count);
                int itemId = equipItems[idx].Id;
                CreateRandom(userId, itemId, 1, itemGrade);
            }
        }

        public void CreateRandom(int userId, int itemId, int itemCount, int itemRank)
        {
            GameItem = (GameItem)Global.GameItems[itemId];
            if (GameItem == null)
                return;

            Name = GameItem.Name;

            UserId = userId;
            ItemId = itemId;
            Count = itemCount;
            if (Count > GameItem.Stack)
                Count = GameItem.Stack;

            GenerateAttribs(itemRank);
        }

        public void GenerateAttribs(int itemRank)
        {
            Attribs.Clear();
            GenerateMainOpts();

            if (GameItem.Kind >= (int)ItemKind.Ring && GameItem.Kind <= (int)ItemKind.Medal)
            {
                if (Grade == 0)
                {
                    if (itemRank >= ItemConfig.GRADE_RANKS.Count)
                        itemRank = ItemConfig.GRADE_RANKS.Count - 1;

                    float[] gradeRanks = ItemConfig.GRADE_RANKS[itemRank];

                    Grade = Helpers.GetRandomIndex(gradeRanks) + 1;
                }

                GenerateExtraOpts(Grade);
            }
        }

        private void GenerateMainOpts()
        {
            foreach (ItemMainOpt opt in GameItem.MainOptions)
            {
                if (opt.Attrib != AttribType.None)
                {
                    ItemAttrib attrib = new ItemAttrib();
                    attrib.Grade = (short)ItemGrade.White;
                    attrib.Attrib = opt.Attrib;
                    attrib.Value = GameItem.GenAttribValue(opt.Step, opt.MinVal, opt.MaxVal);
                    Attribs.Add(attrib);
                }
            }
        }

        private void GenerateExtraOpts(int grade)
        {
            float[] rateValues = null;
            switch ((ItemGrade)grade)
            {
                case ItemGrade.Green:
                    rateValues = ItemConfig.GRADE_RATES.GreenRate;
                    break;

                case ItemGrade.Blue:
                    GenerateExtraOpts(grade - 1);
                    rateValues = ItemConfig.GRADE_RATES.BlueRate;
                    break;

                case ItemGrade.Yellow:
                    GenerateExtraOpts(grade - 1);
                    rateValues = ItemConfig.GRADE_RATES.YellowRate;
                    break;
            }

            if (rateValues == null || rateValues.Length < 1)
                return;

            int optCount = 0;
            float rankValue = (float)Helpers.Random.NextDouble() * rateValues[0];
            while (optCount < rateValues.Length && rateValues[optCount] >= rankValue)
                optCount++;

            int itemKind = GameItem.Kind;
            List<SlotOption> slotOptions = ItemConfig.SLOT_OPTIONS.Where(o => o.Kind == itemKind && o.Grade == grade).ToList();
            for (int i = 0; i < optCount; i++)
            {
                if (slotOptions.Count < 1)
                    break;

                float[] optRates = slotOptions.Select(o => o.Rate).ToArray();
                int optIdx = Helpers.GetRandomIndex(optRates);

                SlotOption slotOpt = slotOptions[optIdx];
                ItemAttrib attrib = new ItemAttrib();
                attrib.Grade = (short)grade;
                attrib.Attrib = (AttribType)slotOpt.Attrib;
                attrib.Value = GameItem.GenAttribValue(slotOpt.Step, slotOpt.MinValue, slotOpt.MaxValue, false);
                Attribs.Add(attrib);

                // Remove used opt
                slotOptions.RemoveAt(optIdx);
            }
        }

#endif
        #endregion
    }
}
