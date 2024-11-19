using UnityEngine;

public class Player : Character
{
    Vector3 startPos;
    Quaternion rot;
    protected override void Start()
    {
        base.Start();
        startPos = transform.position;
        rot = transform.rotation;
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
}
