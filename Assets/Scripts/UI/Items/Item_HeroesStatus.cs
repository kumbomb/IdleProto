using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_HeroesStatus : MonoBehaviour
{
    [SerializeField] GameObject lockObj, plusObj, readyObj;
    [SerializeField] Image characterIcon, hpGauge, mpGauge;   
    [SerializeField] TextMeshProUGUI mpText;
    Character_Scriptable mData;

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        InitData(mData, true);
    }
    // Status UI 세팅 
    public void InitData(Character_Scriptable _data = null, bool isReady = false)
    {
        mData = _data;
        
        lockObj.SetActive(false);
        plusObj.SetActive(_data == null);

        hpGauge.transform.parent.gameObject.SetActive(!isReady && _data != null);
        mpGauge.transform.parent.gameObject.SetActive(!isReady && _data != null);    

        characterIcon.gameObject.SetActive(_data != null);
        if(_data != null)
            characterIcon.sprite = Utils.GetSpriteFromAtlas(ATLAS_ENUM.CharAtlas, mData.iconName);

        readyObj.SetActive(isReady && _data != null);
    } 
    // HP, MP 변화에 따른 ui 세팅 
    public void UpdateStatus(Player player)
    {
        hpGauge.fillAmount = Mathf.Min((float)player.HP / (float)player.MaxHP, 1f);
        mpGauge.fillAmount = Mathf.Min((float)player.MP / (float)mData.mMaxMP, 1f);
        mpText.text = $"{player.MP} / {mData.mMaxMP}";
    }
    //Plus 버튼 클릭시
    public void OnClickPlaced()
    {
        PopupCanvas.instance.GetUI(POPUP.POPUP_HERO);
    }
}
