using System.Collections;
using UnityEngine;

public class Player : Character
{
    public Character_Scriptable CH_Data;
    [SerializeField] ParticleSystem provocation;    // 보스 등장시 나올 이펙트
    public GameObject[] trailObject;
    public string CH_Name;
    public double MaxHP;
    public int MP;    
    public bool isMainCharacter = false; // 메인 캐릭터인지 체크 
    
    Vector3 startPos;
    Quaternion rot;

    #region  상태 변화 델리게이트
    public delegate void MPChangeDelegate(Player player);
    public static event MPChangeDelegate OnMPChanged;   // 모든 Player 클래스의 인스턴스에서 공유하도록 처리
    #endregion

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

    #region 캐릭터 초기 데이터 설정 
    public void SetData(Character_Scriptable data)
    {
        CH_Data = data;
        AttackRange = data.mAttackRange;
        ATK_Speed = data.mAttackSpeed;
        SetStat();
    }
    public void SetStat()
    {
        ATK = BaseManager.Hero.GetAtk(CH_Data.mRarity);
        HP = BaseManager.Hero.GetHp(CH_Data.mRarity);
        MaxHP = HP;
    }
    #endregion

    #region  Game State 관련 
    void OnReady()
    {
        AnimChange("isIdle");
        isDead = false;
        Spawner.m_Players.Add(this);
        SetStat();
        transform.position = startPos;
        transform.rotation = rot;
        if(!isMainCharacter) OnMPChanged?.Invoke(this);
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
    
    //동작 진행
    void Update()
    {
        // if(isDead || isUsingSkill) return;

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
                GetMP(5);       
                Invoke("InitAttack", 1f / ATK_Speed );
            }
        }
    }

    //공격 및 피격시 MP 회복
    public void GetMP(int mp)
    {
        if(isMainCharacter || isUsingSkill) return;
        
        MP += mp;
        MP = Mathf.Min(MP, CH_Data.mMaxMP);       
        
        if(MP == CH_Data.mMaxMP && chSkill != null)
        {
            MP = 0;
            chSkill.SetSkill();
            isUsingSkill = true;
        }

        OnMPChanged?.Invoke(this);
    }
    //피격 처리 
    public override void GetDamage(double damage)
    {
        if(isDead) return;
        var goObj = BaseManager.Pool.PoolingObject("DamageText").Get((value)=>
        {
            value.GetComponent<DamageText>().Init(transform.position, damage, false,true);
        });
        HP -= damage;
        
        GetMP(3);    

        if(HP <= 0)
        {
            isDead = true;
            OnDeadEvent();
        }
    }
    
    #region 근접 공격 처리
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
    #endregion

}
