using UnityEngine;
using UnityEngine.UI;

public class UI_Base : MonoBehaviour
{
    Animator anim;
    protected bool isInit = false;
    public virtual bool Init()
    {
        if(isInit) return false;
        return isInit = true;
    }

    private void Start()
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
    }

    public void DisableObject()
    {
        Destroy(this.gameObject);
    }
}
