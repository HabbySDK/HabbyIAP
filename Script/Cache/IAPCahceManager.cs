using System;
using System.Collections.Concurrent;
using System.IO;
using Habby.Tool.Http.Cache;
using System.Threading.Tasks;
using UnityEngine;
namespace Habby.Business.Cache
{
    public class IAPCahceManager
    {
        private static IAPCahceManager sInstance;

        public static IAPCahceManager Instance
        {
            get
            {
                if (sInstance == null)
                {
                    sInstance = new IAPCahceManager();
                }

                return sInstance;
            }
        }
        
        public string CachePath { get; private set; }

        private IAPCahceManager()
        {
            
        }

        private bool inited = false;

        public void Init()
        {
            if(inited) return;
            inited = true;
            CachePath = $"{Application.persistentDataPath}/IAPCache";

            if (!Directory.Exists(CachePath))
            {
                Directory.CreateDirectory(CachePath);
            }
        }
        
        private ConcurrentDictionary<string, string> cacheData = new ConcurrentDictionary<string, string>();
        
        private ConcurrentQueue <SaveCacheObject> saveList = new ConcurrentQueue<SaveCacheObject>();

        public class SaveCacheObject
        {
            public string path;
            public string json;
        }

        protected string GetPathKey(string key)
        {
            return $"{key}_{IAPManager.SDKVersion}.c";
        }
        public void AddCacheData(string pKey,string pData)
        {
            if(string.IsNullOrEmpty(pKey) || string.IsNullOrEmpty(pData)) return;
            
            var tkey = GetPathKey(pKey);
            if (cacheData.ContainsKey(tkey))
            {
                if (cacheData.TryGetValue(tkey, out string obj))
                {
                    if (!string.Equals(pData, obj))
                    {
                        cacheData.TryUpdate(tkey, pData, null);
                        SaveCache(tkey,pData);
                    }
                }
            }
            else
            {
                cacheData.TryAdd(tkey, pData);
                SaveCache(tkey,pData);
            }

            
        }

        public string GetCache(string pKey)
        {
            var tkey = GetPathKey(pKey);
            if (cacheData.ContainsKey(tkey))
            {
                cacheData.TryGetValue(tkey, out string ret);
                return ret;
            }
            else
            {
                try
                {
                    var tpath = $"{CachePath}/{tkey}";
                    if (File.Exists(tpath))
                    {
                        var tstr = File.ReadAllText(tpath);
                        if (!string.IsNullOrEmpty(tkey) && !string.IsNullOrEmpty(tstr))
                        {
                            cacheData.TryAdd(tkey, tstr);
                        }
                        return tstr;
                    }
                }
                catch (Exception e)
                {
                   IAPLog.LogError(e);
                }
                
            }

            return null;
        }

        public string RemoveCache(string pKey)
        {
            var tkey = GetPathKey(pKey);
            if (cacheData.TryRemove(tkey,out string ret))
            {
                return ret;
            }

            return null;
        }


        private Task taskThread = null;
        protected void SaveCache(string key, string pData)
        {
            if(string.IsNullOrEmpty(key) || string.IsNullOrEmpty(pData)) return;
            var tobj = new SaveCacheObject()
            {
                path = key,
                json = pData,
            };
            
            saveList.Enqueue(tobj);
            if (taskThread == null)
            {
                taskThread = Task.Run(SaveThread);
            }
        }

        void SaveThread()
        {
            try
            {
                while (saveList.Count > 0)
                {
                    if (saveList.TryDequeue(out SaveCacheObject obj))
                    {
                        try
                        {
                            File.WriteAllText($"{CachePath}/{obj.path}", obj.json);
                        }
                        catch (Exception e)
                        {
                            IAPLog.LogError($"[SaveThread]:Path = {obj.path}, Error = {e}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                IAPLog.LogError($"[SaveThread]: Error = {e}");
            }
            
            taskThread = null;
        }
    }
}