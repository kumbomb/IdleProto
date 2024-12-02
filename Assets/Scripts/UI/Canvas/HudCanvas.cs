using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

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
    [SerializeField] private TextMeshProUGUI lvUpPriceText;
    [SerializeField] private TextMeshProUGUI curMoneyText;
    [Header("스테이지 진행도 관련 ")]
    [SerializeField] GameObject stageProgressObj;
    [SerializeField] private Image curProgressFill;
    [SerializeField] private TextMeshProUGUI progressCount;
    [SerializeField] private TextMeshProUGUI stageCountText;
    [SerializeField] private TextMeshProUGUI stageProgressText;
    Color mStageColor = Color.green;
    [Header("보스 체력바 관련 ")]
    [SerializeField] GameObject bossProgressObj;
    [SerializeField] private Image curBossHpFill;
    [SerializeField] private TextMeshProUGUI curBossHpPer;
    [SerializeField] private TextMeshProUGUI curStageNum;
    [Header("보스 재도전  ")]
    [SerializeField] GameObject bossRetryBtn;
    [Header("Top - Noti GetTopGradeItem")]
    [SerializeField] Animator topGradeAnim;
    [SerializeField] Image topGradeItemBg;
    [SerializeField] Image topGradeItemIcon;
    [SerializeField] TextMeshProUGUI topGradeItemText;

    [Header("Top - Noti GetTopGradeItem")]
    [SerializeField] Transform m_ItemContent;
    List<TextMeshProUGUI> m_ItemText = new();
    List<CancellationTokenSource> m_ItemTextCTSList= new();

    CancellationTokenSource getTopGradeCTS;         // 상단 노티 unitask CTS
    bool isOpenTopGrade = false;

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
        //보스 이벤트 등록
        UpdateLvText();
        ToggleStageSlider(false);

        for(int i = 0; i < bottomMenuBtns.Length; i++)
        {
            if(i == 1)
            {
                bottomMenuBtns[i].onClick.AddListener(() => { PopupCanvas.instance.GetUI(POPUP.POPUP_HERO);});
            }
        }
 
        topGradeAnim.gameObject.GetComponent<AnimControl>().AnimFinishAction
            = () => {
                topGradeAnim.gameObject.SetActive(false);
        };

        for(int i=0;i<m_ItemContent.childCount;i++)
        {
            m_ItemTextCTSList.Add(new CancellationTokenSource());
            m_ItemText.Add(m_ItemContent.GetChild(i).GetComponent<TextMeshProUGUI>());
        }

        // OnLevelUp 이벤트에 구독
        BaseManager.Hero.OnLevelUp += UpdateLvText;
        StageManager.mReadyEvent += OnReady;
        StageManager.mBossReadyEvent += OnBoss;
        StageManager.mClearEvent += OnClear;
        StageManager.mPlayerDeadEvent += OnDead;
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
        UpdateLvText();
        ToggleStageSlider(true);
    }
    void OnClear()
    {
        //클리어 후 딜레이 처리 
        StartCoroutine(Co_StageClear());
    }
    void OnDead()
    {
        UpdateLvText();
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
        topLevelText.text = $"Lv.{BaseManager.Data.Level + 1}";
        battlePowerText.text = StringMethod.ToCurrencyString(BaseManager.Hero.ALL_Power());
       
        //IsEnoughMoney
        double lvUpPrice = Utils.levelData.mLevelData.Money();
        lvUpPriceText.text = StringMethod.ToCurrencyString(lvUpPrice);
        lvUpPriceText.color = Utils.IsEnoughMoney(lvUpPrice) ? Color.green : Color.red; 

        curMoneyText.text = StringMethod.ToCurrencyString(BaseManager.Data.Money);

        int totlaStageValue = BaseManager.Data.Stage + 1;
        int chapterValue = (totlaStageValue / 10) + 1;
        int stageValue = totlaStageValue % 10 + 1;

        stageCountText.text = $"스테이지 {chapterValue}-{stageValue}";
        stageProgressText.text = StageManager.isDead ? "반복 중" : "도전 중";
        stageProgressText.color = StageManager.isDead ? Color.yellow : mStageColor;
    }

    private void OnDestroy()
    {
        // 메모리 누수를 방지하기 위해 이벤트 구독 해제
        BaseManager.Hero.OnLevelUp -= UpdateLvText;
    }

    //상단 아이템 획득 토스트 메시지 팝업 
    public void GetTopGradeItemPopup(Item_Scriptable itemData)
    {
        if(isOpenTopGrade)
        {
            topGradeAnim.gameObject.SetActive(false);
        }

        isOpenTopGrade = true;
        topGradeAnim.gameObject.SetActive(true);
        topGradeItemIcon.sprite = Utils.GetSpriteFromAtlas(ATLAS_ENUM.ItemAtlas, itemData.name);
        topGradeItemText.text = $"{Utils.String_Color_Rarity(itemData.rarity)}{itemData.itemName}</color>를 획득하였습니다"; 
        topGradeItemBg.sprite = Utils.GetSpriteFromAtlas(ATLAS_ENUM.CharAtlas, itemData.rarity.ToString());

        getTopGradeCTS?.Cancel();
        getTopGradeCTS = new CancellationTokenSource();   
        CloseGetTopGradePopup(getTopGradeCTS.Token).Forget();
    }

    async UniTask CloseGetTopGradePopup(CancellationToken cts)
    {        
        await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: cts);
        isOpenTopGrade = false;
        topGradeAnim.SetTrigger("CLOSE");
    } 
    
    //아이템 획득 처리 -> 하단 로그 텍스트
    public void GetItem(Item_Scriptable item)
    {
        bool isAllActive = true;
        for(int i=0;i<m_ItemText.Count;i++)
        {
            if(!m_ItemText[i].gameObject.activeSelf)
            {
                m_ItemText[i].gameObject.SetActive(true);
                m_ItemText[i].text = 
                    $"아이템을 획득했습니다 : {Utils.String_Color_Rarity(item.rarity)}[{item.itemName}]</color>";
                
                //기존 텍스트들을 위로 올린다.
                for(int j=0;j<i;j++)
                {
                    RectTransform rect = m_ItemText[i].GetComponent<RectTransform>();
                    rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y + 50f);
                }

                //2초 지연 실행
                m_ItemTextCTSList[i]?.Cancel();
                m_ItemTextCTSList[i] = new CancellationTokenSource();
                GetItemLogText(m_ItemTextCTSList[i].Token, m_ItemText[i].GetComponent<RectTransform>()).Forget();
                
                isAllActive =false;
                break;
            }
        }

        //최대 텍스트를 초과했으면 
        if(isAllActive)
        {       
            //가장 높은 곳에 있는 텍스트 추적 
            GameObject baseRect = null;
            float yCnt = 0f;
            for(int i=0;i<m_ItemText.Count;i++)
            {
                RectTransform rect = m_ItemText[i].GetComponent<RectTransform>();
                if(rect.anchoredPosition.y > yCnt)
                {
                    baseRect = rect.gameObject;
                    yCnt = rect.anchoredPosition.y;
                }
            }

            for(int i = 0; i<m_ItemText.Count;i++)
            {
                if(baseRect == m_ItemText[i].gameObject)
                {
                    m_ItemText[i].gameObject.SetActive(false);
                    m_ItemText[i].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            
                    m_ItemText[i].gameObject.SetActive(true);
                    m_ItemText[i].text = 
                            $"아이템을 획득했습니다 : {Utils.String_Color_Rarity(item.rarity)}[{item.itemName}]</color>";
          
                    //2초 지연 실행
                    m_ItemTextCTSList[i]?.Cancel();
                    m_ItemTextCTSList[i] = new CancellationTokenSource();
                    GetItemLogText(m_ItemTextCTSList[i].Token, m_ItemText[i].GetComponent<RectTransform>()).Forget();      
                }
                else{
                    RectTransform rect = m_ItemText[i].GetComponent<RectTransform>();
                    rect.anchoredPosition = new Vector2(0f, rect.anchoredPosition.y + 50f);
                }
            }
             
        }

        //등급에 따라 상단 노티 처리
        if(item.rarity >= RARITY.LEGENDARY) GetTopGradeItemPopup(item);
    }

    async UniTask GetItemLogText(CancellationToken cts, RectTransform rect)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: cts);
        rect.gameObject.SetActive(false);
        rect.anchoredPosition = Vector2.zero;
    }

}
