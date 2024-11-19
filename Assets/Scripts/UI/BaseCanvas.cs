using UnityEngine;

public class BaseCanvas : MonoBehaviour
{
    public static BaseCanvas instance = null;

    public Transform coinTransform;
    public Transform damageTransform;

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
