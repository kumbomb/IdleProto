using TMPro;
using UnityEngine;
using UnityEngine.UI;

// HUD UI 표현용 Canvas
public class HudCanvas : MonoBehaviour
{
    public static HudCanvas instance = null;
    public Transform mCoinTransform;
    [SerializeField] Button[] bottomMenuBtns;
    [SerializeField] private TextMeshProUGUI topLevelText;
    [SerializeField] private TextMeshProUGUI battlePowerText;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this.gameObject);
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

        for(int i = 0; i < bottomMenuBtns.Length; i++)
        {
            if(i == 1)
            {
                bottomMenuBtns[i].onClick.AddListener(() => { PopupCanvas.instance.GetUI(POPUP.POPUP_HERO);});
            }
        }
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
