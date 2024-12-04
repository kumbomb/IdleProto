using UnityEngine;

public class RenderManager : MonoBehaviour
{
    public static RenderManager instance;
    public RenderHeroes heroes;
    public Camera uiCam;
    private void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    public Vector2 ReturnScreenPos(Transform pos)
    {
        return uiCam.WorldToScreenPoint(pos.position);
    }
}
