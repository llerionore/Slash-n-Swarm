using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActiveItemSlot : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Button mainButton;
    [SerializeField] private GameObject sellConfirmButton;
    [SerializeField] private Image cooldownOverlay;

    [Header("Mode")]
    [SerializeField] private bool useItemOnClick = false;

    private bool subscribed = false;
    private float cooldownTimer = 0f;
    private float cooldownDuration = 0f;

    private void Start()
    {
        TrySubscribe();

        if (mainButton != null)
        {
            mainButton.onClick.RemoveAllListeners();

            if (useItemOnClick)
                mainButton.onClick.AddListener(UseActiveItem);
            else
                mainButton.onClick.AddListener(ToggleSellConfirm);
        }

        if (sellConfirmButton != null)
            sellConfirmButton.SetActive(false);

        Refresh();
    }

    private void OnEnable()
    {
        TrySubscribe();
        Refresh();
    }

    private void OnDisable()
    {
        if (PlayerInventory.Instance != null && subscribed)
        {
            PlayerInventory.Instance.OnInventoryChanged -= Refresh;
            subscribed = false;
        }
    }

    private void TrySubscribe()
    {
        if (subscribed) return;
        if (PlayerInventory.Instance == null) return;

        PlayerInventory.Instance.OnInventoryChanged += Refresh;
        subscribed = true;
    }

    public void Refresh()
    {
        TrySubscribe();

        if (PlayerInventory.Instance == null)
        {
            ClearSlot();
            return;
        }

        ShopItemData item = PlayerInventory.Instance.ActiveItem;
        bool hasItem = item != null;

        if (iconImage != null)
        {
            iconImage.sprite = hasItem ? item.icon : null;
            iconImage.enabled = hasItem && item.icon != null;
        }

        if (nameText != null)
            nameText.text = hasItem ? item.itemName : "";

        if (!hasItem && sellConfirmButton != null)
            sellConfirmButton.SetActive(false);
    }

    private void ClearSlot()
    {
        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }

        if (nameText != null)
            nameText.text = "";
    }

    public void ToggleSellConfirm()
    {
        if (PlayerInventory.Instance == null) return;
        if (PlayerInventory.Instance.ActiveItem == null) return;

        if (sellConfirmButton != null)
            sellConfirmButton.SetActive(!sellConfirmButton.activeSelf);
    }

    public void UseActiveItem()
    {
        if (PlayerInventory.Instance == null) return;
        if (!PlayerInventory.Instance.CanUseActiveItem()) return;

        ShopItemData item = PlayerInventory.Instance.ActiveItem;
        if (item == null) return;

        Debug.Log("Used active item: " + item.itemName);

        if (GameManager.Instance != null)
            GameManager.Instance.PlayActiveItemSound();

        cooldownDuration = 10f;
        cooldownTimer = cooldownDuration;

        PlayerInventory.Instance.SetActiveItemCooldown(1);

        UpdateCooldownUI();
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            UpdateCooldownUI();
        }
    }

    private void UpdateCooldownUI()
    {
        if (cooldownOverlay == null) return;

        if (cooldownTimer <= 0f)
        {
            cooldownOverlay.fillAmount = 0f;
            cooldownOverlay.gameObject.SetActive(false);
        }
        else
        {
            cooldownOverlay.gameObject.SetActive(true);
            cooldownOverlay.fillAmount = cooldownTimer / cooldownDuration;
        }
    }

    public void SellActiveItem()
    {
        if (ShopManager.Instance != null)
            ShopManager.Instance.SellCurrentActiveItem();

        if (sellConfirmButton != null)
            sellConfirmButton.SetActive(false);
    }
}