using UnityEngine;
using UnityEngine.EventSystems;

public class PopupHandler : MonoBehaviour, IPointerDownHandler
{
    private Item_Scriptable item;
    public void Init(Item_Scriptable _data)
    {
        item = _data;
        if(item == null)
        {
            Debug.Log("Init도 null이면 나와요!");
        }
    }
    //클릭 시에 Popup ItemInfo를 호출할 수 있도록 기능 구현        
    public void OnPointerDown(PointerEventData eventData)
    {
        PopupCanvas.instance.GetUI(POPUP.POPUP_ITEMINFO);
        if(item == null)
        {
            Debug.Log("클릭시 null이면 나와요!");
        }
        Debug.Log("팝업 stack " + Utils.UI_Stack.Peek().popupType);
        Utils.UI_Stack.Peek().GetComponent<Popup_ItemInfo>().InitPopup(item, eventData.position);
    }
}
