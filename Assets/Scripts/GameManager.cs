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

    [Header("Currency")]
    [SerializeField] private int currentCoins = 0;
    [SerializeField] private TextMeshProUGUI coinsText;

    [Header("Level Up Message")]
    [SerializeField] private float levelUpMessageDuration = 1.5f;

    [Header("Rounds")]
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private int currentRound = 1;
    [SerializeField] private int targetFruits = 10;
    [SerializeField] private int currentFruits = 0;
    [SerializeField] private int extraSpawn = 2;

    [Header("Bomb")]
    [SerializeField] private float bombPauseDuration = 2f;

    [Header("Freeze")]
    [SerializeField] private float freezeDuration = 1.5f;
    [SerializeField] private float freezeSlowScale = 0.3f;

    private int spawnedFruits = 0;
    private int slicedFruits = 0;

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
    private Coroutine freezeRoutine;

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
        if (Instance == this) Instance = null;
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
        currentRound = 1;
        currentCoins = 0;

        if (levelUpText != null) levelUpText.gameObject.SetActive(false);

        UpdateStaminaUI();
        UpdateXPUI();
        UpdateCoinsUI();
        StartRound();
    }

    private void ClearScene()
    {
        Fruit[] fruits = FindObjectsOfType<Fruit>();
        foreach (Fruit fruit in fruits) Destroy(fruit.gameObject);
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
            staminaFill.fillAmount = CurrentStamina / maxStamina;
    }

    private void UpdateXPUI()
    {
        if (xpFill != null)
            xpFill.fillAmount = (float)currentXP / xpToNextLevel;

        if (levelText != null)
            levelText.text = "LEVEL " + currentLevel;
    }

    private void UpdateCoinsUI()
    {
        if (coinsText != null)
            coinsText.text = currentCoins.ToString();
    }

    private void StartRound()
    {
        currentFruits = 0;
        slicedFruits = 0;
        spawnedFruits = 0;

        targetFruits = 10 + (currentRound * 2);
        int spawnCount = targetFruits + extraSpawn;

        if (spawner != null) spawner.StartSpawning(spawnCount);

        UpdateRoundUI();
    }

    private void UpdateRoundUI()
    {
        if (roundText == null) return;

        roundText.text = currentFruits + " / " + targetFruits;
        roundText.color = currentFruits >= targetFruits ? Color.green : Color.white;
    }

    public void OnSpawnFinished()
    {
        StartCoroutine(CheckEndRound());
    }

    private IEnumerator CheckEndRound()
    {
        yield return new WaitForSeconds(2f);

        if (currentFruits >= targetFruits) WinRound();
        else LoseGame();
    }

    private void WinRound()
    {
        currentRound++;
        StartRound();
    }

    private void LoseGame()
    {
        Debug.Log("GAME OVER");
        Time.timeScale = 0f;
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

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        UpdateCoinsUI();
    }

    public void OnFruitSliced()
    {
        currentFruits++;
        slicedFruits++;
        UpdateRoundUI();
    }

    public void AddPenalty(int amount)
    {
        currentFruits = Mathf.Max(0, currentFruits - amount);
        UpdateRoundUI();
    }

    public void BombExploded(int penalty)
    {
        AddPenalty(penalty);
        PlaySliceSound();
        StartCoroutine(BombRoutine());
    }

    private IEnumerator BombRoutine()
    {
        if (spawner != null) spawner.PauseSpawning();
        ClearScene();

        yield return new WaitForSeconds(bombPauseDuration);

        if (spawner != null) spawner.ResumeSpawning();
    }

    public void StartFreezeEffect()
    {
        if (freezeRoutine != null) StopCoroutine(freezeRoutine);
        freezeRoutine = StartCoroutine(FreezeRoutine());
    }

    private IEnumerator FreezeRoutine()
    {
        FrostEffect frost = Camera.main.GetComponent<FrostEffect>();
        if (frost != null)
        {
            frost.enabled = true;
            frost.FrostAmount = 0.3f;
        }

        Time.timeScale = freezeSlowScale;
        Time.fixedDeltaTime = 0.02f * freezeSlowScale;

        yield return new WaitForSecondsRealtime(freezeDuration);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        if (frost != null)
        {
            float t = 1f;
            while (t > 0f)
            {
                t -= Time.unscaledDeltaTime * 2f;
                frost.FrostAmount = Mathf.Lerp(0f, 0.3f, t);
                yield return null;
            }
            frost.FrostAmount = 0f;
            frost.enabled = false;
        }

        freezeRoutine = null;
    }

    private void PlayLevelUpSound()
    {
        if (audioSource == null || levelUpSound == null) return;
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(levelUpSound, levelUpVolume);
    }

    private void LevelUp()
    {
        currentLevel++;
        xpToNextLevel += 25;
        PlayLevelUpSound();
        ShowLevelUpMessage();
    }

    private void ShowLevelUpMessage()
    {
        if (levelUpText == null) return;
        if (levelUpRoutine != null) StopCoroutine(levelUpRoutine);
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