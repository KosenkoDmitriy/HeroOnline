using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;
using System.Collections.Generic;
using System.Linq;

public class UIHeroInventoryManager : MonoBehaviour {

    [System.Serializable]
    public struct UIInventory
    {
        public UIGrid root;
        public GameObject prefab;
        public UIScrollView scrollView;
        public GameObject background;
    }

    public UIHeroNewManager heroManager;
    public UIInventory uiInventory;

    public UIHeroInventorySlot selectedSlot { get; set; }
   
    private UIPanel _inventoryPanel;
    private List<UIHeroInventorySlot> _slots;

    public void Init()
    {
        _inventoryPanel = GetComponent<UIPanel>();
        InitSlot();
    }

    #region public methods
    public void OpenInventory()
    {
        _inventoryPanel.depth = 2;
    }
    public void FinishedCloseInventory()
    {
        _inventoryPanel.depth = -1;
    }
    public void Filer(ItemKind kind)
    {
        foreach (UIHeroInventorySlot slot in _slots)
        {
            if (kind == (int)ItemKind.None)
            {
                if (!slot.gameObject.activeSelf)
                {
                    slot.gameObject.SetActive(true);
                }
            }
            else if (slot.userItem.GameItem.Kind == (int)kind)
            {
                if (!slot.gameObject.activeSelf)
                {
                    slot.gameObject.SetActive(true);
                }
            }
            else if (slot.userItem.GameItem.Kind != (int)kind)
            {
                if (slot.gameObject.activeSelf)
                {
                    slot.gameObject.SetActive(false);
                }
            }
        }

        uiInventory.scrollView.ResetPosition();
        uiInventory.root.Reposition();   
    }
    public void Filer(ItemSubKind kind)
    {
        foreach (UIHeroInventorySlot slot in _slots)
        {           
            if (slot.userItem.GameItem.SubKind == (int)kind)
            {
                if (!slot.gameObject.activeSelf)
                {
                    slot.gameObject.SetActive(true);                  
                }
            }
            else if (slot.userItem.GameItem.SubKind != (int)kind)
            {
                if (slot.gameObject.activeSelf)
                {
                    slot.gameObject.SetActive(false);              
                }
            }
        }

        uiInventory.scrollView.ResetPosition();
        uiInventory.root.Reposition(); 
    }
    public void OnSelectedSlot(UIHeroInventorySlot curSlot)
    {
        if (selectedSlot != null)
            selectedSlot.OnDeSelected();

        selectedSlot = curSlot;
        selectedSlot.OnSelected();

        heroManager.OpenItemDetail(selectedSlot.userItem);
    }
    public void RefreshSlot()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            UIHeroInventorySlot slot = _slots[i];
            NGUITools.Destroy(slot.gameObject);
            
        }
        _slots.Clear();

        InitSlot();
        //slot.Refresh();
        //if (slot.userItem.Count <= 0 || GameManager.GameUser.UserItems.FirstOrDefault(p => p.Id == slot.userItem.Id) == null
        //    || slot.userItem.RoleUId != 0
        //    )
        //{
        //    NGUITools.Destroy(slot.gameObject);
        //    _slots.Remove(slot);
        //    uiInventory.root.Reposition();
        //    i--;
        //}
    }
    #endregion

    #region private methods
    private void InitSlot()
    {
        _slots = new List<UIHeroInventorySlot>();
        List<UserItem> userItems = GameManager.GameUser.UserItems.Where(p => p.RoleUId == 0 && p.Count > 0).ToList();//.OrderByDescending(p => p.GameItem.Kind).ThenByDescending(p => p.Grade).ToList();
        int index = 0;
        foreach (UserItem userItem in userItems)
        {
            GameObject go = NGUITools.AddChild(uiInventory.root.gameObject, uiInventory.prefab);
            go.name = index.ToString();
            UIHeroInventorySlot slot = go.GetComponent<UIHeroInventorySlot>();
            slot.SetItem(userItem, this);
            _slots.Add(slot);
            if (heroManager.InventoryActiveTab == UIHeroNewManager.InvetoryTab.All || heroManager.InventoryActiveTab == UIHeroNewManager.InvetoryTab.None)
            {
                go.SetActive(true);
            }
            else
            {
                switch (heroManager.InventoryActiveTab)
                {
                    case UIHeroNewManager.InvetoryTab.Ring:
                        if (userItem.GameItem.Kind == (int)ItemKind.Ring)
                            go.SetActive(true);
                        break;
                    case UIHeroNewManager.InvetoryTab.Medal:
                        if (userItem.GameItem.Kind == (int)ItemKind.Medal)
                            go.SetActive(true);
                        break;
                    case UIHeroNewManager.InvetoryTab.Armor:
                        if (userItem.GameItem.Kind == (int)ItemKind.Armor)
                            go.SetActive(true);
                        break;
                    case UIHeroNewManager.InvetoryTab.EXP:
                        if (userItem.GameItem.Kind == (int)ItemKind.Consume)
                            go.SetActive(true);
                        break;
                    case UIHeroNewManager.InvetoryTab.Support:
                        if (userItem.GameItem.Kind == (int)ItemKind.Support)
                            go.SetActive(true);
                        break;
                }
            }

            index++;
            //yield return null;
        }

        uiInventory.root.Reposition();
        // uiInventory.root.Reposition();
        //uiInventory.scrollView.ResetPosition();
    }
    #endregion
}
