using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;

public class UIInventorySlot : MonoBehaviour {

    public UITexture icon;
    public UITexture grade;
    public UISprite cooldown;
    public UISprite disable;
    public UISprite selected;
    public UILabel amount;

    public UserItem item { get; set; }
    public UIInventoryManager inventory { get; set; }

    public void SetItem(UserItem _item)
    {
        item = _item;
        ShowItem();
    }

    public void Refresh()
    {
        ShowItem();
    }

    private void ShowItem()
    {
        switch ((ItemSubKind)item.GameItem.SubKind)
        {
            case ItemSubKind.Equipment:
                icon.mainTexture = Helper.LoadTextureForEquipItem(item.ItemId);
                grade.gameObject.SetActive(true);
                Color color = Helper.ItemColor[item.Grade];
                grade.color = color;
                break;
            default:
                icon.mainTexture = Helper.LoadTextureForSupportItem(item.ItemId);
                grade.gameObject.SetActive(false);
                amount.text = "x" + item.Count;
                break;
        }
    }

    public void OnButton_Click()
    {
        inventory.OnSelectedSlot(this);
    }

    public void OnSelected()
    {
        selected.gameObject.SetActive(true);
    }

    public void OnDeSelected()
    {
        selected.gameObject.SetActive(false);
    }
}
