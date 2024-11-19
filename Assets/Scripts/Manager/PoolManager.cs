using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//일반적으로 오브젝트를 생성 / 파괴할 때 => GC 가 누적됨 => 프렝미저하 유발 
//이를 개선하기 위해 풀링 기법사용

//여러 풀을 생성해서 사용하고자 interface로 만듬
public interface IPool
{
    Transform parentTransform {get; set;}           // 하이어라키 정리용도
    Queue<GameObject> pool {get; set;} 
    GameObject Get(Action<GameObject> action = null); //오브젝트 가져오기
    void Return(GameObject obj, Action<GameObject> action = null); //오브젝트 반환하기 
}

public class ObjectPool : IPool
{
    public Transform parentTransform { get; set; }
    public Queue<GameObject> pool { get; set; } = new Queue<GameObject>();

    //Queue 에서 꺼내온다
    public GameObject Get(Action<GameObject> action = null)
    {
        GameObject obj = pool.Dequeue();
        obj.SetActive(true);
        action?.Invoke(obj);
        return obj;
    }
    //Queue에 반환ㄴ
    public void Return(GameObject obj, Action<GameObject> action = null)
    {
        pool.Enqueue(obj);
        obj.transform.SetParent(parentTransform);
        obj.SetActive(false);
        action?.Invoke(obj);
    }
}

public class PoolManager
{
    Transform baseObj = null;   // 매니저들을 BaseManager 하위로 보내기위해 
    public Dictionary<string, IPool> pool_Dictionary = new Dictionary<string, IPool>();
    public void Initialize(Transform T)
    {
        baseObj = T;
    }
    public IPool PoolingObject(string path)
    {
        if(!pool_Dictionary.ContainsKey(path))
        {
            AddPool(path);
        }
        if(pool_Dictionary[path].pool.Count <= 0) AddQueue(path);
        return pool_Dictionary[path];
    }
    private GameObject AddPool(string path)
    {
        GameObject obj = new GameObject(path + "##POOL");
        obj.transform.SetParent(baseObj);
        ObjectPool T = new ObjectPool();
        pool_Dictionary.Add(path, T);
        T.parentTransform = obj.transform;
        return obj;
    }

    private void AddQueue(string path)
    {
        var gObj = BaseManager.instance.Instantiate_Path(path);
        gObj.transform.SetParent(pool_Dictionary[path].parentTransform);
        pool_Dictionary[path].Return(gObj);
    }
}
