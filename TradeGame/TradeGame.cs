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
        }
    }

    public class TestTradeScene
    {
        public Dictionary<string, float> SellOrderDic = new Dictionary<string, float>();

        public void MakeDeal(Order sell, Order buy)
        {
            if (!sell.isBuy && buy.isBuy && sell.Price == buy.Price)
            {
                float dealNum = Mathf.Min(sell.num, buy.num);
                sell.num = sell.num - dealNum;
                buy.num = buy.num - dealNum;
                sell.onDeal(sell, dealNum);
                buy.onDeal(buy, dealNum);

            }
            else
                Debug.Log("交易不正确!");

        }
    }

    public class Order
    {
        public bool isBuy;
        public string Name;
        public float Price;
        public float num;
        public Action<Order, float> onDeal;
    }

    public interface ITrader
    {
        Action<Order, float> OnOrderDeal { get; }
    }


    public class Town : MonoBehaviour,IJsonData, ITrader
    {
        //TODO 添加市场
        public string TownName;

        public Dictionary<string, float> ContainerDic = new Dictionary<string, float>();
        public Dictionary<string, float> ProductDic = new Dictionary<string, float>();
        public Dictionary<string, float> ConsumeDic = new Dictionary<string, float>();

        #region ITrader
        public Action<Order, float> OnOrderDeal { get { return onOrderDeal; } }

        void onOrderDeal(Order order, float num)
        {
            if(order.isBuy)
                ContainerDic[order.Name] += order.num;
            else
                ContainerDic["money"] += order.num * order.Price;
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

