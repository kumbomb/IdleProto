using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using Unity.VisualScripting;

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
    [Header("보스 재도전  ")]
    [SerializeField] GameObject bossRetryBtn;
    Button btnBossRetry;
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
        UpdateLvText();
        ToggleStageSlider(false);

        StageManager.mReadyEvent += OnReady;
        StageManager.mBossReadyEvent += OnBoss;
        StageManager.mClearEvent += OnClear;
        StageManager.mPlayerDeadEvent += OnDead;

        for(int i = 0; i < bottomMenuBtns.Length; i++)
        {
            if(i == 1)
            {
                bottomMenuBtns[i].onClick.AddListener(() => { PopupCanvas.instance.GetUI(POPUP.POPUP_HERO);});
            }
        }
    }

    #region  Game State 관련
    public void SetBossBattle()
    {
        StageManager.isDead =false;
        StageManager.ChangeStageState(STAGE_STATE.BOSS_READY);
        //ToggleStageSlider(true);
    }
    void OnReady()
    {
        UpdateLvText();
        ToggleStageSlider(false);
    }
    void OnBoss()
    {
        ToggleStageSlider(true);
    }
    void OnClear()
    {
        //클리어 후 딜레이 처리 
        StartCoroutine(Co_StageClear());
    }
    void OnDead()
    {
        StartCoroutine(Co_Dead());
    }

    IEnumerator Co_StageClear()
    {
        yield return new WaitForSeconds(2f);

        yield return new WaitForSeconds(1f);
        StageManager.ChangeStageState(STAGE_STATE.READY);
    }

    IEnumerator Co_Dead()
    {
        yield return StartCoroutine(Co_StageClear());
        ToggleStageSlider(false);
        for(int i=0;i<Spawner.m_Monsters.Count;i++)
        {
            if(Spawner.m_Monsters[i].isBoss) Destroy(Spawner.m_Monsters[i].gameObject);
            else{
                BaseManager.Pool.pool_Dictionary["Enemy_01"].Return(Spawner.m_Monsters[i].gameObject);
            }   
        }
        Spawner.m_Monsters.Clear();
    }

    #endregion

    #region 스테이지 진행관련 슬라이더 처리

    void ToggleStageSlider(bool isBoss)
    {
        if(StageManager.isDead)
        {
            stageProgressObj.SetActive(false);
            bossProgressObj.SetActive(false);
            bossRetryBtn.SetActive(true);
            return;
        }

        bossRetryBtn.SetActive(false);
        bossProgressObj.SetActive(isBoss);
        stageProgressObj.SetActive(!isBoss);
    
        StageProgressEvent();
        float value = isBoss ? 1f : 0f;
        BossProgressEvent(value,1f);
    }

    //일반 몬스터 잡는 경우 
    public void StageProgressEvent()
    {
        if(StageManager.mState == STAGE_STATE.BOSS_PLAY)
            return;

        float value = (float)StageManager.CurCount / (float)StageManager.mMaxCount;
        value = value >= 1f ? 1f: value;

        if(value >= 1f && StageManager.mState != STAGE_STATE.BOSS_READY)
        {
            StageManager.ChangeStageState(STAGE_STATE.BOSS_READY);    
        }

        curProgressFill.fillAmount = value;
        progressCount.text = $"{(value * 100f):F2}%";
    }

    //보스 몬스터 잡는 경우
    public void BossProgressEvent(double hp, double maxHp)
    {
        float value = (float)hp / (float)maxHp;
        value = value <= 0f ? 0f : value;
        curBossHpFill.fillAmount = value;
        curBossHpPer.text = $"{(value * 100f):F2}%";
    }
    #endregion

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
