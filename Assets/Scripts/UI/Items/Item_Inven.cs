using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_Inven : MonoBehaviour
{
    [SerializeField] Image mRarityBG, itemIcon;
    [SerializeField] TextMeshProUGUI countText;

    PopupHandler popupHandler;

    public void Init(Item _item) 
    {
        mRarityBG.sprite = Utils.GetSpriteFromAtlas(ATLAS_ENUM.CharAtlas,$"{_item.data.rarity}");
        itemIcon.sprite = Utils.GetSpriteFromAtlas(ATLAS_ENUM.ItemAtlas,_item.data.name);
        countText.text = $"x{_item.Count}";

        if(popupHandler == null)
        {
            popupHandler = GetComponent<PopupHandler>();
        }
        popupHandler.Init(_item.data);
    }

}
