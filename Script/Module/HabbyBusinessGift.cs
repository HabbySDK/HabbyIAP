using Habby.Tool.Http;
using Habby.Business.Data;
using Habby.Tool;
using Habby.Tool.Http.Tool;

namespace Habby.Business
{
    public class HabbyBusinessGift : HabbyBusinessBase<GiftProduct>, IHabbyBusiness
    {
        public override string productsKey => "giftpackProductIds";

        public HabbyBusinessGift(IAPSetting pSetting) : base(pSetting)
        {
        }
        
        public void UpdateGiftByAction(string pId,string pActon,object pBody, HttpEvent<GiftInfo> onComplete)
        {
            var uid = IAPHttp.EscapeURL(Setting.userId);

            RequestPathObject treqpath = new RequestPathObject(Setting.serverUrl, $"users/{uid}/gifts");
            treqpath.AddKeyword("action", pActon);
            treqpath.AddKeyword("gameProductId", pId);

            IAPHttp.Instance.StartPost<DefaultResponse<GiftInfo>>(treqpath.GetRequestUrl(), pBody,
                (response, error, errorcode) => { CallOnComplete(response, onComplete, errorcode, error); }, 60,
                IAPHttp.IsParamsValid(uid));
        }

        public void MarkDisplay(string pId, HttpEvent<GiftInfo> onComplete)
        {
            var tbody = new { };
            UpdateGiftByAction(pId, "display", tbody, onComplete);
        }

        public void GetList(string giftType, string giftSubType, HttpEvent<GiftInfo[]> onComplete)
        {
            GetInfos(null, giftType, giftSubType, (code, msg, responseData) =>
            {
                CallEventAndCache($"{productsKey}-list-{giftType}-{giftSubType}",code, msg, responseData, onComplete);
            });
        }

        public void GetInfos(string[] ids, HttpEvent<GiftInfo[]> onComplete)
        {
            GetInfos(ids, null, null, (code, msg, responseData) =>
            {
                CallEventAndCache($"{productsKey}-listInfos-{DataConvert.ToJson(ids)}",code, msg, responseData, onComplete);
            });
        }
        
        protected void GetInfos(string[] ids, string giftType, string giftSubType, HttpEvent<GiftInfo[]> onComplete)
        {
            var uid = IAPHttp.EscapeURL(Setting.userId);

            RequestPathObject treqpath = new RequestPathObject(Setting.serverUrl, $"users/{uid}/gifts");
            treqpath.AddArray("gameProductIds", ids);
            treqpath.AddKeyword("giftType", giftType);
            treqpath.AddKeyword("giftSubType", giftSubType);


            IAPHttp.Instance.StartGetResponse<DefaultResponse<GiftInfo[]>>(treqpath.GetRequestUrl(), null,
                (response, error, errorcode) => { CallOnComplete(response, onComplete, errorcode, error); }, 60,
                IAPHttp.IsParamsValid(uid));
        }
    }
}