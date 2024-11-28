using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;

//몬스터는 여러 마리가 n초 단위로 소환된다.
public class Spawner : MonoBehaviour
{
    public int m_Count; // 몬스터 수
    public float m_SpawnTime;   // 몬스터 스폰 시간 텀 

    [Range(0f, 200f)]
    [SerializeField] float mSpawnAreaDist;      // 최대 소환 거리

    [Range(0f, 200f)]
    [SerializeField] float mMinSpawnAreaDist;   // 최소 소환 거리
    
    //몬스터와 플레이어 각각 추적용
    public static List<Monster> m_Monsters = new List<Monster>();
    public static List<Player> m_Players = new List<Player>();

    private CancellationTokenSource stageMonsterSpawnCTS;
    private CancellationTokenSource bossMonsterSpawnCTS;


    void Start()
    {
        StageManager.mPlayEvent += OnPlay;
        StageManager.mBossReadyEvent += OnBoss;
    }
     
     #region Game State
    //일반 몬스터 소환
    public void OnPlay()
    {         
        //보스 관련 중지
        bossMonsterSpawnCTS?.Cancel();

        stageMonsterSpawnCTS?.Cancel();
        stageMonsterSpawnCTS = new CancellationTokenSource();
        SpawnMonsters(stageMonsterSpawnCTS.Token).Forget();
    }
    //보스 몬스터 소환
    public void OnBoss()
    {
        //일반 몬스터 소환 중지
        stageMonsterSpawnCTS?.Cancel();

        bossMonsterSpawnCTS?.Cancel();
        bossMonsterSpawnCTS = new CancellationTokenSource();   
        //일반 몬스터 제거 
        for(int i=0;i<m_Monsters.Count;i++)
        {
            BaseManager.Pool.pool_Dictionary["Enemy_01"].Return(m_Monsters[i].gameObject);
        }
        m_Monsters.Clear();

        //보스 소환
        SpawnBossMonster(bossMonsterSpawnCTS.Token).Forget();
    }
    #endregion

    //몬스터 스폰 임시 => 코루틴 대신 UniTask로 사용
    async UniTask SpawnMonsters(CancellationToken token)
    {
        Vector3 pos;

        while(true)
        {
            for(int i=0;i<m_Count;i++)
            {
                pos = Vector3.zero + Random.insideUnitSphere * mSpawnAreaDist;
                pos.y = 0f;

                //너무 중점에 근접한 소환이 되지 않도록 정의
                while(Vector3.Distance(pos, Vector3.zero) <= mMinSpawnAreaDist)
                {
                    pos = Vector3.zero + Random.insideUnitSphere * mSpawnAreaDist;
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

            await UniTask.Delay(TimeSpan.FromSeconds(m_SpawnTime), cancellationToken: token);
        }
    }

    //보스 몬스터 스폰 
    async UniTask SpawnBossMonster(CancellationToken token)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(2f), cancellationToken: token);
        var bossMonster = Instantiate(Resources.Load<Monster>("PoolingObject/Boss_01"), Vector3.zero, quaternion.identity);
        bossMonster.Init();
        m_Monsters.Add(bossMonster.GetComponent<Monster>());

        Vector3 bossPos = bossMonster.transform.position;

        //플레이어 전체 넉백
        for(int i = 0;i < m_Players.Count;i++)
        {
            if(Vector3.Distance(bossPos, m_Players[i].transform.position) <= 10f)
            {
                m_Players[i].transform.LookAt(bossMonster.transform.position);
                m_Players[i].Knockback();
            }
        }
        await UniTask.Delay(TimeSpan.FromSeconds(1.5f), cancellationToken: token);      

        StageManager.ChangeStageState(STAGE_STATE.BOSS_PLAY);
    }
}
