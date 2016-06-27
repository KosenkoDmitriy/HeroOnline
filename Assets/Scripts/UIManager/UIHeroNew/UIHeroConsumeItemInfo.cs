using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;
using System.Linq;

public class UIHeroConsumeItemInfo : MonoBehaviour {

    public UIHeroNewManager heroManager;

    public GameObject support_info;
    public UITexture support_Icon;
    public UILabel support_name;
    public UILabel support_Des;
    public GameObject background;
    public UIPlayTween playtween;
    public UIPlayTween playTweenClose;

    public UILabel lblBtnUse;
    public UILabel lblBtnUnEquip;
    public UILabel lblBtnSell;
    public UIHeroUseItem useItem;
    private UserItem userItem;

    public UIButton btnUse;
    public UIButton btnUnEquip;

    void Start()
    {
        Localization();
    }

    public void Open()
    {
        playtween.Play(true);
    }

    public void SetItem(UserItem item)
    {       
        userItem = item;
        InitItem();
       
    }
    public void Close()
    {
        playTweenClose.Play(true);
    }
    public void Refresh()
    {
        if (userItem == null || userItem.Count <= 0)
        {
            Close();
            return;
        }
        InitItem();
    }


    #region Button
    public void OnButtonUse_Click()
    {        
        if (userItem == null) return;
        if (userItem.GameItem.SubKind == (int)ItemSubKind.HeroBook)
        {
            useItem.setItem(userItem);
        }
        else if (userItem.GameItem.Kind != (int)ItemKind.Material)
        {
            heroManager.OnEquipItem(userItem);
        }
    }
    public void OnButtonUnEquip_Click()
    {
        heroManager.OnUnEquipItem(userItem);
    }
    public void OnButtonSell_Click()
    {

        UINoticeManager.OnButtonOK_click += new UINoticeManager.NoticeHandle(OnAceptSellItem);
        MessageBox.ShowDialog(string.Format(GameManager.localization.GetText("HeroNew_ConfirmSell"), userItem.Name), UINoticeManager.NoticeType.YesNo);
    }
    #endregion

    #region Private methods
    private void InitItem()
    {        
        support_Icon.mainTexture = Helper.LoadTextureForSupportItem(userItem.ItemId);
        MyLocalization.ItemInfo itemInfo = GameManager.localization.getItem(userItem.ItemId);

        if (userItem.GameItem.Kind == (int)ItemKind.Support)
        {
            if (heroManager.heroSlotSelected != null)
            {
                if (heroManager.heroSlotSelected.userRole != null)
                {
                    UserRole curRole = heroManager.heroSlotSelected.userRole;
                    if (userItem.RoleUId != curRole.Id)
                    {
                        btnUnEquip.gameObject.SetActive(false);
                        btnUse.gameObject.SetActive(true);
                    }
                    else
                    {
                        btnUnEquip.gameObject.SetActive(true);
                        btnUse.gameObject.SetActive(false);
                    }
                }
            }
        }
        else
        {
            btnUnEquip.gameObject.SetActive(false);
            btnUse.gameObject.SetActive(true);
        }

        string EquipedForRole = "";
        if (GameScenes.currentSence == GameScenes.MyScene.Hero || GameScenes.currentSence == GameScenes.MyScene.ItemUpgrade)
        {
            if (userItem.RoleUId > 0)
            {
                UserRole role = GameManager.GameUser.UserRoles.FirstOrDefault(p => p.Id == userItem.RoleUId);
                if (role != null)
                    EquipedForRole = string.Format("[00FF00]({0})[-]", role.Name);
            }
        }
        string level = "";
        if (userItem.GameItem.Kind == (int)ItemKind.Support)
        {
            level = string.Format("({0}{1})", GameManager.localization.GetText("Global_Lvl"), Helper.GetLevelItem(userItem.GameItem.Level));
        }

        support_name.text = itemInfo.Name + level + "\n" + EquipedForRole;
        support_Des.text = itemInfo.Description;

        if (userItem.GameItem.SubKind == (int)ItemSubKind.Equipment ||
            userItem.GameItem.SubKind == (int)ItemSubKind.HeroUpgrade ||
            userItem.GameItem.SubKind == (int)ItemSubKind.ItemUpgrade)
        {
            lblBtnUse.transform.parent.gameObject.SetActive(false);
        }

        if (heroManager.heroSlotSelected == null)
        {
            btnUnEquip.gameObject.SetActive(false);
            btnUse.gameObject.SetActive(false);
        }
    }

    private void OnAceptSellItem()
    {
        heroManager.OnSellItem(userItem);
    }
    private void Localization()
    {
        lblBtnUse.text = GameManager.localization.GetText("HeroNew_ItemInfo_bntEquip");
        lblBtnSell.text = GameManager.localization.GetText("HeroNew_ItemInfo_bntSell");
        lblBtnUnEquip.text = GameManager.localization.GetText("HeroNew_ItemInfo_bntUnEquip");
    }
    #endregion
}
