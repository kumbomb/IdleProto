using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

//운석이 낙하할 수록 indicator 사이즈가 감소
public class Meteor : MonoBehaviour
{
    [Range(0f,100f)]
    public float speed; //메테오 속도
    public ParticleSystem fx_Explosion;
    public Transform meteorObj;
    public Transform indicator;
    SpriteRenderer indicatorSprite;

    Transform parentTransform;

    Coroutine meteorCoroutine;

    public void Init(double dmg)
    {
        if(parentTransform == null)
        {
            parentTransform = transform.parent;
        }        
        transform.parent = null;

        if(indicatorSprite == null)
            indicatorSprite = indicator.GetComponent<SpriteRenderer>();

        meteorCoroutine = StartCoroutine(Co_Meteor(dmg));
    }


    IEnumerator Co_Meteor(double dmg)
    {
        meteorObj.localPosition = 
         new Vector3(Random.Range(-10f,10f), Random.Range(10f,15f), Random.Range(5f, 10f));
        meteorObj.gameObject.SetActive(true);

        meteorObj.LookAt(transform.parent);
        indicator.gameObject.SetActive(true);
        indicator.localScale = Vector3.one * 3;
        while(true)
        {
            //meterobj의 locaposition이 vector3.zero면 바닥에 닿는 위치 
            float distance = Vector3.Distance(meteorObj.localPosition, Vector3.zero);
            if(distance >= 0.1f)
            {
                meteorObj.localPosition = Vector3.MoveTowards(meteorObj.localPosition, Vector3.zero, Time.deltaTime * speed);
                float ScaleValue = distance / speed + 0.3f;
                indicatorSprite.color = new Color(0,0,0, Mathf.Min(distance/speed,0.5f));
                indicator.localScale = new Vector3(ScaleValue,ScaleValue,ScaleValue);
                yield return null;
            }
            else
            {
                fx_Explosion.Play();
                CameraManager.instance.CameraShake();
                for(int i=0;i<Spawner.m_Monsters.Count;i++)
                {
                    if(Vector3.Distance(transform.position, Spawner.m_Monsters[i].transform.position) <= 1.5f)
                    {
                        Spawner.m_Monsters[i].GetDamage(dmg);
                    }
                }
                break;
            }
        }
        
        yield return new WaitForSeconds(0.5f);
        transform.SetParent( parentTransform);
        meteorObj.gameObject.SetActive(false);
        indicator.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
    }

}
