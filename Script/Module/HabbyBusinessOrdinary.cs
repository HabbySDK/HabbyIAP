using Habby.Tool.Http;
using Habby.Business.Data;
using Habby.Tool.Http.Tool;
namespace Habby.Business
{
    public class HabbyBusinessOrdinary : HabbyBusinessBase<OrdinaryInfo>, IHabbyBusiness
    {

        public override string productsKey => "ordinaryProductIds";
        public HabbyBusinessOrdinary(IAPSetting pSetting) : base(pSetting)
        {

        }

        public void GetList(HttpEvent<OrdinaryInfo[]> onComplete)
        {
            GetProducts(null, true, (code, msg, responseData) =>
            {
                CallEventAndCache(productsKey + "-list",code, msg, responseData, onComplete);
            });
        }
    }
}