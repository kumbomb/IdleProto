using System.Collections;
using System.IO;
using UnityEngine;

public class Monster : Character
{
    bool isSpawn = false;   
    Coroutine co_Spawn;

    protected override void Start()
    {
        base.Start();
    }

    public void Init()
    { 
        isDead = false;
        HP =5; 
        co_Spawn = StartCoroutine(Co_SpawnStart());
    }

    IEnumerator Co_SpawnStart()
    {
        float current = 0f;
        float percent = 0f;
        float start =0f;
        float end = transform.localScale.x;
        while(percent < 1)
        {
            current += Time.deltaTime;
            percent = current/ 1f;
            float LerpPos = Mathf.Lerp(start, end,percent); // 선형보간
            transform.localScale = Vector3.one * LerpPos;
            yield return null;
        }
        yield return new WaitForSeconds(0.3f);
        isSpawn = true;
    }

    public void GetDamage(double damage)
    {
        if(isDead) return;

        BaseManager.Pool.PoolingObject("DamageText").Get((value) =>{
            value.GetComponent<DamageText>().Init(transform.position, damage, false);
        });

        HP -= damage;
        if(HP <= 0)
        {
            isDead = true;
            Spawner.m_Monsters.Remove(this);

            var smokeObj = BaseManager.Pool.PoolingObject("Smoke").Get((value) =>{
                value.transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
                BaseManager.instance.ReturnPool(value.GetComponent<ParticleSystem>().main.duration, value, "Smoke");
            });
            BaseManager.Pool.PoolingObject("CoinParent").Get((value)=>{
                value.GetComponent<CoinParent>().Init(transform.position);
            });
            BaseManager.Pool.pool_Dictionary["Enemy_01"].Return(this.gameObject);
        }
    }

    private void Update()
    {
        if(!isSpawn) return;
        
        //지금은 몬스터가 중앙으로만 이동하도록 설정 
        transform.LookAt(Vector3.zero);

        //중앙에 도착했는지 체크 
        float targetDistance = Vector3.Distance(transform.position, Vector3.zero);
        if(targetDistance <= 0.5f)
        {
            //도착했으면 이동 정지
            AnimChange("isIdle");
        }
        else
        {
            //도착안했으면 이동
            transform.position = Vector3.MoveTowards(transform.position, Vector3.zero, m_Speed * Time.deltaTime);
            AnimChange("isMove");
        }
    }
}
