using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class InAppPurchaser : MonoBehaviour, IStoreListener
{
    private static IStoreController storeController;
    private static IExtensionProvider extensionProvider;

    #region 상품ID
    // 상품ID는 구글 개발자 콘솔에 등록한 상품ID와 동일하게 해주세요.
    public class Pentatiles
    {
		public const string RemoveAds = "pentatiles.removeads";
		public const string Hint_110 =  "pentatiles.hint.110";
		public const string Hint_55 =   "pentatiles.hint.55";
		public const string Hint_10 =   "pentatiles.hint.10";
        public class Google
        {
            public const string RemoveAds = "pentatiles.google.removeads";
			public const string Hint_110 =  "pentatiles.google.hint.110";
			public const string Hint_55 =   "pentatiles.google.hint.55";
			public const string Hint_10 =   "pentatiles.google.hint.10";

        }
		public class Apple
		{
			public const string RemoveAds = "pentatiles.apple.removeads";
			public const string Hint_110 = "pentatiles.apple.hint.110";
			public const string Hint_55 = "pentatiles.apple.hint.55";
			public const string Hint_10 = "pentatiles.apple.hint.10";
		}
    }
    #endregion

    void Start()
    {
        InitializePurchasing();
    }

    private bool IsInitialized()
    {
        return (storeController != null && extensionProvider != null);
    }

    public void InitializePurchasing()
    {
        if (IsInitialized())
            return;

        var module = StandardPurchasingModule.Instance();

        ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);

		builder.AddProduct(Pentatiles.RemoveAds, ProductType.Consumable, new IDs {
			{ Pentatiles.Google.RemoveAds, GooglePlay.Name },
			{ Pentatiles.Apple.RemoveAds, AppleAppStore.Name}
        });
			
		builder.AddProduct(Pentatiles.Hint_110, ProductType.Consumable, new IDs {
			{ Pentatiles.Google.Hint_110, GooglePlay.Name },
			{ Pentatiles.Apple.Hint_110, AppleAppStore.Name }
		});

		builder.AddProduct(Pentatiles.Hint_55, ProductType.Consumable, new IDs {
			{ Pentatiles.Google.Hint_55, GooglePlay.Name },
			{ Pentatiles.Apple.Hint_55, AppleAppStore.Name }
		});

		builder.AddProduct(Pentatiles.Hint_10, ProductType.Consumable, new IDs {
			{ Pentatiles.Google.Hint_10, GooglePlay.Name },
			{ Pentatiles.Apple.Hint_10, AppleAppStore.Name }
		});


        UnityPurchasing.Initialize(this, builder);
    }

    public void BuyProductID(string productId)
    {
        try
        {
            if (IsInitialized())
            {
                Product p = storeController.products.WithID(productId);

                if (p != null && p.availableToPurchase)
                {
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", p.definition.id));
                    storeController.InitiatePurchase(p);
                }
                else
                {
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            else
            {
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }
        catch (Exception e)
        {
            Debug.Log("BuyProductID: FAIL. Exception during purchase. " + e);
        }
    }

    public void RestorePurchase()
    {
        if (!IsInitialized())
        {
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
        {
            Debug.Log("RestorePurchases started ...");

            var apple = extensionProvider.GetExtension<IAppleExtensions>();

            apple.RestoreTransactions
                (
                    (result) => { Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore."); }
                );
        }
        else
        {
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }

    public void OnInitialized(IStoreController sc, IExtensionProvider ep)
    {
        Debug.Log("OnInitialized : PASS");

        storeController = sc;
        extensionProvider = ep;
    }

    public void OnInitializeFailed(InitializationFailureReason reason)
    {
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + reason);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));

        switch (args.purchasedProduct.definition.id)
        {
		case Pentatiles.RemoveAds:
			Game.Instance.playData.adsFree = true;
			Game.Instance.playData.Save ();
			break;
		case Pentatiles.Hint_110:
			Game.Instance.AddHint (110);
            break;
		case Pentatiles.Hint_55:
			Game.Instance.AddHint (55);
			break;
		case Pentatiles.Hint_10:
			Game.Instance.AddHint (10);
			break;
        }

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }
}
