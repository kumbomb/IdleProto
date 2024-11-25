using UnityEngine;

// Popup 표현용 캔버스 
public class PopupCanvas : MonoBehaviour
{
    public static PopupCanvas instance = null;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
        else{
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(Utils.UI_Stack.Count > 0)
            {
                Utils.ClosePopup();
            }
            else
            {
                // 게임 종료~
            }
        }
    }

    public void GetUI(POPUP name, bool isFade = false)
    {
        var go = Instantiate(Resources.Load<UI_Base>($"UI/Popup/{name}"),transform);
        Utils.UI_Stack.Push(go);
    }

}
