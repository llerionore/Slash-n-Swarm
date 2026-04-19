using UnityEngine;

public enum UpgradeType
{
    Strength,
    MaxStamina,
    StaminaRegen,
    CritChance,
    Luck
}

public enum UpgradeRarity
{
    Common,
    Rare,
    Epic,
    Legendary,
    Mythic
}

[System.Serializable]
public class UpgradeData
{
    public UpgradeType type;
    public UpgradeRarity rarity;
    public float value;
}