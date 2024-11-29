using System;
using UnityEngine;

//Level Design의 방식은 지수증가 방식을 사용

[CreateAssetMenu(fileName = "LevelData", menuName = "Level Design/Level Data")]
public class LevelDesign : ScriptableObject
{
    public int currentLevel;
    public int currentStage;
    [Space(10f)]
    public LevelData mLevelData;
    [Space(10f)]
    public StageData mStageData;
}

//유저의 레벨 데이터 
[Serializable]
public class LevelData
{
    [Header("[ === Base Level Data === ]")] // 초기 값들 
    public float base_ATK;
    public float base_HP;
    public float base_EXP;
    public float base_MAXEXP;
    public float base_MONEY;

    [Header("[ === Add Value to Level === ]")]
    public float C_ATK;
    public float C_HP;
    public float C_EXP;
    public float C_MAXEXP;
    public float C_MONEY;
}


//유저의 레벨 데이터 
[Serializable]
public class StageData
{
    [Header("[ === Base Level Data === ]")] // 초기 값들 
    public float base_ATK;
    public float base_HP;
    public float base_MONEY;

    [Header("[ === Add Value to Level === ]")]
    public float M_ATK;
    public float M_HP;
    public float M_MONEY;
}