using System.Collections.Generic;
using UnityEngine.Purchasing;

namespace Habby.Business.Data
{
    
    public class GameItem
    {
        public string itemId;
        public string itemType;
        public int count;
        public Dictionary<string, string> customFields;
    }
    
    public class GameAttributes
    {
        public string propertyId;
        public int value;
    }

    public class GameProductItem
    {
        public string gameProductId; // id
        public string gameProductType;// GameProductType ordinary-普通商品,
        public string marketProductId;// storeId
        public string marketProductType; // store product type MarketProductType

        public int purchasedCount;//购买次数
        public int purchasedLimit;//上限次数
        
        public GameItem[] items;//道具
        public GameItem[] bonuses;// 奖金
        public GameAttributes[] attributes;//属性
    }

    public class BusinessModuleDataBase
    {
        public Dictionary<string, string> uiFields;//ui字段
    }

    public enum GameProductType
    {
        battlePass,
        piggyBank,
        gift,
        store,
        ordinary,
    }

    public enum MarketProductType
    {
        consumption,
        nonConsumption,
        renewableSubscription,
    }
}