using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;
using System.Linq;

public class UIHeroUseItem : MonoBehaviour {


    public UIHeroNewManager heroManager;
    public GameObject itemConfirmRoot;
    public UILabel lblUseItem_Header;
    public UILabel lblUseItem_Amount;
    public UILabel lblUseItem_btnCancel;
    public UILabel lblUseItem_btnUSe;
    public UIInput lblUseItem_amount;
    public UIPlayTween playTweenOpen;
    public UIPlayTween playTweenClose;
    public UIHeroInventoryManager inventory;

    private UserItem userItem;
    private UserRole userRole;

    #region public methods
    public void setItem(UserItem item)
    {
        userItem = item;
        userRole = heroManager.heroSelected();
        if (userItem == null || userRole == null) return;
        RefreshGUI();
        playTweenOpen.Play(true);
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
        if (amount > userItem.Count)
        {
            lblUseItem_amount.value = userItem.Count.ToString();
            return;
        }
    }
    public void OnButtonUseItem_Click()
    {
        int amount = 0;
        if (!int.TryParse(lblUseItem_amount.value, out amount))
        {
            return;
        }
        if (amount > userItem.Count)
        {
            MessageBox.ShowDialog(GameManager.localization.GetText("Hero_Inventory_UseItem_Insufficient"), UINoticeManager.NoticeType.Message);
            return;
        }

        GameManager.GameUser.SetItemForRole(userRole, userItem, amount);
        heroManager.OnUseItem(userItem, amount);


        UserItem gameUserIem = GameManager.GameUser.UserItems.FirstOrDefault(p => p.Id == userItem.Id);
        if (gameUserIem != null && userItem != null)
        {
            gameUserIem.Count = userItem.Count;
        }

        if (gameUserIem != null)
        {
            if (gameUserIem.Count <= 0)
            {
                GameManager.GameUser.UserItems.Remove(gameUserIem);
            }
        }
        playTweenClose.Play(true);
    }
    #endregion


    #region private methods
    private void RefreshGUI()
    {
        if (userItem == null || userRole == null) return;
        lblUseItem_amount.value = "1";
        string s = string.Format(GameManager.localization.GetText("Hero_Inventory_UseItem_Header"), userItem.Name, userRole.Name);
        lblUseItem_Header.text = s;
    }

    private void Localization()
    {
        lblUseItem_Amount.text = GameManager.localization.GetText("Hero_Inventory_UseItem_Amount");
        lblUseItem_btnCancel.text = GameManager.localization.GetText("Global_btn_Cancel");
        lblUseItem_btnUSe.text = GameManager.localization.GetText("Hero_Inventory_UseItem_btnUse");
    }

    
    #endregion


}
