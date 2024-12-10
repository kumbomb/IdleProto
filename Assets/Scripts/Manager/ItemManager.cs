using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[System.Serializable]
public class Item
{
    public Item_Scriptable data;
    public int Count;
}


//BaseManager에서 전체 관리 
public class ItemManager
{
    Dictionary<string, Item_Scriptable> Items_Datas = new();

    public void Init()
    {
        var datas = Resources.LoadAll<Item_Scriptable>("Scriptable/Item");
        for(int i=0;i<datas.Length;i++)
        {
            Items_Datas.Add(datas[i].name, datas[i]);
        }
    }

    public List<Item_Scriptable> GetDropSet()
    {   
        List<Item_Scriptable> dropSet = new();
        foreach(var item in Items_Datas)
        {
            float valueCount = Random.Range(0f,100f);
            if(valueCount <= item.Value.dropRate)
            {
                dropSet.Add(item.Value);
            }
        }
        return dropSet;
    }
}
