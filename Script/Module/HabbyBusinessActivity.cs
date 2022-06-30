using Habby.Business.Data;

namespace Habby.Business
{
    public class HabbyBusinessActivity : HabbyBusinessBase<GameProductItem>, IHabbyBusiness
    {
        public override string productsKey => "ActivityProductIds";

        public HabbyBusinessActivity(IAPSetting pSetting) : base(pSetting)
        {
        }

        public void GetActivity(string id, HttpEvent<ActivityInfo> onComplete)
        {
            var uid = IAPHttp.EscapeURL(Setting.userId);
            var tid = IAPHttp.EscapeURL(id);
            RequestPathObject treqpath = new RequestPathObject(Setting.serverUrl, $"users/{uid}/activities/{tid}");
            treqpath.AddKeyword("currency",Setting.defaultActiveStoreId);

            IAPHttp.Instance.StartGetResponse<DefaultResponse<ActivityInfo>>(treqpath.GetRequestUrl(), null,
                (response, error, errorcode) =>
                {
                    CallOnCompleteAndCache($"Activity-{id}", response, onComplete, errorcode, error);
                }, 60,
                IAPHttp.IsParamsValid(uid, tid));
        }

        public void GetLevelReward(string id, string pLevel, HttpEvent<ActivityInfo> onComplete)
        {
            var tbody = new
            {
                level = pLevel,
            };

            SendActivityAction(id, tbody, "receiveProgressAward", onComplete);
        }

        public void GetTaskReward(string id, int pDayIndex, string pTaskId, HttpEvent<ActivityInfo> onComplete)
        {
            var tbody = new
            {
                dayIndex = pDayIndex,
                taskId = pTaskId,
            };

            SendActivityAction(id, tbody, "receiveTaskAward", onComplete);
        }

        public void UpdateActivityExchange(string id, string pExchange, HttpEvent<ActivityInfo> onComplete)
        {
            var tbody = new
            {
                exchangeId = pExchange,
            };

            SendActivityAction(id, tbody, "activityExchange", onComplete);
        }

        protected void SendActivityAction(string id, object pBody, string pAction, HttpEvent<ActivityInfo> onComplete)
        {
            var uid = IAPHttp.EscapeURL(Setting.userId);
            var tid = IAPHttp.EscapeURL(id);
            RequestPathObject treqpath = new RequestPathObject(Setting.serverUrl, $"users/{uid}/activities/{tid}");
            treqpath.AddKeyword("action", pAction);

            IAPHttp.Instance.StartPatchSend<DefaultResponse<ActivityInfo>>(treqpath.GetRequestUrl(), pBody,
                (response, error, errorcode) => { CallOnComplete(response, onComplete, errorcode, error); }, 60,
                IAPHttp.IsParamsValid(uid, tid));
        }
    }
}