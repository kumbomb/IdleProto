using System.Collections;
using UnityEngine;

//매니저 상위 스크립트
public class BaseManager : MonoBehaviour
{
    public static BaseManager instance = null;

    #region  풀링 매니저
    static PoolManager s_Pool = new PoolManager();
    public static PoolManager  Pool {get {return s_Pool;}}
    #endregion

    #region  Hero 매니저
    static HeroManager s_Hero = new HeroManager();
    public static HeroManager Hero {get {return s_Hero;}}
    #endregion

    #region  Data 매니저
    static DataManager s_Data = new DataManager();
    public static DataManager Data {get {return s_Data;}}
    #endregion

    #region  Item 매니저
    static ItemManager s_Item = new ItemManager();
    public static ItemManager Item {get{return s_Item;}}
    #endregion

    #region Character 매니저
    static CharacterManager s_Char = new CharacterManager();
    public static CharacterManager Char {get {return s_Char;}}
    #endregion

    private void Awake() 
    {
        Application.targetFrameRate = 60;
        Initialize();
    }

    private void Initialize()
    {
        if(instance == null)
        {
            instance = this;
            Pool.Initialize(this.transform);
            Data.Init();
            Item.Init();

            Invoke("StartGame", 2f);
            
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void StartGame()
    {
        StageManager.ChangeStageState(STAGE_STATE.READY);
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
