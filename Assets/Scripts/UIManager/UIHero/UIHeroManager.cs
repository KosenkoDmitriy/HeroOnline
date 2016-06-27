using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;
using System.Collections.Generic;
using System.Linq;
using DEngine.Common.Config;

public class UIHeroManager : MonoBehaviour {

    [System.Serializable]
    public struct UITutorial
    {
        public GameObject objArrowViewInfoHero;
        public GameObject objArrowViewInfoSkill;
        public GameObject objArrowExitInfoSkill;
        public GameObject objArrowExitInfoHero;
        public GameObject objArrowEnergy;
        public GameObject objArrowChose3Hero;
    }

    [System.Serializable]
    public struct ItemSlot
    {
        public UIButton button;
        public UITexture border;
        public UITexture icon;
        public UILabel lblamount;
    }


    public UILabel L_btnBack;
    public UILabel L_btnShop;
    public UILabel L_btnInventory;
    public UILabel L_btnStategy;
    public UILabel L_btnUpLevel;
    public UILabel L_btnUpStar;

    public UILabel LblFilter_Support;
    public UILabel LblFilter_Ring;
    public UILabel LblFilter_Armor;
    public UILabel LblFilter_Medal;
    public UILabel LblFilter_ALL;
    public UILabel LblFilter_Auto;

    public UILabel LblInventory_Info;
    public UILabel LblInventory_Use;
    public UILabel LblInventory_Sell;
    public UILabel LblInventory_Enhancement;

    public UILabel L_hp;
    public UILabel L_Mana;
    public UILabel L_PAtk;
    public UILabel L_PDef;
    public UILabel L_MAtk;
    public UILabel L_MDef;
    public UILabel L_ARate;
    public UILabel L_MSpeed;
    public UILabel LSellValue;
    public UILabel LEXP;
    public UILabel LEnergy;

    public UILabel LBLHeroCount;
    public UILabel LBLHeroName;
    public UILabel LBLHroLevel;
    public UILabel LBLGold;
    public UILabel LBLSilver;
    public UILabel LBLStrength;
    public UILabel LBLIntel;
    public UILabel LBLAgility;
    public UILabel LBLPoint;
    public UITexture ImgQuality;
    public UITexture ImgHero;

    public UILabel LblHP;
    public UILabel LblMana;
    public UILabel LblPhyAtk;
    public UILabel LblAtkRate;
    public UILabel LblPhysDefence;
    public UILabel LblMagicAtk;
    public UILabel LblMagicDefence;
    public UILabel LblMovementSpeed;
    public UILabel LExpValue;
    public UILabel LEnergyValue;




    public UIProgressBar energyBar;
    public UIProgressBar expBar;

    public UIButton btnEquip;

    public UIProgressBar BarStrength;
    public UIProgressBar BarIntel;
    public UIProgressBar BarAgility;

    public GameObject HeroSlotRoot;
    public GameObject HeroSlotPrefab;
    public GameObject uiEquipmentRoot;
    public GameObject uiInventoryRoot;
    public GameObject uiItemSlotPrefab;
    public GameObject uiItemSlotRoot;
    public GameObject inforRoot;

    public UIInventoryManager inventory;
    public UIInformationItemManager information;
    public UIStrategyManager strategyManager;
    public UIHeroStarManager starManager;    

    public UIHeroItemSlotManager itemSlotSelected { get; set; }
    public UIHeroSlotManager heroSlotSelected { get; set; }

    public ItemSlot equipConsume;
    public ItemSlot equipRing;
    public ItemSlot equipMedal;
    public ItemSlot equipArmor;
    public UIInformationHeroManager informationHeroManager;
    
    public UILabel lblArrowView;

    public UITutorial uiTutorial;

    private Dictionary<int, UIHeroSlotManager> _heroUISet;
    private List<UIHeroItemSlotManager> _itemSet;
    private ItemKind _curItemTypeSelect;
    public HeroMenuController _heroMenuController;
    private int _maxHero = 99;

	void Start () 
    {
        Localization();

        _heroUISet = new Dictionary<int, UIHeroSlotManager>();
        _heroMenuController = new HeroMenuController(this);
        _itemSet = new List< UIHeroItemSlotManager>();
        _curItemTypeSelect = ItemKind.None;
        _maxHero = GameManager.maxSlotHero;
        _heroMenuController.SendRequestRoleList();
        _heroMenuController.SendRequestRefreshGold();

        inventory.heroManager = this;
        setGoldSivler();

        if (GameScenes.previousSence == GameScenes.MyScene.ItemUpgrade || GameManager.Status == GameStatus.Storage)
            OnButtonInventory_Click();

	}

    void OnDestroy()
    {
    }

    void Update()
    {
        //MyInput.CheckInput();
    }

    #region public Methods
    public void OnClickSlot(UIHeroSlotManager uiHeroSlotManager)
    {
        if (MessageBox.isShow) return;    

        if (heroSlotSelected != null)
            heroSlotSelected.OnDeselect();
        heroSlotSelected = uiHeroSlotManager;

        SetHeroSelectedUI();
        inventory.RefreshGUI();
        if (_curItemTypeSelect != ItemKind.None)
        {
            information.gameObject.SetActive(false);
            ClearItems();
            OnSlot_Click(_curItemTypeSelect);
        }

    }

    public void OnChangedStatusRole(UserRole role)
    {
        GameManager.GameUser.UserRoles.FirstOrDefault(p => p.Id == role.Id).Base.Status = role.Base.Status;
        _heroMenuController.SendRequestUpdateRole(role);
    }

    public void OnSaveStrategy(UserRole role)
    {
        _heroMenuController.SendRequestUpdateRole(role);
    }

    public void OnReciveUserUpdate()
    {
    }
    #endregion

    #region private Methods
    private void SetHeroSelectedUI()
    {
        UserRole curRole = heroSlotSelected.userRole;
        LBLHeroName.text = curRole.Name;
        LBLHroLevel.text = GameManager.localization.GetText("Global_Lvl") + curRole.Base.Level.ToString();
        ImgHero.mainTexture = Helper.LoadTextureForHero(curRole.Base.RoleId);
        ImgQuality.mainTexture = Helper.LoadTextureElement((int)curRole.Base.ElemId);
        
        LblHP.text = curRole.Attrib.MaxHP.ToString();
        LblMana.text = curRole.Attrib.MaxMP.ToString();

        LblPhyAtk.text = curRole.Attrib.AttackValue.ToString("0");
        LblPhysDefence.text = curRole.Attrib.DefenceValue.ToString("0");

        //LblMagicAtk.text = curRole.Attrib.MAttack.ToString("0");
        //LblMagicDefence.text = curRole.Attrib.MDefence.ToString("0");

        LblAtkRate.text = curRole.Attrib.AttackSpeed.ToString("0");

        LblMovementSpeed.text = curRole.Attrib.MoveSpeed.ToString("0");

        energyBar.value = (float)curRole.Base.Energy / (float)RoleConfig.ENERGY_MAX;
        LEnergyValue.text = string.Format("{0:0}%", energyBar.value * 100);

        if (curRole.Base.Level < RoleConfig.LEVEL_MAX)
            expBar.value = (float)curRole.Base.Exp / (float)RoleConfig.LEVELS_EXP[curRole.Base.Level];
        else
            expBar.value = 0;

        if (RoleConfig.LEVELS_EXP[curRole.Base.Level] > 0)
            LExpValue.text = string.Format("{0:0}/{1:0}", curRole.Base.Exp, RoleConfig.LEVELS_EXP[curRole.Base.Level]);
        else
            LExpValue.text = "-/-";

        starManager.SetStart(curRole.Base.Grade);

        ShowRingEquip();
        ShowArmorEquip();
        ShowMedalEquip();
        ShowResumeEquip();

       // strategyManager.SetRole(curRole,this);
    }
    private void ShowRingEquip()
    {
        //equipRing.

        UserItem ring = GameManager.GameUser.UserItems.FirstOrDefault(p => p.RoleUId == heroSlotSelected.userRole.Id && p.GameItem.Kind == (int)ItemKind.Ring);
       
        if (ring != null)
        {
            equipRing.border.gameObject.SetActive(true);
            equipRing.icon.gameObject.SetActive(true);
            equipRing.icon.mainTexture = Helper.LoadTextureForEquipItem(ring.ItemId);
            Color color = Helper.ItemColor[ring.Grade];
            equipRing.border.color = color;

            equipRing.button.normalSprite = "hero_equip";
            equipRing.button.hoverSprite = "hero_equip";
            equipRing.button.pressedSprite = "hero_equip";
        }
        else
        {
            equipRing.border.gameObject.SetActive(false);
            equipRing.icon.gameObject.SetActive(false);
            equipRing.button.normalSprite = "equip_ring_normal";
            equipRing.button.hoverSprite = "equip_ring_over";
            equipRing.button.pressedSprite = "equip_ring_over";
        }
    }
    private void ShowResumeEquip()
    {
        //equipRing.

        UserItem Resume = GameManager.GameUser.UserItems.FirstOrDefault(p => p.RoleUId == heroSlotSelected.userRole.Id && p.GameItem.Kind == (int)ItemKind.Support);

        if (Resume != null)
        {
          //  equipConsume.border.gameObject.SetActive(true);
            equipConsume.icon.gameObject.SetActive(true);
            equipConsume.icon.mainTexture = Helper.LoadTextureForSupportItem(Resume.ItemId);
           // Color color = Helper.ItemColor[Resume.Grade];
           // equipConsume.border.color = color;
            equipConsume.lblamount.text = "x" + Resume.Count;
            equipConsume.button.normalSprite = "hero_equip";
            equipConsume.button.hoverSprite = "hero_equip";
            equipConsume.button.pressedSprite = "hero_equip";
        }
        else
        {
            equipConsume.border.gameObject.SetActive(false);
            equipConsume.icon.gameObject.SetActive(false);
            equipConsume.lblamount.text = "";
            equipConsume.button.normalSprite = "hpmana_macdinh";
            equipConsume.button.hoverSprite = "hpmana_equip";
            equipConsume.button.pressedSprite = "hpmana_equip";
        }
    }
    private void ShowArmorEquip()
    {
        //equipRing.

        UserItem armor = GameManager.GameUser.UserItems.FirstOrDefault(p => p.RoleUId == heroSlotSelected.userRole.Id && p.GameItem.Kind == (int)ItemKind.Armor);
      
        if (armor != null)
        {
            equipArmor.border.gameObject.SetActive(true);
            equipArmor.icon.gameObject.SetActive(true);
            equipArmor.icon.mainTexture = Helper.LoadTextureForEquipItem(armor.ItemId);
            Color color = Helper.ItemColor[armor.Grade];
            equipArmor.border.color = color;

            equipArmor.button.normalSprite = "hero_equip";
            equipArmor.button.hoverSprite = "hero_equip";
            equipArmor.button.pressedSprite = "hero_equip";
        }
        else
        {
            equipArmor.border.gameObject.SetActive(false);
            equipArmor.icon.gameObject.SetActive(false);
            equipArmor.button.normalSprite = "equip_armo_normal";
            equipArmor.button.hoverSprite = "equip_armo_over";
            equipArmor.button.pressedSprite = "equip_armo_over";
        }



    }
    private void ShowMedalEquip()
    {
        //equipRing.

        UserItem medal = GameManager.GameUser.UserItems.FirstOrDefault(p => p.RoleUId == heroSlotSelected.userRole.Id && p.GameItem.Kind == (int)ItemKind.Medal);
      
        if (medal != null)
        {
            equipMedal.border.gameObject.SetActive(true);
            equipMedal.icon.gameObject.SetActive(true);
            equipMedal.icon.mainTexture = Helper.LoadTextureForEquipItem(medal.ItemId);
            Color color = Helper.ItemColor[medal.Grade];
            equipMedal.border.color = color;

            equipMedal.button.normalSprite = "hero_equip";
            equipMedal.button.hoverSprite = "hero_equip";
            equipMedal.button.pressedSprite = "hero_equip";
        }
        else
        {
            equipMedal.border.gameObject.SetActive(false);
            equipMedal.icon.gameObject.SetActive(false);
            equipMedal.button.normalSprite = "medal_macdinh";
            equipMedal.button.hoverSprite = "medal_equip";
            equipMedal.button.pressedSprite = "medal_equip";
        }



    }
    private void setGoldSivler()
    {
        if (GameManager.GameUser != null)
        {
            LBLGold.text = GameManager.GameUser.Base.Gold.ToString();
            LBLSilver.text = GameManager.GameUser.Base.Silver.ToString();
        }
    }
    public void LoadHeroSlots()
    {
        LBLHeroCount.text = string.Format("{0}/{1} " + GameManager.localization.GetText("Hero_Heroes"), GameManager.GameUser.UserRoles.Count, _maxHero);
        int i = 0;
        foreach (UserRole item in GameManager.GameUser.UserRoles)
        {
            if (!_heroUISet.ContainsKey(item.Id))
            {
                GameObject go = NGUITools.AddChild(HeroSlotRoot, HeroSlotPrefab);
                go.transform.localPosition = new Vector3(0, -70 * i, 0);
                UIHeroSlotManager slot = go.GetComponent<UIHeroSlotManager>();
                slot.uiHeroManager = this;
                //Debug.Log("item " + item);
                slot.SetGameRole(item);
                if (i == 0)
                    slot.OnClick();

                _heroUISet[item.Id] = slot;
            }
          
            i++;


        }
    }
    private IEnumerator ChangeValueProcessBar(float value, float max, UIProgressBar bar)
    {
        float curBarValue = bar.value;
        float destValue = value / max;
        float lerpValue = 0;
        float timer = Time.time;

        while (lerpValue != destValue)
        {
            lerpValue = Mathf.Lerp(curBarValue, destValue, (Time.time - timer)*3);        
            bar.value = lerpValue;
            yield return new WaitForSeconds(Time.deltaTime) ;
        }
        if (lerpValue <= 0.05f) bar.value = 0.05f;
    }
    private void OnSlot_Click(ItemKind type)
    {
        itemSlotSelected = null;
        _curItemTypeSelect = type;
        uiEquipmentRoot.SetActive(false);
        uiInventoryRoot.SetActive(true);

        List<UserItem> items;

        if (type == ItemKind.Consume)
        {
            items = GameManager.GameUser.UserItems.Where(p => p.GameItem.Kind == (int)type
                && (p.RoleUId == 0 || p.RoleUId == heroSlotSelected.userRole.Id)
                && p.GameItem.Kind == (int)ItemKind.Consume).ToList();
        }
        else
        {
            items = GameManager.GameUser.UserItems.Where(p => p.GameItem.Kind == (int)type
                && (p.RoleUId == 0 || p.RoleUId == heroSlotSelected.userRole.Id)).ToList();
                //.OrderByDescending(p => p.RoleUId == heroSlotSelected.userRole.Id).ThenByDescending(p => p.GameItem.Level).ThenByDescending(p => p.Grade).ToList();
        }

        for (int i = 0; i < items.Count; i++)
        {            
            GameObject go = NGUITools.AddChild(uiItemSlotRoot, uiItemSlotPrefab);
            UIHeroItemSlotManager slot = go.GetComponent<UIHeroItemSlotManager>();
            slot.SetItem(items[i], this);
            _itemSet.Add(slot);
        }

        uiItemSlotRoot.GetComponent<UIGrid>().Reposition();
    }
    private void ClearItems()
    {
        itemSlotSelected = null;
        foreach (UIHeroItemSlotManager slot in _itemSet)
        {
            NGUITools.Destroy(slot.gameObject);
        }
        _itemSet.Clear();

    }
    private void Localization()
    {
        L_hp.text = GameManager.localization.GetText("Hero_HP");
        L_Mana.text = GameManager.localization.GetText("Hero_MP");
        L_PAtk.text = GameManager.localization.GetText("Hero_PhyAtk");
        L_PDef.text = GameManager.localization.GetText("Hero_PhyDef");
        L_MAtk.text = GameManager.localization.GetText("Hero_Matk");
        L_MDef.text = GameManager.localization.GetText("Hero_Mdef");
        L_ARate.text = GameManager.localization.GetText("Hero_Arate");
        L_MSpeed.text = GameManager.localization.GetText("Hero_Mspeed");
        LSellValue.text = GameManager.localization.GetText("Shop_SellValue");
        LEXP.text = GameManager.localization.GetText("Global_btn_Exp") + ":";
        LEnergy.text = GameManager.localization.GetText("Global_Energy") + ":";
        L_btnStategy.text = GameManager.localization.GetText("Hero_btn_Strategy").ToUpper();
        L_btnUpLevel.text = GameManager.localization.GetText("Hero_btn_LvlUp").ToUpper();
        L_btnUpStar.text = GameManager.localization.GetText("Hero_btn_Upgrade").ToUpper();
        L_btnInventory.text = GameManager.localization.GetText("Hero_Inventory").ToUpper();
        L_btnShop.text = GameManager.localization.GetText("WorldMap_Shop").ToUpper();
        L_btnBack.text = GameManager.localization.GetText("Shop_Back").ToUpper();

        LblFilter_Support.text = GameManager.localization.GetText("Shop_Support").ToUpper();
        LblFilter_Ring.text = GameManager.localization.GetText("Shop_Ring").ToUpper();
        LblFilter_Armor.text = GameManager.localization.GetText("Shop_Armor").ToUpper();
        LblFilter_Medal.text = GameManager.localization.GetText("Shop_Medal").ToUpper();
        LblFilter_Auto.text = GameManager.localization.GetText("Hero_Inventory_Filter_AutoSort").ToUpper();
        LblFilter_ALL.text = GameManager.localization.GetText("Hero_Inventory_Filter_All").ToUpper();

        LblInventory_Info.text = GameManager.localization.GetText("Hero_Inventory_Info");
        LblInventory_Use.text = GameManager.localization.GetText("Hero_Inventory_Use");
        LblInventory_Sell.text = GameManager.localization.GetText("Hero_Inventory_Sell");
        LblInventory_Enhancement.text = GameManager.localization.GetText("Hero_Inventory_Enhancement");


        lblArrowView.text = GameManager.localization.GetText("Tut_ShopHero_ViewInfo");
    }
    #endregion

    #region Button  
    public void OnButtonBackClick()
    {
        if (MessageBox.isShow) return;
        GameScenes.ChangeScense(GameScenes.MyScene.Hero, GameScenes.MyScene.WorldMap);
    }
    public void OnButtonShopClick()
    {
        if (MessageBox.isShow) return;
         
     
        GameScenes.ChangeScense(GameScenes.MyScene.Hero, GameScenes.MyScene.ChargeShop);
    }
    public void OnButtonTeamClick()
    {
        if (MessageBox.isShow) return;
        
     
    }
    public void OnButtonFightClick()
    {
        if (MessageBox.isShow) return;
        

        GameScenes.ChangeScense(GameScenes.MyScene.Hero, GameScenes.MyScene.PVEMap);
    }
    public void OnButtonHead_Click()
    {
        
        OnSlot_Click(ItemKind.Medal);
    }
    public void OnButtonWeapon_Click()
    {
        
        OnSlot_Click(ItemKind.Armor);

    }
    public void OnButtonArmor_Click()
    {
        
        OnSlot_Click(ItemKind.Armor);
    }
    public void OnButtonRing_Click()
    {
        
        OnSlot_Click(ItemKind.Ring);
    }
    public void OnButtonConsume_Click()
    {
        
        OnSlot_Click(ItemKind.Consume);
    }

    public void OnButtonAutoEquip_Click()
    {
        
    }
    public void OnButtonCloseInventory_Click()
    {
        
        uiEquipmentRoot.SetActive(true);
        uiInventoryRoot.SetActive(false);

        ClearItems();
     
        information.gameObject.SetActive(false);

        _curItemTypeSelect = ItemKind.None;
    }
    public void OnEquip()
    {
        
        if (MessageBox.isShow) return;

        if (itemSlotSelected == null) return;


        UserItem item = itemSlotSelected.itemContain;

        if (heroSlotSelected.userRole.Id != item.RoleUId)
        {
            //Euip
            UserItem oldItemEquiped = GameManager.GameUser.UserItems.FirstOrDefault(p => p.RoleUId == heroSlotSelected.userRole.Id && p.GameItem.Kind == item.GameItem.Kind);
            Debug.Log("OnEquip " + itemSlotSelected.itemContain.Name);
            if (oldItemEquiped != null)
            {
                GameManager.GameUser.SetItemForRole(null, oldItemEquiped);
              //  _heroMenuController.SendRequestEquipItem(0, oldItemEquiped.Id);

                UIHeroItemSlotManager uiSlot = _itemSet.FirstOrDefault(p => p.itemContain.Id == oldItemEquiped.Id);
                if (uiSlot != null)
                {
                    uiSlot.Refresh();
                }
            }

           /* if (!GameManager.GameUser.SetItemForRole(heroSlotSelected.userRole, item))
            {
                MessageBox.ShowDialog(GameManager.localization.GetText("Equip_CantEuqip"), UINoticeManager.NoticeType.Message);
                return;
            }
            
            _heroMenuController.SendRequestEquipItem(heroSlotSelected.userRole.Id, item.Id);*/
        }
        else
        {
            //EnEquip
            Debug.Log("EnEquip " + itemSlotSelected.itemContain.Name);
            GameManager.GameUser.SetItemForRole(null, item);
            _heroMenuController.SendRequestEquipItem(0, item.Id);
        }
    }
    public void OnRefreshGold()
    {
    }
    public void OnResponseEquipment()
    {

      //  heroSlotSelected.userRole.OnEquipmentsChanged();
        if (itemSlotSelected != null)
        {
            itemSlotSelected.Refresh();
        }
        SetHeroSelectedUI();
    }
    public void OnButtonStrategy_Click()
    {
        inforRoot.SetActive(false);
        inventory.gameObject.SetActive(false);
        strategyManager.gameObject.SetActive(true);
    }  
    public void OnButtonClose_Click()
    {
        inforRoot.SetActive(true);
        strategyManager.gameObject.SetActive(false);
    }
    public void OnButtonExitInventory_Click()
    {
        inventory.gameObject.SetActive(false);
        inforRoot.SetActive(true);
    }
    public void OnButtonInventory_Click()
    {
        inforRoot.SetActive(false);
        strategyManager.gameObject.SetActive(false);
        inventory.gameObject.SetActive(true);
    }
    public void OnButtonLevelUp_Click()
    {
        GameScenes.ChangeScense(GameScenes.MyScene.Hero, GameScenes.MyScene.HeroUpgrade);
    }
    public void OnButtonUpStar_Click()
    {
        GameScenes.ChangeScense(GameScenes.MyScene.Hero, GameScenes.MyScene.HeroUpStar);
    }
    public void OnHeroImage_Click()
    {
        if (heroSlotSelected != null)
        {
            informationHeroManager.ShowItem(heroSlotSelected.userRole);
            informationHeroManager.gameObject.SetActive(true);
            informationHeroManager.GetComponent<TweenScale>().PlayForward();
        }
    }
    public void OnButtonSkill1_Click()
    {

    }
    public void OnButtonSkill2_Click()
    {
    }
    public void OnButtonExitHeroInfo_Click()
    {
    }
    #endregion

    
    
}
