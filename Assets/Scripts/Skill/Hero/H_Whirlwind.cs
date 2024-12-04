using System;
using System.Collections;
using UnityEngine;

//회전 공격
//타수 5회
public class H_Whirlwind : SkillBase
{
    Coroutine co_UseSkill;
    public override void SetSkill()
    {
        m_Player.AnimChange("useSkill");
        skillEffect.SetActive(true);
        co_UseSkill = StartCoroutine(Co_SkillCoroutine());
        base.SetSkill();
    }

    public override void ReturnSkill()
    {
        skillEffect.SetActive(false);
        base.ReturnSkill();
    }

    IEnumerator Co_SkillCoroutine()
    {   
        for(int i =0;i<5;i++)
        {
            for(int j=0; j<monsters.Length;j++)
            {
                if(CheckDistance(transform.position, monsters[j].transform.position,3f))
                {
                    //스킬 공격력 배수
                    monsters[j].GetDamage(CalcSkillDamage(skillDmg));
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
        ReturnSkill();
    }
}