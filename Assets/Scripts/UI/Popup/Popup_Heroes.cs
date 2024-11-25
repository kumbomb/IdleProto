using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class Popup_Heroes : UI_Base
{
    [SerializeField] Transform Content;
    [SerializeField] GameObject heroItem;
    [SerializeField] Button closeBtn;

    Dictionary<string, Character_Scriptable> m_CharDic = new Dictionary<string, Character_Scriptable>();

    public override bool Init()
    {  
        var Data = Resources.LoadAll<Character_Scriptable>("Scriptable");
        for(int i=0;i<Data.Length;i++)
        {
            m_CharDic.Add(Data[i].charcterName, Data[i]);
        }
        //캐릭터 등급별 정렬
        var sortDic = m_CharDic.OrderByDescending(x=>x.Value.mRarity);
        foreach(var data in sortDic)
        {
            var item = Instantiate(heroItem, Content,transform);
            item.GetComponent<Item_Heroes>().Initialize(data.Value);
            item.transform.localScale = Vector3.one;
        }

        closeBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.AddListener(DisableObj);

        return base.Init();
    }

    public override void DisableObj()
    {
        base.DisableObj();
    }
}
