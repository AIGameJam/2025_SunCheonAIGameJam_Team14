using EnumTypes;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "Scriptable Objects/ItemScriptableObject")]
public class ItemScriptableObject : ScriptableObject
{
    public int ItemID;
    public ItemType ItemType;
    public ItemRank ItemRank;
    public Sprite ItemImage;
    public string ItemName;
    public int ItemCost;
}