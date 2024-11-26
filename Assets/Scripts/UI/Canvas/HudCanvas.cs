using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.Multiplayer.Center.Common;

// HUD UI 표현용 Canvas
public class HudCanvas : MonoBehaviour
{
    public static HudCanvas instance = null;
    public Transform mCoinTransform;
    
    [Header("하단 버튼")]
    [SerializeField] Button[] bottomMenuBtns;
    [Header("상단 플레이어 레벨 / 전투력")]
    [SerializeField] private TextMeshProUGUI topLevelText;
    [SerializeField] private TextMeshProUGUI battlePowerText;
    [Header("스테이지 진행도 관련 ")]
    [SerializeField] GameObject stageProgressObj;
    [SerializeField] private Image curProgressFill;
    [SerializeField] private TextMeshProUGUI progressCount;
    [Header("보스 체력바 관련 ")]
    [SerializeField] GameObject bossProgressObj;
    [SerializeField] private Image curBossHpFill;
    [SerializeField] private TextMeshProUGUI curBossHpPer;
    [SerializeField] private TextMeshProUGUI curStageNum;
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

        //보스 이벤트 등록
        StageManager.mBossReadyEvent += OnBoss;

        UpdateLvText();
        StageProgress_Event();
        for(int i = 0; i < bottomMenuBtns.Length; i++)
        {
            if(i == 1)
            {
                bottomMenuBtns[i].onClick.AddListener(() => { PopupCanvas.instance.GetUI(POPUP.POPUP_HERO);});
            }
        }
    }

    void OnBoss()
    {
        bossProgressObj.SetActive(true);
        stageProgressObj.SetActive(false);


    }

    public void StageProgress_Event()
    {
        float value = (float)StageManager.CurCount / (float)StageManager.mMaxCount;
        value = value >= 1f ? 1f: value;

        if(value >= 1f && StageManager.mState != STAGE_STATE.BOSS_READY)
        {
            StageManager.ChangeStageState(STAGE_STATE.BOSS_READY);    
        }

        curProgressFill.fillAmount = value;
        progressCount.text = $"{value * 100f}%";
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
