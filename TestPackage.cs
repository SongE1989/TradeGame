using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TestPackeageSp
{
    //http://blog.csdn.net/zs234/article/details/7487960

    public class TestPackage
    {
        Fruit[] fruitArr = new Fruit[]{  
                new Fruit("李子",4,4500),  
                new Fruit("苹果",5,5700),  
                new Fruit("橘子",2,2250),  
                new Fruit("草莓",1,1100),  
                new Fruit("甜瓜",6,6700)  
            };
        int maxVol = 8;
        List<State> stateArr = new List<State>();

        public void check()
        {
            for (int i = 0; i < maxVol; i++)
            {
                stateArr.Add(null);
            }

            for (int i = 0, fruitNum = fruitArr.Length; i < fruitNum; i++)
            {
                for (int vol = 0; vol < maxVol; vol++)
                {
                    Fruit curFruit = fruitArr[i];
                    State exState = (vol - 1) >= 0 ? stateArr[vol - 1] : null;
                    State curState = stateArr[vol];

                    if (exState != null)
                    {
                        if (curState != null)
                        {
                            int leftVol = vol - exState.curVol;
                            if (leftVol >= curFruit.vol)
                            {
                                State newState = new State(exState.GetNextVol(), exState.GetNextValue(), curFruit);
                                if (newState.GetNextValue() > curState.GetNextValue())
                                    stateArr[vol] = newState;
                            }
                        }
                        else
                        {
                        }
                    }
                    else
                    {

                    }

                }
            }
        }
    }

    public class State
    {
        public int curVol;
        public int curValue;
        public Fruit addFruit;

        public State(int curVol, int curValue, Fruit addFruit)
        {
            this.curVol = curVol;
            this.curValue = curValue;
            this.addFruit = addFruit;
        }

        public int GetNextValue()
        {
            return curValue + addFruit.value;
        }

        public int GetNextVol()
        {
            return curVol + addFruit.vol;
        }
    }

    public class Fruit
    {
        public string name;
        public int vol;
        public int value;

        public Fruit(string name, int vol, int value)
        {
            this.name = name;
            this.vol = vol;
            this.value = value;
        }
    }

}