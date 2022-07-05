using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Habby.Business;
using UnityEngine.Purchasing;
using Habby.Business;
using Habby.Business.Data;
using Habby.Tool;
using Newtonsoft.Json.Linq;

namespace Habby.Business
{
    public class IAPTest : MonoBehaviour
    {
        public Button temp;

        public RectTransform list;

        // Start is called before the first frame update

        public ProductObject[] products;
        private void Awake()
        {


            string st = "{\"items\":[{\"itemId\":\"222\",\"value\":0},{\"itemId\":\"333\",\"value\":0}]}";

            OrdinaryProduct tj = DataConvert.FromJson<OrdinaryProduct>(st);
            
            var tjobj = new JObject();
            tjobj.Add("code",123);
            if (tjobj != null)
            {
                var tcode = tjobj["code"];

                if (tcode != null)
                {
                    int tt = tcode.Value<int>();
                    Debug.LogError(tt);
                }
            }

            temp.gameObject.SetActive(false);
            for (int i = 0; i < products.Length; i++)
            {
                var item = GameObject.Instantiate(temp.gameObject).GetComponent<Button>();
                var tname = item.transform.GetComponentInChildren<Text>();
                tname.text = products[i].storeId;
                int index = i;
                item.onClick.AddListener(() =>
                {
                    OnBuyClick(index);
                });
                item.gameObject.SetActive(true);

                item.transform.SetParent(list);
            }
            initPur();

            TestMethod();
        }

        void initPur()
        {
            var productList = new List<ProductObject>();
            // var test1 = new ProductObject() { id = "test_1", productType = ProductType.Consumable };
            // var test2 = new ProductObject() { id = "test_2", productType = ProductType.NonConsumable };
            // var test3 = new ProductObject() { id = "test_3", productType = ProductType.Subscription };

            productList.AddRange(products);

            var tsetting = new IAPSetting()
            {
                serverUrl = "https://test-business.kinjarun.com",
                userId = "1",
            };

            Habby.Business.IAPManager.Instance.InitIAP(tsetting, productList);

            Habby.Business.IAPManager.Instance.OnPurchaseComplete += (item, last) =>
            {
            //OnComplete
            //Rewards are now available
                if (last != null)
                {
                //Current Purchased Item
                }
            };

            Habby.Business.IAPManager.Instance.OnPurchaseStart += () =>
            {
            //onStart
            };

            Habby.Business.IAPManager.Instance.OnPurchaseEnd += (codetype, statuscode, last) =>
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

            var tlist = Habby.Business.IAPManager.Instance.GetList();
            
            Debug.Log(tlist);
            
            Habby.Business.IAPManager.Instance.ClearOrderFiles();
            
        }

        void TestMethod()
        {
            TestGift();

            //TestOrdinary();
        }

        void TestBattlePass()
        {
            IAPManager.Instance.BattlePass.GetUserBattlePass("",(a,b,c) =>
            {
                
            });
        }

        void TestGift()
        {
            // IAPManager.Instance.Gift.GetList("period","daily",(a,b,c) =>
            // {
            //
            // });
            
            IAPManager.Instance.Gift.MarkDisplay("1",(a,b,c) =>
            {

            });
        }

        void TestPiggy()
        {
            IAPManager.Instance.PiggyBank.GetInfos((a,b,c) =>
            {
                
            });
        }

        void TestStore()
        {
            IAPManager.Instance.Store.GetList((a,b,c) =>
            {
                
            });
        }
        
        void TestOrdinary()
        {
            IAPManager.Instance.Ordinary.GetList((a,b,c) =>
            {
                
            });
        }

        void OnBuyClick(int pIndex)
        {
            var item = products[pIndex];
            Habby.Business.IAPManager.Instance.DoIAPPurchase(item);
        }
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }


}
