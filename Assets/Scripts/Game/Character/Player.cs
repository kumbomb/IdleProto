using UnityEngine;

public class Player : Character
{
    public Character_Scriptable CH_Data;
    public string CH_Name;
    Vector3 startPos;
    Quaternion rot;
    protected override void Start()
    {
        base.Start();
        SetData(Resources.Load<Character_Scriptable>($"Scriptable/{CH_Name}"));
        Spawner.m_Players.Add(this);
        startPos = transform.position;
        rot = transform.rotation;
    }
    public void SetData(Character_Scriptable data)
    {
        CH_Data = data;
        AttackRange = data.mAttackRange;
    }
    void Update()
    {
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
            return;
        }

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

    public override void GetDamage(double damage)
    {
        base.GetDamage(damage);

        var goObj = BaseManager.Pool.PoolingObject("DamageText").Get((value)=>
        {
            value.GetComponent<DamageText>().Init(transform.position, damage, true);
        });
        HP -= damage;
    }
}
