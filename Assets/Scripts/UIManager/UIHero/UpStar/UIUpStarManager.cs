using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DEngine.Common.GameLogic;
using DEngine.Common.Config;
using System.Linq;

public class UIUpStarManager : MonoBehaviour {


    public UITexture quality;
    public UITexture icon;
    public UILabel lblName;
    public UILabel lblLevel;
    public UIHeroStarManager mainStarManager;
    public EffectUpStarManager effectManager;

    public UILabel lblSkill;
    public UITexture iconSkill1;
    public UITexture iconSkill2;
    public UIProgressBar expBar;
    public UILabel lblLevelRequire;

    public Dictionary<int,int> itemsRequire;
    public UITexture[] items;
    public UIStatsManager statsManager;
    public UserRole _curRoleUpgrade;
    public GameObject informationRoot;
    public UITexture information_icon;
    public UILabel information_name;
    public UILabel information_des;
    public GameObject userRoleRoot;
    public GameObject ChoseHeroPrefab;

    public GameObject arrowSelectHero;
    public UILabel lblArrowSelectHero;
    public GameObject arrowButton;
    public UIPlayTween tweenHideHero;
    public UILabel lblGold;
    public UILabel lblSilver;
    public UILabel lblGoldNeed;
    public UILabel lblSilverNeed;

    private int maxGrade = 5;
    private HeroUpStarController _controller;
    private bool _locked;
    private int _LevelRequired;
    private List<UIHeroSlotChoseForUpgrade> _choseHeroSlots;
    private UIHeroSlotChoseForUpgrade _heroSlotForUpgradeChose;
    private float _silverNeed;
    private float _goldNeed;

    void Start()
    {

        arrowSelectHero.SetActive(true);

        _choseHeroSlots = new List<UIHeroSlotChoseForUpgrade>();      
        _locked = false;
        Localization();
              
        _LevelRequired = 0;
        _controller = new HeroUpStarController(this);
        effectManager.manager = this;

        ShowGold();

        ShowAllUserRoles();

        if (GameManager.CurRoleSelectedUpGrade != null)
        {
            OnSelectedRoleForUpgrade(new UIHeroSlotChoseForUpgrade() { _userRole = GameManager.CurRoleSelectedUpGrade });
            GameManager.CurRoleSelectedUpGrade = null;
        }
    }

    #region Private methods
    private void ShowGold()
    {
        if (GameManager.GameUser != null)
        {
            lblGold.text = GameManager.GameUser.Base.Gold.ToString();
            lblSilver.text = GameManager.GameUser.Base.Silver.ToString();
        }
    }
    private void ShowAllUserRoles()
    {
        foreach (UserRole role in GameManager.GameUser.UserRoles)
        {
            GameObject go = NGUITools.AddChild(userRoleRoot, ChoseHeroPrefab);
            UIHeroSlotChoseForUpgrade slot = go.GetComponent<UIHeroSlotChoseForUpgrade>();
            slot.SetUser(role, this);
            _choseHeroSlots.Add(slot);
        }
        userRoleRoot.GetComponent<UIGrid>().Reposition();
    }
    private void GetItemsRequire()
    {
        RoleUpgrade upgradeData = (from item in RoleConfig.ROLE_UPGRADE
                                   where item.RoleId == _curRoleUpgrade.Base.RoleId && item.Grade == _curRoleUpgrade.Base.Grade
                                   select item).FirstOrDefault();

        if (upgradeData != null)
        {
            itemsRequire = upgradeData.Items;
            _LevelRequired = upgradeData.Level;

            string lvl;
            if (_LevelRequired > _curRoleUpgrade.Base.Level)
                lvl = string.Format("[FF0000]{0}[-]", _LevelRequired);
            else
                lvl = string.Format("[00FF00]{0}[-]", _LevelRequired);

            lblLevelRequire.text = string.Format(GameManager.localization.GetText("Hero_UpStar_LevelNeed"), lvl);

            _silverNeed = upgradeData.Silver;
            _goldNeed = upgradeData.Gold;

            lblGoldNeed.text = upgradeData.Gold.ToString();
            lblSilverNeed.text = upgradeData.Silver.ToString();
        }
    }
    private void ShowInfo()
    {
        UserRole next = new UserRole();
        next.GameRole = _curRoleUpgrade.GameRole;
        next.Base = _curRoleUpgrade.Base;
        next.Attrib = _curRoleUpgrade.Attrib;
        next.Base.Grade += 1;
        
        next.RoleItems.AddRange(_curRoleUpgrade.RoleItems);
       
        if (_curRoleUpgrade.Base.Grade >= maxGrade)
        {
            statsManager.SetRole(_curRoleUpgrade, _curRoleUpgrade);
            return;
        }

        next.InitAttrib();
        statsManager.SetRole(_curRoleUpgrade, next);
      
    }
    private void ShowCurRoleUpgrade()
    {
        lblName.text = _curRoleUpgrade.Name;
        lblLevel.text = GameManager.localization.GetText("Global_Lvl") + _curRoleUpgrade.Base.Level;
        mainStarManager.SetStart(_curRoleUpgrade.Base.Grade);
        quality.mainTexture = Helper.LoadTextureElement((int)_curRoleUpgrade.Base.ElemId);
        icon.mainTexture = Helper.LoadTextureForHero(_curRoleUpgrade.Base.RoleId);

        if (!iconSkill1.gameObject.activeInHierarchy)
            iconSkill1.gameObject.SetActive(true);
        if (!iconSkill2.gameObject.activeInHierarchy)
            iconSkill2.gameObject.SetActive(true);

        iconSkill1.mainTexture = Helper.LoadTextureForSkill(_curRoleUpgrade, 1);
        iconSkill2.mainTexture = Helper.LoadTextureForSkill(_curRoleUpgrade, 2);

    }
    private void Localization()
    {
        lblArrowSelectHero.text = GameManager.localization.GetText("Tut_HeroUpStar_SelectHeroForUp");
        lblSkill.text = GameManager.localization.GetText("Global_Skill");
    }
    private bool CheckUpgrade()
    {
        if (_curRoleUpgrade == null) return false;
        if (_curRoleUpgrade.Base.Grade >= maxGrade)
        {
            MessageBox.ShowDialog(GameManager.localization.GetText("ErrorCode_UpStart_MaxGrade"), UINoticeManager.NoticeType.Message);
            return false;
        }

        if (GameManager.GameUser.Base.Silver < _silverNeed || GameManager.GameUser.Base.Gold < _goldNeed)
        {
            Helper.HandleCashInsufficient();
            return false;
        }

        if (itemsRequire.Count <= 0) return false;
        if (_LevelRequired > _curRoleUpgrade.Base.Level)
        {

            MessageBox.ShowDialog(string.Format(GameManager.localization.GetText("ErrorCode_UpStart_NotEnoghtLevel"), _LevelRequired), UINoticeManager.NoticeType.Message);
            return false;
        }

        foreach (var item in itemsRequire)
        {
            int itemID = item.Key;

            UserItem userItem = GameManager.GameUser.UserItems.FirstOrDefault(p => p.ItemId == itemID);
            int itemCountInInventory = 0;
            if (userItem != null)
            {
                itemCountInInventory = Helper.SumItemCountInList(GameManager.GameUser.UserItems.Where(p => p.ItemId == itemID).ToArray());
            }

            if (itemCountInInventory < item.Value)
            {
                MessageBox.ShowDialog(GameManager.localization.GetText("ErrorCode_UpStart_NotEnoughtMaterial"), UINoticeManager.NoticeType.Message);
                return false;
            }
        }

        return true;
    }
    private void ShowItem()
    {

        int i = 0;
        foreach (var item in itemsRequire)
        {
            items[i].mainTexture = Helper.LoadTextureForSupportItem(item.Key);
            UserItem userItem = GameManager.GameUser.UserItems.FirstOrDefault(p => p.ItemId == item.Key);

            int itemCountInInventory = 0;
            if (userItem != null)
            {
                itemCountInInventory = Helper.SumItemCountInList(GameManager.GameUser.UserItems.Where(p => p.ItemId == item.Key).ToArray());
            }

            string amount = string.Format("{0}/{1}", itemCountInInventory, item.Value);
            if (itemCountInInventory < item.Value)
            {
                amount = "[FF0000]" + amount + "[-]";
            }
            else
            {
                amount = "[00FF00]" + amount + "[-]";
            }

            items[i].transform.GetChild(0).GetComponent<UILabel>().text = amount;
            i++;
        }

        for (int j = i; j < 5; j++)
        {
            items[j].mainTexture = null;
            items[j].transform.GetChild(0).GetComponent<UILabel>().text = "";
        }
    }
    private void ShowInforItem(int index)
    {
        information_icon.mainTexture = items[index].mainTexture;
        int[] itemList = (from item in itemsRequire select item.Key).ToArray();
        MyLocalization.ItemInfo info = GameManager.localization.getItem(itemList[index]);
        information_name.text = info.Name;
        information_des.text = info.Description;
    }
    private void ShowNextItem()
    {
        GetItemsRequire();
        ShowItem();
        effectManager.resourceCount = itemsRequire.Count;
    }
    #endregion

    #region Button
    public void OnButtonItem1_Click()
    {
        if (items.Length < 1) return;
        informationRoot.SetActive(true);
        ShowInforItem(0);
    }
    public void OnButtonItem2_Click()
    {
        if (itemsRequire.Count < 2) return;
        informationRoot.SetActive(true);
        ShowInforItem(1);
    }
    public void OnButtonItem3_Click()
    {
        if (itemsRequire.Count < 3) return;
        informationRoot.SetActive(true);
        ShowInforItem(2);
    }
    public void OnButtonItem4_Click()
    {
        if (itemsRequire.Count < 4) return;
        informationRoot.SetActive(true);
        ShowInforItem(3);
    }
    public void OnButtonItem5_Click()
    {
        if (itemsRequire.Count < 5) return;
        informationRoot.SetActive(true);
        ShowInforItem(4);
    }
    public void OnButtonCloseInformation_Click()
    {
        informationRoot.SetActive(false);
    }
    public void OnButtonClose_Click()
    {        
        GameScenes.ChangeScense(GameScenes.MyScene.HeroUpStar, GameScenes.previousSence);
    }
    public void OnButtonEvolve_Click()
    {
        if (_locked) return;
        if (!CheckUpgrade())
        {
            return;
        }
        _locked = true;
        int[] itemList = (from item in itemsRequire select item.Key).ToArray();
        _controller.SendRequestUpStar(_curRoleUpgrade, itemList);
    }
    #endregion

    #region public method
    public void OnServerResponse()
    {
        effectManager.OnButtonEvolve_Click();
        GameManager.GameUser.UpgradeRoleFromItems(_curRoleUpgrade);
    }
    public void OnFinishedEffect()
    {
        _locked = false;
        Debug.Log("OnFinishedEffect");
        ShowCurRoleUpgrade();
        ShowInfo();
        ShowNextItem();
    }
    public void OnSelectedRoleForUpgrade(UIHeroSlotChoseForUpgrade uiHeroChose)
    {

        if (_heroSlotForUpgradeChose != null)
            _heroSlotForUpgradeChose.OnDeSelected();
        _heroSlotForUpgradeChose = uiHeroChose;
        _curRoleUpgrade = _heroSlotForUpgradeChose._userRole;

        if (_curRoleUpgrade.Base.Grade >= maxGrade)
        {
            MessageBox.ShowDialog(GameManager.localization.GetText("ErrorCode_UpStart_MaxGrade"), UINoticeManager.NoticeType.Message);         
            statsManager.SetRole(_curRoleUpgrade, _curRoleUpgrade);
            return;
        }

        ShowCurRoleUpgrade();
        GetItemsRequire();

        effectManager.resourceCount = itemsRequire.Count;
        ShowInfo();
        ShowItem();

        arrowSelectHero.SetActive(false);
        arrowButton.SetActive(true);

        tweenHideHero.Play(true);

    }
    #endregion

  

}
