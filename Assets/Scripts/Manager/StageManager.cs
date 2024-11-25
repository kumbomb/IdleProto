using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System;

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
                mReadyEvent?.Invoke();
                NextAction(()=>{ChangeStageState(STAGE_STATE.PLAY);}, 2f);
            }
            break;
            case STAGE_STATE.PLAY:
            {
                mPlayEvent?.Invoke();
                //NextAction(()=>{ChangeStageState(STAGE_STATE.BOSS_READY);}, 2f);
            }
            break;
            case STAGE_STATE.BOSS_READY:
            {
                mBossReadyEvent?.Invoke();
                //NextAction(()=>{ChangeStageState(STAGE_STATE.BOSS_PLAY);}, 2f);
            }
            break;
            case STAGE_STATE.BOSS_PLAY:
            {
                mBossPlayEvent?.Invoke();
                //NextAction(()=>{ChangeStageState(STAGE_STATE.PLAYER_DEAD);}, 2f);
            }
            break;
            case STAGE_STATE.PLAYER_DEAD:
            {
                mPlayerDeadEvent?.Invoke();
                //NextAction(()=>{ChangeStageState(STAGE_STATE.CLEAR);}, 2f);
            }
            break;
            case STAGE_STATE.CLEAR:
            {
                mClearEvent?.Invoke();
                //NextAction(()=>{ChangeStageState(STAGE_STATE.READY);}, 2f);
            }
            break;
        }
    }

    public static void NextAction(Action action, float timer)
    {
        //Debug.Log("NextAction");
        ActionTask(action, timer).Forget();
    }

    async static UniTask ActionTask(Action action, float timer)
    {
        //Debug.Log("Uni Task");
        await UniTask.Delay(TimeSpan.FromSeconds(timer));
        action?.Invoke();
    }

}
