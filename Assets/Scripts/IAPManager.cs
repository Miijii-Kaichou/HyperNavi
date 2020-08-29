using System;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviour, IStoreListener
{
    private static IStoreController m_StoreSystem;

    private static IExtensionProvider m_StoreExtensionProvider;

    //Products In the Game which includes
    //Skins, Game Color Themse, and Coins
    //But for now, we'll worry about coins.
    private string Get1000Coins = "Get_1000_Coins";
    private string Get5000Coins = "Get_5000_Coins";
    private string Get10000Coins = "Get_10000_Coins";

    void Start()
    {
        //If we having initalized the purchasing system yet
        if (m_StoreSystem == null)
        {
            // Configure connection to Purchasing
            InitializePurchasing();
        }
    }


    /// <summary>
    /// Initialize Unity's Purchasing System
    /// </summary>
    public void InitializePurchasing()
    {
        //Don't execute the method if we've already initialized the system
        if (IsInitialized()) return;

        //Create a builder, first passing in a suite of Unity Provided Stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        //Add a product to sell or restore by way of its identifier 
        builder.AddProduct(Get1000Coins, ProductType.Consumable);
        builder.AddProduct(Get5000Coins, ProductType.Consumable);
        builder.AddProduct(Get10000Coins, ProductType.Consumable);

        //Then we should expect a respont with OnInitialized or OnInitalizedFailed
        UnityPurchasing.Initialize(this, builder);
    }

    public bool IsInitialized() => m_StoreSystem != null;

  

    //***************** We now create our methods here!!! ******************
    public void Buy1000Coins()
    {

    }

    public void Buy5000Coins()
    {

    }

    public void Buy10000Coins()
    {

    }

    //TODO: Be sure to set up these products for these methods, and add it to the builder
    public void BuyTheme(int index)
    {

    }

    public void BuyPlayerIcon(int index)
    {

    }

    void BuyProductId(string productId)
    {
        //Check if system has been initialized
        if (IsInitialized())
        {
            /* Look up the Product reference with the general prduct identifier and the 
             * Purchasing
             */

            //This will pull up all the Products we adding in during initialization
            //and then pull out one of the Products from that list
            Product product = m_StoreSystem.products.WithID(productId);

            /*If we found the product with the specified product ID, and 
             if it's ready to be sold*/
            if (product != null && product.availableToPurchase)
            {
#if UNITY_EDITOR
                Debug.Log(string.Format("Purchasing product asychronously: {0}", product.definition.id));
#endif
                //Buy the product, expect a response through ProcessPurchase or OnPurchaseFailed
                //Asynchronously purchase product
                m_StoreSystem.InitiatePurchase(product);
            }
            //Otherwise...
            else
            {
#if UNITY_EDITOR
                Debug.Log("BuyProductID: FAILED. Not purchasing product, either it is," +
                    "not found, or is not avaliable for purchase.");
#endif
            }
        }
        //If not initailized however...
        else
        {
#if UNITY_EDITOR
            //Has not initalized
            Debug.Log("BuyProductID FAIL. Not initialzed");
#endif
        }
    }

    /// <summary>
    /// Restore purcahses previous made by this customer. Some platforms automatically restore
    /// purchases like Google.
    /// However, Apply currently requires explicit purchase restoration from IAP, conditionally displaying a 
    /// password prompt.
    /// </summary>
    public void RestorePurchases()
    {
        //If Purchasing hasn't been initialzed
        if (!IsInitialized())
        {
#if UNITY_EDITOR
            //Report that it hasn't initialized and stop.
            Debug.Log("RestorePurchases FAIL. Not Initialized.");
#endif //UNITY_EDITOR
        }
        //Otherwise, we move on by checking if on an Apple Device
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
           Application.platform == RuntimePlatform.OSXPlayer)
        {
            //Begin restoration process

            //Fetch the Apple Store-Specific SubSystem
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();

            /*Begin the asynchronous process of restoring purchases.
             Expect a confirmation respose in the Action<bool> below, and Process Purchase 
            if there are previously purchased products to restore.*/
            apple.RestoreTransactions((result) =>
            {
                /*First phase is restoration. 
                 If no responses, then no purchases are avaliable to restore.*/
#if UNITY_EDITOR
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages," +
                    "no purchases avaliable to restore");
#endif //UNITY_EDITOR
            });
        }

        //Otherwise...
        else
        {
            /*We are not running on an Apple Device. No work or additional 
             code is neccesary.*/
#if UNITY_EDITOR
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = "
                + Application.platform);
#endif //UNITY_EDITOR
        }
    }


    /// <summary>
    /// IStoreListener
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="extensions"></param>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {

        /*Purhcassing has succeeded initializing. 
         * Collect ourPurchasing references.*/
#if UNITY_EDITOR
        Debug.Log("OnInitialized: PASS");
#endif //UNITY_EDITOR

        /*Overall Purchasing system, configured with products for this 
         application*/
        m_StoreSystem = controller;

        /*Store specific subsytem, for accessing device-specific 
         * store features.*/
        m_StoreExtensionProvider = extensions;
    }

    /// <summary>
    /// On the failing of Purchasing Initialization
    /// </summary>
    /// <param name="error"></param>
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        /*Purchasing set-up has not succeeded. Check error for reson.
         Consider sharing this reason with the user.*/
#if UNITY_EDITOR
        Debug.Log("OnINitializeFailed InitializationFailureReson:" + error);
#endif //UNITY_EDITOR
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="i"></param>
    /// <param name="p"></param>
    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        /*A product purchase attempt did not scceed. Check failureReason for more detail.
         Consider sharing this reason with the user to guide their troubleshooting actions.*/
#if UNITY_EDITOR
        Debug.Log(string.Format("OnPurchasedFailed: FAIL. Product: '{0}'," +
            "PurchaseFailureReason: {1}", product.definition.storeSpecificId, reason));
#endif //UNITY_EDITOR
    }


    /// <summary>
    /// Processing a purchase
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        //We set all of our products in here, and check the argument

        /*A consumable product has been purchased
         by this user*/
        if (string.Equals(args.purchasedProduct.definition.id, Get1000Coins, StringComparison.Ordinal))
        {
#if UNITY_EDITOR
            Debug.Log(string.Format("ProcessPurchase: PASS. Product:"));
#endif //UNITY_EDITOR

            /*1000 Gems has succesfully been purchased. Add 1000 Gems to the player.*/
            CurrencySystem.AddToBalance(1000);
        }
        else if (string.Equals(args.purchasedProduct.definition.id, Get1000Coins, StringComparison.Ordinal))
        {
#if UNITY_EDITOR
            Debug.Log(string.Format("ProcessPurchase: PASS. Product:"));
#endif //UNITY_EDITOR

            /*1000 Gems has succesfully been purchased. Add 1000 Gems to the player.*/
            CurrencySystem.AddToBalance(1000);
        }
        else if (string.Equals(args.purchasedProduct.definition.id, Get1000Coins, StringComparison.Ordinal))
        {
#if UNITY_EDITOR
            Debug.Log(string.Format("ProcessPurchase: PASS. Product:"));
#endif //UNITY_EDITOR

            /*1000 Gems has succesfully been purchased. Add 1000 Gems to the player.*/
            CurrencySystem.AddToBalance(1000);
        }
        //At this point. We had failed to go through the purchasing process.
        else
        {
#if UNITY_EDITOR
            Debug.Log(string.Format(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id)));
#endif //UNITY_EDITOR
        }

        /*Return a flag indicating whether this product has completely been
         received, or if the application need to be reminded of thise puchase at next app
        launch. Use PurchaseProcessingResult.Pending when still
        saving purchased prodcuts to the cloud, and when that save is delayed.*/
        return PurchaseProcessingResult.Complete;
    }
}
