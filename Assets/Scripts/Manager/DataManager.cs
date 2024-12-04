using System.Collections.Generic;
using UnityEngine;

//서버에서 관리할 데이터 저장하는 용도 
//DB에 저장할 데이터
public class DataManager 
{
    public double Money;        //현재 보유 골드
    public int Level;           //현재 플레이어 레벨
    public double Exp;          //현재 플레이어 경험치
    public int Stage;           //현재 스테이지 번호 

    Dictionary<string, Character_Holder> m_Data_Character = new(); // 플레이어가 보유한 캐릭터 데이터
    public Dictionary<string,Character_Holder> CharacterData => m_Data_Character;

    public void Init()
    {
        SetCharacter();
    }

    private void SetCharacter()
    {
        var datas = Resources.LoadAll<Character_Scriptable>("Scriptable/Character");
        foreach(var data in datas)
        {
            var character = new Character_Holder();
            character.Data = data;
            character.Level = 0;
            character.Count = 0;
            m_Data_Character.Add(data.charcterName, character);
        }
    }
}

public class Character_Holder
{
    public Character_Scriptable Data;
    public int Level;
    public int Count;
}
