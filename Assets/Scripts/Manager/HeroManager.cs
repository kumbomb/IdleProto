using System;
using System.Collections.Generic;

public class HeroManager
{
    public double Atk = 10;
    public double Hp = 100;
    public double CriticalRate = 20f;
    public double CriticalDamage = 140.0d;

    public event Action OnLevelUp;      // Levelup 시 일어날 이벤트

    public void EXPUP()
    {
        BaseManager.Data.Exp += Utils.levelData.mLevelData.EXP();
        
        if(BaseManager.Data.Exp >= Utils.levelData.mLevelData.MaxEXP())
        {
            BaseManager.Data.Level++;
            Atk += Utils.levelData.mLevelData.ATK();
            Hp += Utils.levelData.mLevelData.HP();
            OnLevelUp?.Invoke(); // 이벤트 호출
            BaseManager.Data.Exp = 0;
            for(int i=0;i<Spawner.m_Players.Count; i++) Spawner.m_Players[i].SetStat();
        }
    }

    public float GetEXPPer()
    {
        float exp = (float)Utils.levelData.mLevelData.MaxEXP();
        double myExp = BaseManager.Data.Exp;
        
        return (float)myExp / exp;
    }
    //버튼 클릭 한번이 몇% 의 경험치를 얻는지 체크
    public float Next_Exp()
    {
        float exp = (float)Utils.levelData.mLevelData.MaxEXP();
        float myExp = (float)Utils.levelData.mLevelData.EXP();
        return (myExp / exp) * 100f;
    }

    public double GetAtk(RARITY rarity)
    {
        return Atk * ((int)rarity + 1);
    }

    public double GetHp(RARITY rarity)
    {
        return Hp * ((int)rarity + 1);
    }

    //전체 전투력 갱신
    public double ALL_Power()
    {
        return Atk+Hp;
    }
}
