using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

using Newtonsoft.Json;
using System.Collections;
using System.Globalization;
using System.IO;
using Habby.Tool;
using Habby.Business;
using Habby.Business.Cache;
using Habby.Business.Data;
using Habby.Tool.Http;
using Habby.Tool.Http.Tool;

namespace Habby.Business
{
    [Serializable]
    public class ProductObject
    {
        public string gameItemId;
        public string storeId;
        public ProductType productType;
        public string lastOrderId{ get; set; }
        public ValidatinoData data { get; set; }

        public void Copy(ProductObject pobj)
        {
            this.gameItemId = pobj.gameItemId;
            this.storeId = pobj.storeId;
            this.productType = pobj.productType;
            this.lastOrderId = pobj.lastOrderId;
        }
    }
    

    public class IAPSetting
    {
        public string serverUrl;
        public string userId;
        /// <summary>
        /// This ID is used to get the currency type
        /// </summary>
        public string defaultActiveStoreId;
    }

    public enum PurchaseProcessType
    {
        none = 0,
        getOrderId,
        waitValidatine,
    }
    

    public class IAPLocalData
    {
        [JsonIgnore]private const string savefile = "HabbyPurchaseLocalData.a";
        [JsonIgnore]private string PathFile => $"{Application.persistentDataPath}/{savefile}";
        
        public bool IsPurchaseing = false;

        public PurchaseProcessType processType;
        
        public ProductObject lastProduct;
        
        
        public void Load()
        {
            try
            {
                if (File.Exists(PathFile))
                {
                    var tstr = File.ReadAllText(PathFile);
                    Debug.Log($"IAPLocalData: {tstr}");
                    if (!string.IsNullOrEmpty(tstr))
                    {
                        Habby.Tool.DataConvert.MergeFromJson(this,tstr);
                    }
                }

            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

        }

        public void Save()
        {
            try
            {
                string tsaveStr = Habby.Tool.DataConvert.ToJson(this);
            
                File.WriteAllText(PathFile,tsaveStr);
            }
            catch (Exception e)
            {
                IAPLog.LogError(e);
            }
        }
    }

    public enum PurchaseCode
    {
        sucess = 0,
        channelIAPError,
        serverError,
        localError,
    }

    public delegate void PurchaseEvent(Product product,ProductObject last);
    public delegate void PurchaseProcessStart();
    public delegate void PurchaseProcessEnd(PurchaseCode purchaseCode,int statuCode, ProductObject last);
    public delegate void ValidatinoSuccess(ProductOrder item, ValidatinoData responseData);
    //Product
    public class IAPManager : MonoBehaviour, IClientData
    {
        public const string SDKVersion = "1.0";
        
        private static IAPManager _Instance;
        public static IAPManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    GameObject tobj = new GameObject("IAPManager");
                    GameObject.DontDestroyOnLoad(tobj);

                    _Instance = tobj.AddComponent<IAPManager>();
                }

                return _Instance;
            }
        }


        public event PurchaseEvent OnPurchaseComplete;
        public event PurchaseProcessStart OnPurchaseStart;
        public event PurchaseProcessEnd OnPurchaseEnd;
        public event ValidatinoSuccess OnValidatinoSuccess;
        public event Action OnInitComplete;
        public event Action OnRestoreComplete;
        public bool InitedPurchase => iap?.InitedPurchase ?? false;

        #region module

        public HabbyBusinessBattlePass BattlePass { get; private set; }
        public HabbyBusinessGift Gift { get; private set; }
        public HabbyBusinessPiggyBank PiggyBank { get; private set; }
        public HabbyBusinessStore Store { get; private set; }
        public HabbyBusinessOrdinary Ordinary { get; private set; }
        public HabbyBusinessActivity Activity { get; private set; }

        #endregion

        protected ProductValidationOrderManager ValidationOrderManager { get; private set; }

        public IAPSetting Setting { get; private set; } = new IAPSetting();

        public IAPModuleBase iap { get; private set; }

        private IAPLocalData localData = new IAPLocalData();
        
        private IAPManager()
        {
            
        }

        bool Inited = false;
        public void InitIAP(IAPSetting pSetting,List<ProductObject> pList ,IAPModuleBase pIap = null)
        {
            if(Inited) return;
            if(pList == null) return;
            
            IAPLog.Log("InitIAP.");
            
            Inited = true;
            localData.Load();

            if(localData.IsPurchaseing && localData.processType != PurchaseProcessType.waitValidatine)
            {
                localData.IsPurchaseing = false;
                localData.Save();
            }
            
            Setting = pSetting;
            
            IAPCahceManager.Instance.Init();

            ValidationOrderManager = new ProductValidationOrderManager();
            ValidationOrderManager.validationDelgate = OnProductOrderValidatinoComplete;
            
            InitClientData();
            InitModule();
            InitIAP(pList, pIap);
        }

        void InitClientData()
        {
            var tfields = new Dictionary<string, object>()
            {
                {"deviceId", SystemInfo.deviceUniqueIdentifier},
                {"appVersion", Application.version},
                {"osVersion", SystemInfo.operatingSystem},
                {"systemLanguage", Application.systemLanguage.ToString()},
                {"appBundle", Application.identifier},
                {"deviceModel", UnityEngine.SystemInfo.deviceModel},
            };

            SetClientFields(tfields);
            IAPHttp.Instance.AddCustomHeader("ClientData",Instance.customClientData);
        }

        void InitModule()
        {
            BattlePass = new HabbyBusinessBattlePass(Setting);
            Gift = new HabbyBusinessGift(Setting);
            PiggyBank = new HabbyBusinessPiggyBank(Setting);
            Store = new HabbyBusinessStore(Setting);
            Ordinary = new HabbyBusinessOrdinary(Setting);
            Activity = new HabbyBusinessActivity(Setting);
        }

        void InitIAP(List<ProductObject> pList, IAPModuleBase pIap)
        {
            if (pIap == null)
            {
#if UNITY_IOS
                iap = new IAPAppleStore(this);
#elif UNITY_ANDROID
                iap = new IapGooglePlay(this);
#else
                iap = new IAPAppleStore(this);
#endif
            }
            
            iap.OnInitComplete += () =>
            {
                try
                {
                    OnInitComplete?.Invoke();
                }
                catch (Exception e)
                {
                    IAPLog.LogError(e);
                }
            };

            iap.OnRestoreComplete += () =>
            {
                try
                {
                    OnRestoreComplete?.Invoke();
                }
                catch (Exception e)
                {
                    IAPLog.LogError(e);
                }
            };

            iap.OnPurchaseSuccess += (item) =>
            {
                Validatino(item);
            };

            iap.OnPurchaseFaild += (item,error) =>
            {
                IAPLog.Log($"Purchase failed.id = {item.definition.id} product = {item.transactionID}, failureReason = {error}");

                if (IsCurPurchase(item))
                {
                    ClearCurPurchaseing();
                    EndPurchase(PurchaseCode.channelIAPError, (int)error);
                }
            };
            
            iap.Init(pList);
        }
        
        void OnProductOrderValidatinoComplete(ProductOrder pItem, int code, ValidatinoData response)
        {
            try
            {
                if (code == 0)
                {
                    OnValidatinoSuccess?.Invoke(pItem, response);
                }
            }
            catch (Exception e)
            {
                IAPLog.LogError(e);
            }
        }

        bool IsCurPurchase(Product pItem)
        {
            return pItem != null && localData != null && localData.lastProduct != null
                   && !string.IsNullOrEmpty(localData.lastProduct.lastOrderId)
                   && string.Equals(localData.lastProduct.storeId, pItem.definition.id);
        }

        bool IsSaveProduct(ProductObject a,ProductObject b)
        {
            if (a == null || b == null) return false;
            if (string.IsNullOrEmpty(a.lastOrderId) || string.IsNullOrEmpty(b.lastOrderId)) return false;

            return string.Equals(a.lastOrderId, b.lastOrderId);
        }

        void FinishedPurchase(Product pItem,ProductObject pObj)
        {
            if (pItem == null) return;
            if (IsSaveProduct(localData.lastProduct, pObj))
            {
                localData.lastProduct = null;
            }
                    
            ClearCurPurchaseing();
            ConfirmPendingPurchase(pItem);
        }

        void ConfirmPendingPurchase(Product pItem)
        {
            try
            {
                switch (pItem.definition.type)
                {
                    case ProductType.Consumable:
                    case ProductType.NonConsumable:
                    {
                        iap.ConfirmPendingPurchase(pItem);
                    }
                        break;
                }
            }
            catch (Exception e)
            {
                IAPLog.LogError(e);
            }
        }
        
        IEnumerator WaitReValidatino(string pOrderId)
        {
            yield return new WaitForSeconds(1);
            ValidationObject(pOrderId);
        }

        void ValidationObject(string pOrderId)
        {
            ValidationOrderManager.Validation(pOrderId, (orderitem, code, vdata) =>
            {
                switch ((ValidationStatusCode) code)
                {
                    case ValidationStatusCode.httpError:
                    case ValidationStatusCode.localError:
                    {
                        StartCoroutine(WaitReValidatino(orderitem.orderId));
                    }
                        break;
                    default:
                    {
                        ProductObject tproductObj = new ProductObject()
                        {
                            gameItemId = orderitem.gameItemId,
                            storeId = orderitem.storeId,
                            productType = orderitem.productType,
                            lastOrderId = orderitem.orderId,
                            data = vdata,
                        };

                        Product tproductItem = WithID(orderitem.storeId);

                        FinishedPurchase(tproductItem, tproductObj);
                        if (code == 0)
                        {
                            EndPurchase(PurchaseCode.sucess, code, tproductObj);
                            CallPurchaseDone(tproductItem, tproductObj);
                        }
                        else
                        {
                            EndPurchase(PurchaseCode.serverError, code, tproductObj);
                        }
                    }
                        break;
                }

            });
        }
        
        void Validatino(Product pItem)
        {
            if(pItem == null) return;

            if (IsCurPurchase(pItem))
            {
                IAPLog.Log($"Match LastProduct id:{pItem.definition.id}  transid:{pItem.transactionID}");

                localData.processType = PurchaseProcessType.waitValidatine;
                localData.Save();

                var obj = localData.lastProduct;
                var torderitem = new ProductOrder()
                {
                    gameItemId = obj.gameItemId,
                    storeId = obj.storeId,
                    productType = obj.productType,

                    orderId = obj.lastOrderId,
                    transactionId = pItem.transactionID,
                    currency = pItem.metadata.isoCurrencyCode,
                    currencyMoney = pItem.metadata.localizedPrice.ToString(CultureInfo.InvariantCulture),
                    region = iap.GetCountryCode(),
                    channelCode = iap.Channel,
                    receipt = pItem.receipt,

                    creatTime = System.DateTime.UtcNow.Ticks / 10000,
                };

                ValidationOrderManager.AddValidatinoProduct(torderitem);

                ValidationObject(torderitem.orderId);

            }
            else
            {
                IAPLog.Log($"Not Match LastProduct id:{pItem.definition.id}  transid:{pItem.transactionID}");
                
                ConfirmPendingPurchase(pItem);
                CallPurchaseDone(pItem,null);
            }
            
        }
        
        void CallPurchaseDone(Product product,ProductObject productObj)
        {
            try
            {
                OnPurchaseComplete?.Invoke(product,productObj);
            }
            catch (Exception e)
            {
                IAPLog.LogError(e);
            }
        }
        
        public Product WithID(string pId)
        {
            try
            {
                if (!iap.InitedPurchase)
                {
                    IAPLog.LogError($" iap.InitedPurchase = {iap.InitedPurchase}");
                }
                return iap.WithID(pId);
            }
            catch (Exception e)
            {
                IAPLog.LogError(e);
            }

            return null;
        }
        
        public bool DoIAPPurchase(ProductObject pItem)
        {
            if (pItem == null || !iap.InitedPurchase || localData.IsPurchaseing)
            {
                IAPLog.LogError($" iap.InitedPurchase = {iap.InitedPurchase}, localData.IsPurchaseing = {localData.IsPurchaseing}");
                return false;
            }

            var tproduct = iap.WithID(pItem.storeId);

            if (tproduct == null || !tproduct.availableToPurchase)
            {
                IAPLog.LogError($"product == {tproduct} || availableToPurchase == {tproduct.availableToPurchase}");
                return false;
            }
            
            IAPLog.Log($"Start GetOrderId. ProductId = {tproduct.definition.id},  ProductType = {tproduct.definition.type}");
            

            SetCurPurchaseing(pItem);
            StartPurchase();

            string tpath = $"{Setting.serverUrl}/orders";

            var treq = new
            {
                userId = Setting.userId, 
                marketProductId = pItem.storeId,
                channelCode = iap.Channel,
                gameProductId = pItem.gameItemId,
                currency = tproduct.metadata.isoCurrencyCode,
                currencyMoney = tproduct.metadata.localizedPrice,
                region = iap.GetCountryCode(),
            };
                
                
            IAPHttp.Instance.StartPost<IAPGetOrderResponse>(tpath, treq,(response,error,errorcode) =>
            {
                bool isresult = response != null && response.code == 0;
                if (isresult)
                {
                    localData.lastProduct.lastOrderId = response.data.orderId;
                    localData.processType = PurchaseProcessType.getOrderId;
                    localData.Save();
                        
                    iap.InitiatePurchase(tproduct,localData.lastProduct.lastOrderId);
                }
                else
                {
                    ClearCurPurchaseing();
                    EndPurchase(PurchaseCode.serverError, response != null ? response.code : -1);
                }

            });

            return true;
        }

        void SetCurPurchaseing(ProductObject item)
        {
            localData.IsPurchaseing = true;
            localData.lastProduct = new ProductObject();
            localData.lastProduct.Copy(item);
            localData.processType = PurchaseProcessType.none;
            localData.Save();
        }

        void ClearCurPurchaseing()
        {
            localData.IsPurchaseing = false;
            localData.processType = PurchaseProcessType.none;
            localData.Save();
        }

        public void RestPurchaseProcess()
        {
            ClearCurPurchaseing();
        }

        void StartPurchase()
        {
            try
            {
                OnPurchaseStart?.Invoke();
            }
            catch (Exception e)
            {
                IAPLog.LogError(e);
            }
            
            IAPLog.Log("StartPurchase");
        }

        void EndPurchase(PurchaseCode purchaseCode, int statuCode, ProductObject last = null)
        {
            try
            {
                OnPurchaseEnd?.Invoke(purchaseCode, statuCode, last);
            }
            catch (Exception e)
            {
                IAPLog.LogError(e);
            }
            

            IAPLog.Log("EndPurchase");
        }
        
        
        public Dictionary<string, object> customClientData { get; private set; } = new Dictionary<string, object>();
        public void SetClientFields(Dictionary<string, object> pFields)
        {
            if (pFields == null) return;
            foreach (var item in pFields)
            {
                if (customClientData.ContainsKey(item.Key))
                {
                    customClientData[item.Key] = item.Value;
                }
                else
                {
                    customClientData.Add(item.Key, item.Value);
                }
            }
        }
        
        public void Validation(string pOrderId, ValidatinoEvent pOnComplete)
        {
            if (!InitedPurchase)
            {
                IAPLog.LogError($"Validation failed. InitedPurchase = {InitedPurchase}");
                pOnComplete?.Invoke(null, (int) ValidationStatusCode.localError, null);
                return;
            }
            ValidationOrderManager.Validation(pOrderId,pOnComplete);
        }

        public List<string> GetList()
        {
            return ValidationOrderManager.GetOrderList();
        }
        
        public void ClearOrderFiles()
        {
            ValidationOrderManager.ClearOrderFiles();
        }
        
        internal void DeleteOrderFile(string pOrderId)
        {
            ValidationOrderManager.DeleteOrderFile(pOrderId);
        }

    }
}