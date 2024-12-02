using System.Collections;
using UnityEngine;

public class CoinParent : MonoBehaviour
{
    Vector3 target;
    Camera cam;

    [Range(0f,500f)]
    [SerializeField] float mDistanceRange, mSpeed;
    RectTransform[] childObjs = new RectTransform[5];

    void Awake()
    {
        cam = Camera.main;
        for(int i=0;i<childObjs.Length;i++) 
        {
            childObjs[i] = transform.GetChild(i).GetComponent<RectTransform>();
        }
    }

    public void Init(Vector3 pos)
    {
        target = pos;
        transform.position = cam.WorldToScreenPoint(pos);
        for(int i=0;i<childObjs.Length;i++)
        {
            childObjs[i].anchoredPosition = Vector2.zero;
        }
        transform.SetParent(BaseCanvas.instance.coinTransform);
        BaseManager.Data.Money += Utils.levelData.mStageData.Money();
        
        StartCoroutine(Co_CoinMove());
    }

    IEnumerator Co_CoinMove(){
        Vector2[] randPos = new Vector2[childObjs.Length];
        for(int i=0;i<childObjs.Length;i++)
        {
            randPos[i] = new Vector2(target.x ,target.y) + Random.insideUnitCircle * Random.Range(-mDistanceRange,mDistanceRange);
        }
        //코인 퍼뜨리기
        while(true)
        {
            for(int i=0;i<childObjs.Length;i++)
            {
                RectTransform rect = childObjs[i];
                rect.anchoredPosition = Vector2.MoveTowards(rect.anchoredPosition, randPos[i], Time.deltaTime * mSpeed);
            }
            if(CheckEndPos(randPos, 0.5f)) break;
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);
        //코인 이동
        while(true)
        {
            for(int i=0;i<childObjs.Length;i++)
            {
                RectTransform rect = childObjs[i];
                rect.position = Vector2.MoveTowards(rect.position, HudCanvas.instance.mCoinTransform.position, Time.deltaTime * mSpeed * 10);
            }
            if(CheckEndPosWorld(0.5f)) 
            {
                BaseManager.Pool.pool_Dictionary["CoinParent"].Return(this.gameObject);
                break;
            }
            yield return null;
        }
        HudCanvas.instance.UpdateLvText();
    }

    // 거리 값을 체크
    // 모두 도착지점에 도착했는지 체크
    bool CheckEndPos(Vector2[] end, float range)
    {
        for(int i=0;i<childObjs.Length;i++)
        {
            float distance = Vector2.Distance(childObjs[i].anchoredPosition, end[i]);
            if(distance > range)
            {
                return false;
            }
        }
        return true;
    }

    bool CheckEndPosWorld(float range)
    {
        for(int i=0;i<childObjs.Length;i++)
        {
            float distance = Vector2.Distance(childObjs[i].position, HudCanvas.instance.mCoinTransform.position);
            if(distance > range)
            {
                return false;
            }
        }
        return true;
    }
}
