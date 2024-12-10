using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup_ItemInfo : UI_Base
{
    [SerializeField] RectTransform rootRect;
    [SerializeField] Image gradeBG,icon;
    [SerializeField] TextMeshProUGUI itemName, gradeName, descText;
    [SerializeField] Button closeBtn;

    public override void Start()
    {
        base.Start();
        closeBtn.onClick.AddListener(DisableObj);
    }

    public override bool Init()
    {
        return base.Init();
    }
    
    public void InitPopup(Item_Scriptable item, Vector2 pos)
    {
        rootRect.pivot = CalcPivotPoint(pos);

        rootRect.anchoredPosition = pos;
        icon.sprite = Utils.GetSpriteFromAtlas(ATLAS_ENUM.ItemAtlas, item.name);
        gradeBG.sprite = Utils.GetSpriteFromAtlas(ATLAS_ENUM.CharAtlas,$"{item.rarity}");
        itemName.text = item.itemName;
        gradeName.text = $"{Utils.String_Color_Rarity(item.rarity)} 등급</color>";
        descText.text = item.itemDesc;
    }

    //화면의 중심점 기준으로 터치 위치를 판단 => pivot 값 결정
    public Vector2 CalcPivotPoint(Vector2 _pos)
    {
        float xPos = _pos.x > Screen.width / 2 ? 1f : 0f;
        float yPos = _pos.y > Screen.height / 2 ? 1f : 0f;

        return new Vector2(xPos,yPos);
    }
}
