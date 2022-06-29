using UnityEngine.Experimental.GlobalIllumination;

namespace Habby.Business.Data
{
    public class ActivityInfo
    {
        public string activityId;
        public string activityName;
        public string activityType;//活动类型 newPlayer、holiday、totalPay

        public long startTime;
        public long endTime;
        public int currentActivityValue;//当前活动值
        public int currentTotalPayValue;//当前活动累计充值

        public ActivityLevel[] activityLevels;// 活动进度的各个等级
        public ActivityExchange[] activityExchanges;//活动兑换内容
        public ActivityTaskGroup[] taskGroups;//任务组
    }

    public class ActivityLevel
    {
        public int level;
        public int activityValue;// 所需数值
        public float activityPayValue;// 累计充值支付值，只针对累计充值活动
        public long receivedTime;
        public bool receivable;// 是否可以领取
        public bool received;// 是否已经领取

        public GameItem[] targetItems;// 领取内容
    }

    public class ActivityExchange
    {
        public string exchangeId;//兑换id编号
        public int maxCount;// 最大兑换次数，节日活动
        public int currentCount;//当前兑换次数
        public long exchangedTime;
        public GameItem[] sourceItems;// 兑换所需物品
        public GameItem[] targetItems;// 兑换目标物品
    }

    public class ActivityTaskGroup
    {
        public int dayIndex;
        public long startTime;
        public long endTime;

        public ActivityTask[] tasks;
    }

    public class ActivityTask
    {
        public string taskId;// 任务编号
        public string source;// 任务内容
        public int sourceValue;// 任务内容值
        public int activityValue;//奖励数值
        public int currentSourceValue;//当前任务的值
        public long receivedTime;//领取时间
        public bool receivable;// 是否可以领取
        public bool received;// 是否已经领取
        public GameItem[] awards;//奖励物品
    }
}