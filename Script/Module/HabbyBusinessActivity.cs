using Habby.Business.Data;

namespace Habby.Business
{
    public class HabbyBusinessActivity : HabbyBusinessBase<GameProductItem>, IHabbyBusiness
    {
        public override string productsKey => "ActivityProductIds";

        public HabbyBusinessActivity(IAPSetting pSetting) : base(pSetting)
        {
        }
        
        /// <summary>
        /// Specify activity information
        /// </summary>
        /// <param name="pActivityId">activity id</param>
        /// <param name="pDayIndex">dayIndex</param>
        /// <param name="onComplete">callback</param>
        /// <returns></returns>
        public void GetActivity(string pActivityId,int pDayIndex, HttpEvent<ActivityInfo> onComplete)
        {
            var uid = IAPHttp.EscapeURL(Setting.userId);
            var tid = IAPHttp.EscapeURL(pActivityId);
            RequestPathObject treqpath = new RequestPathObject($"users/{uid}/activities/{tid}");
            treqpath.AddKeyword("currency",Setting.defaultActiveStoreId);
            treqpath.AddKeyword("dayIndex",pDayIndex.ToString());

            IAPHttp.Instance.StartGetResponse<DefaultResponse<ActivityInfo>>(treqpath.GetRequestUrl(), null,
                (response, error, errorcode) =>
                {
                    CallOnCompleteAndCache($"Activity-{pActivityId}", response, onComplete, errorcode, error);
                }, 60,
                IAPHttp.IsParamsValid(uid, tid));
        }
        
        /// <summary>
        /// Get level rewards
        /// </summary>
        /// <param name="pActivityId">activity id</param>
        /// <param name="pLevel">level</param>
        /// <param name="pDayIndex">dayIndex</param>
        /// <param name="onComplete">callback</param>
        /// <returns></returns>
        public void GetLevelReward(string pActivityId, string pLevel,int pDayIndex, HttpEvent<ActivityInfo> onComplete)
        {
            var tbody = new
            {
                level = pLevel,
                dayIndex = pDayIndex,
            };

            SendActivityAction(pActivityId, tbody, "receiveProgressAward", onComplete);
        }
        
        /// <summary>
        /// Get task rewards
        /// </summary>
        /// <param name="pActivityId">activity id</param>
        /// <param name="pDayIndex">dayIndex</param>
        /// <param name="pTaskId">task id</param>
        /// <param name="onComplete">callback</param>
        /// <returns></returns>
        public void GetTaskReward(string pActivityId, int pDayIndex, string pTaskId, HttpEvent<ActivityInfo> onComplete)
        {
            var tbody = new
            {
                dayIndex = pDayIndex,
                taskId = pTaskId,
            };

            SendActivityAction(pActivityId, tbody, "receiveTaskAward", onComplete);
        }
        
        /// <summary>
        /// activityExchange
        /// </summary>
        /// <param name="pActivityId">activity id</param>
        /// <param name="pExchange">task id</param>
        /// <param name="pDayIndex">dayIndex</param>
        /// <param name="onComplete">callback</param>
        /// <returns></returns>
        public void UpdateActivityExchange(string pActivityId, string pExchange, int pDayIndex, HttpEvent<ActivityInfo> onComplete)
        {
            var tbody = new
            {
                exchangeId = pExchange,
                dayIndex = pDayIndex,
            };

            SendActivityAction(pActivityId, tbody, "activityExchange", onComplete);
        }

        protected void SendActivityAction(string id, object pBody, string pAction, HttpEvent<ActivityInfo> onComplete)
        {
            var uid = IAPHttp.EscapeURL(Setting.userId);
            var tid = IAPHttp.EscapeURL(id);
            RequestPathObject treqpath = new RequestPathObject($"users/{uid}/activities/{tid}");
            treqpath.AddKeyword("action", pAction);

            IAPHttp.Instance.StartPatchSend<DefaultResponse<ActivityInfo>>(treqpath.GetRequestUrl(), pBody,
                (response, error, errorcode) => { CallOnComplete(response, onComplete, errorcode, error); }, 60,
                IAPHttp.IsParamsValid(uid, tid));
        }
    }
}