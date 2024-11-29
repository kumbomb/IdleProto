using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using System;
using Cysharp.Threading.Tasks;

public class Utils
{
    #region 레벨 디자인 관련 
    public static LevelDesign levelData = Resources.Load<LevelDesign>("Scriptable/Design/LevelData");

    #endregion
    //레어리티 컬러 결정 
    public static string String_Color_Rarity (RARITY rare)
    {
        switch(rare)
        {
            case RARITY.COMMON: return "<color=#FFFFFF>";
            case RARITY.UNCOMMON: return "<color=#00FF37>";
            case RARITY.RARE: return "<color=#2733FF>";
            case RARITY.UNIQUE: return "<color=#FF0000>";
            case RARITY.LEGENDARY: return "<color=#FB00FF>";
            case RARITY.EPIC: return "<color=#FFFF00>";
        }
        return "<color=#FFFFFF>";
    }

    #region  string 형 변환 

    //string to float
    public static float StringToFloat(string str)
    {
        bool isParse = float.TryParse(str, out float result);
        return isParse ? result : 0f;
    }
    #endregion

    #region  Sprite 관련 
    public static SpriteAtlas charAtlas = Resources.Load<SpriteAtlas>("Atlas/CharAtlas");  
    public static Sprite GetSpriteFromAtlas(string name)
    {
        if(charAtlas == null) charAtlas = Resources.Load<SpriteAtlas>("Atlas/CharAtlas");  
        return charAtlas.GetSprite(name);
    }
    #endregion

    #region POPUP 관련 
    public static Stack<UI_Base> UI_Stack = new Stack<UI_Base>();

    //전체 팝업 끄기 
    public static void CloseAllPopup()
    {
        while(UI_Stack.Count > 0) ClosePopup();
    }

    //팝업이 없으면 게임 종료 팝업 띄우기
    public static void ClosePopup()
    {
        if(UI_Stack.Count == 0) return;
        
        UI_Base popup = UI_Stack.Peek();
        popup.DisableObj();
    }
    #endregion

    #region 함수 지연 호출 및 실행 
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
    #endregion
   
    #region  계산 관련
    //Level은 0부터 시작
    public static double CalculatedValue(float baseValue, int level, float value)
    {
        return baseValue * Mathf.Pow(level + 1, value);
    }

    public static bool IsEnoughMoney(double value)
    {
        if(BaseManager.Data.Money >= value) return true;
        else return false;
    }
    #endregion

}
