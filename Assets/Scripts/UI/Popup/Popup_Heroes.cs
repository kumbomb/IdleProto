using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class Popup_Heroes : UI_Base
{
    [SerializeField] Transform Content;
    [SerializeField] GameObject heroItem;
    [SerializeField] Button closeBtn;

    [SerializeField] GameObject Parts;
    List<Item_Heroes> heroParts = new();

    Dictionary<string, Character_Scriptable> m_CharDic = new Dictionary<string, Character_Scriptable>();
    Character_Scriptable mSelectChar;
    public override bool Init()
    {  
        InitPlaceBtns();
        RenderManager.instance.heroes.InitHero();

        var Datas = BaseManager.Data.CharacterData;
        foreach(var item in Datas)
        {
            m_CharDic.Add(item.Value.Data.charcterName, item.Value.Data);
        }
        
        //캐릭터 등급별 정렬
        var sortDic = m_CharDic.OrderByDescending(x=>x.Value.mRarity);
        foreach(var data in sortDic)
        {
            var item = Instantiate(heroItem, Content,transform);
            Item_Heroes itemScript = item.GetComponent<Item_Heroes>();
            itemScript.Initialize(data.Value,SetClick);
            item.transform.localScale = Vector3.one;
            heroParts.Add(itemScript);
        }

        closeBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.AddListener(DisableObj);
        
        for(int i=0;i<heroParts.Count;i++)
        {
            heroParts[i].CheckPlacedHero();
        }

        return base.Init();
    }

    public override void DisableObj()
    {
        base.DisableObj();
    }

    // Render Texture에 보이는 영웅 배치 위치에 맞는
    // 버튼 ui를 Cam의 WorldToScreenPoint로 생성
    public void InitPlaceBtns()
    {
        for(int i=0;i<RenderManager.instance.heroes.pos.Length;i++)
        {
            int index = i;  //버튼 이벤트 등록 시, i 를 변수자체로 사용하면 인식불가 오류가 생겨서 방지용 처리
            var go = new GameObject("PlaceButton_"+i).AddComponent<Button>();
            go.onClick.AddListener(()=>{SetCharButton(index);}); //여기서 i가 아니라 index를 사용한다

            go.transform.SetParent(transform);
            Image img = go.gameObject.AddComponent<Image>();
            img.rectTransform.sizeDelta = new Vector2(150f,150f);
            img.color = new Color(0f,0f,0f,0.01f);  // 거의 보이지 않도록 알파 처리
            go.transform.position 
                = RenderManager.instance.ReturnScreenPos(RenderManager.instance.heroes.pos[i]);                
        }
    }

    //해당 버튼을 클릭해서 캐릭터를 배치 -> Ready 상태로 만듬
    private void SetCharButton(int idx)
    {

        BaseManager.Char.GetCharacter(idx, mSelectChar.charcterName);
        RenderManager.instance.heroes.GetParticles(false);
        SetClick(null);
        RenderManager.instance.heroes.InitHero();
        for(int i=0;i<heroParts.Count;i++)
        {
            heroParts[i].CheckPlacedHero();
        }
        HudCanvas.instance.SetCharacterData();
    }

    // 스크롤뷰 하위의 버튼이 클릭할때 실행될 사항 
    // 자식에게 Action으로 전달
    public void SetClick(Item_Heroes item)
    {
        if(item == null)
        { 
            //ui 상태 리셋
            for(int i=0;i<heroParts.Count;i++)
            {   
               heroParts[i].SetToggleActive(heroParts[i].GetCharData() == mSelectChar, true);
            }
        }
        else
        {
            mSelectChar = item.GetCharData();
            for(int i=0;i<heroParts.Count;i++)
            {
               heroParts[i].SetToggleActive(heroParts[i].GetCharData() == mSelectChar);
            }
        }

    }
}
