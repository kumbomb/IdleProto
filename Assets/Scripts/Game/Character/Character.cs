using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] ParticleSystem[] spawnParticles;
    public double HP;
    public double ATK;
    public float m_Speed;
    public float ATK_Speed;
    protected float AttackRange = 7f;    // 공격 범위
    protected float TargetRange = 12f;    // 탐지 범위
    protected bool isAttack = false;
    public bool isUsingSkill = false;   // 스킬 사용중인지 체크 => 스킬 사용중에는 마나회복 불가
    public bool isSkillNonAttack = false; // 스킬을 사용중에 공격을 하면 안되는지 체크 true면 공격 진행불가

    protected Transform mTarget;
    public bool isDead = false;
    [SerializeField] Transform mBulletTransform;
    [SerializeField] string mBulletName;
    
    protected SkillBase chSkill;  //보유한 스킬 

    protected virtual void Start(){
        chSkill = GetComponent<SkillBase>();
    }
    public void AnimChange(string temp)
    {
        //스킬 사용중에는 애니메이션 변경불가처리
        //isSkillNonAttack 상태에 따라 스킬 사용중 일반 공격 사용 가능여부 결정
        if(isSkillNonAttack && isUsingSkill) return;

        anim.SetBool("isIdle", false);
        anim.SetBool("isMove", false);
        
        anim.speed = string.Equals(temp,"isAttack") ? ATK_Speed : 1f;

        if (string.Equals(temp, "isAttack") 
            || string.Equals(temp, "isClear") 
            || string.Equals(temp, "isDead")
            || string.Equals(temp, "useSkill")
            )
        {
            anim.SetTrigger(temp);
            return;
        }

        anim.SetBool(temp, true);
    }

    protected void InitAttack() => isAttack = false;

    //스폰 될때 파티클을 띄워줘야하는게 있다면 
    public virtual void PlaySpawnParticles()
    {
        if(spawnParticles.Length <= 0) return;
        for(int i=0;i<spawnParticles.Length;i++)
            spawnParticles[i].Play();
    }

    //타겟 강제 설정
    public void SetTarget(Transform _mTarget)
    {
        mTarget = _mTarget;
    }

    //인근 추적 => 거리로 체크
    //Component => 는 기본적으로 Transform으로 변환이 가능
    protected void FindClosetTarget<T>(T[] targets) where T : Component
    {
        var monsters = targets;
        Transform closetTarget = null;
        float maxDistance = TargetRange;
        foreach(var monster in monsters)
        {
            float targetDistance = Vector3.Distance(transform.position, monster.transform.position);
            if(targetDistance < maxDistance)
            {
                closetTarget = monster.transform;
                maxDistance = targetDistance;
            }
        }
        mTarget = closetTarget;
        if(mTarget != null) transform.LookAt(mTarget.position);
    }

    #region  대미지 처리 (공통)
    public virtual void GetDamage(double damage)
    {       

    }
    #endregion


    #region  플레이어의 공격 관련 

    //기본 원거리 
    protected virtual void Bullet()
    {
        if(mTarget == null) return;

        BaseManager.Pool.PoolingObject("AttackHelper").Get((value) => {
            value.transform.position = mBulletTransform.position;
            value.GetComponent<Bullet>().Init(mTarget, ATK, mBulletName);
        });
    }

    //기본 근거리 
    protected virtual void MeleeAttack()
    {
        if(mTarget == null) return;

         BaseManager.Pool.PoolingObject("AttackHelper").Get((value) => {
            value.transform.position = mTarget.position;
            value.GetComponent<Bullet>().Init_Melee_Attack(mTarget, ATK);
        });
    }
    
    public virtual void RecoveryHP(double _recoveryValue)
    {
        HP += _recoveryValue;

        var goObj = BaseManager.Pool.PoolingObject("DamageText").Get((value)=>
        {
            value.GetComponent<DamageText>().Init(transform.position, _recoveryValue, true);
        });
    }

    #endregion
}
