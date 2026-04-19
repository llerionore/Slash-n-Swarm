using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopCardUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button lockButton;
    [SerializeField] private GameObject lockedMark;

    private ShopItemData currentItem;
    private System.Action<ShopCardUI> onBuy;
    private System.Action<ShopCardUI> onLock;

    public ShopItemData Item => currentItem;
    public bool IsLocked { get; private set; }

    public void Setup(ShopItemData item, System.Action<ShopCardUI> buyCallback, System.Action<ShopCardUI> lockCallback)
    {
        Debug.Log("[ShopCardUI] Setup called for " + (item != null ? item.itemName : "NULL"));

        currentItem = item;
        onBuy = buyCallback;
        onLock = lockCallback;

        if (iconImage != null)
        {
            iconImage.sprite = item.icon;
            iconImage.enabled = item.icon != null;
        }

        if (titleText != null)
            titleText.text = item.itemName;
        else
            Debug.LogError("[ShopCardUI] titleText is NULL");

        if (descriptionText != null)
            descriptionText.text = item.description;
        else
            Debug.LogError("[ShopCardUI] descriptionText is NULL");

        if (priceText != null)
            priceText.text = item.price.ToString();
        else
            Debug.LogError("[ShopCardUI] priceText is NULL");

        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() => onBuy?.Invoke(this));
        }
        else
        {
            Debug.LogError("[ShopCardUI] buyButton is NULL");
        }

            if (lockButton != null)
        {
            lockButton.onClick.RemoveAllListeners();
            lockButton.onClick.AddListener(() => onLock?.Invoke(this));
        }

        UpdateLockVisual();
        gameObject.SetActive(true);
    }

    public void ToggleLock()
    {
        IsLocked = !IsLocked;
        UpdateLockVisual();
    }

    public void MarkSold()
    {
        currentItem = null;
        gameObject.SetActive(false);
    }

    private void UpdateLockVisual()
    {
        if (lockedMark != null)
            lockedMark.SetActive(IsLocked);
    }
}