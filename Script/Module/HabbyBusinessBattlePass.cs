using System.Collections.Generic;
using Habby.Tool.Http;
using Habby.Business.Data;
using Habby.Tool.Http.Tool;
using Newtonsoft.Json.Linq;

namespace Habby.Business
{
    public class HabbyBusinessBattlePass : HabbyBusinessBase<BattleProduct>, IHabbyBusiness
    {
        #region request

        public class BattlePassAddValueRequest
        {
            public string source;
            public int value;
        };

        #endregion
        
        public override string productsKey => "battlePassProductIds";
        public HabbyBusinessBattlePass(IAPSetting pSetting) : base(pSetting)
        {
        }

        /*
        public void GetInfos(string[] ids, HttpEvent<BattlePassInfo[]> onComplete)
        {
            //users/:userId/battlePasses
            var uid = IAPHttp.EscapeURL(Setting.userId);
            RequestPathObject treqpath = new RequestPathObject(Setting.serverUrl, $"users/{uid}/battlePasses");
            treqpath.AddArray("battlePassIds", ids);

            IAPHttp.Instance.StartGetResponse<DefaultResponse<BattlePassInfo[]>>(treqpath.GetRequestUrl(), null,
                (response, error, errorcode) => { CallOnCompleteAndCache($"{productsKey}-list",response, onComplete, errorcode, error); }, 60,
                IAPHttp.IsParamsValid(uid));
        }

        public void GetInfo(string id, HttpEvent<BattlePassInfo> onComplete)
        {
            var uid = IAPHttp.EscapeURL(Setting.userId);
            var bid = IAPHttp.EscapeURL(id);

            RequestPathObject treqpath = new RequestPathObject(Setting.serverUrl, $"users/{uid}/battlePasses/{bid}");

            IAPHttp.Instance.StartGetResponse<DefaultResponse<BattlePassInfo>>(treqpath.GetRequestUrl(), null,
                (response, error, errorcode) => { CallOnCompleteAndCache($"{productsKey}-info-{id}" ,response, onComplete, errorcode, error); }, 60,
                IAPHttp.IsParamsValid(uid, bid));
        }
       
        
        public void GetInfos(string[] ids, HttpEvent<BattlePassInfo[]> onComplete)
        {
            //users/:userId/battlePasses
            var uid = IAPHttp.EscapeURL(Setting.userId);
            RequestPathObject treqpath = new RequestPathObject(Setting.serverUrl, $"users/{uid}/battlePasses");
            treqpath.AddArray("battlePassIds", ids);

            IAPHttp.Instance.StartGetResponse<DefaultResponse<BattlePassInfo[]>>(treqpath.GetRequestUrl(), null,
                (response, error, errorcode) => { CallOnCompleteAndCache($"{productsKey}-list",response, onComplete, errorcode, error); }, 60,
                IAPHttp.IsParamsValid(uid));
        }
        
        */
        
        /// <summary>
        /// GetBattlePass by type
        /// </summary>
        /// <param name="type">赛季类型 season, 装备投资 equipmentInvestment ,成长基金 growthFund</param>
        /// <param name="onComplete">callback</param>
        public void GetUserBattlePass(string type ,HttpEvent<BattlePassInfo[]> onComplete)
        {
            //users/:userId/battlePasses
            var uid = IAPHttp.EscapeURL(Setting.userId);
            RequestPathObject treqpath = new RequestPathObject($"users/{uid}/battlePasses");
            treqpath.AddKeyword("battlePassType",type);

            IAPHttp.Instance.StartGetResponse<DefaultResponse<BattlePassInfo[]>>(treqpath.GetRequestUrl(), null,
                (response, error, errorcode) => { CallOnCompleteAndCache($"{productsKey}-list",response, onComplete, errorcode, error); }, 60,
                IAPHttp.IsParamsValid(uid));
        }

        public void UpdateBattlePass(string id, HttpEvent<BattlePassInfo> onComplete)
        {
            //users/:userId/battlePasses/:battlePassId?action=addValue
            var uid = IAPHttp.EscapeURL(Setting.userId);
            var bid = IAPHttp.EscapeURL(id);

            var treq = new
            {
   
            };

            RequestPathObject treqpath = new RequestPathObject($"users/{uid}/battlePasses/{bid}");
            treqpath.AddKeyword("action", "addValueByExchange");

            IAPHttp.Instance.StartPatchSend<DefaultResponse<BattlePassInfo>>(treqpath.GetRequestUrl(), treq,
                (response, error, errorcode) => { CallOnComplete(response, onComplete, errorcode, error); }, 60,
                IAPHttp.IsParamsValid(uid, bid));
        }
        
        public void GetReward(string id,int passLevel,string levelGameProductId, HttpEvent<BattlePassInfo> onComplete)
        {
            var uid = IAPHttp.EscapeURL(Setting.userId);
            var bid = IAPHttp.EscapeURL(id);

            RequestPathObject treqpath = new RequestPathObject($"users/{uid}/battlePasses/{bid}");
            treqpath.AddKeyword("action", "receiveNormal");

            var treq = new
            {
                level = passLevel,
                gameProductId = levelGameProductId,
            };


            IAPHttp.Instance.StartPatchSend<DefaultResponse<BattlePassInfo>>(treqpath.GetRequestUrl(), treq,
                (response, error, errorcode) => { CallOnComplete(response, onComplete, errorcode, error); }, 60,
                IAPHttp.IsParamsValid(uid, bid));
        }
        
        public void GetAdditionReward(string id,string pGameProductId, HttpEvent<BattlePassInfo> onComplete)
        {
            var uid = IAPHttp.EscapeURL(Setting.userId);
            var bid = IAPHttp.EscapeURL(id);

            RequestPathObject treqpath = new RequestPathObject($"users/{uid}/battlePasses/{bid}");
            treqpath.AddKeyword("action", "receiveAddition");

            var treq = new
            {
                gameProductId = pGameProductId,
            };


            IAPHttp.Instance.StartPatchSend<DefaultResponse<BattlePassInfo>>(treqpath.GetRequestUrl(), treq,
                (response, error, errorcode) => { CallOnComplete(response, onComplete, errorcode, error); }, 60,
                IAPHttp.IsParamsValid(uid, bid));
        }
    
    }
}