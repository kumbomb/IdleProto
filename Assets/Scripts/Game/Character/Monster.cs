using System.Collections;
using UnityEngine;

public class Monster : Character
{
    bool isSpawn = false;   
    Coroutine co_Spawn;

    public double R_ATK, R_HP, R_ATTACKRANGE;
    [SerializeField]bool isBoss = false;

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        if(!isSpawn) return;

        if(StageManager.mState != STAGE_STATE.PLAY &&
                StageManager.mState != STAGE_STATE.BOSS_PLAY) return;

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
    // Data Init
    public void Init()
    { 
        isSpawn = false;  
        isDead = false; 
        ATK = R_ATK;
        HP = R_HP; 
        AttackRange = (float)R_ATTACKRANGE;
        TargetRange = Mathf.Infinity; // 어느범위에 있던 플레이어를 추적할 수 있도록
        co_Spawn = StartCoroutine(Co_SpawnStart());
    }
    // Monster Spawn Init 
    IEnumerator Co_SpawnStart()
    {
        float current = 0f;
        float percent = 0f;
        float start = 0f;
        float end = transform.localScale.x;
        while(percent < 1)
        {
            current += Time.deltaTime;
            percent = current/ 1f;
            float LerpPos = Mathf.Lerp(start, end,percent); // 선형보간
            transform.localScale = Vector3.one * LerpPos;
            yield return null;
        }
        PlaySpawnParticles();
        yield return new WaitForSeconds(0.3f);
        isSpawn = true;
    }

    public override void PlaySpawnParticles()
    {
        base.PlaySpawnParticles();
    }

    public override void GetDamage(double damage)
    {
        if(isDead) return;

        bool isCritical = CalcCritical(ref damage);     // Critical 여부 체크

        BaseManager.Pool.PoolingObject("DamageText").Get((value) =>{
            value.GetComponent<DamageText>().Init(transform.position, damage, false, isCritical);
        });

        HP -= damage;

        if(isBoss)
        {
            HudCanvas.instance.BossProgressEvent(HP, 5000);
        }

        if(HP <= 0)
        {
            isDead = true;
            Death();
        }
    }

    private void Death()
    {
        if(!isBoss)
        {
            StageManager.CurCount++;
            HudCanvas.instance.StageProgressEvent();
        }
        else
        {
            StageManager.ChangeStageState(STAGE_STATE.CLEAR);
        }

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

        if(!isBoss)
            BaseManager.Pool.pool_Dictionary["Enemy_01"].Return(this.gameObject);
        else
        {   
            Destroy(this.gameObject);
        }  
    }

    private bool CalcCritical(ref double damage)
    {
       float criticalRate = Random.Range(0f,100f);
       if(criticalRate <= BaseManager.Hero.CriticalRate)
       {
            damage *= BaseManager.Hero.CriticalDamage * 0.01f;
            return true;
       }
       return false;
    }

}
