using System.Collections;
using UnityEngine;

public class H_Meteor : SkillBase
{
    Coroutine co_UseSkill;
    public override void SetSkill()
    {
        base.SetSkill();
        co_UseSkill = StartCoroutine(Co_SkillCoroutine());
    }

    IEnumerator Co_SkillCoroutine()
    {
        skillEffect.SetActive(true);
        int value = skillEffect.transform.childCount;
        for(int i=0;i<value;i++)
        {
            var meteor = skillEffect.transform.GetChild(0).GetComponent<Meteor>();
            meteor.gameObject.SetActive(true);
            Vector3 pos = monsters[Random.Range(0,monsters.Length)].transform.position + new Vector3(Random.insideUnitSphere.x * 3f, 0f , Random.insideUnitSphere.z * 3f);

            meteor.transform.position = pos;
            meteor.Init(CalcSkillDamage(150f));
            yield return new WaitForSeconds(0.3f);
        }
        ReturnSkill();
    }


}
