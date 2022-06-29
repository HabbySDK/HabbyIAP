using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
namespace Habby.Business
{
    public class IapGooglePlay : IAPModuleBase, IStoreListener
    {
        override public string Channel { get { return "google"; } }
        private ConfigurationBuilder builder;
        private IGooglePlayStoreExtensions appextensions;

        public IapGooglePlay(MonoBehaviour comp) : base(comp)
        {

        }

        protected override void InitPurchase()
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

        public void OnInitialized(IStoreController pcol, IExtensionProvider extensions)
        {
            IAPLog.Log("Inited UnityPurchasing success.");

            this.controller = pcol;

            this.appextensions = extensions.GetExtension<IGooglePlayStoreExtensions>();

            InitedPurchase = true;

            RestoreTransactions();

            _OnInitComplete();
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            IAPLog.Log("Inited UnityPurchasing Failed. error =" + error);

            monoComp.StartCoroutine(WaitInitPurchase());
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            _OnPurchaseFaild(product, failureReason);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
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


        protected override void RestoreTransactions()
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