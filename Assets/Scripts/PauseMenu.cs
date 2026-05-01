using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject restartConfirmPanel;

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

        if (restartConfirmPanel != null)
            restartConfirmPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (restartConfirmPanel != null && restartConfirmPanel.activeSelf)
            {
                HideRestartConfirm();
                return;
            }

            if (settingsPanel != null && settingsPanel.activeSelf)
            {
                settingsPanel.SetActive(false);
                return;
            }

            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        isOpen = !isOpen;

        if (pausePanel != null)
            pausePanel.SetActive(isOpen);

        if (!isOpen)
        {
            if (restartConfirmPanel != null)
                restartConfirmPanel.SetActive(false);

            if (settingsPanel != null)
                settingsPanel.SetActive(false);
        }

        Time.timeScale = isOpen ? 0f : 1f;

        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMuffled(isOpen);

        if (isOpen)
            RefreshStats();
    }

    public void ResumeGame()
    {
        isOpen = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (restartConfirmPanel != null)
            restartConfirmPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        Time.timeScale = 1f;

        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMuffled(false);
    }

    public void ShowRestartConfirm()
    {
        if (restartConfirmPanel != null)
            restartConfirmPanel.SetActive(true);
    }

    public void HideRestartConfirm()
    {
        if (restartConfirmPanel != null)
            restartConfirmPanel.SetActive(false);
    }

    public void ConfirmRestartGame()
    {
        Time.timeScale = 1f;

        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMuffled(false);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;

        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMuffled(false);

        SceneManager.LoadScene("MainMenu");
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

        if (experienceText != null)
            experienceText.text = "Experience: " + PlayerStats.Instance.Experience.ToString("F0") + "%";
    }
}