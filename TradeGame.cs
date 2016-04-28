using UnityEngine;
using System.Collections;
using System.IO;
using LitJson;
using System;
using System.Collections.Generic;

namespace TradeGameNS
{
    public class TradeGame : MonoBehaviour
    {
        string dataPath = "E:/SVN/WorldEditor/WorldEditor53/Assets/TradeGame/TownData.txt";

        List<Town> TheTownList = new List<Town>();
        TestTradeScene market = new TestTradeScene();

        void Start()
        {
            //读取数据
            string saveStr = File.ReadAllText(dataPath);
            JsonData saveJD = JsonMapper.ToObject(saveStr);
            JsonData townJD = saveJD["TownData"];
            for (int i = 0, length = townJD.Count; i < length; i++)
            {
                Town town = new GameObject().AddComponent<Town>();
                town.ReadJsonData(townJD[i]);
                TheTownList.Add(town);
            }

            TheTownList[0].addOrder(market, "A", 100, 20, true);
            TheTownList[0].addOrder(market, "A", 90, 20, true);
            TheTownList[0].addOrder(market, "A", 80, 20, true);

            TheTownList[1].addOrder(market, "A", 115, 20, false);
            TheTownList[1].addOrder(market, "A", 105, 20, false);
            TheTownList[1].addOrder(market, "A", 95, 20, false);

        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                for (int i = 0, length = TheTownList.Count; i < length; i++)
                {
                    TheTownList[i].Pass();
                    Debug.Log(TheTownList[i].ToJsonData().ToJson());
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                market.MakeAllDeal();
            }
        }
    }

    public interface IMarket
    {
        bool AddOrder(Order order);
        void MakeAllDeal();
    }

    public class TestTradeScene : IMarket
    {
        public Dictionary<string, List<Order>> SellOrderDic = new Dictionary<string, List<Order>>();
        public Dictionary<string, List<Order>> BuyOrderDic = new Dictionary<string, List<Order>>();

        public void MakeAllDeal()
        {
            foreach (var orderName in SellOrderDic.Keys)
            {
                Debug.Log("Trade: " + orderName + " -------");
                List<Order> sellList = SellOrderDic[orderName];
                List<Order> buyList = BuyOrderDic[orderName];
                sellList.Sort();
                buyList.Sort();
                dealOrderList(sellList, buyList);
            }
        }

        void dealOrderList(List<Order> sellList, List<Order> buyList)
        {
            if (sellList.Count > 1 && buyList.Count > 1)
            {
                Order sellOrder = sellList[0];
                Order buyOrder = buyList[0];
                Debug.Log("sell: " + sellOrder.ToString());
                Debug.Log("buy: " + buyOrder.ToString());
                if (sellOrder.Price > buyOrder.Price)
                {
                    Debug.Log("------deal end");
                    return;
                }
                else
                {
                    makeDeal(sellOrder, buyOrder);

                    if (sellOrder.Num == 0)
                        sellList.Remove(sellOrder);
                    if (buyOrder.Num == 0)
                        buyList.Remove(buyOrder);

                    dealOrderList(sellList, buyList);
                }
            }
        }

        void makeDeal(Order sell, Order buy)
        {
            if (!sell.IsBuy && buy.IsBuy && sell.Price <= buy.Price)
            {
                float dealNum = Mathf.Min(sell.Num, buy.Num);
                float dealPrice = sell.Price / 2f + buy.Price / 2f;
                sell.Num = sell.Num - dealNum;
                buy.Num = buy.Num - dealNum;
                sell.OnDeal(sell, dealPrice, dealNum);
                buy.OnDeal(buy, dealPrice, dealNum);
                Debug.Log("deal at price: "+sell.Price + " num: "+dealNum);
            }
            else
                Debug.Log("deal不正确!");
        }

        public bool AddOrder(Order order)
        {
            if (order.IsBuy)
                BuyOrderDic.AddToList(order.GoodName, order);
            else
                SellOrderDic.AddToList(order.GoodName, order);
            return true;
        }

        public float getCurSellPrice(string goodName)
        {
            if(SellOrderDic.ContainsKey(goodName))
            {

            }
            else
                return float.MaxValue;
            return 0;
        }
        public float getCurBuyPrice(string goodName)
        {
            return 0;
        }
    }

    public class Order :IComparable<Order>
    {
        public string GoodName;
        public float Price;
        public float Num;
        public bool IsBuy;
        public Action<Order, float, float> OnDeal;

        public Order(string goodName, float price, float num, bool isBuy, Action<Order, float, float> onDeal)
        {
            this.GoodName = goodName;
            this.Price = price;
            this.Num = num;
            this.IsBuy = isBuy;
            this.OnDeal = onDeal;
        }

        public int CompareTo(Order other)
        {
            if (IsBuy)
                return (int)(other.Price - Price);
            else
                return (int)(Price - other.Price);
        }

        public override string ToString()
        {
            string str1 = IsBuy ? "buy" : "sell";
            return str1 + " price: "+Price +" num: "+Num;
        }
    }

    public interface ITrader
    {
        Action<Order, float, float> OnOrderDeal { get; }
    }



    public class Town : MonoBehaviour,IJsonData, ITrader
    {
        public string TownName;

        public Dictionary<string, float> ContainerDic = new Dictionary<string, float>();
        public Dictionary<string, float> ProductDic = new Dictionary<string, float>();
        public Dictionary<string, float> ConsumeDic = new Dictionary<string, float>();

        #region ITrader
        public Action<Order, float, float> OnOrderDeal { get { return onOrderDeal; } }

        void onOrderDeal(Order order, float price, float num)
        {
            if(order.IsBuy)
                ContainerDic[order.GoodName] += order.Num;
            else
                ContainerDic["money"] += order.Num * price;
        }

        public void addOrder(IMarket market, string goodName, float price, float num, bool isBuy)
        {
            if(isBuy)
            {
                if(ContainerDic["money"] >= price * num)
                {
                    ContainerDic["money"] -= price * num;
                    market.AddOrder(new Order(goodName, price, num, isBuy, onOrderDeal));
                }
                else
                {
                    Debug.Log("X!");
                    return;
                }
            }
            else
            {
                if (ContainerDic[goodName] >= num)
                {
                    ContainerDic[goodName] -= num;
                    market.AddOrder(new Order(goodName, price, num, isBuy, onOrderDeal));
                }
                else
                {
                    Debug.Log("X!");
                    return;
                }
            }
        }


        #endregion

        #region IJSonData

        public IJsonData ReadJsonData(JsonData jd)
        {
            TownName = jd.ReadString("TownName", "DefaultTownName");
            ContainerDic = jd.ReadDic<string, float>("ContainerDic");
            ProductDic = jd.ReadDic<string, float>("ProductDic");
            ConsumeDic = jd.ReadDic<string, float>("ConsumeDic");
            return this;
        }

        public JsonData ToJsonData()
        {
            JsonData jd = new JsonData();
            jd["TownName"] = TownName;
            jd["ContainerDic"] = ContainerDic.ToJsonData<float>();
            jd["ProductDic"] = ProductDic.ToJsonData<float>();
            jd["ConsumeDic"] = ConsumeDic.ToJsonData<float>();
            return jd;
        }
        #endregion

        public void Pass()
        {
            JsonUtils.DicChange(ContainerDic, ProductDic, true);
            JsonUtils.DicChange(ContainerDic, ConsumeDic, false);
        }
    }



}

