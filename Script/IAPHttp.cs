
using Habby.Tool;
using Habby.Tool.Http;
namespace Habby.Business
{
    public class IAPHttp : HttpManager<IAPHttp>
    {
        public IAPHttp()
        {
            Tag = "IAPHttp";
            SetPublicHeader("SDKVersion",IAPManager.SDKVersion);
        }
    }
}