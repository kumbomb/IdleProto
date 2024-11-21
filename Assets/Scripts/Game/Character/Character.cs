using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] Animator anim;
    public double HP;
    public double ATK;
    public float m_Speed;
    public float ATK_Speed;
    protected float AttackRange = 7f;    // 공격 범위
    protected float TargetRange = 12f;    // 탐지 범위
    protected bool isAttack = false;

    protected Transform mTarget;
    public bool isDead = false;
    [SerializeField] Transform mBulletTransform;

    protected virtual void Start(){

    }
    protected void AnimChange(string temp)
    {
        if (string.Equals(temp, "isAttack"))
        {
            anim.SetTrigger("isAttack");
            return;
        }

        anim.SetBool("isIdle", false);
        anim.SetBool("isMove", false);

        anim.SetBool(temp, true);
    }

    protected void InitAttack() => isAttack = false;


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
            value.GetComponent<Bullet>().Init(mTarget, 10, "CH_01");
        });
    }

    //기본 근거리 
    protected virtual void MeleeAttack()
    {
        if(mTarget == null) return;

         BaseManager.Pool.PoolingObject("AttackHelper").Get((value) => {
            value.transform.position = mTarget.position;
            value.GetComponent<Bullet>().Init_Melee_Attack(mTarget, 10);
        });
    }

    #endregion
}
