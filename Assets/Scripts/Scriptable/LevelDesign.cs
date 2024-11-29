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
    public int base_ATK;
    public int base_HP;
    public int base_EXP;
    public int base_MAXEXP;
    public int base_MONEY;

    [Header("[ === Add Value to Level === ]")]
    public float C_ATK;
    public float C_HP;
    public float C_EXP;
    public float C_MAXEXP;
    public float C_MONEY;

    public double ATK() => Utils.CalculatedValue(base_ATK, BaseManager.Data.Level, C_ATK);
    public double HP() => Utils.CalculatedValue(base_HP, BaseManager.Data.Level, C_HP);
    public double EXP() => Utils.CalculatedValue(base_EXP, BaseManager.Data.Level, C_EXP);
    public double MaxEXP() => Utils.CalculatedValue(base_MAXEXP, BaseManager.Data.Level, C_MAXEXP);
    public double Money() => Utils.CalculatedValue(base_MONEY, BaseManager.Data.Level, C_MONEY);

}


//유저의 레벨 데이터 
[Serializable]
public class StageData
{
    [Header("[ === Base Level Data === ]")] // 초기 값들 
    public int base_ATK;
    public int base_HP;
    public int base_MONEY;

    [Header("[ === Add Value to Level === ]")]
    public float M_ATK;
    public float M_HP;
    public float M_MONEY;

    
    public double ATK() => Utils.CalculatedValue(base_ATK, BaseManager.Data.Stage, M_ATK);
    public double HP() => Utils.CalculatedValue(base_HP, BaseManager.Data.Stage, M_HP);
    public double Money() => Utils.CalculatedValue(base_MONEY, BaseManager.Data.Stage, M_MONEY);
}