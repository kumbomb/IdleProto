using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_Heroes : MonoBehaviour
{
    [SerializeField] Image mSlider, mCharImg, mRarityBG;
    [SerializeField] TextMeshProUGUI levelText, mCount; 
    [SerializeField] GameObject mLockObj;
 
    RARITY rarity;
    public void Initialize(Character_Scriptable data) 
    {
        rarity = data.mRarity;
        mRarityBG.sprite = Utils.GetSpriteFromAtlas($"{rarity}");
        mCharImg.sprite = Utils.GetSpriteFromAtlas(data.iconName);
        mCharImg.SetNativeSize();
    }
}
