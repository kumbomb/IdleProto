using UnityEngine;
using UnityEngine.UI;


// Start 안에서 Init 을 끝내고 Start가 마무리된다.

public class UI_Base : MonoBehaviour
{
    Animator anim;
    protected bool isInit = false;
    public POPUP popupType;
    public virtual bool Init()
    {
        if(isInit) return false;
        return isInit = true;
    }

    public virtual void Start()
    {
        Init();

        if(anim == null)
            anim = GetComponent<Animator>();

        if(anim != null)
        {
            anim.enabled = true;
            anim.SetTrigger("Open");
        }
    }

    public virtual void DisableObj()
    {
        Utils.UI_Stack.Pop();
        if(anim != null) anim.SetTrigger("Close");
        else DisableObject();
    }

    public void DisableObject()
    {
        Destroy(this.gameObject);
    }
}
