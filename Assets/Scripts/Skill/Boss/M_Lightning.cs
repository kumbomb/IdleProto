using System.Collections;
using UnityEngine;

public class M_Lightning : SkillBase
{
    public override void SetSkill()
    {
        base.SetSkill();
        StartCoroutine(Co_UseSkill());
    }

    IEnumerator Co_UseSkill()
    {
        for(int i=0;i<5;i++)
        {
            Player mPlayer = players[Random.Range(0,players.Length)];
            var go =  Instantiate(Resources.Load<GameObject>("PoolingObject/Skill/Lightning"),mPlayer.transform.position, Quaternion.identity);
            CameraManager.instance.CameraShake();
            mPlayer.GetDamage(10);
            yield return new WaitForSeconds(0.2f);
        }
    }
}
