using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using JetBrains.Annotations;

// State Pattern 사용 


#region 델리게이트 모음 
public delegate void OnReadyEvent();
public delegate void OnPlayEvent();
public delegate void OnBossReadyEvent();
public delegate void OnBossPlayEvent();
public delegate void OnPlayerDeadEvent();
public delegate void OnClearEvent();
#endregion

public class StageManager
{
    //현재 스테이트
    public static STAGE_STATE mState = STAGE_STATE.READY;

    public static int mMaxCount = 10;        // 스테이지 내 보스 등장까지 필요한 마리수 
    static int mCurCount;        // 현재 처치한 마리수 
    public static int CurCount
    {
        get{
            return mCurCount;
        }
        set{
            mCurCount = value;
            //HudCanvas.instance.StageProgress_Event();
        }
    } 
    
    public static int mStage; // 현재 스테이지 번호

    public static bool isDead;  // 한번 사망했을때 같은 스테이지면 반복하도록 처리 

    //델리게이트 체인 
    //하나의 델리게이트가 여러 함수 참조 가능 <= 여러개의 함수를 등록가능
    public static OnReadyEvent mReadyEvent;
    public static OnPlayEvent mPlayEvent;
    public static OnBossReadyEvent mBossReadyEvent;
    public static OnBossPlayEvent mBossPlayEvent;
    public static OnPlayerDeadEvent mPlayerDeadEvent;
    public static OnClearEvent mClearEvent;
    public static void ChangeStageState(STAGE_STATE state)
    {
        mState = state;
        switch(state)
        {
            case STAGE_STATE.READY:
            {
                Debug.Log("Ready");
                mReadyEvent?.Invoke();
                Utils.NextAction(()=>{ChangeStageState(STAGE_STATE.PLAY);}, 2f);
            }
            break;
            case STAGE_STATE.PLAY:
            {
                Debug.Log("PLAY");
                mPlayEvent?.Invoke();
                //NextAction(()=>{ChangeStageState(STAGE_STATE.BOSS_READY);}, 2f);
            }
            break;
            case STAGE_STATE.BOSS_READY:
            {
                Debug.Log("BOSS READY");
                mCurCount = 0;
                mBossReadyEvent?.Invoke();
                //NextAction(()=>{ChangeStageState(STAGE_STATE.BOSS_PLAY);}, 2f);
            }
            break;
            case STAGE_STATE.BOSS_PLAY:
            {
                Debug.Log("BOSS PLAY");
                mBossPlayEvent?.Invoke();
                //NextAction(()=>{ChangeStageState(STAGE_STATE.PLAYER_DEAD);}, 2f);
            }
            break;
            case STAGE_STATE.PLAYER_DEAD:
            {
                Debug.Log("PLAYER_DEAD");
                isDead = true;
                mPlayerDeadEvent?.Invoke();
                //NextAction(()=>{ChangeStageState(STAGE_STATE.CLEAR);}, 2f);
            }
            break;
            case STAGE_STATE.CLEAR:
            {
                mStage++;
                Debug.Log("CLEAR");
                mClearEvent?.Invoke();
                //NextAction(()=>{ChangeStageState(STAGE_STATE.READY);}, 2f);
            }
            break;
        }
    }
    
}
