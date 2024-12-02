using UnityEngine;
using System;
using System.Collections.Generic;

public class AnimControl : MonoBehaviour
{
    public Action AnimStarAction;
    public List<Action> AnimIntervalAction;
    public Action AnimFinishAction;


    public void RunStartAction()
    {
        AnimStarAction?.Invoke();
    }
    public void RunIntervalAnim(int num)
    {
        if(AnimIntervalAction.Count < num)
            return;
        AnimIntervalAction[num]?.Invoke();
    }
    public void RunFinishAction()
    {
        AnimFinishAction?.Invoke();
    }
}
