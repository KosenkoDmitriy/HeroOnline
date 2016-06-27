using DEngine.Common;
using DEngine.Common.Config;
using DEngine.Common.GameLogic;
using DEngine.HeroServer.GameData;
using ExitGames.Logging;
using Photon.SocketServer;
using System.Linq;

namespace DEngine.HeroServer
{
    public enum PlayMode
    {
        Default,
        Mission,
        Dungeon,
        Pillage,
    }

    public abstract class GamePlayService
    {
        #region Fields

        protected static ILogger Log = LogManager.GetCurrentClassLogger();

        protected GameAward GameAward = new GameAward();

        #endregion

        #region Properties

        public int Id { get; set; }

        public abstract PlayMode Mode { get; }

        public ZoneService Zone { get; protected set; }

        public GameUser GameUser { get; protected set; }

        public int ItemMaxUse { get; protected set; }

        #endregion

        #region Methods

        public abstract void OnBattleBegin(BattleService battle);

        public abstract void OnBattleEnd(BattleService battle, int result);

        public abstract void OnGamePlayBegin();

        public abstract void OnGamePlayResume();

        public abstract void OnGamePlayEnd(int result);

        public abstract void OnItemDrop(float[] vecPos = null);


        //Send Item 
        protected void SendItemDrop(DropItem[] dropItems, float[] vecPos = null)
        {
            if (vecPos == null)
                vecPos = new float[] { 0, 0, 0 };

            float[] dropRates = dropItems.Select(i => i.Rate).ToArray();

            int idx = Helpers.GetRandomIndex(dropRates);
            int itemId = dropItems[idx].Id;
            int itemQty = dropItems[idx].Quantity;

            if (itemId == 0)
                return;

            EventData eventData = new EventData((byte)EventCode.Battle, new object());
            eventData[(byte)ParameterCode.SubCode] = SubCode.ItemDrop;
            eventData[(byte)ParameterCode.ItemId] = itemId;
            eventData[(byte)ParameterCode.Quantity] = itemQty;
            eventData[(byte)ParameterCode.TargetPos] = vecPos;

            if (itemId == -1) // Gold
            {
                GameUser.GamePlay.AddMoneyAward(0, itemQty);
                eventData[(byte)ParameterCode.ItemKind] = (int)ItemKind.Gold;
                eventData[(byte)ParameterCode.ItemData] = 3;
            }
            else if (itemId == -2) // Silver
            {
                GameUser.GamePlay.AddMoneyAward(itemQty, 0);
                eventData[(byte)ParameterCode.ItemKind] = (int)ItemKind.Silver;
                eventData[(byte)ParameterCode.ItemData] = 2;
            }
            else
            {
                GameItem gameItem = (GameItem)Global.GameItems[itemId];
                if (gameItem != null)
                {
                    GameUser.GamePlay.AddItemAdward(itemId, itemQty);
                    eventData[(byte)ParameterCode.ItemKind] = gameItem.Kind;
                    eventData[(byte)ParameterCode.ItemData] = gameItem.Level;
                }
                else
                {
                    Log.WarnFormat("GameItem {0} is null", itemId);
                    return;
                }
            }

            Zone.SendUserEvent(GameUser, eventData);
        }

        public void AddMoneyAward(int silver, int gold)
        {
            GameAward.Silver += silver;
            GameAward.Gold += gold;
        }

        public void AddItemAdward(int itemId, int itemCount)
        {
            if (itemId > 0)
            {
                if (GameAward.Items.ContainsKey(itemId))
                    GameAward.Items[itemId] += itemCount;
                else
                    GameAward.Items[itemId] = itemCount;
            }
        }

        public void AddRoleAdward(int roleId)
        {
            if (GameAward.Roles.ContainsKey(roleId))
                GameAward.Roles[roleId] += 1;
            else
                GameAward.Roles[roleId] = 1;
        }

        #endregion
    }
}
