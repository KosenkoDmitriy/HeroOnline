using UnityEngine;
using System.Collections;
using System.Linq;
using DEngine.Common.GameLogic;

public class UIItemUpgradeResource : MonoBehaviour {

    public GameObject disable;
    public UILabel lablel;
    public UITexture icon;
    public bool drop;
    public UIItemUpgradeManager manager;

    public int resourceIdNeed { get; set; }
    public int resourceAmountNeed { get; set; }
    public UserItem itemEquip { get; set; }

    void OnClick()
    {
        UIUpgradeItemSlot.OnDragRelease();
        OnDoubleClick();
    }

    void OnDoubleClick()
    {
        if (GameManager.Status == GameStatus.Lab)
        {
            if (!drop) return;
        }
        else
        {
            if (drop) return;
        }

        manager.arrowItem.SetActive(false);
        manager.arrowMaterial.SetActive(true);

        UIUpgradeItemSlot.OnDragRelease();
        if (!drop)
            StartCoroutine(manager.FilerInventory(ItemSubKind.Equipment));
        else
            StartCoroutine(manager.FilerInventory(ItemSubKind.ItemUpgrade));
    }
    
    public void OnSetItem(UserItem curItem)
    {
        if (curItem.GameItem.SubKind == (int)ItemSubKind.Equipment)
        {
            icon.mainTexture = Helper.LoadTextureForEquipItem(curItem.ItemId);
            itemEquip = curItem;
        }
        else
        {
            icon.mainTexture = Helper.LoadTextureForSupportItem(curItem.ItemId);
        }

        disable.SetActive(false);
    }
    
    public void SetResourceNeed(int itemId, int amount)
    {
        resourceIdNeed = itemId;
        resourceAmountNeed = amount;
        icon.mainTexture = Helper.LoadTextureForSupportItem(itemId);



        if (!drop)
        {
            disable.SetActive(false);
            return;
        }

        string s = "";
        int resourceAmount = Helper.SumItemCountInList(GameManager.GameUser.UserItems.Where(p => p.ItemId == itemId).ToArray());

        if (resourceAmount >= resourceAmountNeed)
        {
            if (amount > 0)
            {
                s = string.Format("[00FF00]{0}/{1}[-]", resourceAmount, amount);
                disable.SetActive(false);
            }
            else
            {
                s = "[FF0000]0/1[-]";
            }
        }
        else
        {
            s = string.Format("[FF0000]{0}/{1}[-]", resourceAmount , amount);
        }
        lablel.text = s;
    }

    public void Clear()
    {
        itemEquip = null;
        resourceIdNeed = -1;
        resourceAmountNeed = -1;
        icon.mainTexture = null;
        disable.gameObject.SetActive(true);
        lablel.text = "";
    }

}
