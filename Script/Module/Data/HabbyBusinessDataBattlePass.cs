namespace Habby.Business.Data
{
    public enum BattlePassPassItemType
    {
        free,
        paid
    }
    
    public enum BattlePassLevelType
    {
        normal,
        additional,
    }
    
    public class BattleProduct : GameProductItem
    {
            
    }

    public class BattlePassAddition
    {
        public string gameProductId;
        
        public int maxTriggerNums;//最大可领取次数
        public int triggerTimes;//已经触发了几次
        public int receiveTimes;//已经领取了几次
        public int canReceiveNums;//当前可领取次数
        
        public GameItem[] items;
    }
    
    public class BattlePassInfo : BusinessModuleDataBase
    {
        #region class
        
        public class BattleLevel
        {
            public class PassItem
            {
                public string type;// 类型：free、paid(付费) BattlePassPassItemType
                public bool receivable;// 是否可以领取
                public bool received;// 是否已经领取
                public GameItem[] items;//奖励
                    
                public string gameProductId;// 商品 ID
            }
                
            public int level;
            public string type;// 一般前30级为normal，后面为additional(补充奖励)
            public long startValue;// 开启当前等级所需的经验

            public PassItem[] pass;
        }

        #endregion
        
        public string battlePassId;//id
        public string battlePassType;//类型
        public int nextLevelGems; //开启下一层需要的钻石数
        public long startTime;// 开始时间
        public long endTime; // 结束时间
        public long currentValue;// 当前经验
        public int currentLevel;// 当前等级
        public bool purchased;// 是否已经购买

        public BattlePassAddition[] additionLevel;///额外的层级。付费的battlePass大于30级有额外的循环奖励
        public BattleLevel[] levels; //级别列表
        public BattleProduct[] gameProducts;//包含的商品列表
    }
    
}