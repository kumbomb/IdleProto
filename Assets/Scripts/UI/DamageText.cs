using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    Vector3 target;
    Camera cam;
    public TextMeshProUGUI mText; // 데미지 텍스트

    [SerializeField] GameObject mCritical;

    float UpRange = 0f;

    public void Start()
    {
        cam = Camera.main;
    }


    public void Init(Vector3 pos, double damage, bool isRecovery = false ,bool isMonster = false, bool isCritical = false)
    {
        // 대미지 텍스트 표현의 약간의 랜덤성 추가
        pos.x += Random.Range(-0.1f, 0.1f);
        pos.z += Random.Range(-0.1f, 0.1f);

        target = pos;
        mText.color = isMonster ? Color.red : Color.white;   
        mText.color = isRecovery ? Color.green : mText.color; // 체력 회복의 경우 색상 변경
        mText.text = StringMethod.ToCurrencyString(damage);
        transform.SetParent(BaseCanvas.instance.damageTransform);

        //몬스터 피격 데미지 전용 => 크리티컬 판단
        mCritical.SetActive(isCritical);
        Color criticalTextColor = isCritical ? Color.yellow : Color.white;
        mText.colorGradient = new VertexGradient(criticalTextColor, criticalTextColor, Color.white, Color.white);

        BaseManager.instance.ReturnPool(2f,this.gameObject,"DamageText");
    }

    void Update()
    {   
        //몬스터 머리위에 
        Vector3 targetPos = new Vector3(target.x, target.y + UpRange, target.z);
        transform.position = cam.WorldToScreenPoint(targetPos);
        if(UpRange <= 0.3f)
        {
            UpRange += Time.deltaTime;
        }
    }

    void ReturnText()
    {
        BaseManager.Pool.pool_Dictionary["DamageText"].Return(this.gameObject);
    }
}
