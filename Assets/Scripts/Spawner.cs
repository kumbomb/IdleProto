using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using Random = UnityEngine.Random;

//몬스터는 여러 마리가 n초 단위로 소환된다.
public class Spawner : MonoBehaviour
{
    public GameObject monster_Prefab; //  몬스터 프리팹 

    public int m_Count; // 몬스터 수
    public float m_SpawnTime;   // 몬스터 스폰 시간 텀 


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

                var go = Instantiate(monster_Prefab, pos, Quaternion.identity);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(m_SpawnTime));
        }
    }
}
