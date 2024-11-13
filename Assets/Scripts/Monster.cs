using System.Collections;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public float m_Speed;
    Animator anim;
    bool isSpawn = false;   
    Coroutine co_Spawn;

    private void Start()
    {
        anim = GetComponent<Animator>();   
        co_Spawn = StartCoroutine(Co_SpawnStart());
    }

    IEnumerator Co_SpawnStart()
    {
        float current = 0f;
        float percent = 0f;
        float start =0f;
        float end = transform.localScale.x;
        while(percent < 1)
        {
            current += Time.deltaTime;
            percent = current/ 1f;
            float LerpPos = Mathf.Lerp(start, end,percent); // 선형보간
            transform.localScale = Vector3.one * LerpPos;
            yield return null;
        }
        yield return new WaitForSeconds(0.3f);
        isSpawn = true;
    }

    private void Update()
    {
        if(!isSpawn) return;
        
        //지금은 몬스터가 중앙으로만 이동하도록 설정 
        transform.LookAt(Vector3.zero);

        //중앙에 도착했는지 체크 
        float targetDistance = Vector3.Distance(transform.position, Vector3.zero);
        if(targetDistance <= 0.5f)
        {
            //도착했으면 이동 정지
            AnimChange("isIdle");
        }
        else
        {
            //도착안했으면 이동
            transform.position = Vector3.MoveTowards(transform.position, Vector3.zero, m_Speed * Time.deltaTime);
            AnimChange("isMove");
        }
    }

    void AnimChange(string temp)
    {
        anim.SetBool("isIdle", false);
        anim.SetBool("isMove", false);

        anim.SetBool(temp, true);
    }
}
