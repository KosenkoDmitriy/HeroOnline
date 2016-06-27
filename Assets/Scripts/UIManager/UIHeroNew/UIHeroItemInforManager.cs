using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;
using System.Linq;

public class UIHeroItemInforManager : MonoBehaviour
{

    [System.Serializable]
    public struct UIAtrribute
    {
        public UILabel lblValue;
        public UILabel lblGrade;

        public void Clear()
        {
            lblValue.text = "";
            lblGrade.text = "";
            lblValue.gameObject.SetActive(false);
        }

        public void AddText(string s)
        {
            if (!lblValue.gameObject.activeInHierarchy)
                lblValue.gameObject.SetActive(true);
            if (lblValue.text != "")
                lblValue.text += "\n";
            lblValue.text += s;
        }
    }

    public UIHeroNewManager heroNewManager;

    public UITexture uiIcon;
    public UILabel uiInformation;
    public UIAtrribute[] uiAttribute;
    public UILabel uiSellValue;
    public UITexture uiBorder;
    public UIPlayTween playTween;
    public UIPlayTween playTweenClose;

    public UIButton btnSell;
    public UIButton btnEquip;
    public UIButton btnUnEquip;
    
    private UserItem _userItem;
    #region Common methods

    void Awake()
    {
        Localization();
    }
    #endregion

    #region Public methods
    public void Open()
    {
        playTween.Play(true);
    }
    public void SetItem(UserItem item, UserRole role)
    {      
        _userItem = item;
        InitItem(role);

    }  
    public void OnButtonClose_Click()
    {
        //gameObject.SetActive(false);
    }
    public void Close()
    {
        playTweenClose.Play(true);
    }
    #endregion

    #region Public methods
    public void OnButtonSell_Click()
    {
        UINoticeManager.OnButtonOK_click += new UINoticeManager.NoticeHandle(OnAceptSellItem);
        MessageBox.ShowDialog(string.Format(GameManager.localization.GetText("HeroNew_ConfirmSell"), _userItem.Name), UINoticeManager.NoticeType.YesNo);
    }
    public void OnButtonEquip_Click()
    {
        heroNewManager.OnEquipItem(_userItem);
    }
    public void OnButtonUnEquip_Click()
    {
        heroNewManager.OnUnEquipItem(_userItem);
    }
    #endregion


    #region Private methods
    private void OnAceptSellItem()
    {
        heroNewManager.OnSellItem(_userItem);
    }
    private void InitItem(UserRole curRole)
    {

        if (_userItem == null) return;

        bool equiped = false;

        if (curRole != null)
        {
            if (_userItem.RoleUId != curRole.Id)
            {
                equiped = false;
            }
            else
            {
                equiped = true;
            }
            if (equiped)
            {
                btnUnEquip.gameObject.SetActive(true);
                btnEquip.gameObject.SetActive(false);
                btnSell.gameObject.SetActive(false);
            }
            else
            {
                btnUnEquip.gameObject.SetActive(false);
                btnEquip.gameObject.SetActive(true);
                btnSell.gameObject.SetActive(true);
            }
        }
        else
        {
            btnUnEquip.gameObject.SetActive(false);
            btnEquip.gameObject.SetActive(false);
            btnSell.gameObject.SetActive(false);
        }

       

        Color color = Helper.ItemColor[_userItem.Grade];


        uiBorder.color = color;

        if (_userItem.GameItem.SubKind != (int)ItemSubKind.Equipment)
            uiIcon.mainTexture = Helper.LoadTextureForSupportItem(_userItem.ItemId);
        else
            uiIcon.mainTexture = Helper.LoadTextureForEquipItem(_userItem.ItemId);


        string EquipedForRole = "";
        if (GameScenes.currentSence == GameScenes.MyScene.Hero || GameScenes.currentSence == GameScenes.MyScene.ItemUpgrade)
        {
            if (_userItem.RoleUId > 0)
            {
                UserRole role = GameManager.GameUser.UserRoles.FirstOrDefault(p => p.Id == _userItem.RoleUId);
                if (role != null)
                    EquipedForRole = string.Format("[00FF00]({0})[-]", role.Name);
            }
        }

        string rank = "";
        uiInformation.text = string.Format("[{0}]{1}[-]{2} {3}\n" +
            GameManager.localization.GetText("Global_Level") + "{4} (" + GameManager.localization.GetText("Global_LevelRoleNeed") + " {5})\n" +
            GameManager.localization.GetText("Global_Type") + " {6}", Helper.ColorToHex(color), _userItem.Name, rank, EquipedForRole, _userItem.GameItem.Level,
            _userItem.MinRoleLevel, (ItemKind)_userItem.GameItem.Kind);



        foreach (UIAtrribute att in uiAttribute)
        {
            att.Clear();
        }

        for (int i = 0; i < uiAttribute.Length; i++)
        {
            if (_userItem.Ranks[i + 1] > 0)
                uiAttribute[i].lblGrade.text = string.Format("+{0}", _userItem.Ranks[i + 1].ToString());
            else
                uiAttribute[i].lblGrade.text = "";
        }

        foreach (ItemAttrib att in _userItem.Attribs)
        {
            if (att.Attrib != AttribType.None)
            {
                string s = FormatLine(att);
                uiAttribute[(int)att.Grade - 1].AddText(s);
            }

        }
           
        uiSellValue.text = string.Format("{0:0}", _userItem.GameItem.SellPrice);
    }
    private void Localization()
    {
        btnSell.transform.FindChild("Label").GetComponent<UILabel>().text = GameManager.localization.GetText("HeroNew_ItemInfo_bntSell");
        btnEquip.transform.FindChild("Label").GetComponent<UILabel>().text = GameManager.localization.GetText("HeroNew_ItemInfo_bntEquip");
        btnUnEquip.transform.FindChild("Label").GetComponent<UILabel>().text = GameManager.localization.GetText("HeroNew_ItemInfo_bntUnEquip");
    }

    private string FormatLine(ItemAttrib att)
    {
        string lineColor = Helper.ColorToHex(Helper.ItemColor[att.Grade]);
        string result = "";
        string attname = "";

        attname = Helper.FormatAtributeText(att.Attrib);
        attname = "[" + lineColor + "]" + attname + "[-]";
        result = string.Format(attname, att.Value);

        return result;
    }
    #endregion


    internal void Refresh(UserRole role)
    {
        if (_userItem == null || _userItem.Count <= 0)
        {
            Close();
            return;
        }
        InitItem(role);
    }
}
