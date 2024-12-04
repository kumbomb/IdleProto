using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_Heroes : MonoBehaviour
{
    [SerializeField] Image mSlider, mCharImg, mRarityBG;
    [SerializeField] TextMeshProUGUI levelText, mCount; 
    [SerializeField] GameObject mLockObj,selectableObj,placedObj;
    [SerializeField] Outline selectableOutline;
    
    Action<Item_Heroes> callback;
    Character_Scriptable m_CharData;
    RARITY rarity;
    public void Initialize(Character_Scriptable data, Action<Item_Heroes> _callback) 
    {
        m_CharData = data;
        callback = _callback;
        rarity = data.mRarity;
        mRarityBG.sprite = Utils.GetSpriteFromAtlas(ATLAS_ENUM.CharAtlas,$"{rarity}");
        mCharImg.sprite = Utils.GetSpriteFromAtlas(ATLAS_ENUM.CharAtlas,data.iconName);
       // mCharImg.SetNativeSize();
    }
    public Character_Scriptable GetCharData()
    {
        return m_CharData;
    }
    // 배치할 캐릭터 클릭
    public void ClickHero()
    {
        callback?.Invoke(this);
        RenderManager.instance.heroes.GetParticles(true);
    }

    // 배치 진행 중일때 ui 처리 
    public void SetToggleActive(bool isFlag, bool isReset = false)
    {
        selectableObj.SetActive(!isReset && !isFlag);
        selectableOutline.enabled = !isReset && isFlag;
    }
    public void CheckPlacedHero()
    {
        //딕셔너리에 해당 영웅

        placedObj.SetActive(
            BaseManager.Char.m_Set_Character.Values.FirstOrDefault(t=>t.Data == m_CharData) != null
            );
    }

}
