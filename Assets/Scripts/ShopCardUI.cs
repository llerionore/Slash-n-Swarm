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
    [SerializeField] private Image mainScrollImage;
    [SerializeField] private Animator mainScrollAnimator;

    [Header("Visual")]
    [SerializeField] private GameObject visualRoot;

    [Header("Animation")]
    [SerializeField] private ScrollAnimationUI scrollAnimation;

    private ShopItemData currentItem;
    private System.Action<ShopCardUI> onBuy;

    public ShopItemData Item => currentItem;

    public void Setup(ShopItemData item, System.Action<ShopCardUI> buyCallback, System.Action<ShopCardUI> lockCallback)
    {
        currentItem = item;
        onBuy = buyCallback;

        gameObject.SetActive(true);

        if (visualRoot != null)
            visualRoot.SetActive(true);

        SetupIcon(item);

        if (titleText != null)
            titleText.text = item.itemName;

        if (descriptionText != null)
            descriptionText.text = item.description;

        if (priceText != null)
        {
            int price = ShopManager.Instance.GetScaledPrice(item.price);
            priceText.text = price.ToString();
        }

        if (buyButton != null)
        {
            buyButton.interactable = true;
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() => onBuy?.Invoke(this));
        }

        if (scrollAnimation != null)
            scrollAnimation.PlayOpenAnimation();

        if (mainScrollImage != null)
            mainScrollImage.enabled = true;

        if (mainScrollAnimator != null)
            mainScrollAnimator.enabled = true;
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

    public void MarkSold()
    {
        currentItem = null;

        if (buyButton != null)
        {
            buyButton.interactable = true;
            buyButton.onClick.RemoveAllListeners();
        }

        if (visualRoot != null)
            visualRoot.SetActive(false);

        if (mainScrollAnimator != null)
            mainScrollAnimator.enabled = false;

        if (mainScrollImage != null)
            mainScrollImage.enabled = false;
    }
}