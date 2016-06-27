using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DEngine.Common.GameLogic;
using System.Linq;
using DEngine.Common.Config;

public class UIItemUpgradeManager : MonoBehaviour {

    public enum UpgradeType
    {
        Equipment,
        Material
    }

    public enum PriceType
    {
        Gold,
        Silver
    }


    public UILabel lblUp;
    public EffectUpgradeItem effect;

    public UIGrid slotRoot;
    public GameObject slotPrefab;
    public UIScrollView scrollView;
    public UIItemUpgradeResource[] itemResource;
    public UIItemUpgradeResource itemResult;
    public GameObject uiResult;
    public UpgradeType upgradeType;
    public UILabel lblItemInfo;
    public UIHeroItemInforManager itemInfo;
    public UILabel lblGold;
    public UILabel lblSilver;


    public GameObject arrowMaterial;
    public GameObject arrowItem;
    public UILabel lblArrowMaterial;
    public UILabel lblArrowItem;

    public UILabel lblUpgradeType;
    public UIPopupList cboUpgradeType;

    public UILabel lblUseGold;
    public UILabel lblPriceNeedValue;
    public UISprite spritePriceNeedType;
    public UIToggle chkUseGold;
    public UILabel lblSilverForUpgrade;

    private List<UIUpgradeItemSlot> _slots;
    private UserItem _userItem;
    private Dictionary<int,int> _materialsNeed;
    private bool _locked;
    private int _goldForUpgrade;
    private ItemUpgradeController _controller;
    private ItemGrade _upgradeRow;
    private int _silverForUpgrade;

    void Start()
    {
        _locked = true;
        _slots = new List<UIUpgradeItemSlot>();
        _controller = new ItemUpgradeController(this);       
        Localization();
        InitSlot();
        ShowGold();

        if (GameManager.Status == GameStatus.Lab)
        {
            //StartCoroutine(FilerInventory(ItemSubKind.ItemUpgrade));
            arrowMaterial.SetActive(true);
            arrowItem.SetActive(false);
            lblArrowMaterial.text = GameManager.localization.GetText("Tut_ItemUpgrade_SelectMaterialUpgrade");
            lblUpgradeType.gameObject.SetActive(false);
            StartCoroutine(FilerInventory(ItemSubKind.ItemUpgrade));
        }
        else
        {
            //StartCoroutine(FilerInventory(ItemSubKind.Equipment));
            arrowMaterial.SetActive(false);
            arrowItem.SetActive(true);
            lblArrowItem.text = GameManager.localization.GetText("Tut_ItemUpgrade_SelectItem");
            lblArrowMaterial.text = GameManager.localization.GetText("Tut_ItemUpgrade_SelectItemUpgrade");
        }
    }


    #region private Methods
    private void ShowGold()
    {
        if (GameManager.GameUser != null)
        {
            lblGold.text = GameManager.GameUser.Base.Gold.ToString();
            lblSilver.text = GameManager.GameUser.Base.Silver.ToString();
        }
    }
    private void InitSlot()
    {
        List<UserItem> userItems = GameManager.GameUser.UserItems.Where(p => p.GameItem.SubKind == (int)ItemSubKind.ItemUpgrade || p.GameItem.SubKind == (int)ItemSubKind.Equipment).ToList();
            // .OrderByDescending(p => p.GameItem.Kind).ThenByDescending(p => p.Grade).ToList();

        foreach (UserItem userItem in userItems)
        {
            GameObject go = NGUITools.AddChild(slotRoot.gameObject, slotPrefab);
            UIUpgradeItemSlot slot = go.GetComponent<UIUpgradeItemSlot>();
            slot.SetItem(userItem);
            slot.manager = this;           
            _slots.Add(slot);
            slotRoot.Reposition();
        }
        scrollView.ResetPosition();
    }
    private void ClearSlot()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            NGUITools.Destroy(_slots[i].gameObject);            
        }
        _slots.Clear();
    }
    private void ShowInfoItemEquip()
    {
        Color color = Helper.ItemColor[_userItem.Grade];


        string EquipedForRole = "";

        if (GameScenes.currentSence == GameScenes.MyScene.Hero || GameScenes.currentSence == GameScenes.MyScene.ItemUpgrade)
        {
            if (_userItem.RoleUId > 0)
            {
                UserRole role = GameManager.GameUser.UserRoles.FirstOrDefault(p => p.Id == _userItem.RoleUId);
                EquipedForRole = string.Format("[FFFF00]({0})[-]", role.Name);
            }
        }

        string rank = "";
        lblItemInfo.text = string.Format("[{0}]{1}[-]{2} {3}\n" +
            GameManager.localization.GetText("Global_Level") + "{4}\n" +
            GameManager.localization.GetText("Global_Type") + " {5}\n", Helper.ColorToHex(color), _userItem.Name, rank, EquipedForRole,
            Helper.GetLevelItem(_userItem.GameItem.Level), (ItemKind)_userItem.GameItem.Kind);

        UserItem userItemNext = _userItem.GetNextRankItem((int)_upgradeRow );

        for (int i = 0; i < _userItem.Attribs.Count; i++)
        {
            ItemAttrib curAtt = _userItem.Attribs[i];
            ItemAttrib nextAtt = userItemNext.Attribs[i];
            if (curAtt.Attrib != AttribType.None && curAtt.Grade == (int)_upgradeRow )
            {
                string s = FormatLine(curAtt);
                lblItemInfo.text += string.Format("\n{0}(+{1:0.0})", s, nextAtt.Value - curAtt.Value);
            }
        }
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

    private void ShowInfoResource(int id)
    {
        GameItem gameItem = (GameItem)GameManager.GameItems[id];
        MyLocalization.ItemInfo itemInfo = GameManager.localization.getItem(id);
      
        lblItemInfo.text = string.Format("[{0}]{1}[-]\n" +
        GameManager.localization.GetText("Global_Level") + "{2}\n" +
        GameManager.localization.GetText("Global_Type") + " {3}\n[i]{4}[/i]", Helper.ColorToHex(Color.white), gameItem.Name,
        Helper.GetLevelItem(gameItem.Level), (ItemKind)gameItem.Kind, itemInfo.Description);

    
    }
    private void Localization()
    {
        lblUp.text = GameManager.localization.GetText("Hero_Inventory_btnUpgradeITem");
        lblUpgradeType.text = GameManager.localization.GetText("ItemUpgrade_UpgradeType");
        cboUpgradeType.AddItem(GameManager.localization.GetText("ItemUpgrade_White"));
        cboUpgradeType.AddItem(GameManager.localization.GetText("ItemUpgrade_Green"));
        cboUpgradeType.AddItem(GameManager.localization.GetText("ItemUpgrade_Blue"));
        cboUpgradeType.AddItem(GameManager.localization.GetText("ItemUpgrade_Yellow"));
        cboUpgradeType.value = cboUpgradeType.items[0];
    }

    private bool CheckUpgrade()
    {
        if (_materialsNeed == null) return false;

        foreach (var r in _materialsNeed)
        {
            int amount = Helper.SumItemCountInList(GameManager.GameUser.UserItems.Where(p => p.ItemId == r.Key).ToArray());
            if (amount < r.Value)
            {
                MessageBox.ShowDialog(GameManager.localization.GetText("ErrorCode_ItemUpgradeFail"), UINoticeManager.NoticeType.Message);
                return false;
            }
        }


        return true;
    }

    private void ShowInfoUpgradeItem()
    {
        UserItem item = _userItem;
              
        _materialsNeed = new Dictionary<int, int>();

        int curentRank = item.Ranks[(int)_upgradeRow];

        RankUpgrade[] rankUpgrades = ItemConfig.WHITE_UPGRADES;

        if (_upgradeRow == ItemGrade.Green)
        {
            rankUpgrades = ItemConfig.GREEN_UPGRADES;
        }
        else if (_upgradeRow == ItemGrade.Blue)
        {
            rankUpgrades = ItemConfig.BLUE_UPGRADES;
        }
        else if (_upgradeRow == ItemGrade.Yellow)
        {
            rankUpgrades = ItemConfig.YELLOW_UPGRADES;
        }

        RankUpgrade rankUpgrade = rankUpgrades[item.GameItem.Level];

        if (curentRank < rankUpgrade.Materials.Length)
        {
            RankUpgrade.ItemMat mat = rankUpgrade.Materials[curentRank];
            _materialsNeed[mat.ItemId] = mat.Count;

            int amount = Helper.SumItemCountInList(GameManager.GameUser.UserItems.Where(p => p.ItemId == mat.ItemId).ToArray());
            for (int i = 0; i < 3; i++)
            {
                if (i < amount)
                    itemResource[i].SetResourceNeed(mat.ItemId, 1);
                else
                    itemResource[i].SetResourceNeed(mat.ItemId, -1);
            }

            _silverForUpgrade = rankUpgrade.RequiredSilver[curentRank];
            lblSilverForUpgrade.text = _silverForUpgrade.ToString();

            int gold = rankUpgrade.RequiredGold[curentRank];
            if (gold > 0)
            {
                lblUseGold.text = GameManager.localization.GetText("ItemUpgrade_UseGold");
                lblPriceNeedValue.text = rankUpgrade.RequiredGold[curentRank].ToString();
                chkUseGold.value = false;
                lblUseGold.gameObject.SetActive(true);
                _goldForUpgrade = gold;

            }
            else
            {
                chkUseGold.value = false;
                lblUseGold.gameObject.SetActive(false);
                _goldForUpgrade = 0;
            }

            _locked = false;
        }
        else
        {
            _locked = true;
        }
    }
    private void UpgradeMaterial()
    {
        UserItem item = _userItem;

        if (item.GameItem.Level + 1 > 10)
        {
            _locked = true;
            for (int i = 0; i < 3; i++)
            {
                itemResource[i].Clear();
            }
        }
        else
        {
            upgradeType = UpgradeType.Material;
            int amount = Helper.SumItemCountInList(GameManager.GameUser.UserItems.Where(p => p.ItemId == item.ItemId).ToArray());

            _silverForUpgrade = 0;
            lblSilverForUpgrade.text = _silverForUpgrade.ToString("0");

            _materialsNeed = new Dictionary<int, int>();
            _materialsNeed[item.ItemId] = 3;
            for (int i = 0; i < 3; i++)
            {
                if (i < amount)
                    itemResource[i].SetResourceNeed(item.ItemId, 1);
                else
                    itemResource[i].SetResourceNeed(item.ItemId, -1);
            }

            _locked = false;
        }
    }
    private void ShowComboboxUpgrade()
    {
        cboUpgradeType.Clear();
        if (_userItem.Grade > 0)
            cboUpgradeType.AddItem(GameManager.localization.GetText("ItemUpgrade_White"));
        if (_userItem.Grade > 1)
            cboUpgradeType.AddItem(GameManager.localization.GetText("ItemUpgrade_Green"));
        if (_userItem.Grade > 2)
            cboUpgradeType.AddItem(GameManager.localization.GetText("ItemUpgrade_Blue"));
        if (_userItem.Grade > 3)
            cboUpgradeType.AddItem(GameManager.localization.GetText("ItemUpgrade_Yellow"));
        cboUpgradeType.value = cboUpgradeType.items[0];
    }
    #endregion
    
    #region public methods
    public IEnumerator FilerInventory(ItemSubKind kind)
    {
        foreach (UIUpgradeItemSlot slot in _slots)
        {
            if (slot.item.GameItem.SubKind == (int)kind)
                slot.gameObject.SetActive(true);
            else
                slot.gameObject.SetActive(false);
        }
        slotRoot.Reposition();
        scrollView.ResetPosition();
        yield return 0;
    }
    public void OnButtonUp_Click()
    {
        if (_userItem == null) return;
        if (_locked) return;
        if (!CheckUpgrade()) return;

        if (chkUseGold.value)
        {
            if (_goldForUpgrade > GameManager.GameUser.Base.Gold)
            {
                Helper.HandleCashInsufficient();
                return;
            }
        }

        if (_silverForUpgrade > 0)
        {
            if (_silverForUpgrade > GameManager.GameUser.Base.Silver)
            {
                Helper.HandleCashInsufficient();
                return;
            }
        }

        if (_userItem.GameItem.SubKind != (int)ItemSubKind.Equipment)
        {
            itemResult.Clear();
        }

        _controller.SendUpgradeItem(_userItem, (int)_upgradeRow , chkUseGold.value);
    }
    public void OnFinishedEffect()
    {
        Debug.Log("OnFinishedEffect");

        ClearSlot();
        InitSlot();

        if (chkUseGold.value)
        {
            GameManager.GameUser.Base.Gold -= _goldForUpgrade;
        }
        if (_silverForUpgrade > 0)
        {
            GameManager.GameUser.Base.Silver -= _silverForUpgrade;
        }
        if (GameManager.GameUser != null)
        {
            lblGold.text = GameManager.GameUser.Base.Gold.ToString();
            lblSilver.text = GameManager.GameUser.Base.Silver.ToString();
        }

        if (_userItem.GameItem.SubKind == (int)ItemSubKind.Equipment)
        {
            _userItem = GameManager.GameUser.UserItems.Where(p => p.Id == _userItem.Id).FirstOrDefault();
            ShowInfoItemEquip();
        }
        else
        {
            itemResult.SetResourceNeed(_userItem.ItemId + 1, 1);
            ShowInfoResource(_userItem.ItemId + 1);
        }

        OnSelectedItem(_userItem);
        ShowGold();
    }
    public void OnClose_Click()
    {
        GameScenes.ChangeScense(GameScenes.MyScene.ItemUpgrade, GameScenes.previousSence);
    }
    public void OnSelectedItem(UserItem item)
    {
        cboUpgradeType.value = cboUpgradeType.items[0];
        _userItem = item;
        arrowMaterial.SetActive(false);
        arrowItem.SetActive(false);
        if (item.GameItem.SubKind == (int)ItemSubKind.Equipment)
        {
            upgradeType = UpgradeType.Equipment;
            itemResult.OnSetItem(item);            
            ShowComboboxUpgrade();
            OnSelectedUpgradeType();
            StartCoroutine(FilerInventory(ItemSubKind.ItemUpgrade));
        }
        else
        {            
            UpgradeMaterial();            
        }
    }
    public void ShowItemInfo(UserItem item)
    {
        itemInfo.Open();
        itemInfo.SetItem(item, null);
    }
    public void OnResponseFromServer()
    {
        effect.OnButtonEvolve_Click();
    }
    public void OnSelectedUpgradeType()
    {
        if (_userItem == null) return;
        _upgradeRow = (ItemGrade)(cboUpgradeType.items.IndexOf(cboUpgradeType.value) + 1);
        ShowInfoUpgradeItem();
        ShowInfoItemEquip();
    }
    #endregion


}
