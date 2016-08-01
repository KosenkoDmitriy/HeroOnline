using UnityEngine;
using System.Collections;
using System.IO;
using Unibill.Impl;
using System.Linq;
using DEngine.Common.GameLogic;
using System.Collections.Generic;
using SimpleJSON;

public class PaymentManager : MonoBehaviour {

    private PurchasableItem[] items;

    public static PaymentManager Instance;

    void Start()
    {
        Init();
        Instance = this;
    }

    public void Init()
    {
        if (UnityEngine.Resources.Load("unibillInventory.json") == null)
        {
            Debug.LogError("You must define your purchasable inventory within the inventory editor!");
            return;
        }

        // We must first hook up listeners to Unibill's events.
        Unibiller.onBillerReady += onBillerReady;
        Unibiller.onTransactionsRestored += onTransactionsRestored;
        Unibiller.onPurchaseCancelled += onCancelled;
        Unibiller.onPurchaseFailed += onFailed;
        Unibiller.onPurchaseCompleteEvent += onPurchased;
        Unibiller.onPurchaseDeferred += onDeferred;
        Unibiller.onDownloadProgressedEvent += (item, progress) =>
        {
            Debug.Log(item.name + " " + progress);
        };

        Unibiller.onDownloadFailedEvent += (arg1, arg2) =>
        {
            Debug.LogError(arg2);
        };

        Unibiller.onDownloadCompletedEventString += (obj, dir) =>
        {
            Debug.Log("Completed download: " + obj.name);
#if !(UNITY_WP8 || UNITY_METRO || UNITY_WEBPLAYER)
            foreach (var f in new DirectoryInfo(dir).GetFiles())
            {
                Debug.Log(f.Name);
                if (f.Name.EndsWith("txt") && f.Length < 10000)
                {
#if !(UNITY_WP8 || UNITY_METRO || UNITY_WEBPLAYER)
                    Debug.Log(Util.ReadAllText(f.FullName));
#endif
                }
            }
#endif
        };

        // Now we're ready to initialise Unibill.
        Unibiller.Initialise();

        items = Unibiller.AllPurchasableItems;
    }

    public delegate void HanderResponeUniBill(string productID, string token);
    public static event HanderResponeUniBill OnResponseUniBill;

    public void Buy(ShopItem item)
    {
        PurchasableItem itemBuy = items.FirstOrDefault(p => p.Id == Helper.ShopItemToPurchasableItemID[item.Id]);
        if (itemBuy != null)
            Unibiller.initiatePurchase(itemBuy);
        else
        {
            Debug.Log("Null " + item.Id);
        }
    }

    private void onBillerReady(UnibillState state)
    {
        UnityEngine.Debug.Log("onBillerReady:" + state);
    }

    /// <summary>
    /// This will be called after a call to Unibiller.restoreTransactions().
    /// </summary>
    private void onTransactionsRestored(bool success)
    {
        Debug.Log("Transactions restored.");
    }

    /// <summary>
    /// This will be called when a purchase completes.
    /// </summary>
    private void onPurchased(PurchaseEvent e)
    {
        Debug.Log("Purchase OK: " + e.PurchasedItem.Id);
        Debug.Log("Receipt: " + e.Receipt);
        Debug.Log(string.Format("{0} has now been purchased {1} times.",
            e.PurchasedItem.name, Unibiller.GetPurchaseCount(e.PurchasedItem)));
               

        try
        {

#if UNITY_ANDROID
            var jsonNode = JSON.Parse(e.Receipt);
            jsonNode = JSON.Parse(jsonNode[0].Value);

            string purchaseState = jsonNode["purchaseState"].Value;
            string productId = jsonNode["productId"].Value;
            string purchaseToken = jsonNode["purchaseToken"].Value;

            if (purchaseState == "0")
            {
                if (OnResponseUniBill != null)
                {
                    OnResponseUniBill.Invoke(productId, purchaseToken);
                    OnResponseUniBill -= OnResponseUniBill;
                }
            }
#elif UNITY_IOS
            if (OnResponseUniBill != null)
            {
                OnResponseUniBill.Invoke(e.PurchasedItem.Id, e.Receipt);
                OnResponseUniBill -= OnResponseUniBill;
            }
#endif
        }
        catch
        {
            if (OnResponseUniBill != null)
            {
                OnResponseUniBill.Invoke("", "");
                OnResponseUniBill -= OnResponseUniBill;
            }
        }
                
    }

    /// <summary>
    /// This will be called if a user opts to cancel a purchase
    /// after going to the billing system's purchase menu.
    /// </summary>
    private void onCancelled(PurchasableItem item)
    {
        Debug.Log("Purchase cancelled: " + item.Id);
    }

    /// <summary>
    /// iOS Specific.
    /// This is called as part of Apple's 'Ask to buy' functionality,
    /// when a purchase is requested by a minor and referred to a parent
    /// for approval.
    /// 
    /// When the purchase is approved or rejected, the normal purchase events
    /// will fire.
    /// </summary>
    /// <param name="item">Item.</param>
    private void onDeferred(PurchasableItem item)
    {
        Debug.Log("Purchase deferred blud: " + item.Id);
    }

    /// <summary>
    /// This will be called is an attempted purchase fails.
    /// </summary>
    private void onFailed(PurchasableItem item)
    {
        Debug.Log("Purchase failed: " + item.Id);
    }



}
