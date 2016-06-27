using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DEngine.Common.GameLogic;
using DEngine.Common;

public class UIShopManager : MonoBehaviour
{
    [System.Serializable]
    public struct UITutorial
    {
        public GameObject ArrowToHero;
        public GameObject ArrowToMenuHero;
    }

    [System.Serializable]
    public struct CardInfo
    {
        public UILabel lblPrice;
        public UILabel lblGold;
        public UILabel lblBonus;
        public UILabel lblTotal;
        public UILabel lblButtonNext;
    }

    public enum ActiveTab
    {
        None,
        Support,
        Gear,
        Hero,
        Vault,
        Ring,
        Armor,
        Medal,
        Level1_5,
        Level6_10,
        Jewel,
        Jewel_White,
        Jewel_Green,
        Jewel_Blue,
        Jewel_Yellow,
        Energy,
        IOSReCharge
    }

    public UILabel L_Support;
    public UILabel L_Jewel;
    public UILabel L_Heroes;
    public UILabel L_Vaults;
    public UILabel L_SellValue;
    public UILabel L_Jewel_White;
    public UILabel L_Jewel_Green;
    public UILabel L_Jewel_Blue;
    public UILabel L_Jewel_Yellow;
    public UILabel L_Back;


    public GameObject itemRoot;
    public GameObject itemPrefab;

    public UIButton btnSupport;
    public UIButton btnGear;
    public UIButton btnHero;
    public UIButton btnvault;
    public UIButton btnRing;
    public UIButton btnArmor;
    public UIButton btnMedal;
    public UIButton btnLevel1_5;
    public UIButton btnLevel6_10;


    public UILabel lblGold;
    public UILabel lblSilver;

    public UILabel lblGoldRandomHero;
    public UILabel lblSilverRandomHero;

    public UILabel lblNotification;
    public UIShopItemReview uiItemReview;

    public GameObject defaultTab;
    public GameObject equipmentTab;
    public GameObject levelTab;

    public GameObject shopRoot;
    public UIRandomHeroManager randomHeroRoot;

    public GameObject reChargeRoot;
    public GameObject itemListRoot;
    public UIPlayTween openCardWindow;
    public UIPlayTween openCardInfo;
    public UIPlayTween closeCardInfo;
    public CardInfo cardInfo;
    public UIReChargeCard reChargeCard;

    public UITutorial uiTutorial;
    public ShopController Controller { get { return _controller; } }
    public ActiveTab activeTab { get { return _activeTab; } }

    private ActiveTab _activeTab;
    private List<GameObject> _itemsShowing = new List<GameObject>();
    private ShopController _controller;

    private bool _isBuyHero;

    public static bool BUYGOLD = false;

    void Start()
    {
        Localization();

        _controller = new ShopController(this);
        _controller.SendRequestRefreshGold();

        if (GameManager.Status == GameStatus.ShopHero)
        {
            shopRoot.SetActive(false);
            randomHeroRoot.gameObject.SetActive(true);
        }

        _activeTab = ActiveTab.None;

        _isBuyHero = true;
        setGoldSivler();

        if (BUYGOLD)
        {
            OnChangeTab(ActiveTab.Vault);
            BUYGOLD = false;
        }
        else
        {
            OnChangeTab(ActiveTab.Support);
        }
    }

    void OnDestroy()
    {
    }

    void Update()
    {
        // MyInput.CheckInput();
    }

    #region private methods
    private void setGoldSivler()
    {
        if (GameManager.GameUser != null)
        {
            lblGold.text = GameManager.GameUser.Base.Gold.ToString();
            lblSilver.text = GameManager.GameUser.Base.Silver.ToString();

            lblGoldRandomHero.text = GameManager.GameUser.Base.Gold.ToString();
            lblSilverRandomHero.text = GameManager.GameUser.Base.Silver.ToString();
        }
    }

    private void OnChangeTab(ActiveTab tab, bool changed = false)
    {
        if (MessageBox.isShow) return;

        if (!changed)
            if (tab == _activeTab) return;

        reChargeRoot.SetActive(false);
        itemListRoot.SetActive(true);

        _activeTab = tab;
        switch (_activeTab)
        {
            case ActiveTab.Support:
                LoadItem(ActiveTab.Support);
                break;
            case ActiveTab.Jewel:
                LoadItem(ActiveTab.Jewel);
                break;
            case ActiveTab.Energy:
                LoadItem(ActiveTab.Energy);
                break;
            case ActiveTab.Vault:
                reChargeRoot.SetActive(true);
                Transform transformCard = reChargeRoot.transform.FindChild("btnCard");
                Transform transformGooglePlay = reChargeRoot.transform.FindChild("btnIOS");

#if UNITY_ANDROID || UNITY_IOS

#else
                 transformGooglePlay.gameObject.SetActive(false);
                 transformCard.position = Vector3.zero;
#endif

                 if (Global.language != Global.Language.VIETNAM)
                 {
                     transformCard.gameObject.SetActive(false);
                     transformGooglePlay.position = Vector3.zero;
                 }

                itemListRoot.SetActive(false);
                break;
            case ActiveTab.IOSReCharge:
                LoadItem(ActiveTab.IOSReCharge);
                break;
        }
    }

    private void ClearOldItems()
    {
        foreach (GameObject go in _itemsShowing)
        {
            NGUITools.Destroy(go);
        }
        _itemsShowing.Clear();
    }

    private void LoadItem(ActiveTab type)
    {
        ClearOldItems();
        List<GameObj> shopItems;

        if (type == ActiveTab.Support)
            shopItems = GameManager.ChargeShop.Where(p => ((ShopItem)p).ItemKind == ItemKind.Support).ToList();
        else if (type == ActiveTab.Jewel)
            shopItems = GameManager.ChargeShop.Where(p => ((ShopItem)p).ItemKind == ItemKind.Material).ToList();
        else if (type == ActiveTab.Energy)
            shopItems = GameManager.ChargeShop.Where(p => ((ShopItem)p).ItemKind == ItemKind.Consume).ToList();
        else if (type == ActiveTab.IOSReCharge)
            shopItems = GameManager.ChargeShop.Where(p => ((ShopItem)p).ItemKind == ItemKind.Gold || ((ShopItem)p).ItemKind == ItemKind.Silver).ToList();
        else
            shopItems = GameManager.ChargeShop.ToList();

        int i = 0;
        foreach (ShopItem shopItem in shopItems)
        {

            GameObject go = NGUITools.AddChild(itemRoot, itemPrefab);
            _itemsShowing.Add(go);
            UIShopSlotItemManger itemManager = go.GetComponent<UIShopSlotItemManger>();
            itemManager.SetGameItem(shopItem, this);

            if (shopItem.UserLevel > GameManager.GameUser.Base.Level)
            {
                itemManager.OnDisable();
            }

            i++;
        }
        itemRoot.transform.parent.GetComponent<UIScrollView>().ResetPosition();
        itemRoot.GetComponent<UIGrid>().Reposition();
    }

    private void Localization()
    {
        L_Support.text = GameManager.localization.GetText("Shop_Support");
        L_Heroes.text = GameManager.localization.GetText("Shop_Energy");
        L_Vaults.text = GameManager.localization.GetText("Shop_Vaults");
        L_SellValue.text = GameManager.localization.GetText("Shop_SellValue");
        L_Jewel.text = GameManager.localization.GetText("Shop_Jewel");
        L_Jewel_White.text = GameManager.localization.GetText("Shop_Jewel_White");
        L_Jewel_Green.text = GameManager.localization.GetText("Shop_Jewel_Green");
        L_Jewel_Blue.text = GameManager.localization.GetText("Shop_Jewel_Blue");
        L_Jewel_Yellow.text = GameManager.localization.GetText("Shop_Jewel_Yellow");
        L_Back.text = GameManager.localization.GetText("Shop_Back");


        cardInfo.lblPrice.text = GameManager.localization.GetText("Shop_Card_Header");
        cardInfo.lblGold.text = GameManager.localization.GetText("Shop_Card_Gold");
        cardInfo.lblBonus.text = GameManager.localization.GetText("Shop_Card_Bonus");
        cardInfo.lblTotal.text = GameManager.localization.GetText("Shop_Card_Total");
        cardInfo.lblButtonNext.text = GameManager.localization.GetText("Shop_card_Next");

    }
    #endregion

    #region public

    public void SendBuyGoldWithVND(ChargeType chargeType, int ShopItemId, string productID, string token)
    {
        Controller.SendChargeCashPlayStore(chargeType, ShopItemId, productID, token);
    }

    public void SendChargeCashCard(CardType cardType, string series, string cardCode)
    {
        _controller.SendChargeCashCard(cardType, series, cardCode);
    }

    public void OnResponseCashCard(int cardValue, int goldValue, int silverAdd)
    {
        if (reChargeCard.gameObject.activeInHierarchy == true)
            reChargeCard.OnResponeFromServer(cardValue, goldValue);
        else
        {
            if (goldValue > 0)
                MessageBox.ShowDialog(string.Format(GameManager.localization.GetText("Shop_PaymentSucessGold"), goldValue), UINoticeManager.NoticeType.Message);
            if (silverAdd > 0)
                MessageBox.ShowDialog(string.Format(GameManager.localization.GetText("Shop_PaymentSucessSilver"), silverAdd), UINoticeManager.NoticeType.Message);

        }
        setGoldSivler();
    }

    public void OnRefreshGold()
    {
        setGoldSivler();
    }

    public void OnBuyItemResponse(ErrorCode errorCode)
    {
        Debug.Log("OnBuyItemResponse " + errorCode);
        MessageBox.CloseDialog();


        lblNotification.text = GameManager.localization.GetText("Shop_Success");
        setGoldSivler();

        if (!_isBuyHero)
        {
            //  UserItem itemBuy = GameManager.GameUser.UserItems[GameManager.GameUser.UserItems.Count - 1];
            //  Debug.Log("itemBuy" + itemBuy);
            //  uiItemReview.gameObject.SetActive(true);
            // uiInformationIcon.GetComponent<UIInformationItemManager>().SetItem(itemBuy);
            MessageBox.ShowDialog(GameManager.localization.GetText("Shop_Success"), UINoticeManager.NoticeType.Message);
        }
        else
        {
            UserRole roleBuy = GameManager.GameUser.UserRoles[GameManager.GameUser.UserRoles.Count - 1];
            randomHeroRoot.OnResponseFromServer(roleBuy);
        }


    }
    #endregion

    #region button
    public void OnButtonBuyItem_Click(ShopItem item, int shopMode)
    {

        if (MessageBox.isShow) return;

        if (GameManager.GameUser == null) return;

        //Check Gold
        if (shopMode == 2 && item.PriceGold > GameManager.GameUser.Base.Gold)
        {
            lblNotification.text = GameManager.localization.GetText("Shop_NotEnoughtGold");
            return;
        }

        //Check Silver
        if (shopMode == 1 && item.PriceSilver > GameManager.GameUser.Base.Silver)
        {
            lblNotification.text = GameManager.localization.GetText("Shop_NotEnoughtSilver");
            return;
        }

        //MessageBox.ShowDialog(TextScript.Dialog_Waiting, UINoticeManager.NoticeType.Waiting);

        if (item.ItemKind == ItemKind.Hero)
        {
            _isBuyHero = true;
            shopMode = 0;
        }
        else
        {
            _isBuyHero = false;
        }

        _controller.SendRequestBuyItem(item.Id, shopMode);

    }
    private void PrefreshTab()
    {
        btnSupport.normalSprite = "tieu_de_shop";
        btnGear.normalSprite = "tieu_de_shop";
        btnHero.normalSprite = "tieu_de_shop";
        btnvault.normalSprite = "tieu_de_shop";
        btnRing.normalSprite = "tieu_de_shop";
        btnArmor.normalSprite = "tieu_de_shop";
        btnMedal.normalSprite = "tieu_de_shop";
        btnLevel1_5.normalSprite = "tieu_de_shop";
        btnLevel6_10.normalSprite = "tieu_de_shop";

    }
    public void OnButtonSupport_Click()
    {

        if (MessageBox.isShow) return;

        PrefreshTab();
        btnSupport.normalSprite = "tieu_de_shop_down";
        OnChangeTab(ActiveTab.Support);
    }
    public void OnButtonGear_Click()
    {

        if (MessageBox.isShow) return;

        PrefreshTab();
        btnGear.normalSprite = "tieu_de_shop_down";
        OnChangeTab(ActiveTab.Jewel);

        //  defaultTab.SetActive(false);
        // equipmentTab.SetActive(true);
    }
    public void OnButtonHeroes_Click()
    {
        if (MessageBox.isShow) return;
        PrefreshTab();
        btnHero.normalSprite = "tieu_de_shop_down";
        OnChangeTab(ActiveTab.Energy);

        //  shopRoot.SetActive(false);
        //   randomHeroRoot.gameObject.SetActive(true);

        /*  PrefreshTab();
          btnHero.normalSprite = "tieu_de_shop_down";
          OnChangeTab(ActiveTab.Hero);*/
    }
    public void OnButtonVaults_Click()
    {

        if (MessageBox.isShow) return;

        PrefreshTab();
        btnvault.normalSprite = "tieu_de_shop_down";
        OnChangeTab(ActiveTab.Vault);
    }
    public void OnButtonWhite_Click()
    {
        PrefreshTab();
        btnRing.normalSprite = "tieu_de_shop_down";
        // _euipmentTab = ActiveTab.Ring;

        //   equipmentTab.SetActive(false);
        //    levelTab.SetActive(true);

        OnChangeTab(ActiveTab.Ring);
    }
    public void OnButtonGreen_Click()
    {
        PrefreshTab();
        btnArmor.normalSprite = "tieu_de_shop_down";

        //    equipmentTab.SetActive(false);
        //    levelTab.SetActive(true);

        //  _euipmentTab = ActiveTab.Armor;
        OnChangeTab(ActiveTab.Armor);
    }
    public void OnButtonBlue_Click()
    {
        PrefreshTab();
        btnMedal.normalSprite = "tieu_de_shop_down";

        //   equipmentTab.SetActive(false);
        //   levelTab.SetActive(true);

        // _euipmentTab = ActiveTab.Medal;
        OnChangeTab(ActiveTab.Medal);
    }
    public void OnButtonYellow_Click()
    {
        PrefreshTab();
        btnMedal.normalSprite = "tieu_de_shop_down";

        //   equipmentTab.SetActive(false);
        //   levelTab.SetActive(true);

        // _euipmentTab = ActiveTab.Medal;
        OnChangeTab(ActiveTab.Medal);
    }
    public void OnButtonBackDefault_Click()
    {
        PrefreshTab();
        btnSupport.normalSprite = "tieu_de_shop_down";
        OnChangeTab(ActiveTab.Support);


        defaultTab.SetActive(true);
        equipmentTab.SetActive(false);
    }
    public void OnButtonBuyGold_Click()
    {

        if (MessageBox.isShow) return;

    }
    public void OnButtonBack_Click()
    {

        if (MessageBox.isShow) return;

        GameScenes.ChangeScense(GameScenes.MyScene.ChargeShop, GameScenes.MyScene.WorldMap);
    }
    public void OnButtonHero_Click()
    {

        if (MessageBox.isShow) return;


        GameScenes.ChangeScense(GameScenes.MyScene.ChargeShop, GameScenes.MyScene.Hero);
    }
    public void OnButtonLobby_Click()
    {

        if (MessageBox.isShow) return;

        GameScenes.ChangeScense(GameScenes.MyScene.ChargeShop, GameScenes.MyScene.WorldMap);
    }
    public void OnButtonServive_Click()
    {

        if (MessageBox.isShow) return;

    }
    public void OnButtonLevel1_5_Click()
    {
        PrefreshTab();
        btnLevel1_5.normalSprite = "tieu_de_shop_down";
        //_levelTap = ActiveTab.Level1_5;
        // OnChangeTab(ActiveTab.Level1_5);
    }
    public void OnButtonLevel6_10_Click()
    {
        PrefreshTab();
        btnLevel6_10.normalSprite = "tieu_de_shop_down";
        //  _levelTap = ActiveTab.Level6_10;
        // OnChangeTab(ActiveTab.Level6_10);
    }
    public void OnButtonBackEquipment_Click()
    {
        PrefreshTab();
        btnRing.normalSprite = "tieu_de_shop_down";
        OnChangeTab(ActiveTab.Ring);


        defaultTab.SetActive(false);
        equipmentTab.SetActive(true);
        levelTab.SetActive(false);
    }
    public void OnButtonCloseRandomHero_Click()
    {


        UIRandomHeroManager.State state = randomHeroRoot.state;
        if (state == UIRandomHeroManager.State.Playing || state == UIRandomHeroManager.State.ReduceSpeed) return;

        if (GameScenes.previousSence == GameScenes.MyScene.WorldMap)
        {
            GameScenes.ChangeScense(GameScenes.MyScene.ChargeShop, GameScenes.MyScene.WorldMap);
        }
        else
        {
            shopRoot.SetActive(true);
            randomHeroRoot.gameObject.SetActive(false);
            OnChangeTab(ActiveTab.Support, true);
        }
        Resources.UnloadUnusedAssets();

    }
    public void OnBuyHero(int ItemID)
    {
        _controller.SendRequestBuyItem(ItemID, 0);
    }
    public void OnButtonIOS_Click()
    {

        if (MessageBox.isShow) return;

        PrefreshTab();
        btnvault.normalSprite = "tieu_de_shop_down";
        OnChangeTab(ActiveTab.IOSReCharge);
    }
    public void OnButtonCard_Click()
    {
        openCardInfo.Play(true);
    }
    public void OnButtonNextCard_Click()
    {

        closeCardInfo.Play(true);
        openCardWindow.Play(true);
    }
    #endregion


}
