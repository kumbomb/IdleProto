using UnityEngine;

public class SkillBase : MonoBehaviour
{
    protected Monster[] monsters {get {return Spawner.m_Monsters.ToArray();}}
    protected Player[] players {get {return Spawner.m_Players.ToArray();}}

    protected Character m_Player{get{return GetComponent<Character>();}}

    [SerializeField] protected GameObject skillEffect;
    [SerializeField] protected double skillDmg;

    void Start()
    {
        StageManager.mPlayerDeadEvent += OnDead;
    }

    public virtual void SetSkill()
    {

    }

    //거리값 계산 => 스킬마다 계산할 필요가 있을 수니 공통사용
    protected bool CheckDistance(Vector3 startPos, Vector3 endPos, float distanceVal)
    {
        return Vector3.Distance(startPos,endPos) <= distanceVal;
    } 
    //레벨 상승에 따른 기본 공격력에 스킬 공격력 배수만큼 계산 
    protected double CalcSkillDamage(double value)
    {
        return m_Player.ATK * (value / 100f);
    }
    //캐릭터 강제 세팅 등의 용도로 처리할 수 있으므로 Public
    public virtual void ReturnSkill()
    {
        m_Player.isUsingSkill = false;
        m_Player.AnimChange("isIdle");
    }
    //체력이 가장 적은 or 가장 높은 캐릭터 찾기 
    public Character CheckHeroesHP(bool HPLower = true)
    {
        Character retPlayer = null;
        double compareHP = HPLower ?  Mathf.Infinity : Mathf.NegativeInfinity;

        for(int i =0; i < players.Length;i++)
        {
            double hp = players[i].HP;
            if(HPLower)
            {
                if(hp <= compareHP)
                {
                    compareHP = hp;
                    retPlayer = players[i];
                }
            }
            else
            {
                if(hp >= compareHP)
                {
                    compareHP = hp;
                    retPlayer = players[i];
                }
            }
        }
        return retPlayer;
    }

    //사망처리 
    public void OnDead()
    {
        StopAllCoroutines();
    }

}
