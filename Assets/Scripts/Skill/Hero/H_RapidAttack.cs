using UnityEngine;
using System.Collections;

//공격속도 증가
public class H_RapidAttack : SkillBase
{
    Coroutine co_UseSkill;
    public override void SetSkill()
    {
        base.SetSkill();
        co_UseSkill = StartCoroutine(Co_SkillCoroutine());
    }

    public override void ReturnSkill()
    {
        m_Player.ATK_Speed = 1f;
        skillEffect.SetActive(false);
        base.ReturnSkill();
    }

    IEnumerator Co_SkillCoroutine()
    {
        m_Player.ATK_Speed = 2f;
        skillEffect.SetActive(true);
        yield return new WaitForSeconds(5f);
        ReturnSkill();
    }


}
