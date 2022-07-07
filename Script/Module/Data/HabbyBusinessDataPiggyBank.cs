namespace Habby.Business.Data
{
    public class PiggyBankItem : GameItem
    {

    }
    public class PiggyBankProduct : GameProductItem
    {
    }
    
    public class PiggyBankInfo : BusinessModuleDataBase
    {
        public string piggyBankId;//id
        public string status;//状态 结束"end"
        
        public long initValue; // 存钱罐的初始值
        public long maxValue;// 存钱罐的最大值
        public long currentValue;// 当前值
        public long baseValue;////基数值，用来计算当前是几倍超值
        
        public long createTime;//创建时间
        public long endTime;//结束时间 7.6
        public bool purchasable;// 是否可以购买
        public int round;// 第几轮
        
        public PiggyBankItem[] items;//物品
        public PiggyBankProduct gameProduct;//商品信息
        
    }
}