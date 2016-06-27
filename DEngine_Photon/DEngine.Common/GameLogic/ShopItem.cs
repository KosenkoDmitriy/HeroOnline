using DEngine.Common;

#if _SERVER
using DEngine.HeroServer.GameData;
#endif

namespace DEngine.Common.GameLogic
{
    public class ShopItem : GameObj
    {
        #region Fields

        public ItemKind ItemKind;

        public int ItemId;

        public int UserLevel;

        public int Promotion;

        public int PriceSilver;

        public int PriceGold;

        public float PriceUSD;

        public float PriceVND;

        public int Discount;

        #endregion

        #region Properties

        public int PriceSilverSale { get; set; }

        public int PriceGoldSale { get; set; }

        #endregion

        #region Methods

        public override void Serialize(BinSerializer serializer)
        {
            base.Serialize(serializer);

            if (serializer.Mode == SerializerMode.Read)
                ItemKind = (ItemKind)serializer.Reader.ReadInt32();
            else
                serializer.Writer.Write((int)ItemKind);

            serializer.Serialize(ref ItemId);
            serializer.Serialize(ref UserLevel);
            serializer.Serialize(ref Promotion);
            serializer.Serialize(ref PriceSilver);
            serializer.Serialize(ref PriceGold);
            serializer.Serialize(ref PriceUSD);
            serializer.Serialize(ref PriceVND);
            serializer.Serialize(ref Discount);

            if (serializer.Mode == SerializerMode.Read)
            {
                PriceSilverSale = PriceSilver * (100 - Discount) / 100;
                PriceGoldSale = PriceGold * (100 - Discount) / 100;
            }
        }

#if _SERVER
        public void InitData(Shop shop)
        {
            ItemKind = (ItemKind)shop.ItemKind;
            ItemId = shop.ItemId;
            UserLevel = shop.UserLevel;
            PriceSilver = shop.PriceSilver;
            PriceGold = shop.PriceGold;
            Discount = shop.Discount;
            PriceUSD = shop.PriceUSD;
            PriceVND = shop.PriceVND;
            Promotion = shop.Promotion;

            PriceSilverSale = PriceSilver * (100 - Discount) / 100;
            PriceGoldSale = PriceGold * (100 - Discount) / 100;
        }
#endif
        #endregion
    }
}
