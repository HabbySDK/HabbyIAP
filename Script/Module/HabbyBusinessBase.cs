using System;
using System.Text;
using Habby.Business.Cache;
using Habby.Tool.Http;
using UnityEngine.Purchasing;
using Habby.Tool.Http.Tool;
using Habby.Business.Data;
using Habby.Tool;

namespace Habby.Business
{
    public interface IHabbyBusiness
    {
    }

    public abstract class BusinessBase
    {
        public static void AddCache(string key, object pData)
        {
            if (pData == null) return;
            try
            {
                IAPCahceManager.Instance.AddCacheData(key, DataConvert.ToJson(pData));
            }
            catch (Exception e)
            {
                IAPLog.LogError(e);
            }
        }

        public static T GetCache<T>(string key)
        {
            var tjson = IAPCahceManager.Instance.GetCache(key);

            if (tjson != null)
            {
                var ret = DataConvert.FromJson<T>(tjson);
                if (ret == null)
                {
                    IAPCahceManager.Instance.RemoveCache(key);
                }

                return ret;
            }

            return default(T);
        }

        #region callevent

        public static void CallEventAndCache<T>(string key, int code, string msg, T data, HttpEvent<T> onComplete)
        {
            try
            {
                if (code == 0)
                {
                    onComplete?.Invoke(code, msg, data);
                    AddCache(key, data);
                }
                else
                {
                    onComplete?.Invoke(code, msg, GetCache<T>(key));
                }
            }
            catch (Exception e)
            {
                IAPLog.LogError(e);
            }
        }

        public static void CallEvent<T>(int code, string msg, T data, HttpEvent<T> onComplete) where T : class
        {
            try
            {
                onComplete?.Invoke(code, msg, data);
            }
            catch (Exception e)
            {
                IAPLog.LogError(e);
            }
        }

        public static void CallOnCompleteAndCache<T>(string key, DefaultResponse<T> response, HttpEvent<T> onComplete,
            int errorCode, string error) where T : class
        {
            try
            {
                if (response != null)
                {
                    CallEventAndCache(key, response.code, response.message, response.data, onComplete);
                }
                else
                {
                    CallEventAndCache(key, errorCode, error, null, onComplete);
                }
            }
            catch (Exception e)
            {
                IAPLog.LogError(e);
            }
        }

        public static void CallOnComplete<T>(DefaultResponse<T> response, HttpEvent<T> onComplete, int errorCode,
            string error) where T : class
        {
            try
            {
                if (response != null)
                {
                    CallEvent(response.code, response.message, response.data, onComplete);
                }
                else
                {
                    CallEvent(errorCode, error, null, onComplete);
                }
            }
            catch (Exception e)
            {
                IAPLog.LogError(e);
            }
        }

        #endregion
    }

    public abstract class HabbyBusinessBase<TProduct> : BusinessBase
    {
        public IAPSetting Setting { get; protected set; }

        public abstract string productsKey { get; }

        public HabbyBusinessBase(IAPSetting pSetting)
        {
            Setting = pSetting;
        }


        public void GetProducts(string[] ids, bool showAll, HttpEvent<TProduct[]> onComplete)
        {
            var uid = IAPHttp.EscapeURL(Setting.userId);

            RequestPathObject treqpath = new RequestPathObject(Setting.serverUrl, $"users/{uid}/gameProducts");
            if (ids != null && ids.Length > 0)
            {
                treqpath.AddArray(productsKey, ids);
            }
            else
            {
                treqpath.AddKeyword(productsKey, "all");
            }

            treqpath.AddKeyword("showAllProducts", showAll.ToString());

            IAPHttp.Instance.StartGetResponse<DefaultResponse<TProduct[]>>(treqpath.GetRequestUrl(), null,
                (response, error, errorcode) => { CallOnComplete(response, onComplete, errorcode, error); }, 60,
                IAPHttp.IsParamsValid(uid));
        }
    }


    public sealed class RequestPathObject : RequestPathObjectBase
    {
        public RequestPathObject(string server, string pPath) : base(server, pPath)
        {
            
        }
    }
}