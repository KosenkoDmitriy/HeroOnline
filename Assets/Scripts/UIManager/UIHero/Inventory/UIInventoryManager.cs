using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DEngine.Common.GameLogic;
using System.Linq;

public class UIInventoryManager : MonoBehaviour {



    public UIHeroManager heroManager { get; set; }
    public UIGrid slotRoot;
    public GameObject slotPrefab;
    public UIScrollBar srollBar;
    public UIScrollView scrollView;
    public static UIInventorySlot selectedSlot;

    public GameObject support_info;
    public UITexture support_Icon;
    public UILabel support_name;
    public UILabel support_Des;

    public GameObject itemConformRoot;
    public UILabel lblUseItem_Header;
    public UILabel lblUseItem_Amount;
    public UILabel lblUseItem_btnCancel;
    public UILabel lblUseItem_btnUSe;
    public UIInput lblUseItem_amount;

    private int _maxSlot;
    private List<UIInventorySlot> _slots;

    void Start()
    {
        _maxSlot = GameManager.maxSlotInventory;
        _slots = new List<UIInventorySlot>();
        Localization();
        StartCoroutine(InitSlot());
    }

    #region private
    private void Localization()
    {
        lblUseItem_Amount.text = GameManager.localization.GetText("Hero_Inventory_UseItem_Amount");
        lblUseItem_btnCancel.text = GameManager.localization.GetText("Global_btn_Cancel");
        lblUseItem_btnUSe.text = GameManager.localization.GetText("Hero_Inventory_UseItem_btnUse");
    }
    private IEnumerator InitSlot()
    {
        List<UserItem> userItems = GameManager.GameUser.UserItems.ToList();//.OrderByDescending(p => p.GameItem.Kind).ThenByDescending(p=>p.Grade).ToList();
        foreach (UserItem userItem in userItems)
        {
            GameObject go = NGUITools.AddChild(slotRoot.gameObject, slotPrefab);
            UIInventorySlot slot = go.GetComponent<UIInventorySlot>();
            slot.SetItem(userItem);
            slot.inventory = this;
            _slots.Add(slot);
            slotRoot.Reposition();   
            yield return null;
        }
   
        scrollView.ResetPosition();
    }  
    private void ShowInforSupport()
    {
        support_Icon.mainTexture = selectedSlot.icon.mainTexture;
        MyLocalization.ItemInfo itemInfo = GameManager.localization.getItem(selectedSlot.item.ItemId);
        support_name.text = itemInfo.Name;
        support_Des.text = itemInfo.Description;
    }
    private void ShowInfo()
    {
        if (selectedSlot.item.GameItem.SubKind != (int)ItemSubKind.Equipment)
        {
            if (support_info.activeInHierarchy)
                ShowInforSupport();

            if (heroManager.information.gameObject.activeInHierarchy)
            {
                heroManager.information.gameObject.SetActive(false);
                support_info.SetActive(true);
                ShowInforSupport();
            }
        }
        else
        {
            if (heroManager.information.gameObject.activeInHierarchy)
                heroManager.information.SetItem(selectedSlot.item);

            if (support_info.activeInHierarchy)
            {
                support_info.SetActive(false);
                heroManager.information.gameObject.SetActive(true);
                heroManager.information.SetItem(selectedSlot.item);
            }
        }
    }
    private void Filter(ItemKind kind)
    {
        foreach (UIInventorySlot slot in _slots)
        {

            if (kind == ItemKind.Material)
            {
                if (slot.item.GameItem.Kind == (int)ItemKind.Material || slot.item.GameItem.Kind == (int)ItemKind.Consume)
                    if (!slot.gameObject.activeInHierarchy)
                        slot.gameObject.SetActive(true);

            }
            else if (kind == (int)ItemKind.None || slot.item.GameItem.Kind == (int)kind)
            {
                if (!slot.gameObject.activeInHierarchy)
                    slot.gameObject.SetActive(true);
            }
            else if (slot.item.GameItem.Kind != (int)kind)
            {
                if (slot.gameObject.activeInHierarchy)
                    slot.gameObject.SetActive(false);
            }
        }

        slotRoot.Reposition();
        scrollView.ResetPosition();

    }
    #endregion


    #region button
    public void OnButtonSupport_Click()
    {
        Filter(ItemKind.Material);
    }
    public void OnButtonRing_Click()
    {
        Filter(ItemKind.Ring);
    }
    public void OnButtonArmor_Click()
    {
        Filter(ItemKind.Armor);
    }
    public void OnButtonMedal_Click()
    {
        Filter(ItemKind.Medal);
    }
    public void OnButtonAll_Click()
    {
        Filter(ItemKind.None);
    }
    public void OnButtonAuto_Click()
    {
    }
    public void OnButtonEnhancement_Click()
    {
        GameScenes.ChangeScense(GameScenes.MyScene.Hero, GameScenes.MyScene.ItemUpgrade);
    }
    public void OnButtonInfo_Click()
    {
        if (selectedSlot == null) return;
        if (selectedSlot.item.GameItem.SubKind == (int)ItemSubKind.Equipment)
        {

            heroManager.information.gameObject.SetActive(true);
            heroManager.information.SetItem(selectedSlot.item);
        }
        else
        {
            support_info.SetActive(true);
            ShowInforSupport();
        }
    }    
    public void OnButtonUse_Click()
    {
        if (selectedSlot == null) return;
        if (selectedSlot.item.GameItem.SubKind == (int)ItemSubKind.HeroBook)
        {
            RefreshGUI();
            itemConformRoot.SetActive(true);           
        }
        else if (selectedSlot.item.GameItem.Kind != (int)ItemKind.Material)
        {
            EquipItemForRole();
        }
    }
    public void OnButtonSell_Click()
    {
    }
    public void OnButtonCloseInfo_Click()
    {
        support_info.SetActive(false);
    }
    public void OnButtonCloseUseItem_Click()
    {
        itemConformRoot.SetActive(false);
    }
    public void OnButtonUseItem_Click()
    {
        int amount = 0;
        if (!int.TryParse(lblUseItem_amount.value, out amount))
        {
            return;
        }
        if (amount > selectedSlot.item.Count)
        {
            MessageBox.ShowDialog(GameManager.localization.GetText("Hero_Inventory_UseItem_Insufficient"), UINoticeManager.NoticeType.Message);
            return;
        }

        GameManager.GameUser.SetItemForRole(heroManager.heroSlotSelected.userRole, selectedSlot.item, amount);
        heroManager._heroMenuController.SendRequestEquipItem(heroManager.heroSlotSelected.userRole.Id, selectedSlot.item.Id, amount);

        StartCoroutine(RefreshSlot());

        itemConformRoot.SetActive(false);
    }
    public void txtAmount_TextChanged()
    {
        int amount = 0;
        if (!int.TryParse(lblUseItem_amount.value, out amount))
        {
            return;
        }
        if (amount == 0)
        {
            lblUseItem_amount.value = "1";
        }
        if (amount < 0)
        {
            lblUseItem_amount.value = Mathf.Abs(amount).ToString();
        }
        if (amount > selectedSlot.item.Count)
        {
            lblUseItem_amount.value = selectedSlot.item.Count.ToString();
            return;
        }
    }
    public void RefreshGUI()
    {
        if (selectedSlot == null || selectedSlot.item == null) return;
        lblUseItem_amount.value = "1";
        string s = string.Format(GameManager.localization.GetText("Hero_Inventory_UseItem_Header"), selectedSlot.item.Name, heroManager.heroSlotSelected.userRole.Name);
        lblUseItem_Header.text = s;
    }
    #endregion

    #region Public methods
    public IEnumerator RefreshSlot()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            UIInventorySlot slot = _slots[i];
            slot.Refresh();
            if (slot.item.Count <= 0)
            {
                NGUITools.Destroy(slot.gameObject);
                _slots.Remove(slot);
                slotRoot.Reposition();
                scrollView.ResetPosition();
                i--;
            }
            yield return null;
        }
    }
    public void OnSelectedSlot(UIInventorySlot curSlot)
    {
        if (selectedSlot != null)
            selectedSlot.OnDeSelected();

        selectedSlot = curSlot;

        selectedSlot.OnSelected();

        ShowInfo();
    }
    #endregion

    #region Equip
    private void EquipItemForRole()
    {
        string s = string.Format(GameManager.localization.GetText("Hero_Inventory_EquipItem"), selectedSlot.item.Name, heroManager.heroSlotSelected.userRole.Name);
        UINoticeManager.OnButtonOK_click += OnOkEquip;
        MessageBox.ShowDialog(s, UINoticeManager.NoticeType.YesNo);
    }
    private void OnOkEquip()
    {
        UserRole userRole = heroManager.heroSlotSelected.userRole;
        UserItem itemEquip = selectedSlot.item;

        if (userRole.Id == itemEquip.RoleUId) return;// đang trang bị

        if (itemEquip.RoleUId > 0)// đang trang bị cho role khác
        {
            GameManager.GameUser.SetItemForRole(null, itemEquip);
            heroManager._heroMenuController.SendRequestEquipItem(0, itemEquip.Id);
        }

        ////Đang trang bi cùng loại////
        UserItem oldItemEquiped = GameManager.GameUser.UserItems.FirstOrDefault(p => p.RoleUId == userRole.Id && p.GameItem.Kind == itemEquip.GameItem.Kind);
        if (oldItemEquiped != null)
        {
            GameManager.GameUser.SetItemForRole(null, oldItemEquiped);           
        }
        ///////////////////////////////


     /*   if (!GameManager.GameUser.SetItemForRole(userRole, itemEquip))
        {
            MessageBox.ShowDialog(GameManager.localization.GetText("Equip_CantEuqip"), UINoticeManager.NoticeType.Message);
            return;
        }*/

     //   heroManager._heroMenuController.SendRequestEquipItem(userRole.Id, itemEquip.Id);
    }
    #endregion
}
