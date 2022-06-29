using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Purchasing;
namespace Habby.Business
{

    public class IAPAppleStore : IAPModuleBase, IStoreListener
    {
#if UNITY_IOS
        [DllImport("__Internal")]
        public static extern string _GetCountryCode();
#endif
        
        override public string Channel { get { return "apple"; } }
        private ConfigurationBuilder builder;
        private IAppleExtensions appextensions;
        
        public IAPAppleStore(MonoBehaviour comp) : base(comp)
        {

        }
        
        override public string GetCountryCode()
        {
            #if UNITY_EDITOR
                return "US_EDITOR";
            #elif UNITY_IOS
                return _GetCountryCode();
            #else
                return "ErrorCode";
            #endif
        }
        
        override protected void InitPurchase()
        {
            var tmodule = StandardPurchasingModule.Instance();
            builder = ConfigurationBuilder.Instance(tmodule);

            for (int i = 0, UPPER = productList.Count; i < UPPER; i++)
            {
                var item = productList[i];
                builder.AddProduct(item.storeId, item.productType);
            }
            
            IAPLog.Log("UnityPurchasing.Initialize}");
            UnityPurchasing.Initialize(this, builder);
        }

        void IStoreListener.OnInitialized(IStoreController pcol, IExtensionProvider extensions)
        {
            IAPLog.Log("Inited UnityPurchasing success.");

            this.controller = pcol;

            this.appextensions = extensions.GetExtension<IAppleExtensions>();
            this.appextensions.RegisterPurchaseDeferredListener(OnDeferred);

            InitedPurchase = true;

            RestoreTransactions();

            _OnInitComplete();
        }

        void IStoreListener.OnInitializeFailed(InitializationFailureReason error)
        {
            IAPLog.Log("Inited UnityPurchasing Failed. error =" + error);

            monoComp.StartCoroutine(WaitInitPurchase());
        }

        void IStoreListener.OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            _OnPurchaseFaild(product, failureReason);
        }

        PurchaseProcessingResult IStoreListener.ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            _OnPurchaseSuccess(purchaseEvent.purchasedProduct);

            var item = purchaseEvent.purchasedProduct;

            switch(item.definition.type)
            {
                case ProductType.Consumable:
                case ProductType.NonConsumable:
                    return PurchaseProcessingResult.Pending;
                default:
                    return PurchaseProcessingResult.Complete;
            }
        }

        void OnDeferred(Product item)
        {
            IAPLog.Log("net error.lag" + item.transactionID);
        }


        override protected void RestoreTransactions()
        {
            if (!InitedPurchase) return;

            this.appextensions.RestoreTransactions((result) =>
            {
                IAPLog.Log($"Restore result = {result}");

                _OnRestoreComplete();
            });
        }
    }
}