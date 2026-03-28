using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Blade blade;
    [SerializeField] private Spawner spawner;
    [SerializeField] private Image staminaFill;

    [Header("XP UI")]
    [SerializeField] private Image xpFill;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI levelUpText;

    [Header("Stamina")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaDrainPerSecond = 35f;
    [SerializeField] private float staminaRegenPerSecond = 20f;
    [SerializeField] private float regenDelay = 0.35f;

    [Header("Experience")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int currentXP = 0;
    [SerializeField] private int xpToNextLevel = 100;
    [SerializeField] private int xpPerFruit = 20;

    [Header("Level Up Message")]
    [SerializeField] private float levelUpMessageDuration = 1.5f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] sliceSounds;
    [SerializeField] private AudioClip levelUpSound;
    [SerializeField] private float levelUpVolume = 1f;
    [SerializeField] private float minSlicePitch = 0.9f;
    [SerializeField] private float maxSlicePitch = 1.1f;
    [SerializeField] private float sliceVolume = 1f;

    public float CurrentStamina { get; private set; }

    private float lastUseTime;
    private Coroutine levelUpRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        NewGame();
    }

    private void Update()
    {
        RegenerateStamina();
        UpdateStaminaUI();
        UpdateXPUI();
    }

    private void NewGame()
    {
        Time.timeScale = 1f;
        ClearScene();

        if (blade != null) blade.enabled = true;
        if (spawner != null) spawner.enabled = true;

        CurrentStamina = maxStamina;
        lastUseTime = -999f;

        currentLevel = 1;
        currentXP = 0;
        xpToNextLevel = 100;

        if (levelUpText != null)
        {
            levelUpText.gameObject.SetActive(false);
        }

        UpdateStaminaUI();
        UpdateXPUI();
    }

    private void ClearScene()
    {
        Fruit[] fruits = FindObjectsOfType<Fruit>();

        foreach (Fruit fruit in fruits)
        {
            Destroy(fruit.gameObject);
        }
    }

    public bool HasStamina()
    {
        return CurrentStamina > 0f;
    }

    public bool TryUseStamina(float deltaTime)
    {
        if (CurrentStamina <= 0f)
        {
            CurrentStamina = 0f;
            return false;
        }

        CurrentStamina -= staminaDrainPerSecond * deltaTime;
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0f, maxStamina);

        lastUseTime = Time.time;

        return CurrentStamina > 0f;
    }

    private void RegenerateStamina()
    {
        if (Time.time < lastUseTime + regenDelay) return;
        if (CurrentStamina >= maxStamina) return;

        CurrentStamina += staminaRegenPerSecond * Time.deltaTime;
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0f, maxStamina);
    }

    private void UpdateStaminaUI()
    {
        if (staminaFill != null)
        {
            staminaFill.fillAmount = CurrentStamina / maxStamina;
        }
    }

    private void UpdateXPUI()
    {
        if (xpFill != null)
        {
            xpFill.fillAmount = (float)currentXP / xpToNextLevel;
        }

        if (levelText != null)
        {
            levelText.text = "Level " + currentLevel;
        }
    }

    public void AddXP(int amount)
    {
        currentXP += amount;

        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            LevelUp();
        }

        UpdateXPUI();
    }

    public void AddFruitXP()
    {
        AddXP(xpPerFruit);
    }

    private void PlayLevelUpSound()
    {
        if (audioSource == null) return;
        if (levelUpSound == null) return;

        audioSource.pitch = 1f;
        audioSource.PlayOneShot(levelUpSound, levelUpVolume);
    }

    private void LevelUp()
    {
        currentLevel++;

        // Пока просто немного увеличим требуемый опыт на следующий уровень
        xpToNextLevel += 25;

        PlayLevelUpSound();
        ShowLevelUpMessage();
    }

    private void ShowLevelUpMessage()
    {
        if (levelUpText == null) return;

        if (levelUpRoutine != null)
        {
            StopCoroutine(levelUpRoutine);
        }

        levelUpRoutine = StartCoroutine(LevelUpMessageRoutine());
    }

    private IEnumerator LevelUpMessageRoutine()
    {
        levelUpText.gameObject.SetActive(true);
        levelUpText.text = "LEVEL UP!";

        yield return new WaitForSeconds(levelUpMessageDuration);

        levelUpText.gameObject.SetActive(false);
        levelUpRoutine = null;
    }

    public void PlaySliceSound()
    {
        if (audioSource == null) return;
        if (sliceSounds == null || sliceSounds.Length == 0) return;

        AudioClip clip = sliceSounds[Random.Range(0, sliceSounds.Length)];
        audioSource.pitch = Random.Range(minSlicePitch, maxSlicePitch);
        audioSource.PlayOneShot(clip, sliceVolume);
    }
}