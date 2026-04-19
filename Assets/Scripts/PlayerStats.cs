using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("Base Stats")]
    [SerializeField] private int strength = 1;
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaRegen = 20f;
    [SerializeField] private float critChance = 0.05f;
    [SerializeField] private float luck = 0f;
    [SerializeField] private float income = 0f;
    [SerializeField] private float experience = 0f;

    public System.Action OnStatsChanged;

    public int Strength => strength;
    public float MaxStamina => maxStamina;
    public float StaminaRegen => staminaRegen;
    public float CritChance => critChance;
    public float Luck => luck;
    public float Income => income;
    public float Experience => experience;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddStrength(int amount)
    {
        strength += amount;
        OnStatsChanged?.Invoke();
    }

    public void AddMaxStamina(float amount)
    {
        maxStamina += amount;
        if (maxStamina < 1f)
            maxStamina = 1f;

        OnStatsChanged?.Invoke();
    }

    public void AddStaminaRegen(float amount)
    {
        staminaRegen += amount;
        if (staminaRegen < 0f)
            staminaRegen = 0f;

        OnStatsChanged?.Invoke();
    }

    public void AddCritChance(float amount)
    {
        critChance += amount;
        critChance = Mathf.Clamp01(critChance);
        OnStatsChanged?.Invoke();
    }

    public void AddLuck(float amount)
    {
        luck += amount;
        OnStatsChanged?.Invoke();
    }

    public void AddIncome(float amount)
    {
        income += amount;
        OnStatsChanged?.Invoke();
    }

    public void AddExperience(float amount)
    {
        experience += amount;
        OnStatsChanged?.Invoke();
    }
}