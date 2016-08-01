using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;

public class UIUpgradeItemSlot : MonoBehaviour
{

    public UITexture icon;
    public UITexture grade;
    public UISprite cooldown;
    public UISprite disable;
    public UISprite selected;
    public UILabel amount;

    public UserItem item { get; set; }
    public UIItemUpgradeManager manager { get; set; }

    public static UIUpgradeItemSlot dragItem;


    #region public methods
    public void SetItem(UserItem _item)
    {
        item = _item;
        ShowItem();
    }

    public void Refresh()
    {
        ShowItem();
    }

    public void OnButton_Click()
    {
    }

    public void OnSelected()
    {
        selected.gameObject.SetActive(true);
    }

    public void OnDeSelected()
    {
        selected.gameObject.SetActive(false);
    }
    #endregion

    #region common methods    
    void OnClick()
    {
        if (item.GameItem.SubKind == (int)ItemSubKind.Equipment)
            manager.ShowItemInfo(item);
    }

    void OnDoubleClick()
    {

        if (GameManager.Status == GameStatus.Lab)
        {
            if (item.GameItem.SubKind != (int)ItemSubKind.ItemUpgrade)
                return;
        }
        else
        {
            if (item.GameItem.SubKind != (int)ItemSubKind.Equipment)
               return;
        }

        manager.itemInfo.Close();
        

        manager.itemResult.Clear();
        manager.OnSelectedItem(item);
    }

    #endregion

    private void ShowItem()
    {
        if (item.GameItem.SubKind == (int)ItemSubKind.Equipment)
            icon.mainTexture = Helper.LoadTextureForEquipItem(item.ItemId);
        else
            icon.mainTexture = Helper.LoadTextureForSupportItem(item.ItemId);

        grade.gameObject.SetActive(false);
        amount.text = "x" + item.Count;
    }

    public static void OnDragRelease()
    {
        UICursorManager.Set(null);
    }
}
