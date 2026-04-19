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
        float roll = Random.Range(0f, 100f);

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
            rare += 0f;
            epic += 3f;
            legendary += 4f;
            mythic += 3f;
        }

        float border1 = common;
        float border2 = border1 + rare;
        float border3 = border2 + epic;
        float border4 = border3 + legendary;

        if (roll < border1) return UpgradeRarity.Common;
        if (roll < border2) return UpgradeRarity.Rare;
        if (roll < border3) return UpgradeRarity.Epic;
        if (roll < border4) return UpgradeRarity.Legendary;
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
            UpgradeType.Luck
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
                    case UpgradeRarity.Common: return 2f;
                    case UpgradeRarity.Rare: return 4f;
                    case UpgradeRarity.Epic: return 6f;
                    case UpgradeRarity.Legendary: return 8f;
                    case UpgradeRarity.Mythic: return 12f;
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
        }

        return 1f;
    }
}