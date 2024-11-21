using UnityEngine;

public class Utils : MonoBehaviour
{
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
}
