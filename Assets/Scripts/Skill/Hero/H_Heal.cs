using System.Collections;
using UnityEngine;

public class H_Heal : SkillBase
{
    float startDelay = 5f;
    Coroutine Co_Skill;

    private void Start()
    {
        StageManager.mReadyEvent += OnReady;
    }

    public override void SetSkill()
    {
        m_Player.isUsingSkill = true;
        m_Player.AnimChange("useSkill");

        var targetHero = CheckHeroesHP();
        m_Player.transform.LookAt(targetHero.transform.position);

        targetHero.RecoveryHP(CalcSkillDamage(skillDmg));
        skillEffect.SetActive(true);
        skillEffect.transform.position = targetHero.transform.position;

        Invoke("ReturnSkill", 0.5f);
        base.SetSkill();
    }

    public override void ReturnSkill()
    {
        OnReady();
        skillEffect.SetActive(false);
        base.ReturnSkill();
    }

    public void OnReady()
    {
        if(Co_Skill != null)
           StopCoroutine(Co_Skill);
        Co_Skill = StartCoroutine(Co_SkillCoroutine(startDelay));
    }

    IEnumerator Co_SkillCoroutine(float value)
    {
        float timer = value;
        while(timer > 0f)
        {
            timer -= Time.deltaTime;
            HudCanvas.instance.UpdateMainCharSkillState(timer / value);
            yield return null;
        }
        SetSkill();
    }
}
