using System.Collections;
using UnityEngine;

//매니저 상위 스크립트
public class BaseManager : MonoBehaviour
{
    public static BaseManager instance = null;

    #region  풀링 매니저
    public static PoolManager s_Pool = new PoolManager();
    public static PoolManager  Pool {get {return s_Pool;}}
    #endregion

    #region  Hero 매니저
    public static HeroManager s_Hero = new HeroManager();
    public static HeroManager Hero {get {return s_Hero;}}
    #endregion

    private void Awake() 
    {
        Initialize();
    }

    private void Initialize()
    {
        if(instance == null)
        {
            instance = this;
            Pool.Initialize(this.transform);
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public GameObject Instantiate_Path(string path)
    {
       return Instantiate(Resources.Load<GameObject>(path));
    }

    public void ReturnPool(float time, GameObject obj, string path){
        StartCoroutine(Co_ReturnPool(time, obj, path));
    }
    IEnumerator Co_ReturnPool(float time, GameObject obj, string path)
    {
        yield return new WaitForSeconds(time);
        BaseManager.Pool.pool_Dictionary[path].Return(obj);
    }
}
