using System.Collections.Generic;
using UnityEngine;

//획득한 아이템을 관리
public class InvenManager
{
    public Dictionary<string, Item> mItems = new();

    public void GetItem(Item_Scriptable _data)
    {
        if(mItems.ContainsKey(_data.name))
        {
            mItems[_data.name].Count++;
        }
        else
        {
            mItems.Add(_data.name,new Item{data = _data, Count = 1});            
        }
    }
 
}
