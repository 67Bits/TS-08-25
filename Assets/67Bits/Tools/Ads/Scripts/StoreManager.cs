using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing;
using System.Collections;
using System;

public class StoreManager : Singleton<StoreManager>, IDetailedStoreListener
{
    public StoreProduct noAds;
    public UnityEvent onCompletePurchaseNoAdsEvent = new UnityEvent();


    private IStoreController storeController;
    private IExtensionProvider extensionProvider;

    protected override void Awake()
    {
        base.Awake();
        if (storeController == null)
        {
            InitializePurchasing();
        }
    }

    private void InitializePurchasing()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        var gpConfig = builder.Configure<IGooglePlayConfiguration>();

        //Add no Ads product
        builder.AddProduct(noAds.productId, ProductType.NonConsumable);

        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
        extensionProvider = extensions;

        // No Ads 
        var noAdsProduct = controller.products.WithID(noAds.productId);
        if (noAdsProduct != null)
            noAds.SetIAPProduct(noAdsProduct);


        Debug.Log("IAP initialized successfully");
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError($"IAP initialization failed: {error}");
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError($"IAP initialization failed: {error} - {message}");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.LogError($"Purchase failed: {product.definition.id}, {failureDescription}");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogError($"Purchase failed: {product.definition.id}, {failureReason}");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        var product = args.purchasedProduct;
        string productId = product.definition.id;

        try
        {
            StoreProduct purchasedProduct = FindProductInLists(productId);

            if (purchasedProduct != null)
            {

                // Rest of your existing logic...
                switch (purchasedProduct.productType)
                {
                    case ProductType.Consumable:
                        Debug.Log($"Consumable product purchased: {purchasedProduct.productTitle}");
                        break;
                    case ProductType.NonConsumable:
                        Debug.Log($"Non-consumable product unlocked: {purchasedProduct.productTitle}");
                        break;
                    case ProductType.Subscription:
                        Debug.Log($"Subscription activated: {purchasedProduct.productTitle}");
                        break;
                }

                InvokeCompletionEvent(productId, purchasedProduct);

                return PurchaseProcessingResult.Complete;
            }
            else
            {
                Debug.LogWarning($"Purchase completed for unknown product: {productId}");
                return PurchaseProcessingResult.Complete;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error processing purchase {productId}: {e}");
            return PurchaseProcessingResult.Complete;
        }
    }

    private StoreProduct FindProductInLists(string productId)
    {
        // Search through all product lists
        if (productId == noAds.productId) return noAds;

        return null;
    }

    public void BuyProduct(string productId)
    {
        if (storeController == null)
        {
            Debug.LogError("Store not initialized");
            return;
        }

        Product product = storeController.products.WithID(productId);

        if (product != null && product.availableToPurchase)
        {
            storeController.InitiatePurchase(product);
        }
        else
        {
            Debug.LogError($"Product {productId} not available");
        }
    }

    private void InvokeCompletionEvent(string productId, StoreProduct product)
    {
        if (productId == noAds.productId)
        {
            onCompletePurchaseNoAdsEvent.Invoke();
        }

        SaveSystem.SaveGame();
    }

    public IEnumerator RestorePurchaseAfterInitialize()
    {
        yield return new WaitUntil(() => storeController != null);
        if (storeController != null)
        {
            RestorePurchase();
        }
        yield break;
    }

    public void RestorePurchase()
    {
        if (!SaveData.Instance.HasRestorePurchase)
        {
            if (noAds.HasBeenPurchased()) onCompletePurchaseNoAdsEvent.Invoke();
            SaveData.Instance.HasRestorePurchase = true;
        }
    }

    [System.Serializable]
    public class StoreProduct
    {
        // Basic Info
        public string productId;
        public string productTitle;
        public string productDescription;
        public ProductType productType;

        // Runtime Info
        [HideInInspector] public string localizedPriceString;
        [HideInInspector] public Product unityIapProduct;

        public void SetIAPProduct(Product product)
        {
            unityIapProduct = product;
            if (unityIapProduct != null)
            {
                localizedPriceString = unityIapProduct.metadata.localizedPriceString;
            }
        }

        public bool HasBeenPurchased()
        {
            if (unityIapProduct == null)
            {
                Debug.LogWarning($"Product {productId} not initialized in Unity IAP");
                return false;
            }
            return unityIapProduct.hasReceipt;
        }

        public bool IsAvailableToPurchase()
        {
            return unityIapProduct != null && unityIapProduct.availableToPurchase;
        }
    }
}
