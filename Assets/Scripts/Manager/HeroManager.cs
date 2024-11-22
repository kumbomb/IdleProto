using System;
using System.Collections.Generic;

public class HeroManager
{
    public int Level;
    public double Exp;
    public double Atk = 10;
    public double Hp = 100;
    public double CriticalRate = 20f;
    public double CriticalDamage = 140.0d;

    List<Dictionary<string, object>> expData = new List<Dictionary<string, object>>();
    bool isInitExpData = false;

    public event Action OnLevelUp;      // Levelup 시 일어날 이벤트

    //경험치 데이터가 초기화 되지 않았으면 가져온다
    void Init()
    {
        if(isInitExpData) return;
        if(expData.Count <= 0 || expData == null)
        {
            isInitExpData = true;            
            expData = new List<Dictionary<string, object>>(CSVManager.EXP);
        }
    }

    public void EXPUP()
    {
        if(!isInitExpData)
        {
            Init();
        }
      
        Exp += Utils.StringToFloat(expData[Level]["Get_EXP"].ToString());

        if(Exp >= Utils.StringToFloat(expData[Level]["EXP"].ToString()))
        {
            Level++;
            Atk += Next_Atk();
            Hp += Next_Hp();
            OnLevelUp?.Invoke(); // 이벤트 호출

            for(int i=0;i<Spawner.m_Players.Count; i++) Spawner.m_Players[i].SetStat();
        }
    }

    public float GetEXPPer()
    {
        if(!isInitExpData)
        {
            Init();
        }
      
        float exp = Utils.StringToFloat(expData[Level]["EXP"].ToString());
        double myExp = Exp;
        
        //레벨업할때마다 구간값 처리를 위해 0%로 표기하기위해
        if(Level >= 1)
        {
            exp -= Utils.StringToFloat(expData[Level-1]["EXP"].ToString());
            myExp -= Utils.StringToFloat(expData[Level-1]["EXP"].ToString());
        }
        return (float)myExp / exp;
    }
    //버튼 클릭 한번이 몇% 의 경험치를 얻는지 체크
    public float Next_Exp()
    {
        float exp = Utils.StringToFloat(expData[Level]["EXP"].ToString());
        float myExp = Utils.StringToFloat(expData[Level]["Get_EXP"].ToString());
        if(Level >= 1)
        {
            exp -= Utils.StringToFloat(expData[Level-1]["EXP"].ToString());
        }
        return (myExp / exp) * 100f;
    }
    public double Next_Atk()
    {
        return Utils.StringToFloat(expData[Level]["Get_EXP"].ToString()) * (Level + 1) / 5;
    }

    public double Next_Hp()
    {
        return Utils.StringToFloat(expData[Level]["Get_EXP"].ToString()) * (Level + 1) / 3;
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
