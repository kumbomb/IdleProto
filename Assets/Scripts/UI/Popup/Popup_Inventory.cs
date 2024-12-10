using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum InvenTabState
{
    ALL,
    EQUIPMENT,
    CONSUMABLE,
    OTHERS
}

//barUI는 Horizontal Layout Group의 자식으로 들어가 있지만
//Layout Element의 ignore 옵션 활성화해서 사용
public class Popup_Inventory : UI_Base
{
    [SerializeField] Button closeBtn;
    [SerializeField] Button[] tabBtns;
    [SerializeField] Transform content;
    [SerializeField] Item_Inven invenPart;
    [SerializeField] RectTransform btnParentRect;
    [SerializeField] RectTransform barUI;

    public InvenTabState curState = InvenTabState.ALL;  //현재 상태 
    
    //보여질 아이템들 


    // Init이 끝나고 base.Start가 끝나면 팝업의 초기 애니메이션 및 세팅이 완료

    public override void Start() 
    {
        base.Start();     
        closeBtn.onClick.AddListener(DisableObj);  
    }

    public override bool Init()
    {
        var itemDic = BaseManager.Inven.mItems.OrderByDescending(t=>t.Value.data.rarity);
        foreach(var item in itemDic)
        {
            Instantiate(invenPart, content).Init(item.Value);
        }

        for(int i=0;i<tabBtns.Length;i++)
        {
            int index = i;
            tabBtns[index].onClick.AddListener(()=>{CheckInvenTab((InvenTabState)index);});
        }

        CheckInvenTab((InvenTabState)0);    //상태 초기화

        return base.Init();
    }

    //탭 변경 버튼 
    public void CheckInvenTab(InvenTabState _state)
    {
        curState = _state;
        StartCoroutine(Co_MoveTabBar(
            tabBtns[(int)_state].GetComponent<RectTransform>().anchoredPosition,
            tabBtns[(int)_state].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x));
    }

    //해당 버튼의 위치 (이동할 위치) 값을 매개변수로 받음
    //Bar UI의 애니메이션 처리
    IEnumerator Co_MoveTabBar(Vector2 endPos, float endXPos)
    {
        float current = 0f;
        float percent = 0f;
        Vector2 start = barUI.anchoredPosition;
        Vector2 end = endPos;

        float startX = barUI.sizeDelta.x;
        float endx = endXPos + 30f;

        while(percent < 1)
        {
            current += Time.deltaTime;
            percent = current / 0.1f;
            Vector3 lerpPos = Vector2.Lerp(start,end,percent);
            float lerpPosX = Mathf.Lerp(startX, endx,percent);
            barUI.anchoredPosition = lerpPos;
            barUI.sizeDelta = new Vector2(Mathf.Clamp(lerpPosX, 230f, 300f), barUI.sizeDelta.y);
            yield return null;
        }
    }
}
