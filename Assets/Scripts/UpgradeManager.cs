using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject levelUpPanel;
    [SerializeField] private GameObject shopPanel;

    [Header("Cards")]
    [SerializeField] private UpgradeCardUI[] cards;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI rerollCostText;

    [Header("Reroll")]
    [SerializeField] private int baseRerollCost = 2;

    [Header("Stats UI")]
    [SerializeField] private PlayerStatsPanelUI statsPanelUI;

    private int currentRound = 1;
    private int rerollCount = 0;
    private int remainingSelections = 0;
    private bool waitingForChoice = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (levelUpPanel != null)
            levelUpPanel.SetActive(false);

        if (shopPanel != null)
            shopPanel.SetActive(false);
    }

    public void StartUpgradeSequence(int round, int numberOfSelections)
    {
        currentRound = round;
        remainingSelections = numberOfSelections;
        rerollCount = 0;
        waitingForChoice = true;

        if (shopPanel != null)
            shopPanel.SetActive(false);

        if (levelUpPanel != null)
            levelUpPanel.SetActive(true);

        Time.timeScale = 0f;

        GenerateCards();
        UpdateRerollText();
        RefreshStatsPanel();

        Debug.Log("Upgrade sequence started. Remaining = " + remainingSelections);
    }

    public void OpenShopOnly()
    {
        Debug.Log("OpenShopOnly called");

        if (levelUpPanel != null)
            levelUpPanel.SetActive(false);

        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.OpenShop(currentRound);
        }
        else
        {
            Debug.LogError("ShopManager.Instance is NULL");
        }

        Time.timeScale = 0f;
        RefreshStatsPanel();
    }

    public void CloseShopAndContinue()
    {
        if (shopPanel != null)
            shopPanel.SetActive(false);

        Time.timeScale = 1f;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.BeginNextRoundAfterShop();
        }
    }

    public void RerollAll()
    {
        if (!waitingForChoice) return;
        if (GameManager.Instance == null) return;

        int cost = GetCurrentRerollCost();

        if (!GameManager.Instance.TrySpendCoins(cost))
        {
            Debug.Log("Not enough coins for reroll");
            return;
        }

        rerollCount++;
        GenerateCards();
        UpdateRerollText();
        RefreshStatsPanel();
    }

    private void GenerateCards()
    {
        HashSet<UpgradeType> usedTypes = new HashSet<UpgradeType>();

        for (int i = 0; i < cards.Length; i++)
        {
            UpgradeData data = UpgradeGenerator.GenerateRandom(currentRound, usedTypes);
            usedTypes.Add(data.type);
            cards[i].Setup(data, OnUpgradeSelected);
        }
    }

    private void OnUpgradeSelected(UpgradeData data)
    {
        ApplyUpgrade(data);
        RefreshStatsPanel();

        remainingSelections--;

        Debug.Log("Upgrade selected. Remaining = " + remainingSelections);

        if (remainingSelections > 0)
        {
            rerollCount = 0;
            GenerateCards();
            UpdateRerollText();
            return;
        }

        waitingForChoice = false;

        Debug.Log("Trying to open shop");

        if (levelUpPanel != null)
            levelUpPanel.SetActive(false);

        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.OpenShop(currentRound);
        }
        else
        {
            Debug.LogError("ShopManager.Instance is NULL");
        }

        Time.timeScale = 0f;
        RefreshStatsPanel();
    }

    private void ApplyUpgrade(UpgradeData data)
    {
        if (PlayerStats.Instance == null) return;

        switch (data.type)
        {
            case UpgradeType.Strength:
                PlayerStats.Instance.AddStrength((int)data.value);
                break;

            case UpgradeType.MaxStamina:
                PlayerStats.Instance.AddMaxStamina(data.value);
                break;

            case UpgradeType.StaminaRegen:
                PlayerStats.Instance.AddStaminaRegen(data.value);
                break;

            case UpgradeType.CritChance:
                PlayerStats.Instance.AddCritChance(data.value / 100f);
                break;

            case UpgradeType.Luck:
                PlayerStats.Instance.AddLuck(data.value);
                break;
        }
    }

    private int GetCurrentRerollCost()
    {
        return baseRerollCost + rerollCount;
    }

    private void UpdateRerollText()
    {
        if (rerollCostText != null)
        {
            rerollCostText.text = "Reroll - " + GetCurrentRerollCost();
        }
    }

    private void RefreshStatsPanel()
    {
        if (statsPanelUI != null)
        {
            statsPanelUI.RefreshStats();
        }
        else
        {
            Debug.LogError("statsPanelUI is NULL in UpgradeManager");
        }
    }
}