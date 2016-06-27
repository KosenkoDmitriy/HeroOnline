using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;

public class UIHeroInventorySlot : MonoBehaviour {

    public UITexture icon;
    public UITexture grade;
    public UISprite cooldown;
    public UISprite disable;
    public UITexture selected;
    public UILabel amount;

    public UserItem userItem { get; set; }
    public UIHeroInventoryManager _inventory { get; set; }

    public void SetItem(UserItem item, UIHeroInventoryManager inventory)
    {
        userItem = item;
        _inventory = inventory;
        ShowItem();
    }

    public void Refresh()
    {
        ShowItem();
    }

    private void ShowItem()
    {
        switch ((ItemSubKind)userItem.GameItem.SubKind)
        {
            case ItemSubKind.Equipment:
                icon.mainTexture = Helper.LoadTextureForEquipItem(userItem.ItemId);
                grade.gameObject.SetActive(true);
                Color color = Helper.ItemColor[userItem.Grade];
                grade.color = color;
                break;
            default:
                icon.mainTexture = Helper.LoadTextureForSupportItem(userItem.ItemId);
                grade.gameObject.SetActive(false);
                amount.text = "x" + userItem.Count;
                break; 
        }
    }

    public void OnButton_Click()
    {
        _inventory.OnSelectedSlot(this);
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
