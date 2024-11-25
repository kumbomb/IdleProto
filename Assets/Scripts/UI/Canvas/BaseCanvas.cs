using UnityEngine;

// 대미지 표기, 아이템 드랍, 코인 획득 표현용 Canvas
public class BaseCanvas : MonoBehaviour
{
    public static BaseCanvas instance = null;

    public Transform coinTransform;
    public Transform damageTransform;
    public Transform itemTransform;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else{
            Destroy(this.gameObject);
        }
    }
}
