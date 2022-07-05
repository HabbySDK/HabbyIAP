using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
namespace Habby.Business
{
    public abstract class IAPModuleBase
    {
        #region evt
        public event Action OnInitComplete;
        public event Action OnRestoreComplete;

        public event Action<Product> OnPurchaseSuccess;
        public event Action<Product, PurchaseFailureReason> OnPurchaseFaild;

        protected void _OnInitComplete()
        {
            try
            {
                OnInitComplete?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        protected void _OnRestoreComplete()
        {
            try
            {
                OnRestoreComplete?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        protected void _OnPurchaseSuccess(Product item)
        {
            try
            {
                OnPurchaseSuccess?.Invoke(item);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        protected void _OnPurchaseFaild(Product item, PurchaseFailureReason err)
        {
            try
            {
                OnPurchaseFaild?.Invoke(item, err);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }
        #endregion


        public IStoreController controller { get; protected set; }
        virtual public string Channel { get { return "default"; } }
        public bool InitedPurchase { get; protected set; } = false;

        protected MonoBehaviour monoComp;
        protected List<ProductObject> productList;

        public IAPModuleBase(MonoBehaviour comp)
        {
            monoComp = comp;
        }

        bool Inited = false;
        public void Init(List<ProductObject> pList)
        {
            if (Inited) return;
            Inited = true;
            productList = pList;
            IAPLog.Log($"IAPModule. type = {this.GetType()}");
            
            InitPurchase();
        }

        abstract protected void InitPurchase();
        abstract protected void RestoreTransactions();

        public void InitiatePurchase(Product pItem, string payLoad)
        {
            controller.InitiatePurchase(pItem, payLoad);
        }

        public Product WithID(string pId)
        {
            if (controller == null || controller.products == null) return null;
            return controller.products.WithID(pId);
        }

        virtual public string GetCountryCode()
        {
            return null;
        }

        public void ConfirmPendingPurchase(string pId)
        {
            if (controller == null) return;
            var tproduct = controller.products.WithID(pId);
            if (tproduct != null)
            {
                ConfirmPendingPurchase(tproduct);
            }
            else
            {
                IAPLog.LogError($"cant found {pId}");
            }
        }

        public void ConfirmPendingPurchase(Product pItem)
        {
            if (pItem == null) return;
            controller.ConfirmPendingPurchase(pItem);
        }


        protected IEnumerator WaitInitPurchase()
        {
            IAPLog.Log("Start Init Purchase");
            yield return new WaitForSeconds(1);
            InitPurchase();
        }

        protected IEnumerator WaitRestorePurchase()
        {
            IAPLog.Log("Start RestorePurchase");
            yield return new WaitForSeconds(1);
            RestoreTransactions();
        }

    }
}
