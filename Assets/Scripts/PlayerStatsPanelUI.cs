using TMPro;
using UnityEngine;

public class PlayerStatsPanelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI strengthText;
    [SerializeField] private TextMeshProUGUI maxStaminaText;
    [SerializeField] private TextMeshProUGUI staminaRegenText;
    [SerializeField] private TextMeshProUGUI critChanceText;
    [SerializeField] private TextMeshProUGUI luckText;
    [SerializeField] private TextMeshProUGUI incomeText;
    [SerializeField] private TextMeshProUGUI experienceText;
    [SerializeField] private TextMeshProUGUI staminaStealText;

    private void OnEnable()
    {
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.OnStatsChanged += RefreshStats;

        RefreshStats();
    }

    private void OnDisable()
    {
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.OnStatsChanged -= RefreshStats;
    }

    public void RefreshStats()
    {
        if (PlayerStats.Instance == null)
        {
            Debug.LogError("PlayerStats.Instance is NULL");
            return;
        }

        strengthText.text = "Strength: " + PlayerStats.Instance.Strength;
        maxStaminaText.text = "Max Stamina: " + PlayerStats.Instance.MaxStamina.ToString("F0");
        staminaRegenText.text = "Stamina Regen: " + PlayerStats.Instance.StaminaRegen.ToString("F0");
        critChanceText.text = "Crit Chance: " + (PlayerStats.Instance.CritChance * 100f).ToString("F0") + "%";
        luckText.text = "Luck: " + PlayerStats.Instance.Luck.ToString("F0");
        incomeText.text = "Income: " + PlayerStats.Instance.Income.ToString("F0") + "%";
        experienceText.text = "Experience: " + PlayerStats.Instance.Experience.ToString("F0") + "%";
        staminaStealText.text = "Stamina Steal: " + PlayerStats.Instance.StaminaSteal.ToString("F0") + "%";
    }
}