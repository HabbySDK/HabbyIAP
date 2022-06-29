using Habby.Business.Data;
using Habby.Tool.Http.Tool;

namespace Habby.Business
{
    public delegate void HttpEvent<T>(int code, string message, T pObject);
    public class DefaultResponse<T> : BaseResponse<T>
    {

    }

    public class IAPGetOrderResponse : DefaultResponse<IAPGetOrderResponse.Data>
    {
        public class Data
        {
            public string orderId;
        }
    }
    
    
    public class ValidatinoData
    {
        public string orderId;
        public string gameProductId;
        public string customData;
        public GameItem[] items;
    }
    public class IAPValidatinoResponse : DefaultResponse<ValidatinoData>
    {
        
    }
}