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

    private void Start()
    {
        if (mainButton != null)
        {
            mainButton.onClick.RemoveAllListeners();
            mainButton.onClick.AddListener(ToggleSellConfirm);
        }

        if (sellConfirmButton != null)
        {
            sellConfirmButton.SetActive(false);
        }

        Refresh();
    }

    private void OnEnable()
    {
        if (PlayerInventory.Instance != null)
            PlayerInventory.Instance.OnInventoryChanged += Refresh;
    }

    private void OnDisable()
    {
        if (PlayerInventory.Instance != null)
            PlayerInventory.Instance.OnInventoryChanged -= Refresh;
    }

    public void Refresh()
    {
        ShopItemData item = PlayerInventory.Instance != null ? PlayerInventory.Instance.ActiveItem : null;
        bool hasItem = item != null;

        if (iconImage != null)
        {
            iconImage.enabled = hasItem && item.icon != null;
            iconImage.sprite = hasItem ? item.icon : null;
        }

        if (nameText != null)
        {
            nameText.text = hasItem ? item.itemName : "";
        }

        if (!hasItem && sellConfirmButton != null)
        {
            sellConfirmButton.SetActive(false);
        }
    }

    public void ToggleSellConfirm()
    {
        if (PlayerInventory.Instance == null) return;
        if (PlayerInventory.Instance.ActiveItem == null) return;

        if (sellConfirmButton != null)
        {
            sellConfirmButton.SetActive(!sellConfirmButton.activeSelf);
        }
    }

    public void SellActiveItem()
    {
        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.SellCurrentActiveItem();
        }

        if (sellConfirmButton != null)
        {
            sellConfirmButton.SetActive(false);
        }
    }
}