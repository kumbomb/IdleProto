using System.Collections.Generic;
using UnityEngine;

public class CharacterManager
{
   public Dictionary<int, Character_Holder> m_Set_Character = new();
   public void GetCharacter(int value, string charName)
   {
      //키값 없으면 삽입 
      if(!m_Set_Character.ContainsKey(value))
      {
         m_Set_Character.Add(value, BaseManager.Data.CharacterData[charName]);
      }
      else // 내용 교체 
      {
         m_Set_Character[value] = BaseManager.Data.CharacterData[charName];
      }
   }

}
