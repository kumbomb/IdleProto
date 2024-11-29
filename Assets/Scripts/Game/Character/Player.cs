using System.Collections;
using UnityEngine;

public class Player : Character
{
    public Character_Scriptable CH_Data;
    [SerializeField] ParticleSystem provocation;    // 보스 등장시 나올 이펙트
    public GameObject[] trailObject;
    public string CH_Name;
    Vector3 startPos;
    Quaternion rot;
    protected override void Start()
    {
        base.Start();
        SetData(Resources.Load<Character_Scriptable>($"Scriptable/Character/{CH_Name}"));
        Spawner.m_Players.Add(this);

        StageManager.mReadyEvent += OnReady;
        StageManager.mBossReadyEvent += OnBoss;
        StageManager.mClearEvent += OnClear;
        StageManager.mPlayerDeadEvent +=OnDead;

        startPos = transform.position;
        rot = transform.rotation;
    }
    public void SetData(Character_Scriptable data)
    {
        CH_Data = data;
        AttackRange = data.mAttackRange;
        SetStat();
    }

    public void SetStat()
    {
        ATK = BaseManager.Hero.GetAtk(CH_Data.mRarity);
        HP = BaseManager.Hero.GetHp(CH_Data.mRarity);
    }

    #region  Game State 관련 
    void OnReady()
    {
        AnimChange("isIdle");
        isDead = false;
        Spawner.m_Players.Add(this);
        SetStat();
        transform.position = startPos;
        transform.rotation = rot;
    }
    void OnBoss()
    {
        AnimChange("isIdle");
        provocation.Play();
    }
    void OnClear()
    {
        AnimChange("isClear");
    }
    void OnDead()
    {
        Spawner.m_Players.Add(this);
    }
    void OnDeadEvent()
    {   
        Spawner.m_Players.Remove(this);
        if(Spawner.m_Players.Count <= 0 && !StageManager.isDead )
        {
            StageManager.ChangeStageState(STAGE_STATE.PLAYER_DEAD);
        }
        AnimChange("isDead");
        mTarget = null;
    }
    #endregion

    #region 보스 등장시 처리 // 넉백 연출 포함 
    public void Knockback()
    {
        StartCoroutine(Co_Knockback(15f, 0.3f));
    }

    IEnumerator Co_Knockback(float power, float duration)
    {
        float t = duration;
        Vector3 force = this.transform.forward * -power;
        force.y = 0f;

        while(t > 0f)
        {
            t -= Time.deltaTime;
            transform.position += force * Time.deltaTime;
            yield return null;
        }
    }

    #endregion 

    void Update()
    {
        if(isDead) return;

        if(StageManager.mState != STAGE_STATE.PLAY 
        && StageManager.mState != STAGE_STATE.BOSS_PLAY)  return;

        FindClosetTarget(Spawner.m_Monsters.ToArray());
        
        if(mTarget == null)
        {
            float targetPos = Vector3.Distance(transform.position, startPos);
            if(targetPos > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, startPos, Time.deltaTime * m_Speed);
                transform.LookAt(startPos);
                AnimChange("isMove");
            }
            else
            {
                transform.rotation = rot;
                AnimChange("isIdle");
            }
        }
        else{ 
            if(mTarget.GetComponent<Character>().isDead) FindClosetTarget(Spawner.m_Monsters.ToArray());
                
            float targetDistance = Vector3.Distance(transform.position, mTarget.position);
            //추적범위 안 && 공격 범위에 안 닿으면 
            if(targetDistance <= TargetRange && targetDistance > AttackRange && !isAttack)
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

    public override void GetDamage(double damage)
    {
        if(isDead) return;
        var goObj = BaseManager.Pool.PoolingObject("DamageText").Get((value)=>
        {
            value.GetComponent<DamageText>().Init(transform.position, damage, true);
        });
        HP -= damage;
        if(HP <= 0)
        {
            isDead = true;
            OnDeadEvent();
        }
    }
    protected override void MeleeAttack()
    {
        base.MeleeAttack();
        for(int i=0;i<trailObject.Length;i++)
        {
            trailObject[i].SetActive(true);
        }
        Invoke("TrailDisable",.5f);
    }
    private void TrailDisable()
    {
        for(int i=0;i<trailObject.Length;i++){
            trailObject[i].SetActive(false);
        }
    }
}
