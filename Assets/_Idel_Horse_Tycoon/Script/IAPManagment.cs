﻿using GameAnalyticsSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManagment : MonoBehaviour, IStoreListener
{

    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

#if UNITY_ANDROID
    public string[] packs = { "A_packna", "2_pack_special_2x_earning", "3_pack_personal_manager", "1_packgiftbox" };
#elif UNITY_IOS
    public string[] packs = { "A_packna", "2_pack_special_2x_earning", "3_pack_personal_manager", "1_packgiftbox" };
#endif


    public static string kProductIDNonConsumable = "nonconsumable";
    //public static string kProductIDSubscription = "subscription";
    //public static string Weekly = "Weekly";
    //public static string Monthly = "monthly";
    //public static string Yearly = "yearly";

    //// Apple App Store-specific product identifier for the subscription product.
    //private static string kProductNameAppleSubscription = "com.unity3d.subscription.new";

    //// Google Play Store-specific product identifier subscription product.
    //private static string WeeklyGooglePlaySubscription = "com.unity3d.subscription.Weekly";
    //private static string MonthlyGooglePlaySubscription = "com.unity3d.subscription.monthly";
    //private static string yearlyGooglePlaySubscription = "com.unity3d.subscription.yearly";

    void Start()
    {
        // If we haven't set up the Unity Purchasing reference
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }

    }

    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }

        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // Add a product to sell / restore by way of its identifier, associating the general identifier
        // with its store-specific identifiers.
        builder.AddProduct(packs[3], ProductType.Consumable);
        //  builder.AddProduct(packs[1], ProductType.Consumable);
        //  builder.AddProduct(packs[2], ProductType.Consumable);
        //  builder.AddProduct(packs[3], ProductType.Consumable);



        // Continue adding the non-consumable product.
        builder.AddProduct(packs[0], ProductType.NonConsumable);
        builder.AddProduct(packs[1], ProductType.NonConsumable);
        builder.AddProduct(packs[2], ProductType.NonConsumable);
        // And finish adding the subscription product. Notice this uses store-specific IDs, illustrating
        // if the Product ID was configured differently between Apple and Google stores. Also note that
        // one uses the general kProductIDSubscription handle inside the game - the store-specific IDs 
        // must only be referenced here. 

        //builder.AddProduct(Monthly, ProductType.Subscription, new IDs(){
        //        { kProductNameAppleSubscription, AppleAppStore.Name },
        //        { MonthlyGooglePlaySubscription, GooglePlay.Name },
        //    });
        //builder.AddProduct(Yearly, ProductType.Subscription, new IDs(){
        //        { kProductNameAppleSubscription, AppleAppStore.Name },
        //        { yearlyGooglePlaySubscription, GooglePlay.Name },
        //    });
        //builder.AddProduct(Weekly, ProductType.Subscription, new IDs(){
        //        { kProductNameAppleSubscription, AppleAppStore.Name },
        //        { WeeklyGooglePlaySubscription, GooglePlay.Name },
        //    });

        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);
    }


    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public string GetPrice(string productID)
    {
        if (m_StoreController == null) InitializePurchasing();
        return m_StoreController.products.WithID(productID).metadata.localizedPriceString;
    }


    public void BuyPack(int no)
    {
        // Buy the consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.

        BuyProductID(packs[no]);
    }

    public void BuyNoAds()
    {
        // Buy the non-consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        BuyProductID(packs[0]);
    }

    public void BuyNonConsumable()
    {
        // Buy the non-consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        BuyProductID(kProductIDNonConsumable);
    }


    //public void BuyWeeklySubscription()
    //{
    //    // Buy the subscription product using its the general identifier. Expect a response either 
    //    // through ProcessPurchase or OnPurchaseFailed asynchronously.
    //    // Notice how we use the general product identifier in spite of this ID being mapped to
    //    // custom store-specific identifiers above.
    //    BuyProductID(Weekly);
    //}
    //public void BuyMonthlySubscription()
    //{
    //    // Buy the subscription product using its the general identifier. Expect a response either 
    //    // through ProcessPurchase or OnPurchaseFailed asynchronously.
    //    // Notice how we use the general product identifier in spite of this ID being mapped to
    //    // custom store-specific identifiers above.
    //    BuyProductID(Monthly);
    //}
    //public void BuyYearlySubscription()
    //{
    //    // Buy the subscription product using its the general identifier. Expect a response either 
    //    // through ProcessPurchase or OnPurchaseFailed asynchronously.
    //    // Notice how we use the general product identifier in spite of this ID being mapped to
    //    // custom store-specific identifiers above.
    //    BuyProductID(Yearly);
    //}


    void BuyProductID(string productId)
    {
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = m_StoreController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase(product);
            }
            // Otherwise ...
            else
            {
                // ... report the product look-up failure situation  
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }


    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) =>
            {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }


    //  
    // --- IStoreListener
    //

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        // A consumable product has been purchased by this user.
        if (String.Equals(args.purchasedProduct.definition.id, packs[0], StringComparison.Ordinal))
        {


            Debug.Log(string.Format("ProcessPurchase: PASS." + packs[0], args.purchasedProduct.definition.id));
            // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.
        }
        else if (String.Equals(args.purchasedProduct.definition.id, packs[1], StringComparison.Ordinal))
        {

            TwoX_pnl.TwoXEarning = true;
            UiManager.inst.TwoX_Pnl.SetActive(false);
            UiManager.inst.Twox_IAP_btn.gameObject.SetActive(!TwoX_pnl.TwoXEarning);


            Debug.Log(string.Format("ProcessPurchase: PASS." + packs[1], args.purchasedProduct.definition.id));

            Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventEarnVirtualCurrency, new Firebase.Analytics.Parameter[] { new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterVirtualCurrencyName, "special_2x_earning"), });
            // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.

            Facebook.Unity.FB.LogAppEvent("special_2x_earning");
            GameAnalytics.SetCustomDimension03("special_2x_earning");
        }
        else if (String.Equals(args.purchasedProduct.definition.id, packs[2], StringComparison.Ordinal))
        {

            PersonalManager_pnl.PersonalManager = true;

            UiManager.inst.PrasnalManager_Btn.gameObject.SetActive(!PersonalManager_pnl.PersonalManager);

            if (UiManager.inst.WelComeBack_pnl.activeSelf)
                UiManager.inst.WelComeBack_pnl.GetComponent<Wecomeback_pnl>().Start();

            if (UiManager.inst.PrasnalManager_pnl.activeSelf)
                UiManager.inst.PrasnalManager_pnl.SetActive(false);



            Debug.Log(string.Format("ProcessPurchase: PASS." + packs[2], args.purchasedProduct.definition.id));
            // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.

            Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventEarnVirtualCurrency, new Firebase.Analytics.Parameter[] { new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterVirtualCurrencyName, "Personal_Manager"), });
            Facebook.Unity.FB.LogAppEvent("Personal_Manager");
            GameAnalytics.SetCustomDimension03("Personal_Manager");
        }
        else if (String.Equals(args.purchasedProduct.definition.id, packs[3], StringComparison.Ordinal))
        {

            GameManager.GiftBoxCounter += 4;

            UiManager.inst.GiftBox_Pnl.GetComponent<GiftBox_pnl>().OnEnable();
            //UiManager.inst.GiftBox_Pnl.SetActive(false);

            Debug.Log(string.Format("ProcessPurchase: PASS." + packs[3], args.purchasedProduct.definition.id));
            // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.
            Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventEarnVirtualCurrency, new Firebase.Analytics.Parameter[] { new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterVirtualCurrencyName, "Gift_Box"), });
            Facebook.Unity.FB.LogAppEvent("Gift_Box");
            GameAnalytics.SetCustomDimension03("Gift_Box");
        }
        else
        {
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
        }

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Complete;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }
}