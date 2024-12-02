using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Item Data/Items")]
public class Item_Scriptable : ScriptableObject
{
    public string itemName;
    public string itemDesc;
    public RARITY rarity;
    public float dropRate;

}
