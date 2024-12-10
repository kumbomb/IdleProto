using System.Collections;
using TMPro;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField] Transform itemTextRect;
    [SerializeField] TextMeshProUGUI mItemText;
    [SerializeField] GameObject[] mRarity;
    [SerializeField] private ParticleSystem m_PickUP;

    [SerializeField] float firingAngle = 45f;
    [SerializeField] float gravity = 9.8f;

    bool isEnd = false;
    RARITY rarity;

    Item_Scriptable mItemInfo;

    public void Init(Vector3 pos, Item_Scriptable dropItemInfo)
    {
        mItemInfo = dropItemInfo;
        isEnd = false;
        rarity = mItemInfo.rarity;
        transform.position = pos;
        Vector3 targetPos = new Vector3(pos.x + (Random.insideUnitSphere.x * 2f), 0.5f, pos.z + (Random.insideUnitSphere.z * 2f));
        StartCoroutine(CoSimulateProjectile(targetPos));
    }
    void RarityCheck()
    {
        isEnd = true;
        transform.rotation = Quaternion.identity;
        mRarity[(int)rarity].SetActive(true);
        itemTextRect.gameObject.SetActive(true);
        itemTextRect.SetParent(BaseCanvas.instance.itemTransform);
        mItemText.text = Utils.String_Color_Rarity(rarity) + mItemInfo.itemName + "</color>";
        StartCoroutine(Co_GetItem());
    }

    void Update()
    {
        if(!isEnd ) return;
        itemTextRect.position = Camera.main.WorldToScreenPoint(transform.position);
    }

    IEnumerator CoSimulateProjectile(Vector3 pos)
    {
        //Mathf.Deg2Rad => sin, cos (degree를 사용하는게 아니라 radian(호도법)을 사용) => Degree to Radian
        float targetDistance = Vector3.Distance(transform.position, pos);
        float projectile_Velocity = targetDistance / (Mathf.Sin(2*firingAngle * Mathf.Deg2Rad)/ gravity);
        
        float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);   // 빗변 1인 가로 길이
        float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);  // 빗변 1인 높이

        float flightDuration = targetDistance / Vx;

        transform.rotation = Quaternion.LookRotation(pos - transform.position); // 오브젝트의 로테이션을 바라보면서 변경

        float time = 0f;
        while(time < flightDuration)
        {
            transform.Translate(0, (Vy - (gravity * time)) * Time.deltaTime, Vx * Time.deltaTime);
            time += Time.deltaTime;
            yield return null;
        }
        RarityCheck();
    }

    IEnumerator Co_GetItem()
    {
        yield return new WaitForSeconds(Random.Range(1f,1.5f));
        for(int i=0;i<mRarity.Length;i++) mRarity[i].SetActive(false);

        itemTextRect.transform.SetParent(this.transform);
        itemTextRect.gameObject.SetActive(false);

        m_PickUP.Play();

        //아이템 획득처리 =-> Epic 일때만
        HudCanvas.instance.GetItem(mItemInfo);
        BaseManager.Inven.GetItem(mItemInfo);

        yield return new WaitForSeconds(0.5f);
        BaseManager.Pool.pool_Dictionary["ItemObject"].Return(this.gameObject);
    }
}
