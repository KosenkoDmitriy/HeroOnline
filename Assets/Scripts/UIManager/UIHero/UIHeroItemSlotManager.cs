using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;
using System.Linq;

public class UIHeroItemSlotManager : MonoBehaviour {


    public UIButton slot;
    public UILabel uiItemName;
    public UISprite uiEquiped;
    public UISprite uiPriceIcon;
    public UISprite uiBackgroundLevel;
    public UILabel uiLevel;
    public GameObject selected;
    public UserItem itemContain;
    private UIHeroManager _uiHeroManager;

    public void SetItem(UserItem item,UIHeroManager manager)
    {
        itemContain = item;
        _uiHeroManager = manager;

        uiItemName.text = itemContain.Name;
        uiLevel.text = "Lvl " + itemContain.GameItem.Level;

        Color color;
        if (item.GameItem.SubKind == (int)ItemSubKind.Equipment)
            color = Helper.ItemColor[itemContain.Grade];
        else
        {
            color = Color.white;
            uiItemName.text += " x" + item.Count;
        }
           
        uiItemName.color = color;
        uiLevel.color = color;       
        
        uiPriceIcon.spriteName = "gia_equip_vang";

        Refresh();
    }

    public void Refresh()
    {
        if(_uiHeroManager.heroSlotSelected.userRole.Id == itemContain.RoleUId)
        {
            uiEquiped.gameObject.SetActive(true);
            _uiHeroManager.btnEquip.normalSprite = "unequip";
            _uiHeroManager.btnEquip.hoverSprite = "unequip_down";
            _uiHeroManager.btnEquip.pressedSprite = "unequip_down";
        }
        else
        {
            uiEquiped.gameObject.SetActive(false);
            _uiHeroManager.btnEquip.normalSprite = "equip";
            _uiHeroManager.btnEquip.hoverSprite = "equip_down";
            _uiHeroManager.btnEquip.pressedSprite = "equip_down";
        }
    }

    public void OnButtonSell_Click()
    {
        Debug.Log("OnButtonSell_Click");
    }

    public void OnSelected()
    {
        slot.normalSprite = "equip_bar_down";
        selected.SetActive(true);
        if (_uiHeroManager.itemSlotSelected != null)
        {
            _uiHeroManager.itemSlotSelected.OnDeSelected();
        }

        _uiHeroManager.itemSlotSelected = this;

        _uiHeroManager.information.gameObject.SetActive(false);

        Refresh();
    }

    public void OnDeSelected()
    {
        selected.SetActive(false);
        slot.normalSprite = "equip_bar";
    }

    public void OnInformation_Click()
    {
        _uiHeroManager.information.gameObject.SetActive(true);
        _uiHeroManager.information.SetItem(itemContain);

    }
}
