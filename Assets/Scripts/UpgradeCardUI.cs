using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    [System.Serializable]
    public class UpgradeVisual
    {
        public UpgradeType type;
        public Sprite icon;
        public RuntimeAnimatorController animatorController;
    }

    [Header("UI")]
    [SerializeField] private Image cardBackground;          
    [SerializeField] private Image cardTitleBackground;     
    [SerializeField] private Image itemFrame;               
    [SerializeField] private TextMeshProUGUI titleText;     
    [SerializeField] private TextMeshProUGUI descriptionText; 

    [Header("Hover")]
    [SerializeField] private Vector3 hoverScale = new Vector3(1.05f, 1.05f, 1f);

    [SerializeField] private Image upgradeIconImage;
    [SerializeField] private Animator upgradeIconAnimator;
    [SerializeField] private UpgradeVisual[] upgradeVisuals;

    private Vector3 originalScale;
    private Color baseCardColor;
    private Color baseTitleColor;
    private Color baseFrameColor;

    private UpgradeData currentData;
    private System.Action<UpgradeData> onClicked;

    private void Awake()
    {
        originalScale = transform.localScale;

        if (cardBackground != null)
            baseCardColor = cardBackground.color;

        if (cardTitleBackground != null)
            baseTitleColor = cardTitleBackground.color;

        if (itemFrame != null)
            baseFrameColor = itemFrame.color;
    }

    public void Setup(UpgradeData data, System.Action<UpgradeData> clickCallback)
    {
        currentData = data;
        onClicked = clickCallback;

        if (titleText != null)
            titleText.text = GetUpgradeName(data.type);

        if (descriptionText != null)
            descriptionText.text = GetDescription(data);

        SetupIcon(data.type);

        Color rarityColor = GetColorByRarity(data.rarity);

        if (cardBackground != null)
        {
            baseCardColor = Color.white;
            cardBackground.color = baseCardColor;
        }

        if (cardTitleBackground != null)
        {
            baseTitleColor = rarityColor;
            cardTitleBackground.color = baseTitleColor;
        }

        if (itemFrame != null)
        {
            baseFrameColor = rarityColor;
            itemFrame.color = baseFrameColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = hoverScale;

        if (cardBackground != null)
        {
            cardBackground.color = new Color(0.92f, 0.92f, 0.92f, 1f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale;

        if (cardBackground != null)
            cardBackground.color = baseCardColor;

        if (cardTitleBackground != null)
            cardTitleBackground.color = baseTitleColor;

        if (itemFrame != null)
            itemFrame.color = baseFrameColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClicked?.Invoke(currentData);
    }

    private string GetUpgradeName(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.Strength: return "Strength";
            case UpgradeType.MaxStamina: return "Max Stamina";
            case UpgradeType.StaminaRegen: return "Stamina Regen";
            case UpgradeType.CritChance: return "Crit Chance";
            case UpgradeType.Luck: return "Luck";
            case UpgradeType.Income: return "Income";
            case UpgradeType.Experience: return "Experience";
            case UpgradeType.StaminaSteal: return "Stamina Steal";
            default: return "Upgrade";
        }
    }

    private string GetDescription(UpgradeData data)
    {
        switch (data.type)
        {
            case UpgradeType.Strength:
                return "+" + data.value + " Strength";
            case UpgradeType.MaxStamina:
                return "+" + data.value + " Max Stamina";
            case UpgradeType.StaminaRegen:
                return "+" + data.value + " Stamina Regen";
            case UpgradeType.CritChance:
                return "+" + data.value + "% Crit Chance";
            case UpgradeType.Luck:
                return "+" + data.value + " Luck";
            case UpgradeType.Income:
                return "+" + data.value + "% Income";
            case UpgradeType.Experience:
                return "+" + data.value + "% Experience";
            case UpgradeType.StaminaSteal:
                return "+" + data.value + "% Stamina Steal";
            default:
                return "+" + data.value;
        }
    }

    private Color GetColorByRarity(UpgradeRarity rarity)
    {
        switch (rarity)
        {
            case UpgradeRarity.Common:
                return new Color(0.75f, 0.75f, 0.75f);
            case UpgradeRarity.Rare:
                return new Color(0.35f, 0.55f, 1f);
            case UpgradeRarity.Epic:
                return new Color(0.7f, 0.4f, 1f);
            case UpgradeRarity.Legendary:
                return new Color(1f, 0.82f, 0.2f);
            case UpgradeRarity.Mythic:
                return new Color(0.35f, 1f, 0.9f);
            default:
                return Color.white;
        }
    }

    private void SetupIcon(UpgradeType type)
    {
        UpgradeVisual visual = null;

        foreach (UpgradeVisual v in upgradeVisuals)
        {
            if (v.type == type)
            {
                visual = v;
                break;
            }
        }

        if (upgradeIconImage != null)
        {
            upgradeIconImage.sprite = visual != null ? visual.icon : null;
            upgradeIconImage.enabled = visual != null && visual.icon != null;
        }

        if (upgradeIconAnimator != null)
        {
            upgradeIconAnimator.runtimeAnimatorController =
                visual != null ? visual.animatorController : null;

            upgradeIconAnimator.enabled =
                visual != null && visual.animatorController != null;

            if (upgradeIconAnimator.enabled)
            {
                upgradeIconAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
                upgradeIconAnimator.Rebind();
                upgradeIconAnimator.Update(0f);
                upgradeIconAnimator.Play(0, 0, 0f);
            }
        }
    }
}