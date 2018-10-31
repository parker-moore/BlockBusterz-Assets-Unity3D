using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
//*******************************
//How to use script:
//  Fill out all product ID's before testing (productID can be found in ItunesConnect under IAP section for this particular game)
//  REMEMBER to fill out product ID's for each item with the script "SelectProduct"
//*******************************


// Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
public class IAPManager : MonoBehaviour, IStoreListener
{
    public static IAPManager Instance { set; get; }

    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

    public string[] NONCONSUMABLE_PRODUCT_ID;//set all non consumable product id's here
    public string[] CONSUMABLE_PRODUCT_ID;//set all consumable product id's here
    public string[] SUBSCRIPTION_PRODUCT_ID;//set all subscription product id's here
    private int SELECTED_PRODUCT_ID;

    void Start()
    {
        Instance = this;

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

        //initilize nonconsumable products
        if (NONCONSUMABLE_PRODUCT_ID.Length > 0)
        {
            for (int i = 0; i < NONCONSUMABLE_PRODUCT_ID.Length; i++)
            {
                builder.AddProduct(NONCONSUMABLE_PRODUCT_ID[i], ProductType.NonConsumable);
            }
        }

        //initilize Consumable products
        if (CONSUMABLE_PRODUCT_ID.Length > 0)
        {
            for (int i = 0; i < CONSUMABLE_PRODUCT_ID.Length; i++)
            {
                builder.AddProduct(CONSUMABLE_PRODUCT_ID[i], ProductType.Consumable);
            }
        }

        //initilize Subscription products
        if (SUBSCRIPTION_PRODUCT_ID.Length > 0)
        {
            for (int i = 0; i < SUBSCRIPTION_PRODUCT_ID.Length; i++)
            {
                builder.AddProduct(SUBSCRIPTION_PRODUCT_ID[i], ProductType.Subscription);
            }
        }
        UnityPurchasing.Initialize(this, builder);
    }

    private bool IsInitialized()
    {
        //Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public void BuyNonConsumable(string nc_productID)
    {
        if (NONCONSUMABLE_PRODUCT_ID.Length > 0)
        {
            for (int i = 0; i < NONCONSUMABLE_PRODUCT_ID.Length; i++)
            {
                if (NONCONSUMABLE_PRODUCT_ID[i] == nc_productID)
                {
                    SELECTED_PRODUCT_ID = i;
                    i = NONCONSUMABLE_PRODUCT_ID.Length;
                }

            }
        }
        BuyProductID(nc_productID);
    }

    public void BuyConsumable(string c_productID)
    {
        if (CONSUMABLE_PRODUCT_ID.Length > 0)
        {
            for (int i = 0; i < CONSUMABLE_PRODUCT_ID.Length; i++)
            {
                if (CONSUMABLE_PRODUCT_ID[i] == c_productID)
                {
                    SELECTED_PRODUCT_ID = i;
                    i = CONSUMABLE_PRODUCT_ID.Length;
                }

            }
        }
        BuyProductID(c_productID);
    }

    public void BuySubscription(string s_productID)
    {
        if (SUBSCRIPTION_PRODUCT_ID.Length > 0)
        {
            for (int i = 0; i < SUBSCRIPTION_PRODUCT_ID.Length; i++)
            {
                if (SUBSCRIPTION_PRODUCT_ID[i] == s_productID)
                {
                    SELECTED_PRODUCT_ID = i;
                    i = SUBSCRIPTION_PRODUCT_ID.Length;
                }

            }
        }
        BuyProductID(s_productID);
    }


    private void BuyProductID(string productId)
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

        if (String.Equals(args.purchasedProduct.definition.id, NONCONSUMABLE_PRODUCT_ID[SELECTED_PRODUCT_ID], StringComparison.Ordinal))
        {
            //set item's product id to true (use to track if that item is unlocked)
            Debug.Log("purchase of " + NONCONSUMABLE_PRODUCT_ID[SELECTED_PRODUCT_ID] + " successful");

            if (NONCONSUMABLE_PRODUCT_ID[SELECTED_PRODUCT_ID] == "blockbusters.1000gems")
            {
                PlayerPrefManager.SetCoin(PlayerPrefManager.GetCoins() + 1000);
                GameManager.gm.coinsCollected = PlayerPrefManager.GetCoins();
                GameManager.gm.AddCoin(0); // updates coin count on screen
            }

            if (NONCONSUMABLE_PRODUCT_ID[SELECTED_PRODUCT_ID] == "blockbusterz.noads")
            {
                PlayerPrefManager.SetNoAdsPurchased(true);
                MainMenuManager.instance.purchaseNoAdsButton.SetActive(false);
            }


            PlayerPrefManager.SetBool(NONCONSUMABLE_PRODUCT_ID[SELECTED_PRODUCT_ID], true);
            MainMenuManager.instance.HideAdButton();
        }
        else if (String.Equals(args.purchasedProduct.definition.id, CONSUMABLE_PRODUCT_ID[SELECTED_PRODUCT_ID], StringComparison.Ordinal))
        {
            //set item's product id to true (use to track if that item is unlocked)  
        }
        else if (String.Equals(args.purchasedProduct.definition.id, SUBSCRIPTION_PRODUCT_ID[SELECTED_PRODUCT_ID], StringComparison.Ordinal))
        {
            //set item's product id to true (use to track if that item is unlocked)  
        }
        else
        {
            Debug.Log("Purchase Failed");
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
