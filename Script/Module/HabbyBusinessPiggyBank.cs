using Habby.Tool.Http;
using Habby.Business.Data;
using Habby.Tool;
using Habby.Tool.Http.Tool;

namespace Habby.Business
{
    public class HabbyBusinessPiggyBank : HabbyBusinessBase<PiggyBankProduct>, IHabbyBusiness
    {
        public override string productsKey => "piggyBankProductIds";

        public HabbyBusinessPiggyBank(IAPSetting pSetting) : base(pSetting)
        {
        }

        public void GetInfos(HttpEvent<PiggyBankInfo[]> onComplete)
        {
            var uid = IAPHttp.EscapeURL(Setting.userId);

            RequestPathObject treqpath = new RequestPathObject(Setting.serverUrl, $"users/{uid}/piggyBanks");

            IAPHttp.Instance.StartGetResponse<DefaultResponse<PiggyBankInfo[]>>(treqpath.GetRequestUrl(), null,
                (response, error, errorcode) => { CallOnCompleteAndCache($"{productsKey}-listInfos" ,response, onComplete, errorcode, error); }, 60,
                IAPHttp.IsParamsValid(uid));
        }
        
    }
}