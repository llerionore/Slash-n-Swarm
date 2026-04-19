using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [Header("Panel")]
    [SerializeField] private GameObject shopPanel;

    [Header("Pool")]
    [SerializeField] private ShopItemData[] itemPool;

    [Header("Cards")]
    [SerializeField] private ShopCardUI[] shopCards;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI rerollCostText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private Button nextRoundButton;

    [Header("Inventory UI")]
    [SerializeField] private Transform passiveItemsRoot;
    [SerializeField] private GameObject passiveItemPrefab;
    [SerializeField] private ActiveItemSlot activeItemSlotUI;

    [Header("Stats UI")]
    [SerializeField] private PlayerStatsPanelUI statsPanelUI;

    [Header("Settings")]
    [SerializeField] private int baseRerollCost = 1;

    private int currentRound;
    private int rerollCount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (shopPanel != null)
            shopPanel.SetActive(false);
    }

    private void Start()
    {
        Debug.Log("[ShopManager] Start");
        Debug.Log("[ShopManager] itemPool length = " + (itemPool == null ? -1 : itemPool.Length));
        Debug.Log("[ShopManager] shopCards length = " + (shopCards == null ? -1 : shopCards.Length));

        if (nextRoundButton != null)
        {
            nextRoundButton.onClick.RemoveAllListeners();
            nextRoundButton.onClick.AddListener(CloseShopAndContinue);
        }
    }

    private void OnEnable()
    {
        if (PlayerInventory.Instance != null)
            PlayerInventory.Instance.OnInventoryChanged += RefreshInventoryUI;
    }

    private void OnDisable()
    {
        if (PlayerInventory.Instance != null)
            PlayerInventory.Instance.OnInventoryChanged -= RefreshInventoryUI;
    }

    public void OpenShop(int round)
    {
        Debug.Log("[ShopManager] OpenShop called. round = " + round);

        currentRound = round;
        rerollCount = 0;

        if (shopPanel != null)
            shopPanel.SetActive(true);
        else
            Debug.LogError("[ShopManager] shopPanel is NULL");

        Time.timeScale = 0f;

        GenerateShop(false);
        RefreshAllUI();
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

    public void RerollShop()
    {
        if (GameManager.Instance == null) return;

        int cost = GetRerollCost();
        if (!GameManager.Instance.TrySpendCoins(cost))
        {
            Debug.Log("Not enough coins for reroll");
            return;
        }

        rerollCount++;
        GenerateShop(true);
        RefreshAllUI();
    }

    public void SellCurrentActiveItem()
    {
        if (PlayerInventory.Instance == null) return;
        if (GameManager.Instance == null) return;

        ShopItemData activeItem = PlayerInventory.Instance.ActiveItem;
        if (activeItem == null) return;
        if (activeItem.itemType != ShopItemType.Active) return;

        RemoveItemEffects(activeItem);

        ShopItemData removed = PlayerInventory.Instance.RemoveActiveItem();
        if (removed == null) return;

        int refund = Mathf.RoundToInt(removed.price * PlayerInventory.Instance.ActiveSellRefundPercent);
        GameManager.Instance.AddCoins(refund);

        RefreshAllUI();
    }

    private int GetRerollCost()
    {
        return baseRerollCost + rerollCount;
    }

    private void GenerateShop(bool reroll)
    {
        Debug.Log("[ShopManager] GenerateShop called");
        Debug.Log("[ShopManager] itemPool null = " + (itemPool == null));
        Debug.Log("[ShopManager] itemPool length = " + (itemPool == null ? -1 : itemPool.Length));
        Debug.Log("[ShopManager] shopCards null = " + (shopCards == null));
        Debug.Log("[ShopManager] shopCards length = " + (shopCards == null ? -1 : shopCards.Length));

        if (itemPool == null || itemPool.Length == 0)
        {
            Debug.LogError("[ShopManager] itemPool is empty");
            return;
        }

        for (int i = 0; i < shopCards.Length; i++)
        {
            ShopCardUI card = shopCards[i];

            if (card == null)
            {
                Debug.LogError("[ShopManager] shopCards[" + i + "] is NULL");
                continue;
            }

            if (reroll && card.IsLocked && card.Item != null)
                continue;

            ShopItemData item = itemPool[Random.Range(0, itemPool.Length)];

            if (item == null)
            {
                Debug.LogError("[ShopManager] selected item is NULL");
                continue;
            }

            Debug.Log("[ShopManager] Setting card " + i + " -> " + item.itemName);
            card.Setup(item, TryBuyItem, ToggleLockCard);
        }
    }

    private void TryBuyItem(ShopCardUI card)
    {
        Debug.Log("[ShopManager] TryBuyItem called");

        if (card == null)
        {
            Debug.LogError("[ShopManager] card is NULL");
            return;
        }

        if (card.Item == null)
        {
            Debug.LogError("[ShopManager] card.Item is NULL");
            return;
        }

        Debug.Log("[ShopManager] Buying item: " + card.Item.itemName);

        if (GameManager.Instance == null)
        {
            Debug.LogError("[ShopManager] GameManager.Instance is NULL");
            return;
        }

        if (PlayerInventory.Instance == null)
        {
            Debug.LogError("[ShopManager] PlayerInventory.Instance is NULL");
            return;
        }

        ShopItemData item = card.Item;

        if (item.itemType == ShopItemType.Active && PlayerInventory.Instance.HasActiveItem())
        {
            Debug.LogWarning("[ShopManager] Active item slot already occupied");
            return;
        }

        if (!GameManager.Instance.TrySpendCoins(item.price))
        {
            Debug.LogWarning("[ShopManager] Not enough coins");
            return;
        }

        bool added = PlayerInventory.Instance.TryAddItem(item);
        Debug.Log("[ShopManager] TryAddItem result = " + added);

        if (!added)
            return;

        ApplyItemEffects(item);
        card.MarkSold();
        RefreshAllUI();
    }

    private void ToggleLockCard(ShopCardUI card)
    {
        if (card == null) return;
        card.ToggleLock();
    }

    private void ApplyItemEffects(ShopItemData item)
    {
        if (PlayerStats.Instance == null || item == null) return;

        foreach (ShopEffect effect in item.effects)
        {
            switch (effect.effectType)
            {
                case ShopEffectType.Strength:
                    PlayerStats.Instance.AddStrength(Mathf.RoundToInt(effect.value));
                    break;

                case ShopEffectType.MaxStamina:
                    PlayerStats.Instance.AddMaxStamina(effect.value);
                    break;

                case ShopEffectType.StaminaRegen:
                    PlayerStats.Instance.AddStaminaRegen(effect.value);
                    break;

                case ShopEffectType.CritChance:
                    PlayerStats.Instance.AddCritChance(effect.value / 100f);
                    break;

                case ShopEffectType.Luck:
                    PlayerStats.Instance.AddLuck(effect.value);
                    break;

                case ShopEffectType.Income:
                    PlayerStats.Instance.AddIncome(effect.value);
                    break;

                case ShopEffectType.Experience:
                    PlayerStats.Instance.AddExperience(effect.value);
                    break;
            }
        }
    }

    private void RemoveItemEffects(ShopItemData item)
    {
        if (PlayerStats.Instance == null || item == null) return;

        foreach (ShopEffect effect in item.effects)
        {
            switch (effect.effectType)
            {
                case ShopEffectType.Strength:
                    PlayerStats.Instance.AddStrength(-Mathf.RoundToInt(effect.value));
                    break;

                case ShopEffectType.MaxStamina:
                    PlayerStats.Instance.AddMaxStamina(-effect.value);
                    break;

                case ShopEffectType.StaminaRegen:
                    PlayerStats.Instance.AddStaminaRegen(-effect.value);
                    break;

                case ShopEffectType.CritChance:
                    PlayerStats.Instance.AddCritChance(-effect.value / 100f);
                    break;

                case ShopEffectType.Luck:
                    PlayerStats.Instance.AddLuck(-effect.value);
                    break;

                case ShopEffectType.Income:
                    PlayerStats.Instance.AddIncome(-effect.value);
                    break;
                case ShopEffectType.Experience:
                    PlayerStats.Instance.AddExperience(-effect.value);
                    break;
            }
        }
    }

    private void RefreshAllUI()
    {
        RefreshCoinsUI();
        RefreshRerollUI();
        RefreshWaveUI();
        RefreshInventoryUI();

        if (statsPanelUI != null)
            statsPanelUI.RefreshStats();
    }

    private void RefreshCoinsUI()
    {
        if (coinsText != null && GameManager.Instance != null)
            coinsText.text = GameManager.Instance.CurrentCoins.ToString();
    }

    private void RefreshRerollUI()
    {
        if (rerollCostText != null)
            rerollCostText.text = "Reroll - " + GetRerollCost();
    }

    private void RefreshWaveUI()
    {
        if (waveText != null)
            waveText.text = "Shop (Round " + currentRound + ")";
    }

    private void RefreshInventoryUI()
    {
        RefreshPassiveInventoryUI();

        if (activeItemSlotUI != null)
            activeItemSlotUI.Refresh();
    }

    private void RefreshPassiveInventoryUI()
    {
        ClearChildren(passiveItemsRoot);

        if (PlayerInventory.Instance == null) return;
        if (passiveItemsRoot == null || passiveItemPrefab == null) return;

        foreach (ShopItemData item in PlayerInventory.Instance.PassiveItems)
        {
            GameObject obj = Instantiate(passiveItemPrefab, passiveItemsRoot);
            InventoryItem ui = obj.GetComponent<InventoryItem>();

            if (ui != null)
                ui.Setup(item);
        }
    }

    private void ClearChildren(Transform root)
    {
        if (root == null) return;

        for (int i = root.childCount - 1; i >= 0; i--)
        {
            Destroy(root.GetChild(i).gameObject);
        }
    }
}