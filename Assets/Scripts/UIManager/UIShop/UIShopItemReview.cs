using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;

public class UIShopItemReview : MonoBehaviour {

    public UITexture textture;
    public UILabel lblItemName;
    public UILabel lblQuality;
    public UILabel lblType;
    public UILabel lblLevel;
    public UILabel lblDes;

    public void ShowItem(ShopItem shopItem, Texture text)
    {

        textture.mainTexture = text;

        lblItemName.text = shopItem.Name;

        lblType.text = string.Format(GameManager.localization.GetText("Shop_Item_Info_Type"), shopItem.ItemKind.ToString());
        lblLevel.text = string.Format(GameManager.localization.GetText("Shop_Item_Info_LevelRequired"), shopItem.UserLevel.ToString());


        lblDes.text = "";

        Debug.Log("ItemKind "  + shopItem.ItemKind);

        string info = "";
        if (shopItem.ItemKind == ItemKind.Ring)
            info = GameManager.localization.GetText("Shop_Item_Info_Ring_Stats");
        else if (shopItem.ItemKind == ItemKind.Armor)
            info = GameManager.localization.GetText("Shop_Item_Info_Armor_Stats");
        else if (shopItem.ItemKind == ItemKind.Medal)
            info = GameManager.localization.GetText("Shop_Item_Info_Medal_Stats");
        else
        {
            MyLocalization.ItemInfo item = GameManager.localization.getItem(shopItem.ItemId);
            info = item.Description;
        }

        info = Helper.StringToMultiLine(info);

       // Debug.Log(info);
        lblDes.text = info;
    }

}
