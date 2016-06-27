using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;
using DEngine.Common;
using System.Linq;

public class UIShopSlotItemManger : MonoBehaviour {

    public UILabel lblName;
    public UIButton PriceGoldButton;
    public UIButton PriceSilverButton;
    public UITexture IconOfItem;
    public UILabel LblGoldPrice;
    public UILabel LblSilverPrice;
    public GameObject uiDisable;
    public UILabel lblLevel;
    public UIButton btnInformation;

    public ShopItem shopItem;
    public UIShopManager uiShopManager { get; set; }

    private int _goldPrice;
    public int goldPrice
    {
        get
        {
            return _goldPrice;
        }
        set
        {
            _goldPrice = value;
            LblGoldPrice.text = _goldPrice.ToString();

            if (_goldPrice <= 0)
            {
                PriceGoldButton.gameObject.SetActive(false);
            }
        }
    }

    private int _silverPrice;
    public int silverPrice
    {
        get
        {
            return _silverPrice;
        }
        set
        {
            _silverPrice = value;
            LblSilverPrice.text = _silverPrice.ToString();

            if (_silverPrice <= 0)
            {
                PriceSilverButton.gameObject.SetActive(false);
            }
        }
    }

    public GameObject itemPriceRoot;
    public GameObject vaultRoot;
    public GameObject bonusRoot;
    public UILabel lblBonus;
    public UILabel lblVaultVND;
    public UILabel lblVaultGold;

    private bool _disable;
    private int _shopMode;
    UIPlayTween _playTween;

    void Start()
    {
        _shopMode = 0;

    }

    public void SetGameItem(ShopItem item, UIShopManager manager)
    {
        uiShopManager = manager;
        shopItem = item;
        goldPrice = shopItem.PriceGold;
        silverPrice = shopItem.PriceSilver;

        lblName.text = GameManager.localization.getItem(item.ItemId).Name;

        _playTween = btnInformation.GetComponent<UIPlayTween>();

        
        _playTween.tweenTarget = uiShopManager.uiItemReview.gameObject;
        btnInformation.gameObject.SetActive(true);
        IconOfItem.mainTexture = Helper.LoadTextureForSupportItem(item.ItemId);
        IconOfItem.SetDimensions(120, 120);
        if (item.UserLevel > 0)
            lblLevel.text = string.Format(GameManager.localization.GetText("Shop_MinLevel"), item.UserLevel, item.UserLevel + 4);
                
        _disable = false;

        if (manager.activeTab == UIShopManager.ActiveTab.IOSReCharge)
        {
            itemPriceRoot.SetActive(false);
            vaultRoot.SetActive(true);

            float VNDPrice = 0;

            if (Global.language == Global.Language.VIETNAM)
            {
                VNDPrice = item.PriceVND;
            }
            else
            {
                VNDPrice = item.PriceUSD;
            }

            if (item.PriceGoldSale > 0)
                lblVaultGold.text = string.Format(GameManager.localization.GetText("Shop_GoldFormat"), item.PriceGoldSale);
            else if (item.PriceSilverSale > 0)
                lblVaultGold.text = string.Format(GameManager.localization.GetText("Shop_SilverFormat"), item.PriceSilverSale);

            lblVaultVND.text = string.Format(GameManager.localization.GetText("Shop_VNDFormat"), VNDPrice);

            if (item.Promotion > 0)
            {
                bonusRoot.SetActive(true);
                lblBonus.text = string.Format(GameManager.localization.GetText("Shop_GoldFormat"), item.Promotion);

            }
        }
        else
        {
            itemPriceRoot.SetActive(true);
            vaultRoot.SetActive(false);
        }



    }

    #region Public methods
    public void OnBuyVND_Click()
    {
        if (_disable) return;

        UINoticeManager.OnButtonOK_click += new UINoticeManager.NoticeHandle(HadleAcceptBuyVND);
        string s = string.Format(GameManager.localization.GetText("Shop_Question_BuyVND"), shopItem.Name);
        MessageBox.ShowDialog(s, UINoticeManager.NoticeType.YesNo);
    }
    
    public void OnButtonBuyGold_Click()
    {
        if (_disable) return;
        CheckBuyGoldSilver();
       // HandleBuyItem(2);
       // uiShopManager.OnButtonBuyItem_Click(shopItem, 2);
    }
    
    public void OnButtonBuySilver_Click()
    {
        if (_disable) return;
        CheckBuyGoldSilver();
       // HandleBuyItem(1);
      //  uiShopManager.OnButtonBuyItem_Click(shopItem, 1);
    }

    public void OnButtonInformation_Click()
    {
        uiShopManager.uiItemReview.ShowItem(shopItem, IconOfItem.mainTexture);
    }
    #endregion

    public void OnDisable()
    {
        _disable = true;
        uiDisable.SetActive(true);
    }
    
    private void CheckBuyGoldSilver()
    {
        if (shopItem.PriceGold > 0)
        {
            HandleBuyItem(2);
        }
        else if (shopItem.PriceSilver > 0)
        {
            HandleBuyItem(1);
        }
    }

    private void HadleAcceptBuyVND()
    {       
        PaymentManager.OnResponseUniBill += OnResponeUniPill;
        PaymentManager.Instance.Buy(shopItem);
        
    }

    private void OnResponeUniPill(string productID, string token)
    {
     //   if (PaymentManager.OnResponseUniBill)
      //      PaymentManager.OnResponseUniBill -= OnResponeUniPill;

        if (productID == "" || token == "")
        {
            MessageBox.ShowDialog(GameManager.localization.GetText("ErrorCode_Payment_Error"), UINoticeManager.NoticeType.Message);
            return;
        }
#if UNITY_ANDROID
        uiShopManager.SendBuyGoldWithVND(ChargeType.GoogleStore, shopItem.Id, productID, token);
#elif UNITY_IOS
        uiShopManager.SendBuyGoldWithVND(ChargeType.AppleStore, shopItem.Id, productID, token);
#endif

    }

    private void HandleBuyItem(int shopMode)
    {
        if (shopItem.ItemKind == ItemKind.Hero)
        {
            if (GameManager.GameUser.UserRoles.Count >= GameManager.maxSlotHero)
            {
                MessageBox.ShowDialog(string.Format(GameManager.localization.GetText("Shop_MaxHeroSlot"), GameManager.maxSlotHero), UINoticeManager.NoticeType.Message);
                return;
            }
        }

        _shopMode = shopMode;

        UINoticeManager.OnButtonOK_click += new UINoticeManager.NoticeHandle(HadleAccept);

        string priceType = "";
        int price = 0;
        if (_shopMode == 1)
        {
            priceType = "silver";
            price = shopItem.PriceSilver;
        }
        else
        {
            priceType = "gold";
            price = shopItem.PriceGold;
        }

        string s = string.Format(GameManager.localization.GetText("Shop_Question_Buy"), price, priceType, shopItem.Name);
        MessageBox.ShowDialog(s, UINoticeManager.NoticeType.YesNo);
    }

    private void HadleAccept()
    {
        uiShopManager.OnButtonBuyItem_Click(shopItem, _shopMode);
    }
}
