using System.Collections.Generic;
using UnityEngine;

public static class UpgradeGenerator
{
    public static UpgradeData GenerateRandom(int round, HashSet<UpgradeType> excludedTypes = null)
    {
        UpgradeData data = new UpgradeData();

        data.rarity = RollRarity(round);
        data.type = RollType(excludedTypes);
        data.value = GetValue(data.type, data.rarity);

        return data;
    }

    private static UpgradeRarity RollRarity(int round)
    {
        float luck = 0f;

        if (PlayerStats.Instance != null)
            luck = PlayerStats.Instance.GetLuckRarityBonus();

        float common = 60f;
        float rare = 25f;
        float epic = 10f;
        float legendary = 4f;
        float mythic = 1f;

        if (round >= 3)
        {
            common -= 10f;
            rare += 2f;
            epic += 4f;
            legendary += 3f;
            mythic += 1f;
        }

        if (round >= 6)
        {
            common -= 10f;
            rare += 3f;
            epic += 5f;
            legendary += 4f;
            mythic += 2f;
        }

        if (round >= 10)
        {
            common -= 10f;
            epic += 3f;
            legendary += 4f;
            mythic += 3f;
        }

        float luckPower = Mathf.Clamp(luck, 0f, 50f);

        common -= luckPower * 0.6f;
        rare -= luckPower * 0.2f;
        epic += luckPower * 0.35f;
        legendary += luckPower * 0.3f;
        mythic += luckPower * 0.15f;

        common = Mathf.Max(5f, common);
        rare = Mathf.Max(5f, rare);
        epic = Mathf.Max(1f, epic);
        legendary = Mathf.Max(0.5f, legendary);
        mythic = Mathf.Max(0.2f, mythic);

        float total = common + rare + epic + legendary + mythic;
        float roll = Random.Range(0f, total);

        if (roll < common) return UpgradeRarity.Common;
        roll -= common;

        if (roll < rare) return UpgradeRarity.Rare;
        roll -= rare;

        if (roll < epic) return UpgradeRarity.Epic;
        roll -= epic;

        if (roll < legendary) return UpgradeRarity.Legendary;

        return UpgradeRarity.Mythic;
    }

    private static UpgradeType RollType(HashSet<UpgradeType> excludedTypes = null)
    {
        List<UpgradeType> available = new List<UpgradeType>
        {
            UpgradeType.Strength,
            UpgradeType.MaxStamina,
            UpgradeType.StaminaRegen,
            UpgradeType.CritChance,
            UpgradeType.Luck,
            UpgradeType.Income,
            UpgradeType.Experience,
            UpgradeType.StaminaSteal
        };

        if (excludedTypes != null)
        {
            available.RemoveAll(t => excludedTypes.Contains(t));
        }

        return available[Random.Range(0, available.Count)];
    }

    private static float GetValue(UpgradeType type, UpgradeRarity rarity)
    {
        switch (type)
        {
            case UpgradeType.Strength:
                switch (rarity)
                {
                    case UpgradeRarity.Common: return 1f;
                    case UpgradeRarity.Rare: return 2f;
                    case UpgradeRarity.Epic: return 3f;
                    case UpgradeRarity.Legendary: return 4f;
                    case UpgradeRarity.Mythic: return 5f;
                }
                break;

            case UpgradeType.MaxStamina:
                switch (rarity)
                {
                    case UpgradeRarity.Common: return 10f;
                    case UpgradeRarity.Rare: return 20f;
                    case UpgradeRarity.Epic: return 35f;
                    case UpgradeRarity.Legendary: return 50f;
                    case UpgradeRarity.Mythic: return 70f;
                }
                break;

            case UpgradeType.StaminaRegen:
                switch (rarity)
                {
                    case UpgradeRarity.Common: return 1f;
                    case UpgradeRarity.Rare: return 2f;
                    case UpgradeRarity.Epic: return 4f;
                    case UpgradeRarity.Legendary: return 6f;
                    case UpgradeRarity.Mythic: return 10f;
                }
                break;

            case UpgradeType.CritChance:
                switch (rarity)
                {
                    case UpgradeRarity.Common: return 2f;
                    case UpgradeRarity.Rare: return 4f;
                    case UpgradeRarity.Epic: return 6f;
                    case UpgradeRarity.Legendary: return 9f;
                    case UpgradeRarity.Mythic: return 12f;
                }
                break;

            case UpgradeType.Luck:
                switch (rarity)
                {
                    case UpgradeRarity.Common: return 1f;
                    case UpgradeRarity.Rare: return 2f;
                    case UpgradeRarity.Epic: return 4f;
                    case UpgradeRarity.Legendary: return 6f;
                    case UpgradeRarity.Mythic: return 8f;
                }
                break;

            case UpgradeType.Income:
                switch (rarity)
                {
                    case UpgradeRarity.Common: return 5f;
                    case UpgradeRarity.Rare: return 8f;
                    case UpgradeRarity.Epic: return 12f;
                    case UpgradeRarity.Legendary: return 18f;
                    case UpgradeRarity.Mythic: return 25f;
                }
                break;

            case UpgradeType.Experience:
                switch (rarity)
                {
                    case UpgradeRarity.Common: return 5f;
                    case UpgradeRarity.Rare: return 8f;
                    case UpgradeRarity.Epic: return 12f;
                    case UpgradeRarity.Legendary: return 18f;
                    case UpgradeRarity.Mythic: return 25f;
                }
                break;

            case UpgradeType.StaminaSteal:
                switch (rarity)
                {
                    case UpgradeRarity.Common: return 2f;
                    case UpgradeRarity.Rare: return 4f;
                    case UpgradeRarity.Epic: return 6f;
                    case UpgradeRarity.Legendary: return 8f;
                    case UpgradeRarity.Mythic: return 12f;
                }
                break;
        }

        return 1f;
    }
}