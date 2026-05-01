using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;

    public void Setup(ShopItemData item)
    {
        if (item == null) return;

        SetupIcon(item);

        if (nameText != null)
            nameText.text = item.itemName;
    }

    private void SetupIcon(ShopItemData item)
    {
        if (iconImage == null || item == null) return;

        iconImage.sprite = item.icon;
        iconImage.enabled = item.icon != null;

        Animator animator = iconImage.GetComponent<Animator>();

        if (animator == null) return;

        animator.runtimeAnimatorController = item.iconAnimator;
        animator.enabled = item.iconAnimator != null;

        if (animator.enabled)
        {
            animator.Rebind();
            animator.Update(0f);
            animator.Play(0, 0, 0f);
        }
    }
}