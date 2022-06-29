using System.Collections.Generic;

namespace Habby.Business.Data
{
    public class GiftItem : GameItem
    {

    }
    public class GiftProduct : GameProductItem
    {

    }

    public class GiftInfo : BusinessModuleDataBase
    {
        public int priority;//优先级
        
        public string giftType;//类型
        public string giftSubType;//子类型

        public long startTime;//开始时间
        public long endTime;//结束时间
        public long refreshTime;//刷新时间
        public long durationSecond;// //针对推送礼包，持续展示时间秒级
        
        public GiftProduct gameProduct;
    }
    
    
    // public enum GiftType
    // {
    //     period,
    //     activity,
    //     push,
    // }
    //
    // public enum GiftSubType
    // {
    //     daily,
    //     weekly,
    //     monthly,
    // }
}