using UnityEngine;

[CreateAssetMenu(fileName = "Character_Scriptable", menuName = "Objects/CharacterData", order = int.MaxValue)]
public class Character_Scriptable : ScriptableObject
{
    public string charcterName;
    public string iconName;
    public float mAttackRange;
    public float mAttackSpeed;
    public double mHP;
    public int mMaxMP;
    public RARITY mRarity;
}
