
# HabbyIAP
## Import
### 1.Import UnityIAP

Window->Package Manager

Select (In App Purchasing)

Install 4.1.4

### 2.Import HabbyIAP

Assets->import Package->Custom Package...

select HabbyIAP.unitypackage


## Example

Assets/HabbySDK/HabbyIAP/Example/IAPSample.unity

## Initialize

```c#
        using Habby.Business;


        var productList = new List<ProductObject>();
        productList.AddRange(products);

        var tsetting = new IAPSetting()
        {
            serverUrl = "http://test-business.kinjarun.com",
            userId = "33225478",
        };

        Habby.Business.IAPManager.Instance.InitIAP(tsetting,productList);

        Habby.Business.IAPManager.Instance.OnPurchaseComplete += (item,last) =>
        {
            //OnComplete
            //Rewards are now available
            if(last != null )
            {
                //Current Purchased Item
            }
        };
        
        Habby.Business.IAPManager.Instance.OnPurchaseStart += () =>
        {
            //onStart
        };
        
        Habby.Business.IAPManager.Instance.OnPurchaseEnd += (codetype ,statuscode,last) =>
        {
            if (codetype == PurchaseCode.sucess)
            {
                //success
            }
            else
            {
                //error
            }
        };
    }

```


## Event

```c#
        public delegate void PurchaseEvent(Product product,ProductObject last);
        public delegate void PurchaseProcessStart();
        public delegate void PurchaseProcessEnd(PurchaseCode purchaseCode,int statuCode, ProductObject last);

        public event PurchaseEvent OnPurchaseComplete;
        public event PurchaseProcessStart OnPurchaseStart;
        public event PurchaseProcessEnd OnPurchaseEnd;
```

## Module

- Pruduct Type

```c#
  store,
  gift,
  piggyBank,
  battlePass,
  ordinary,
```

### BattlePass
```c#
  public HabbyBusinessBattlePass battlePass;
```

- Level Type
```c#
  normal,
  additional
```

- Pass Type
```c#
  free,
  paid
```

### Gift
```c#
   public HabbyBusinessGift gift;
```

- Gift Type
```c#
  period,
  activity,
  push,
```

- Sub Type
```c#
  daily,
  weekly,
  monthly,
```

### PiggyBank
```c#
   public HabbyBusinessPiggyBank piggyBank;
```


### Store
```c#
   public HabbyBusinessStore store;
```

### Ordinary
```c#
   public HabbyBusinessOrdinary Ordinary;
```


## API

- All Products (befor initialize)
```c#
         IAPManager.Instance.GetAllProducts(url,userid,(code,msg,obj)=>
         {
         
         });
```

- Init
```c#
        var productList = new List<ProductObject>();
        productList.AddRange(products);

        var tsetting = new IAPSetting()
        {
            serverUrl = "http://test-business.kinjarun.com",
            userId = "33225478",
        };

        Habby.Business.IAPManager.Instance.InitIAP(tsetting,productList);
```

- Purchase

```c#
        var item = new ProductObject();
        item.gameItemId = "test_1";
        item.storeId = "test_1";//markProductId
        item.productType = ProductType.Consumable;

        Habby.Business.IAPManager.Instance.DoIAPPurchase(item);
```

- Get Product

```c#
       var item = Habby.Business.IAPManager.Instance.WithID(markProductId);
```



### Module API

- Get Products
```c#
       var item = Habby.Business.IAPManager.Instance.Store.GetProducts(ids,showall,(code,msg,obj)=>
       {
       
       });
```


#### BattlePass


- GetUserBattlePass
```c#
       var item = Habby.Business.IAPManager.Instance.BattlePass.GetUserBattlePass(type,(code,msg,obj)=>
       {
       
       });
```

- Update Value
```c#
       var item = Habby.Business.IAPManager.Instance.BattlePass.UpdateBattlePass(id,value,(code,msg,obj)=>
       {
       
       });
```

- Get Reward
```c#
       var item = Habby.Business.IAPManager.Instance.BattlePass.GetReward(id,passlevel,gameProductId,(code,msg,obj)=>
       {
       
       });
```

- Get Addition 
```c#
       var item = Habby.Business.IAPManager.Instance.BattlePass.GetAdditionReward(id,gameProductId,(code,msg,obj)=>
       {
       
       });
```


#### Gift

- Get List
```c#
       var item = Habby.Business.IAPManager.Instance.Gift.GetList(type,subType,(code,msg,obj)=>
       {
       
       });
```

- Get info
```c#
       var item = Habby.Business.IAPManager.Instance.Gift.GetInfos(ids,(code,msg,obj)=>
       {
       
       });
```


- Mark Display
```c#
       var item = Habby.Business.IAPManager.Instance.Gift.MarkDisplay(id,(code,msg,obj)=>
       {
       
       });
```

#### PiggyBank

- Get Info
```c#
       var item = Habby.Business.IAPManager.Instance.PiggyBank.GetInfos(ids,(code,msg,obj)=>
       {
       
       });
```


#### Store

- Get All Products
```c#
       var item = Habby.Business.IAPManager.Instance.Store.GetList((code,msg,obj)=>
       {
       
       });
```

#### Ordinary

- Get All Products
```c#
       var item = Habby.Business.IAPManager.Instance.Ordinary.GetList((code,msg,obj)=>
       {
       
       });
```


#### Activity

- GetActivity
```c#
       var item = Habby.Business.IAPManager.Instance.Activity.GetActivity(id,(code,msg,obj)=>
       {
       
       });
```

- GetLevelReward
```c#
       var item = Habby.Business.IAPManager.Instance.Activity.GetLevelReward(id,level,(code,msg,obj)=>
       {
       
       });
```


- GetTaskReward
```c#
       var item = Habby.Business.IAPManager.Instance.Activity.GetTaskReward(id,dayindex,taskid,(code,msg,obj)=>
       {
       
       });
```

- UpdateActivityExchange
```c#
       var item = Habby.Business.IAPManager.Instance.Activity.UpdateActivityExchange(id,exchange,(code,msg,obj)=>
       {
       
       });
```



## PurchaseCode

```c#
        public enum PurchaseCode
        {
            sucess = 0,
            channelIAPError,
            serverError,
            localError,
        }
 ```

### StatusCode 

- **channelIAPError**

```c#
        public enum PurchaseFailureReason
        {
            /// <summary>
            /// Purchasing may be disabled in security settings.
            /// </summary>
            PurchasingUnavailable = 0,

            /// <summary>
            /// Another purchase is already in progress.
            /// </summary>
            ExistingPurchasePending,

            /// <summary>
            /// The product was reported unavailable by the purchasing system.
            /// </summary>
            ProductUnavailable,

            /// <summary>
            /// Signature validation of the purchase's receipt failed.
            /// </summary>
            SignatureInvalid,

            /// <summary>
            /// The user opted to cancel rather than proceed with the purchase.
            /// This is not specified on platforms that do not distinguish
            /// cancellation from other failure (Amazon, Microsoft).
            /// </summary>
            UserCancelled,

            /// <summary>
            /// There was a problem with the payment.
            /// This is unique to Apple platforms.
            /// </summary>
            PaymentDeclined,

            /// <summary>
            /// The transaction has already been completed successfully. This error can occur
            /// on Apple platforms if the transaction is finished successfully while the user
            /// is logged out of the app store, using a receipt generated while the user was
            /// logged in.
            /// </summary>
            DuplicateTransaction,

            /// <summary>
            /// A catch all for remaining purchase problems.
            /// Note: Use Enum.Parse to use this named constant if targeting Unity 5.3
            /// or 5.4. Its value differs for 5.5+ which introduced DuplicateTransaction.
            /// </summary>
            Unknown
        }

```

- **serverError**

    See postman 1.Creat Orders. 2. Verify Order

    > https://habbydev.postman.co/workspace/KinjaRun~652b2966-7747-4b03-87bd-cfe3d18a0cd0/example/18060313-feb2c632-8827-4e53-abb6-d7764942a6b0

- **localError**

    -1

    