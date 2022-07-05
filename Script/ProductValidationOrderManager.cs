using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Habby.Tool;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Habby.Business
{
    public enum ValidationStatusCode
    {
        httpError = -2,
        localError = -1,
        success = 0,
    }
    public delegate void ValidatinoEvent(ProductOrder item, int code, ValidatinoData responseData);
    public class ProductOrder
    {
        public string gameItemId;
        public string storeId;
        public ProductType productType;
        
        public string orderId;
        public string receipt;
        public string transactionId;
        public string currency;
        public string currencyMoney;
        public string region;
        public string channelCode;

        public long creatTime;
        internal void Validation(ValidatinoEvent onComplete)
        {
            var treqpath = new RequestPathObject($"orders/{orderId}?action=payCallback");

            var treq = new
            {
                receipt = receipt,
                transactionId = transactionId,
                channelCode = channelCode,
                currency = currency,
                currencyMoney = currencyMoney,
                region = region,
            };
            
            IAPHttp.Instance.StartPatchSend<IAPValidatinoResponse>(treqpath.GetRequestUrl(), treq,(response,error,errorcode) =>
            {
                ValidatinoData tdata = null;
                int code = (int)ValidationStatusCode.localError;
                if (response != null)
                {
                    if (response.code == 0)
                    {
                        tdata = response.data;
                    }

                    code = response.code;
                }
                else
                {
                    code = (int)ValidationStatusCode.httpError;
                }

                try
                {
                    onComplete?.Invoke(this,code, tdata);
                }
                catch (Exception e)
                {
                    IAPLog.LogError($"ProductOrder Error: {e}");
                }
                
            });
        }
        
        internal static void Save(string pPathFile, ProductOrder pItem)
        {
            if(pItem == null) return;
            try
            {
                File.WriteAllText(pPathFile, DataConvert.ToJson(pItem));
            }
            catch (Exception e)
            {
                IAPLog.LogError(e);
            }
        }

        internal static ProductOrder Load(string pPathFile)
        {
            try
            {
                if (!File.Exists(pPathFile)) return null;
                
                string tjson = File.ReadAllText(pPathFile);
                
                if (string.IsNullOrEmpty(tjson)) return null;
                
                return DataConvert.FromJson<ProductOrder>(tjson);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return null;
        }
    }
    
    internal class ValidationOrderData
    {
        [JsonIgnore]private const string savefile = "HabbyProductOrdersData.a";
        [JsonIgnore]private const string orderExName = ".order";
        [JsonIgnore]private readonly string PathFile;
        [JsonIgnore]private readonly string orderPath;
        
        [JsonIgnore]internal ValidatinoEvent validationDelgate;
        
        internal Dictionary<string, ProductOrder> orderMap = new Dictionary<string, ProductOrder>();

        internal ValidationOrderData()
        {
            PathFile = $"{Application.persistentDataPath}/HabbyIAP/{savefile}";
            orderPath = $"{Application.persistentDataPath}/HabbyIAP/Orders";
        }

        internal ProductOrder this[string key]
        {
            get
            {
                if (!orderMap.ContainsKey(key)) return null;
                return orderMap[key];
            }
            set
            {
                Add(value);
            }
        }

        internal void ClearOrderFiles()
        {
            try
            {
                if (!Directory.Exists(orderPath)) return;
                DirectoryInfo tdirfolder = new DirectoryInfo(orderPath);

                FileInfo[] tfileinfos = tdirfolder.GetFiles("*.*", System.IO.SearchOption.AllDirectories);
            
                if (tfileinfos.Length == 0) return;
                
                foreach (var item in tfileinfos)
                {
                    item.Delete();
                }
                
            }
            catch (Exception e)
            {
                IAPLog.LogError($"ClearOrderFiles Error = {e}");
            }
        }

        internal List<string> GetOrdersFromFile()
        {
            try
            {
                if (!Directory.Exists(orderPath)) return null;
                DirectoryInfo tdirfolder = new DirectoryInfo(orderPath);

                FileInfo[] tfileinfos = tdirfolder.GetFiles("*.*", System.IO.SearchOption.AllDirectories);
            
                if (tfileinfos.Length == 0) return null;

                List<string> ret = new List<string>(tfileinfos.Length);
                foreach (var item in tfileinfos)
                {
                    if(!item.Name.EndsWith(orderExName)) continue;

                    string torderid = item.Name.Replace(orderExName,"");
                    ret.Add(torderid);
                }

                return ret;
            }
            catch (Exception e)
            {
                IAPLog.LogError($"GetOrdersFromFile Error = {e}");
            }

            return null;
        }

        internal ProductOrder GetOrderFromFile(string pOrderId)
        {
            string torderfile = GetOrderFilePath(pOrderId);
            try
            {
                if (!File.Exists(torderfile)) return null;
                return ProductOrder.Load(torderfile);
            }
            catch (Exception e)
            {
                IAPLog.LogError($"GetOrderFromFile file = {torderfile}, Error = {e}");
            }

            return null;
        }

        internal string GetOrderFilePath(string pOrderId)
        {
            return $"{orderPath}/{pOrderId}{orderExName}";
        }

        internal bool OrderFileExits(string pOrderId)
        {
            try
            {
                string torderfile = GetOrderFilePath(pOrderId);
                return File.Exists(torderfile);
            }
            catch (Exception e)
            {
                IAPLog.LogError($"OrderFileExits order = {pOrderId}");
            }

            return false;
        }

        internal void DeleteOrderFile(string pOrderId)
        {
            try
            {
                string torderfile = GetOrderFilePath(pOrderId);
                if (File.Exists(torderfile))
                {
                    File.Delete(torderfile);
                }
            }
            catch (Exception e)
            {
                IAPLog.LogError($"DeleteOrderFile order = {pOrderId}");
            }
        }
        
        internal void Add(ProductOrder pItem)
        {
            if (pItem == null || string.IsNullOrEmpty(pItem.orderId))
            {
                IAPLog.LogError($"Validation Add failed. item = {pItem}, orderId = {pItem?.orderId}");
                return;
            }
            if (orderMap.ContainsKey(pItem.orderId))
            {
                IAPLog.Log($"Update orderId = {pItem.orderId}");
                orderMap[pItem.orderId] = pItem;
            }
            else
            {
                orderMap.Add(pItem.orderId, pItem);
            }

            string torderfile = GetOrderFilePath(pItem.orderId);
            ProductOrder.Save(torderfile, pItem);
        }

        internal void Remove(string key)
        {
            if (string.IsNullOrEmpty(key)) return;
            if (!orderMap.ContainsKey(key)) return;
            orderMap.Remove(key);
        }

        internal void Validation(string pOrderId,ValidatinoEvent pOnComplete)
        {
            if (!orderMap.ContainsKey(pOrderId))
            {
                var titem = GetOrderFromFile(pOrderId);
                if (titem != null)
                {
                    orderMap.Add(pOrderId,titem);
                }
                else
                {
                    IAPLog.LogError("No ProductOrder found.");
                    pOnComplete?.Invoke(null, (int) ValidationStatusCode.localError, null);
                    return;
                }

            }

            var item = orderMap[pOrderId];
            
            item.Validation((obj,code,responseData) =>
            {
                if (code == 0)
                {
                    Remove(pOrderId);
                    DeleteOrderFile(pOrderId);
                }

                try
                {
                    pOnComplete?.Invoke(obj,code,responseData);
                }
                catch (Exception e)
                {
                    IAPLog.LogError($"ValidationOrderData Error = {e}");
                }

                validationDelgate?.Invoke(obj,code,responseData);
            });
        }
    }
    
    
    public class ProductValidationOrderManager
    {
        internal ValidatinoEvent validationDelgate;

        private ValidationOrderData data;
        
        internal ProductValidationOrderManager()
        {
            data = new ValidationOrderData();
            data.validationDelgate = OnProductOrderValidatinoComplete;
        }
        internal void AddValidatinoProduct(ProductOrder pItem)
        {
            try
            {
                if(pItem == null) return;
                
                data.Add(pItem);
            }
            catch (Exception e)
            {
                IAPLog.Log($"AddProductOrder Error = {e} ");
            }
        }
        
        internal List<string> GetOrderList()
        {
            return data.GetOrdersFromFile();
        }

        internal void ClearOrderFiles()
        {
            data.ClearOrderFiles();
        }

        internal void DeleteOrderFile(string pOrderId)
        {
            data.DeleteOrderFile(pOrderId);
        }

        internal void Validation(string pOrderId, ValidatinoEvent pOnComplete)
        {
            try
            {
                data.Validation(pOrderId,pOnComplete);
            }
            catch (Exception e)
            {
                IAPLog.LogError(e);
            }
        }

        void OnProductOrderValidatinoComplete(ProductOrder pItem,int code,ValidatinoData response)
        {
            try
            {
                validationDelgate?.Invoke(pItem,code,response);
            }
            catch (Exception e)
            {
                IAPLog.LogError(e);
            }
        }
    }
}