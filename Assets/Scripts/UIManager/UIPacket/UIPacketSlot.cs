using UnityEngine;
using System.Collections;
using DEngine.Common.Config;

public class UIPacketSlot : MonoBehaviour {

    public UIPacketManager manager;
    public UISprite promotion;
    public UISprite loot;
    public UILabel lblHeader;
    public UILabel lblPrice;
    public int slotIndex = 0;

    public void Init(int slot, UIPacketManager _manager)
    {
        if (Global.language == Global.Language.VIETNAM)
        {
            promotion.spriteName = "km_vn";
        }
        else
        {
            promotion.spriteName = "km_en";
        }

        loot.spriteName = string.Format("Packet{0}", slot);
        lblHeader.text = GameConfig.PROMOTION_PACKS[slot - 1].Name;
        lblPrice.text = string.Format("{0} Gold", GameConfig.PROMOTION_PACKS[slot - 1].Price);
        manager = _manager;
        slotIndex = slot;
    }

    public void OnButton_Click()
    {
        manager.SelectedSlot(this);
    }
}
