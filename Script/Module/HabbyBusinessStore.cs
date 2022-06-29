using Habby.Tool.Http;
using Habby.Business.Data;
using Habby.Tool.Http.Tool;
namespace Habby.Business
{
    public class HabbyBusinessStore : HabbyBusinessBase<StoreInfo>, IHabbyBusiness
    {

        public override string productsKey => "storeProductIds";
        public HabbyBusinessStore(IAPSetting pSetting) : base(pSetting)
        {

        }
        
        public void GetList(HttpEvent<StoreInfo[]> onComplete)
        {
            GetProducts(null, true, (code, msg, responseData) =>
            {
                CallEventAndCache(productsKey + "-list",code, msg, responseData, onComplete);
            });
        }
    }
}