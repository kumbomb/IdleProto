using UnityEngine;

public class SkillBase : MonoBehaviour
{
    protected Monster[] monsters {get {return Spawner.m_Monsters.ToArray();}}
    protected Player[] players {get {return Spawner.m_Players.ToArray();}}

    void Start()
    {
        StageManager.mPlayerDeadEvent += OnDead;
    }

    public virtual void SetSkill()
    {

    }

    public void OnDead()
    {
        StopAllCoroutines();
    }
}
