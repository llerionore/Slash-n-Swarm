using System.Collections.Generic;
using UnityEngine;

public enum ShopItemType
{
    Passive,
    Active
}

public enum ShopEffectType
{
    Strength,
    MaxStamina,
    StaminaRegen,
    CritChance,
    Luck,
    Income,
    Experience
}

[System.Serializable]
public class ShopEffect
{
    public ShopEffectType effectType;
    public float value;
}

[CreateAssetMenu(fileName = "ShopItem", menuName = "Game/Shop Item")]
public class ShopItemData : ScriptableObject
{
    [Header("Info")]
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;
    public int price = 5;
    public ShopItemType itemType = ShopItemType.Passive;

    [Header("Effects")]
    public List<ShopEffect> effects = new List<ShopEffect>();
}