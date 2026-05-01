using UnityEngine;
using UnityEngine.UI;

public class PauseInventoryUI : MonoBehaviour
{
    [Header("Passive Items")]
    [SerializeField] private Transform passiveItemsContent;
    [SerializeField] private InventoryItem passiveItemPrefab;

    [Header("Active Item")]
    [SerializeField] private Image activeItemIcon;
    [SerializeField] private GameObject activeItemObject;

    private void OnEnable()
    {
        if (PlayerInventory.Instance != null)
            PlayerInventory.Instance.OnInventoryChanged += Refresh;

        Refresh();
    }

    private void OnDisable()
    {
        if (PlayerInventory.Instance != null)
            PlayerInventory.Instance.OnInventoryChanged -= Refresh;
    }

    public void Refresh()
    {
        RefreshPassiveItems();
        RefreshActiveItem();
    }

    private void RefreshPassiveItems()
    {
        if (passiveItemsContent == null || passiveItemPrefab == null)
            return;

        foreach (Transform child in passiveItemsContent)
            Destroy(child.gameObject);

        if (PlayerInventory.Instance == null)
            return;

        foreach (ShopItemData item in PlayerInventory.Instance.PassiveItems)
        {
            InventoryItem newItem = Instantiate(passiveItemPrefab, passiveItemsContent);
            newItem.Setup(item);
        }
    }

    private void RefreshActiveItem()
    {
        if (PlayerInventory.Instance == null)
        {
            if (activeItemObject != null)
                activeItemObject.SetActive(false);

            return;
        }

        ShopItemData activeItem = PlayerInventory.Instance.ActiveItem;
        bool hasActiveItem = activeItem != null;

        if (activeItemObject != null)
            activeItemObject.SetActive(hasActiveItem);

        if (activeItemIcon != null)
        {
            activeItemIcon.enabled = hasActiveItem && activeItem.icon != null;
            activeItemIcon.sprite = hasActiveItem ? activeItem.icon : null;
        }
    }
}