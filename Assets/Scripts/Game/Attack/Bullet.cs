using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float mSpeed;
    Transform mTarget;
    Vector3 mTargetPos;
    double damage;
    string mCharName;
    bool isHit = false;

    [SerializeField] ParticleSystem MeleeAttack_Particle;
    Dictionary<string, GameObject> mProjectiles = new();
    Dictionary<string, ParticleSystem> mMuzzles = new();
    
    private void Awake()
    {
        Transform projectiles = transform.GetChild(0);
        Transform muzzles = transform.GetChild(1);

        for(int i =0;i<projectiles.childCount;i++)
        {
            mProjectiles.Add(projectiles.GetChild(i).name, projectiles.GetChild(i).gameObject);
        }
        for(int i =0;i<muzzles.childCount;i++)
        {
            mMuzzles.Add(muzzles.GetChild(i).name, muzzles.GetChild(i).GetComponent<ParticleSystem>());
        }
    }

    //기본 근거리 공격
    public void Init_Melee_Attack(Transform target, double _damage)
    {
        mTarget = target;
        if(mTarget != null)
        {
            mTarget.GetComponent<Character>().GetDamage(_damage);
        }
        isHit = true;
        MeleeAttack_Particle.Play();
        StartCoroutine(ReturnObj(MeleeAttack_Particle.main.duration));
    }

    //기본 원거리 공격
    public void Init(Transform target, double _damage, string charName)
    {
        mTarget = target;
        transform.LookAt(mTarget);
        isHit = false;
        mTargetPos = mTarget.position;
        damage = _damage;
        mCharName = charName;
        mProjectiles[mCharName].gameObject.SetActive(true);
    }

    void Update()
    {
        if(isHit) return;
        mTargetPos.y = 1f;
        transform.position = Vector3.MoveTowards(transform.position, mTargetPos, Time.deltaTime * mSpeed);
        if(Vector3.Distance(transform.position, mTargetPos) <= 0.1f)
        {
            if(mTarget != null && !isHit)
            {
                isHit = true;
                mTarget.GetComponent<Character>().GetDamage(damage);
                mProjectiles[mCharName].gameObject.SetActive(false);
                mMuzzles[mCharName].Play();
                StartCoroutine(ReturnObj(mMuzzles[mCharName].main.duration));
            }
        }
    }

    IEnumerator ReturnObj(float timer)
    {
        yield return new WaitForSeconds(timer);
        BaseManager.Pool.pool_Dictionary["AttackHelper"].Return(this.gameObject);
    }
}
