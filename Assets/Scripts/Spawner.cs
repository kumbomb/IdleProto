using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using Random = UnityEngine.Random;
using Cysharp.Threading.Tasks.Triggers;
using Unity.VisualScripting;
using System.Collections.Generic;

//몬스터는 여러 마리가 n초 단위로 소환된다.
public class Spawner : MonoBehaviour
{
    public int m_Count; // 몬스터 수
    public float m_SpawnTime;   // 몬스터 스폰 시간 텀 
    
    //몬스터와 플레이어 각각 추적용
    public static List<Monster> m_Monsters = new List<Monster>();
    public static List<Player> m_Players = new List<Player>();

    void Start()
    {
        SpawnMonsters().Forget();
    }
    //몬스터 스폰 임시 => 코루틴 대신 UniTask로 사용
    async UniTask SpawnMonsters()
    {
        Vector3 pos;

        while(true)
        {
            for(int i=0;i<m_Count;i++)
            {
                pos = Vector3.zero + Random.insideUnitSphere * 20f;
                pos.y = 0f;

                //너무 중점에 근접한 소환이 되지 않도록 정의
                while(Vector3.Distance(pos, Vector3.zero) <= 10f)
                {
                    pos = Vector3.zero + Random.insideUnitSphere * 20f;
                    pos.y = 0f;
                }

                //var go = Instantiate(monster_Prefab, pos, Quaternion.identity);
                var go = BaseManager.Pool.PoolingObject("Enemy_01").Get((value) => {
                    value.GetComponent<Monster>().Init();
                    value.transform.position = pos;
                    value.transform.LookAt(Vector3.zero);
                    m_Monsters.Add(value.GetComponent<Monster>());
                });
            }

            await UniTask.Delay(TimeSpan.FromSeconds(m_SpawnTime));
        }
    }
}
