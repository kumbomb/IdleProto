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

    private void Update()
    {
        if(!isSpawn) return;
        if(mTarget == null)
            FindClosetTarget(Spawner.m_Players.ToArray());
        else
        {
            if(mTarget.GetComponent<Character>().isDead) FindClosetTarget(Spawner.m_Players.ToArray());

            float targetDistance = Vector3.Distance(transform.position, mTarget.position);
            //추적범위 안 && 공격 범위에 안 닿으면 
            if(targetDistance > AttackRange && !isAttack)
            {
                AnimChange("isMove");
                transform.position = Vector3.MoveTowards(transform.position, mTarget.position, Time.deltaTime * m_Speed);
                transform.LookAt(mTarget.position);
            }
            else if(targetDistance <= AttackRange && !isAttack)
            {
                isAttack= true;
                AnimChange("isAttack");
                Invoke("InitAttack", 1f);
            }
        }
    }

    public void Init()
    { 
        isDead = false;
        HP =500000; 
        AttackRange = 3f;
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

    public override void GetDamage(double damage)
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

            //테스트 아이템 드랍
            for(int i=0;i<3;i++)
            {
                BaseManager.Pool.PoolingObject("ItemObject").Get((value) =>{
                    value.GetComponent<ItemObject>().Init(transform.position);
                });
            }

            BaseManager.Pool.pool_Dictionary["Enemy_01"].Return(this.gameObject);
        }
    }

}
