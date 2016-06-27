using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DEngine.Common.GameLogic;
using DEngine.Common.Config;
using DEngine.Common;

public class UIHeroNewManager : MonoBehaviour
{

    #region Struct
    [System.Serializable]
    public struct UILabelList
    {
        public UILabel lblHeroCount;
        public UILabel lblEquip_Ring;
        public UILabel lblEquip_Armor;
        public UILabel lblEquip_Sup;
        public UILabel lblEquip_Medal;
        public UILabel lblEXP;
        public UILabel lblEnergy;
        public UILabel lblInBattle;
        public UILabel lblViewHeroInfo;
        public UILabel lblViewSkillInfo;
        public UILabel lblBtnUpStar;
        public UILabel lblBtnUpLevel;
        public UILabel lblBtnUpExp;
        public UILabel lblBtnStrategy;
        public UILabel lblInventory;
    }
    [System.Serializable]
    public struct UIHeroList
    {
        public UIGrid root;
        public GameObject prefab;
    }
    [System.Serializable]
    public struct UIHero
    {
        public UITexture quality;
        public UITexture icon;
        public UISprite element;
        public UISprite roleClass;
        public UIHeroStarManager starManager;
        public UILabel lblName;
        public UILabel lblLevel;
        public UIProgressBar exp;
        public UIProgressBar energy;
        public UILabel expValue;
        public UILabel energyValue;
        public UIToggle inBattle;
        public UIButton ring;
        public UIButton armor;
        public UIButton support;
        public UIButton medal;
        public UIButton skillDef;
        public UIButton skill1;
        public UIButton skill2;
        public UILabel biography;
    }
    [System.Serializable]
    public struct UITab
    {
        public UIToggle ring;
        public UIToggle armor;
        public UIToggle medal;
        public UIToggle support;
        public UIToggle All;
    }
    [System.Serializable]
    public struct UITutorial
    {
        public GameObject arrowHero1;
        public GameObject arrowHero2;
        public GameObject arrowHero3;
        public GameObject arrowInBattle;
        public GameObject arrowExit;

        public GameObject arrowSlot;
        public GameObject arrowItem;
        public GameObject arrowButtonEquip;
    }
    [System.Serializable]
    public struct UIHireHero
    {
        public UILabel lblHeroForHide;
        public UILabel lblAcceptToHire;
        public UILabel lblButtonSave;
        public UIInput txtGold;
        public UIInput txtSilver;
        public UIToggle togAcceptToHire;
    }
    #endregion

    public enum InvetoryTab
    {
        None,
        Ring,
        Armor,
        Medal,
        Consume,
        Support,
        EXP,
        All
    }

    #region public variable
    public UILabelList uiLabelList;
    public UIHeroList uiHeroList;
    public UIHero uiHero;
    public UITab uiTab;
    public UISkillInformationManager skillManager;
    public UIHeroInventoryManager inventory;
    public UIHeroConsumeItemInfo consumeInfo;
    public UIHeroItemInforManager infoItemSelected;
    public UIHeroItemInforManager infoItemEuqiped;
    public UIStrategyManager strategy;
    public UIHeroNewDetail heroDetail;
    public InvetoryTab InventoryActiveTab { get { return _inventoryTab; } }
    public UIHeroNewHeroSlot heroSlotSelected { get { return _heroSlotSelected; } }
    public UITutorial uiTutorial;
    public UIHireHero uiHireHero;

    #endregion

    #region private variable
    private float _timerClickSkill;
    private List<UIHeroNewHeroSlot> _heroSlots;
    private UIHeroNewHeroSlot _heroSlotSelected;
    public InvetoryTab _inventoryTab;
    private HeroNewController _controller;
    #endregion

    #region Common methods
    void Awake()
    {     
        _inventoryTab = InvetoryTab.All;
        uiTab.All.value = true;
        Localization();
        
        _controller = new HeroNewController(this);

        _controller.SendRequestRoleList();

    }
    void Start()
    {
        Tutorial();
        _timerClickSkill = 0;
    }
    void Update()
    {
        UpdateArrow();
    }
    #endregion

    #region Button
    public void OnButtonSkillDef_Click()
    {
        if (Time.time - _timerClickSkill <= 0.9f) return;
        _timerClickSkill = Time.time;
        skillManager.SetSkill(_heroSlotSelected.userRole, 0, null);
        StartCoroutine(AnimationTweenSkillDetail(0));
    }
    public void OnButtonSkill1_Click()
    {
        if (Time.time - _timerClickSkill <= 0.9f) return;
        _timerClickSkill = Time.time;
        skillManager.SetSkill(_heroSlotSelected.userRole, 1, null);
        StartCoroutine(AnimationTweenSkillDetail(110));
    }
    public void OnButtonSkill2_Click()
    {
        if (Time.time - _timerClickSkill <= 0.9f) return;
        _timerClickSkill = Time.time;
        skillManager.SetSkill(_heroSlotSelected.userRole, 2, null);
        StartCoroutine(AnimationTweenSkillDetail(220));
    }
    public void OnButtonHeroAvatar_Click()
    {
        if (_heroSlotSelected == null) return;
        heroDetail.SetHero(_heroSlotSelected.userRole);
    }
    public void OnButtonExit_Click()
    {

        if (GameManager.tutorial.step < TutorialManager.TutorialStep.Finished)
        {
            if (GameManager.GameUser.UserRoles.Count(p=>p.Base.Status == RoleStatus.Active) < 3)
            {
                // MessageBox.ShowDialog(GameManager.localization.GetText("ErrorCode_SummonTutorial_Not3Hero"), UINoticeManager.NoticeType.Message);
                GameManager.tutorial.CreateNPC(UINPCTutorialManager.Status.Normal, GameManager.localization.GetText("ErrorCode_Hero_TeamNotFull"));
                return;
            }
        }

        if (GameManager.tutorial.step == TutorialManager.TutorialStep.Equip_Arrow)
        {
            GameManager.tutorial.CreateNPC(UINPCTutorialManager.Status.Normal, GameManager.localization.GetText("ErroCode_Hero_NotEquip"));
            return;
        }

        GameScenes.ChangeScense(GameScenes.MyScene.Hero, GameScenes.MyScene.WorldMap);
    }
    public void OnButtonUpStar_Click()
    {
        if (GameManager.tutorial.step < TutorialManager.TutorialStep.Finished) return;

        if (_heroSlotSelected == null) return;
        GameManager.CurRoleSelectedUpGrade = _heroSlotSelected.userRole;
        GameScenes.ChangeScense(GameScenes.MyScene.Hero, GameScenes.MyScene.HeroUpStar);
    }
    public void OnButtonUpLevel_Click()
    {
        if (GameManager.tutorial.step < TutorialManager.TutorialStep.Finished) return;

        if (_heroSlotSelected == null) return;
        GameManager.CurRoleSelectedUpGrade = _heroSlotSelected.userRole;
        GameScenes.ChangeScense(GameScenes.MyScene.Hero, GameScenes.MyScene.HeroUpgrade);
    }
    public void OnButtonStrategy_Click()
    {

        if (_heroSlotSelected == null) return;
        strategy.SetRole(_heroSlotSelected.userRole, this);
    }
    public void OnToogleInBattle_Click()
    {
        if (_heroSlotSelected == null) return;
        UserRole role = _heroSlotSelected.userRole;
        if (uiHero.inBattle.value)
        {
            if (role.Base.Energy < RoleConfig.ENERGY_MIN)
            {
                MessageBox.ShowDialog(GameManager.localization.GetText("Dialog_NotEnoughtEnergy"), UINoticeManager.NoticeType.Message);
                uiHero.inBattle.value = false;
                return;
            }

            int activeHeroCount = GameManager.GameUser.UserRoles.Count(p => p.Base.Status == RoleStatus.Active);

            if (activeHeroCount >= 3)
            {
                MessageBox.ShowDialog(GameManager.localization.GetText("Dialog_FullHeroEquip"), UINoticeManager.NoticeType.Message);
                uiHero.inBattle.value = false;
                return;
            }

            role.Base.Status = RoleStatus.Active;            
        }
        else
        {           
            role.Base.Status = RoleStatus.Default;
            uiHero.inBattle.value = false;
        }

        GameManager.GameUser.UserRoles.FirstOrDefault(p => p.Id == role.Id).Base.Status = role.Base.Status;
        _controller.SendRequestUpdateRole(role);
    }
    public void OnButtonUpEXP_Click()
    {
        if (_inventoryTab == InvetoryTab.EXP) return;

        OnChangeTab(InvetoryTab.Consume);
        inventory.Filer(ItemKind.Consume);
    }
    public void OnEquipRing_Click()
    {
        OnChangeTab(InvetoryTab.Ring);
        inventory.Filer(DEngine.Common.GameLogic.ItemKind.Ring);

        if (_heroSlotSelected != null)
        {
            UserItem item = _heroSlotSelected.userRole.RoleItems.FirstOrDefault(p => p.RoleUId == _heroSlotSelected.userRole.Id && p.GameItem.Kind == (int)ItemKind.Ring);
            if (item != null)
            {
                infoItemEuqiped.SetItem(item, _heroSlotSelected.userRole);
                infoItemEuqiped.Open();
            }
        }
    }
    public void OnEquipArmor_Click()
    {
        OnChangeTab(InvetoryTab.Armor);
        inventory.Filer(DEngine.Common.GameLogic.ItemKind.Armor);

        if (_heroSlotSelected != null)
        {
            UserItem item = _heroSlotSelected.userRole.RoleItems.FirstOrDefault(p => p.RoleUId == _heroSlotSelected.userRole.Id && p.GameItem.Kind == (int)ItemKind.Armor);
            if (item != null)
            {
                infoItemEuqiped.SetItem(item, _heroSlotSelected.userRole);
                infoItemEuqiped.Open();
            }
        }
        OnTabEquipClick();
    }
    public void OnEquipSupport_Click()
    {
        OnChangeTab(InvetoryTab.Support);
        inventory.Filer(ItemKind.Support);

        if (_heroSlotSelected != null)
        {
            UserItem item = _heroSlotSelected.userRole.RoleItems.FirstOrDefault(p => p.RoleUId == _heroSlotSelected.userRole.Id && p.GameItem.Kind == (int)ItemKind.Support);
            if (item != null)
            {
                consumeInfo.SetItem(item);
                consumeInfo.Open();
            }
        }

    }
    public void OnEquipMedal_Click()
    {
        OnChangeTab(InvetoryTab.Medal);
        inventory.Filer(DEngine.Common.GameLogic.ItemKind.Medal);

        if (_heroSlotSelected != null)
        {
            UserItem item = _heroSlotSelected.userRole.RoleItems.FirstOrDefault(p => p.RoleUId == _heroSlotSelected.userRole.Id && p.GameItem.Kind == (int)ItemKind.Medal);
            if (item != null)
            {
                infoItemEuqiped.SetItem(item, _heroSlotSelected.userRole);
                infoItemEuqiped.Open();
            }
        }
    }
    public void OnTabRing_Click()
    {
        if (uiTab.ring.value == false) return;
        if (_inventoryTab == InvetoryTab.Ring) return;

        OnChangeTab(InvetoryTab.Ring);
        inventory.Filer(ItemKind.Ring);
    }
    public void OnTabArmor_Click()
    {
        if (uiTab.armor.value == false) return;
        if (_inventoryTab == InvetoryTab.Armor) return;

        OnChangeTab(InvetoryTab.Armor);
        inventory.Filer(ItemKind.Armor);
    }
    public void OnTabMedal_Click()
    {
        if (uiTab.medal.value == false) return;
        if (_inventoryTab == InvetoryTab.Medal) return;

        OnChangeTab(InvetoryTab.Medal);
        inventory.Filer(ItemKind.Medal);
    }
    public void OnTabSupport_Click()
    {
        if (uiTab.support.value == false) return;
        if (_inventoryTab == InvetoryTab.Support) return;

        OnChangeTab(InvetoryTab.Support);
        inventory.Filer(ItemKind.Support);
    }
    public void OnTabAll_Click()
    {

        if (uiTab.All.value == false) return;
        if (_inventoryTab == InvetoryTab.All) return;

        OnChangeTab(InvetoryTab.All);
        inventory.Filer(ItemKind.None);
    }
    public void OnButtonSaveHire_Click()
    {
        if (_heroSlotSelected != null)
        {
            bool AcceptHire = uiHireHero.togAcceptToHire.value;
            int gold = 0;
            int silver = 0;
            int.TryParse(uiHireHero.txtGold.value, out gold);
            int.TryParse(uiHireHero.txtSilver.value, out silver);

            //if (AcceptHire && gold == 0 && silver == 0)
            //{
            //    MessageBox.ShowDialog(GameManager.localization.GetText("ErroCode_Hero_NotSetPriceForHire"), UINoticeManager.NoticeType.Message);
            //    return;
            //}
            _controller.SendRequestSetHire(_heroSlotSelected.userRole.Id, AcceptHire ? 1 : 0, Mathf.Abs(gold), Mathf.Abs(silver));
        }
    }
    #endregion

    #region Public methods
    public void RefreshCurHeroUI()
    {
        ShowHeroInfo();
        RefreshHeroList();
    }
    public UserRole heroSelected()
    {
        if (_heroSlotSelected == null)
            return null;
        else
            return _heroSlotSelected.userRole;
    }
    public void OnSelectedHeroSlot(UIHeroNewHeroSlot slot)
    {
        if (_heroSlotSelected != null)
            _heroSlotSelected.DeSelected();
        _heroSlotSelected = slot;

        OnHeroSelected();

        ShowHeroInfo();

        if (heroDetail.gameObject.activeInHierarchy)
            heroDetail.SetHero(_heroSlotSelected.userRole);

        if (strategy.gameObject.activeInHierarchy)
            strategy.SetRole(_heroSlotSelected.userRole, this);

        consumeInfo.Close();
        infoItemSelected.Close();
        infoItemEuqiped.Close();
    }
    public void OpenInventory()
    {
        inventory.uiInventory.background.SetActive(true);
        inventory.OpenInventory();

        UIPlayTween playTween = new UIPlayTween();
        playTween.tweenTarget = inventory.uiInventory.background;
        playTween.playDirection = AnimationOrTween.Direction.Forward;
        playTween.ifDisabledOnPlay = AnimationOrTween.EnableCondition.EnableThenPlay;
        playTween.disableWhenFinished = AnimationOrTween.DisableCondition.DisableAfterReverse;      
        playTween.Play(true);
    }
    public void OpenItemDetail(UserItem item)
    {
        if (item.GameItem.SubKind == (int)ItemSubKind.Equipment)
        {
            infoItemSelected.Open();
            if (item.RoleUId != _heroSlotSelected.userRole.Id)
            {
                infoItemSelected.SetItem(item, _heroSlotSelected.userRole);

                UserItem userItem = _heroSlotSelected.userRole.RoleItems.FirstOrDefault(p => p.GameItem.Kind == item.GameItem.Kind);
                if (userItem != null)
                {
                    infoItemEuqiped.Open();
                    infoItemEuqiped.SetItem(userItem, _heroSlotSelected.userRole);
                }
            }
            else
            {
                infoItemSelected.SetItem(item, _heroSlotSelected.userRole);
            }
        }
        else
        {
            consumeInfo.Open();
            consumeInfo.SetItem(item);
        }
    }
    public void OnSaveStrategy(UserRole role)
    {
        _controller.SendRequestUpdateRole(role);
    }
    public void OnSellItem(UserItem item)
    {
        if (GameManager.tutorial.step == TutorialManager.TutorialStep.Equip_Arrow)
        {
            return;
        }
        _controller.SendRequestSellItem(item.Id);
    }
    public void OnEquipItem(UserItem item)
    {
        if (_heroSlotSelected == null) return;

        UserRole userRole = _heroSlotSelected.userRole;
        UserItem itemEquip = item;

        if (userRole.Id == itemEquip.RoleUId) return;// đang trang bị

        if (itemEquip.RoleUId > 0)// đang trang bị cho role khác
        {
            GameManager.GameUser.SetItemForRole(null, itemEquip);
            _controller.SendRequestEquipItem(0, itemEquip.Id);
        }

        ////Đang trang bi cùng loại////
        UserItem oldItemEquiped = GameManager.GameUser.UserItems.FirstOrDefault(p => p.RoleUId == userRole.Id && p.GameItem.Kind == itemEquip.GameItem.Kind);
        if (oldItemEquiped != null)
        {
            GameManager.GameUser.SetItemForRole(null, oldItemEquiped);
        }
        ///////////////////////////////


        ErrorCode errorCode = GameManager.GameUser.SetItemForRole(userRole, itemEquip);
        if (errorCode != ErrorCode.Success)
        {
            if (errorCode == ErrorCode.RoleLevelNotEnough)
                MessageBox.ShowDialog(GameManager.localization.GetText("HeroNew_RoleLevelNotEnough"), UINoticeManager.NoticeType.Message);
            else
                MessageBox.ShowDialog(GameManager.localization.GetText("Equip_CantEuqip ") + errorCode, UINoticeManager.NoticeType.Message);

            return;
        }

        UserItem userItem = GameManager.GameUser.UserItems.FirstOrDefault(p => p.Id == itemEquip.Id);
        if (userItem != null)
        {
            userItem.RoleUId = userRole.Id;
        }
        OnEquipItem();
        _controller.SendRequestEquipItem(userRole.Id, itemEquip.Id);
    }
    public void OnUseItem(UserItem item, int amount)
    {
        if (_heroSlotSelected == null) return;

        UserRole userRole = _heroSlotSelected.userRole;
        UserItem itemEquip = item;

        _controller.SendRequestEquipItem(userRole.Id, itemEquip.Id, amount);
    }
    public void OnUnEquipItem(UserItem item)
    {
        GameManager.GameUser.SetItemForRole(null, item);
        _controller.SendRequestEquipItem(0, item.Id);

        UserItem userItem = GameManager.GameUser.UserItems.FirstOrDefault(p => p.Id == item.Id);
        if (userItem != null)
        {
            userItem.RoleUId = 0;
        }

    }  
    #endregion

    #region ResponeFormServer  
    public void OnResponseSetHeroHire()
    {
        RefreshHeroList();
    }
    public void OnResponseHeroListFormServer()
    {
        InitHeroList();

        if (_heroSlots.Count > 0)
            _heroSlotSelected = _heroSlots[0];

        ShowHeroInfo();
        inventory.Init();
        _controller.SendRequestGetHireHero();
    }
    public void OnResponseRoleUpdate()
    {
        RefreshHeroList();

        OnCheckToBattle();
    }
    public void OnResponseSellItem()
    {
        if (infoItemSelected != null)
        {
            infoItemSelected.Close();
        }

        if (infoItemEuqiped != null)
        {
            infoItemEuqiped.Close();
        }
        if (consumeInfo != null)
        {
            consumeInfo.Close();
        }
        inventory.RefreshSlot();
    }
    public void OnResponseEquipment()
    {
        if(_heroSlotSelected==null) return;
        if (infoItemSelected != null)
        {
            infoItemSelected.Refresh(_heroSlotSelected.userRole);
        }

        if (infoItemEuqiped != null)
        {
            infoItemEuqiped.Refresh(_heroSlotSelected.userRole);
        }

        if (consumeInfo != null)
        {
            consumeInfo.Refresh();
        }
        ShowHeroInfo();
        inventory.RefreshSlot();
        RefreshHeroList();
    }
    #endregion

    #region Private methods
    private void OnChangeTab(InvetoryTab tab)
    {
        if (!inventory.uiInventory.background.activeInHierarchy)
            OpenInventory();

        _inventoryTab = tab;

        switch (tab)
        {
            case InvetoryTab.All:
                if (!uiTab.All.value)
                    uiTab.All.value = true;
                break;
            case InvetoryTab.Armor:
                if (!uiTab.armor.value)
                    uiTab.armor.value = true;
                break;
            case InvetoryTab.EXP:
                if (!uiTab.support.value)
                    uiTab.support.value = true;
                break;
            case InvetoryTab.Medal:
                if (!uiTab.medal.value)
                    uiTab.medal.value = true;
                break;
            case InvetoryTab.Ring:
                if (!uiTab.ring.value)
                    uiTab.ring.value = true;
                break;
            case InvetoryTab.Support:
                if (!uiTab.support.value)
                    uiTab.support.value = true;
                break;
        }
    }
    private void ShowHeroInfo()
    {
        if (_heroSlotSelected == null) return;
        UserRole userRole = _heroSlotSelected.userRole;
        if (userRole == null) return;

        uiHero.quality.mainTexture = Helper.LoadTextureElement((int)userRole.Base.ElemId);
        uiHero.icon.mainTexture = Helper.LoadTextureForHero(userRole.Base.RoleId);
        uiHero.starManager.SetStart(userRole.Base.Grade);
        uiHero.lblName.text = userRole.Name;
        uiHero.lblLevel.text = GameManager.localization.GetText("Global_Lvl") + userRole.Base.Level;
        uiHero.exp.value = (float)userRole.Base.Exp / (float)RoleConfig.LEVELS_EXP[userRole.Base.Level];
        uiHero.energy.value = (float)userRole.Base.Energy / (float)RoleConfig.ENERGY_MAX;
        uiHero.expValue.text = userRole.Base.Exp + "/" + RoleConfig.LEVELS_EXP[userRole.Base.Level];
        uiHero.energyValue.text = userRole.Base.Energy + "/" + RoleConfig.ENERGY_MAX;
        uiHero.inBattle.value = userRole.Base.Status == RoleStatus.Active;

        uiHero.skillDef.transform.Find("Texture").GetComponent<UITexture>().mainTexture = Helper.LoadTextureForSkill(userRole.RoleSkills[0].SkillId);
        uiHero.skill1.transform.Find("Texture").GetComponent<UITexture>().mainTexture = Helper.LoadTextureForSkill(userRole.RoleSkills[1].SkillId);
        uiHero.skill2.transform.Find("Texture").GetComponent<UITexture>().mainTexture = Helper.LoadTextureForSkill(userRole.RoleSkills[2].SkillId);
        uiHero.biography.text = "";


        uiHero.element.spriteName = Helper.GetSpriteNameOfElement(userRole.Base.ElemId);
        uiHero.roleClass.spriteName = Helper.GetSpriteNameOfRoleClass(userRole.Base.Class);

        UserItem ring = userRole.RoleItems.FirstOrDefault(p => p.RoleUId == userRole.Id && p.GameItem.Kind == (int)ItemKind.Ring);
        UserItem armor = userRole.RoleItems.FirstOrDefault(p => p.RoleUId == userRole.Id && p.GameItem.Kind == (int)ItemKind.Armor);
        UserItem support = userRole.RoleItems.FirstOrDefault(p => p.RoleUId == userRole.Id && p.GameItem.Kind == (int)ItemKind.Support);
        UserItem medal = userRole.RoleItems.FirstOrDefault(p => p.RoleUId == userRole.Id && p.GameItem.Kind == (int)ItemKind.Medal);

        ShowEquipItem(ring, uiHero.ring);
        ShowEquipItem(armor, uiHero.armor);
        ShowEquipItem(support, uiHero.support);
        ShowEquipItem(medal, uiHero.medal);

        CloseSkillWindow();


        if (GameManager.GameUser.HireRoles.Count > 0)
        {
            UserRoleHire hireRole = GameManager.GameUser.HireRoles.FirstOrDefault(p => p.Id == userRole.Id);
            if (hireRole != null)
            {
                uiHireHero.txtGold.value = hireRole.HireGold.ToString();
                uiHireHero.txtSilver.value = hireRole.HireSilver.ToString();
             
                if (hireRole.HireMode != 0)
                {
                    uiHireHero.togAcceptToHire.value = true;
                }
                else
                {
                    uiHireHero.togAcceptToHire.value = false;
                }
            }
            else
            {
                uiHireHero.txtGold.value = "0";
                uiHireHero.txtSilver.value = "0";
                uiHireHero.togAcceptToHire.value = false;
            }
        }

    }
    private void ShowEquipItem(UserItem item, UIButton button)
    {
        if (item != null)
        {
            UITexture border = button.transform.Find("Border").GetComponent<UITexture>();
            UITexture icon = button.transform.Find("Icon").GetComponent<UITexture>();
            if (item.GameItem.Kind != (int)ItemKind.Support)
            {
                icon.mainTexture = Helper.LoadTextureForEquipItem(item.ItemId);
            }
            else
            {
                icon.mainTexture = Helper.LoadTextureForSupportItem(item.ItemId);
                UILabel lblAmount = button.transform.Find("lblAmount").GetComponent<UILabel>();
                lblAmount.gameObject.SetActive(true);
                lblAmount.text = "x" + item.Count.ToString();
            }

            border.color = Helper.ItemColor[item.Grade];
            button.transform.Find("Label").gameObject.SetActive(false);
            border.gameObject.SetActive(true);
            icon.gameObject.SetActive(true);
        }
        else
        {
            Transform lblAmount = button.transform.Find("lblAmount");
            if (lblAmount != null)
            {
                lblAmount.gameObject.SetActive(false);
            }
            UITexture border = button.transform.Find("Border").GetComponent<UITexture>();
            UITexture icon = button.transform.Find("Icon").GetComponent<UITexture>();
            button.transform.Find("Label").gameObject.SetActive(true);
            border.gameObject.SetActive(false);
            icon.gameObject.SetActive(false);
        }
    }   
    private void InitHeroList()
    {
        _heroSlots = new List<UIHeroNewHeroSlot>();
        for (int i = 0; i < GameManager.GameUser.UserRoles.Count; i++)
        {
            GameObject go = NGUITools.AddChild(uiHeroList.root.gameObject, uiHeroList.prefab);
            go.SetActive(true);
            UIHeroNewHeroSlot heroSlot = go.GetComponent<UIHeroNewHeroSlot>();
            heroSlot.SetHero(GameManager.GameUser.UserRoles[i], this);
            _heroSlots.Add(heroSlot);
        }
        uiHeroList.root.Reposition();
    }
    private void RefreshHeroList()
    {
        foreach (UIHeroNewHeroSlot slot in _heroSlots)
        {
            slot.Refresh();
        }
    }
    private IEnumerator AnimationTweenSkillDetail(float posX)
    {
        if (skillManager.gameObject.activeInHierarchy)
        {
            CloseSkillWindow();
            yield return new WaitForSeconds(0.5f);
        }

        TweenPosition tweenPos = skillManager.GetComponent<TweenPosition>();
        tweenPos.from = new Vector3(posX, 0, 0);
        tweenPos.to = new Vector3(posX, 226.3f, 0);

        UIPlayTween playTween = new UIPlayTween();
        playTween.tweenTarget = skillManager.gameObject;
        playTween.playDirection = AnimationOrTween.Direction.Forward;
        playTween.ifDisabledOnPlay = AnimationOrTween.EnableCondition.EnableThenPlay;
        playTween.disableWhenFinished = AnimationOrTween.DisableCondition.DisableAfterReverse;
        playTween.Play(true);

        yield return null;
    }
    private void CloseSkillWindow()
    {
        UIPlayTween playTweenInverse = new UIPlayTween();
        playTweenInverse.tweenTarget = skillManager.gameObject;
        playTweenInverse.playDirection = AnimationOrTween.Direction.Reverse;
        playTweenInverse.ifDisabledOnPlay = AnimationOrTween.EnableCondition.EnableThenPlay;
        playTweenInverse.Play(true);
    }
    private void Localization()
    {
        uiLabelList.lblHeroCount.text = string.Format(GameManager.localization.GetText("HeroNew_HeroCount"), GameManager.GameUser.UserRoles.Count, GameManager.maxSlotHero);
        uiLabelList.lblEquip_Ring.text = GameManager.localization.GetText("HeroNew_Equip_Ring");
        uiLabelList.lblEquip_Armor.text = GameManager.localization.GetText("HeroNew_Equip_Armor");
        uiLabelList.lblEquip_Sup.text = GameManager.localization.GetText("HeroNew_Equip_Support");
        uiLabelList.lblEquip_Medal.text = GameManager.localization.GetText("HeroNew_Equip_Medal");
        uiLabelList.lblEXP.text = GameManager.localization.GetText("HeroNew_EXP");
        uiLabelList.lblEnergy.text = GameManager.localization.GetText("Global_Energy");
        uiLabelList.lblInBattle.text = GameManager.localization.GetText("HeroNew_InBattle");
        uiLabelList.lblViewHeroInfo.text = GameManager.localization.GetText("HeroNew_ViewInfoHero");
        uiLabelList.lblViewSkillInfo.text = GameManager.localization.GetText("HeroNew_ViewInfoSkill");
        uiLabelList.lblInventory.text = GameManager.localization.GetText("HeroNew_Inventory");
        uiLabelList.lblBtnUpStar.text = GameManager.localization.GetText("HeroNew_btnUpStar");
        uiLabelList.lblBtnUpLevel.text = GameManager.localization.GetText("HeroNew_btnUpLevel");
        uiLabelList.lblBtnUpExp.text = GameManager.localization.GetText("HeroNew_btnUpExp");
        uiLabelList.lblBtnStrategy.text = GameManager.localization.GetText("HeroNew_Strategy");
        uiHireHero.lblHeroForHide.text = GameManager.localization.GetText("HeroNew_LblHeroForHire");
        uiHireHero.lblAcceptToHire.text = GameManager.localization.GetText("HeroNew_CHKHeroForHire");
        uiHireHero.lblButtonSave.text = GameManager.localization.GetText("HeroNew_BTNSave");      
    }
    #endregion

    #region Tutorial
    private void Tutorial()
    {
        if (GameManager.tutorial.step == TutorialManager.TutorialStep.BuyHero_Exit)
        {
            GameManager.tutorial.CreateNPC(UINPCTutorialManager.Status.Normal, GameManager.localization.GetText("Tut_Party"));
            GameManager.tutorial.ChangeStep(TutorialManager.TutorialStep.Party_NPC);

        }
        if (GameManager.tutorial.step == TutorialManager.TutorialStep.Equip_Arrow)
        {
            uiTutorial.arrowHero1.SetActive(true);
        }
    }
    public void OnHeroSelected()
    {
        if (GameManager.tutorial.step == TutorialManager.TutorialStep.Equip_Arrow)
        {
            uiTutorial.arrowHero1.SetActive(false);
            uiTutorial.arrowHero2.SetActive(false);
            uiTutorial.arrowHero3.SetActive(false);

            uiTutorial.arrowHero1.SetActive(false);
            uiTutorial.arrowSlot.SetActive(true);
        }
    }
    private void OnTabEquipClick()
    {
        if (GameManager.tutorial.step == TutorialManager.TutorialStep.Equip_Arrow)
        {
            uiTutorial.arrowSlot.SetActive(false);
            uiTutorial.arrowItem.SetActive(true);
            uiTutorial.arrowButtonEquip.SetActive(true);
        }
    }
    private void OnEquipItem()
    {
        if (GameManager.tutorial.step == TutorialManager.TutorialStep.Equip_Arrow)
        {
            infoItemSelected.Close();
            uiTutorial.arrowItem.SetActive(false);
            uiTutorial.arrowButtonEquip.SetActive(false);
            uiTutorial.arrowExit.SetActive(true);
            GameManager.tutorial.ChangeStep(TutorialManager.TutorialStep.Equip_Finished);
        }
    }
    private void UpdateArrow()
    {
        if (GameManager.tutorial.step == TutorialManager.TutorialStep.Party_NPC)
        {
            if (_heroSlotSelected != null)
            {
                if (_heroSlotSelected.userRole.Base.Status == RoleStatus.Active)
                {
                    uiTutorial.arrowInBattle.SetActive(false);
                    OnCheckToBattle();
                }
                else
                {
                    uiTutorial.arrowInBattle.SetActive(true);
                }
            }
        }
    }
    public void OnCheckToBattle()
    {
        if (GameManager.tutorial.step == TutorialManager.TutorialStep.Party_NPC)
        {
            int heroActiveCount = GameManager.GameUser.UserRoles.Count(p => p.Base.Status == RoleStatus.Active);
            if (heroActiveCount >= 3)
            {
                // GameManager.tutorial.CreateNPC(UINPCTutorialManager.Status.Normal, GameManager.localization.GetText("Tut_PartyFinished"));
                GameManager.tutorial.ChangeStep(TutorialManager.TutorialStep.Party_NPCFinished);

                uiTutorial.arrowHero1.SetActive(false);
                uiTutorial.arrowHero2.SetActive(false);
                uiTutorial.arrowHero3.SetActive(false);
                uiTutorial.arrowExit.SetActive(true);
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    if (GameManager.GameUser.UserRoles[i].Base.Status == RoleStatus.Default)
                    {
                        if (i == 0)
                        {
                            uiTutorial.arrowHero1.SetActive(true);
                            uiTutorial.arrowHero2.SetActive(false);
                            uiTutorial.arrowHero3.SetActive(false);
                            break;
                        }
                        if (i == 1)
                        {
                            uiTutorial.arrowHero2.SetActive(true);
                            uiTutorial.arrowHero1.SetActive(false);
                            uiTutorial.arrowHero3.SetActive(false);
                            break;
                        }
                        if (i == 2)
                        {
                            uiTutorial.arrowHero1.SetActive(false);
                            uiTutorial.arrowHero2.SetActive(false);
                            uiTutorial.arrowHero3.SetActive(true);
                            break;
                        }
                    }
                }
            }
          
        }
    }
    #endregion
}
