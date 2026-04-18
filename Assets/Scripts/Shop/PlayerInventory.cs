using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    [Header("Sell Settings")]
    [SerializeField][Range(0f, 1f)] private float activeSellRefundPercent = 0.5f;

    private readonly List<ShopItemData> passiveItems = new List<ShopItemData>();
    private ShopItemData activeItem;

    public IReadOnlyList<ShopItemData> PassiveItems => passiveItems;
    public ShopItemData ActiveItem => activeItem;
    public float ActiveSellRefundPercent => activeSellRefundPercent;

    public Action OnInventoryChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }

        Instance = this;
    }

    public bool HasActiveItem()
    {
        return activeItem != null;
    }

    public bool TryAddItem(ShopItemData item)
    {
        if (item == null) return false;

        if (item.itemType == ShopItemType.Active)
        {
            if (activeItem != null)
                return false;

            activeItem = item;
        }
        else
        {
            passiveItems.Add(item);
        }

        OnInventoryChanged?.Invoke();
        return true;
    }

    public ShopItemData RemoveActiveItem()
    {
        if (activeItem == null) return null;

        ShopItemData removed = activeItem;
        activeItem = null;

        OnInventoryChanged?.Invoke();
        return removed;
    }

    public void ClearAll()
    {
        passiveItems.Clear();
        activeItem = null;
        OnInventoryChanged?.Invoke();
    }
}