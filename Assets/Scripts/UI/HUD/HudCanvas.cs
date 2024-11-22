using TMPro;
using UnityEngine;

public class HudCanvas : MonoBehaviour
{
    public static HudCanvas instance = null;
    public Transform mCoinTransform;
    [SerializeField] private TextMeshProUGUI topLevelText;
    [SerializeField] private TextMeshProUGUI battlePowerText;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else{
            Destroy(this.gameObject);
        }
    }
    private void Start()
    {
        // OnLevelUp 이벤트에 구독
        BaseManager.Hero.OnLevelUp += UpdateLvText;
        UpdateLvText();
    }

    public void UpdateLvText()
    {
        topLevelText.text = $"Lv.{BaseManager.Hero.Level + 1}";
        battlePowerText.text = $"{StringMethod.ToCurrencyString(BaseManager.Hero.ALL_Power())}";
    }

    private void OnDestroy()
    {
        // 메모리 누수를 방지하기 위해 이벤트 구독 해제
        BaseManager.Hero.OnLevelUp -= UpdateLvText;
    }

}
