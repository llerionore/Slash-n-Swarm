using TMPro;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;

    [Header("Stat Texts")]
    [SerializeField] private TextMeshProUGUI strengthText;
    [SerializeField] private TextMeshProUGUI maxStaminaText;
    [SerializeField] private TextMeshProUGUI staminaRegenText;
    [SerializeField] private TextMeshProUGUI critChanceText;
    [SerializeField] private TextMeshProUGUI luckText;
    [SerializeField] private TextMeshProUGUI incomeText;
    [SerializeField] private TextMeshProUGUI experienceText;

    private bool isOpen = false;

    private void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ToggleMenu();
    }

    private void OnEnable()
    {
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.OnStatsChanged += RefreshStats;
    }

    private void OnDisable()
    {
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.OnStatsChanged -= RefreshStats;
    }

    public void ToggleMenu()
    {
        isOpen = !isOpen;

        if (pausePanel != null)
            pausePanel.SetActive(isOpen);

        Time.timeScale = isOpen ? 0f : 1f;

        if (isOpen)
            RefreshStats();
    }

    public void ResumeGame()
    {
        isOpen = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1f;
    }

    public void RefreshStatsPublic()
    {
        RefreshStats();
    }

    private void RefreshStats()
    {
        if (PlayerStats.Instance == null) return;

        strengthText.text = "Strength: " + PlayerStats.Instance.Strength;
        maxStaminaText.text = "Max Stamina: " + PlayerStats.Instance.MaxStamina.ToString("F0");
        staminaRegenText.text = "Stamina Regen: " + PlayerStats.Instance.StaminaRegen.ToString("F1");
        critChanceText.text = "Crit Chance: " + (PlayerStats.Instance.CritChance * 100f).ToString("F0") + "%";
        luckText.text = "Luck: " + PlayerStats.Instance.Luck.ToString("F0");

        if (incomeText != null)
            incomeText.text = "Income: " + PlayerStats.Instance.Income.ToString("F0") + "%";

        if (incomeText != null)
            experienceText.text = "Experience: " + PlayerStats.Instance.Experience.ToString("F0") + "%";
    }
}