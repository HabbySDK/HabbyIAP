# Changelog

## [1.0.2] - 2022-07-6

### Activity Module
- Add the "dayIndex" parameter for all activity API
```c#
    public void GetActivity(string pActivityId,int pDayIndex, HttpEvent<ActivityInfo> onComplete)
    public void GetLevelReward(string pActivityId, string pLevel,int pDayIndex, HttpEvent<ActivityInfo> onComplete)
    public void UpdateActivityExchange(string pActivityId, string pExchange, int pDayIndex, HttpEvent<ActivityInfo> onComplete)
```


## [1.0.1] - 2022-07-6

### Add Field
- piggyBank add endTime.
```json
{
    "code": 0,
    "message": "success",
    "data": [
        {
            "round": 1,
            "userId": "tester8",
            "items": [
            ],
            "full": false,
            "currentValue": 20,
            "endTime":1111111111     //新增字段。---------------------
            "initValue": 0,
            "maxValue": 100,
            "piggyBankId": "Piggy001",
            "status": "collecting",
            "purchasable": true
            //省略其他字段
        }
    ]
}
```
- battlepass add levelValue,maxValue,levelMaxValue

```json
{
    "code": 0,
    "message": "success",
    "data": [
        {
            "currentValue": 0,
            "currentLevel": 1,
            "maxValue":1000,   //battlePass经验上限-------------------------
            "levels": [
                {
                    "level": 1,
                    "type": "normal",
                    "startValue": 0,
                    "levelValue":5,  //记录每个level的进度，新增字段-----------
                    "levelMaxValue":11,//每个level进度的最大值，新增字段-----------
                    "passes": [     
                        {
                            "type": "free",
                            "items": [
                                {
                                    "itemId": "101",
                                    "value": 10
                                }
                            ],
                            "receivable": true,
                            "received": false
                        },
                        {
                            "type": "paid",
                            "items": [
                                {
                                    "itemId": "101",
                                    "value": 20
                                }
                            ],
                            "gameProductId": "11111",
                            "receivable": true,
                            "received": false
                        }
                    ]
                }
            ]
        }
    ]
}
```

## [1.0.0] - 2022-05-06
release1.0.0