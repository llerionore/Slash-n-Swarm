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
    [SerializeField] private PlayerStats playerStats;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("XP UI")]
    [SerializeField] private Image xpFill;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI levelUpText;

    [Header("Bomb")]
    [SerializeField] private float bombPauseDuration = 2f;

    [Header("Freeze")]
    [SerializeField] private float freezeDuration = 1.5f;
    [SerializeField] private float freezeSlowScale = 0.3f;

    [Header("Stamina")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaDrainPerSecond = 35f;
    [SerializeField] private float staminaRegenPerSecond = 20f;
    [SerializeField] private float regenDelay = 0.35f;

    [Header("Experience")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int currentXP = 0;
    [SerializeField] private int xpToNextLevel = 100;

    [Header("XP Multiplier")]
    [SerializeField] private float xpMultiplier = 1f;

    [Header("Gold Multiplier")]
    [SerializeField] private float goldMultiplier = 1f;

    [Header("Armor Reduction")]
    [SerializeField] private int armorReductionLevel = 0;

    [Header("Currency")]
    [SerializeField] private int currentCoins = 0;
    [SerializeField] private TextMeshProUGUI coinsText;

    [Header("Level Up Message")]
    [SerializeField] private float levelUpMessageDuration = 1.5f;

    [Header("Active Item")]
    [SerializeField] private AudioClip activeItemSound;
    [SerializeField] private float activeItemVolume = 1f;

    [Header("Rounds")]
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private int currentRound = 1;
    [SerializeField] private int targetFruits = 10;
    [SerializeField] private int currentFruits = 0;
    [SerializeField] private int extraSpawn = 2;

    [SerializeField] private int baseTargetFruits = 10;
    [SerializeField] private int fruitsPerRound = 3;
    [SerializeField] private int maxTargetFruits = 38;

    [SerializeField] private float baseSpawnSpeed = 1f;
    [SerializeField] private float spawnSpeedPerRound = 0.12f;
    [SerializeField] private float maxSpawnSpeed = 2.1f;

    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private float countdownStepTime = 0.6f;

    [Header("Round Multipliers")]
    [SerializeField] private float xpPerRound = 0.06f;
    [SerializeField] private float goldPerRound = 0.08f;

    [SerializeField] private float maxXpMultiplier = 1.8f;
    [SerializeField] private float maxGoldMultiplier = 2.0f;

    private int slicedFruits = 0;
    private int pendingLevelUps = 0;
    private bool infiniteStamina = false;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] sliceSounds;
    [SerializeField] private AudioClip levelUpSound;
    [SerializeField] private float levelUpVolume = 1f;
    [SerializeField] private float minSlicePitch = 0.9f;
    [SerializeField] private float maxSlicePitch = 1.1f;
    [SerializeField] private float sliceVolume = 1f;

    public float CurrentStamina { get; private set; }
    public int CurrentCoins => currentCoins;
    public int CurrentRound => currentRound;

    private float lastUseTime;
    private Coroutine levelUpRoutine;
    private Coroutine bombRoutine;
    private bool bombInProgress = false;
    private Coroutine freezeRoutine;

    private Vector3 defaultCameraPos;
    private float defaultCameraSize;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
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
        defaultCameraPos = Camera.main.transform.position;
        defaultCameraSize = Camera.main.orthographicSize;
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

        CurrentStamina = playerStats != null ? playerStats.MaxStamina : maxStamina;
        lastUseTime = -999f;
        infiniteStamina = false;
        xpMultiplier = 1f;
        goldMultiplier = 1f;

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
        if (infiniteStamina) return true;
        return CurrentStamina > 0f;
    }

    public bool TryUseStamina(float deltaTime)
    {
        if (infiniteStamina) return true;

        float staminaCap = playerStats != null ? playerStats.MaxStamina : maxStamina;

        if (CurrentStamina <= 0f)
        {
            CurrentStamina = 0f;
            return false;
        }

        CurrentStamina -= staminaDrainPerSecond * deltaTime;
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0f, staminaCap);

        lastUseTime = Time.time;
        return CurrentStamina > 0f;
    }

    public void TryStaminaSteal()
    {
        if (PlayerStats.Instance == null) return;

        float chance = PlayerStats.Instance.StaminaSteal;

        if (chance <= 0f) return;

        float roll = Random.Range(0f, 100f);

        if (roll > chance) return;

        float staminaCap = PlayerStats.Instance.MaxStamina;

        float restoreAmount = staminaCap * 0.05f;

        CurrentStamina += restoreAmount;
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0f, staminaCap);

        UpdateStaminaUI();
    }

    public void PlayActiveItemSound()
    {
        if (audioSource == null) return;
        if (activeItemSound == null) return;

        audioSource.pitch = 1f;
        audioSource.PlayOneShot(activeItemSound, activeItemVolume);
    }

    public void SetInfiniteStamina(bool value)
    {
        infiniteStamina = value;
        if (value) CurrentStamina = playerStats != null ? playerStats.MaxStamina : maxStamina;
    }

    public void SetXPMultiplier(float multiplier)
    {
        xpMultiplier = multiplier;
    }

    public void SetGoldMultiplier(float multiplier)
    {
        goldMultiplier = multiplier;
    }

    public void UpgradeArmorReduction()
    {
        armorReductionLevel++;
    }

    public int GetArmorReduction()
    {
        return armorReductionLevel;
    }

    public Spawner GetSpawner()
    {
        return spawner;
    }

    public void ReturnCamera(float speed)
    {
        StartCoroutine(ReturnCameraRoutine(defaultCameraPos, defaultCameraSize, speed));
    }

    private IEnumerator ReturnCameraRoutine(Vector3 originalPos, float originalSize, float speed)
    {
        Camera cam = Camera.main;
        float t = 0f;
        Vector3 currentPos = cam.transform.position;
        float currentSize = cam.orthographicSize;

        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            cam.orthographicSize = Mathf.Lerp(currentSize, originalSize, t);
            cam.transform.position = Vector3.Lerp(currentPos, originalPos, t);
            yield return null;
        }

        cam.orthographicSize = originalSize;
        cam.transform.position = originalPos;

        if (spawner != null) spawner.ResumeSpawning();
    }

    private void RegenerateStamina()
    {
        if (infiniteStamina) return;

        float staminaCap = playerStats != null ? playerStats.MaxStamina : maxStamina;
        float regenValue = playerStats != null ? playerStats.StaminaRegen : staminaRegenPerSecond;

        if (Time.time < lastUseTime + regenDelay) return;
        if (CurrentStamina >= staminaCap) return;

        CurrentStamina += regenValue * Time.deltaTime;
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0f, staminaCap);
    }

    private void UpdateStaminaUI()
    {
        if (staminaFill != null)
        {
            float staminaCap = playerStats != null ? playerStats.MaxStamina : maxStamina;
            staminaFill.fillAmount = CurrentStamina / staminaCap;
        }
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

    public bool TrySpendCoins(int amount)
    {
        if (currentCoins < amount) return false;
        currentCoins -= amount;
        UpdateCoinsUI();
        return true;
    }

    private void StartRound()
    {
        StartCoroutine(StartRoundRoutine());
    }

    private IEnumerator StartRoundRoutine()
    {
        currentFruits = 0;
        slicedFruits = 0;

        targetFruits = Mathf.Min(
            baseTargetFruits + ((currentRound - 1) * fruitsPerRound),
            maxTargetFruits
        );

        int spawnCount = targetFruits + extraSpawn;

        float spawnSpeed = Mathf.Min(
            baseSpawnSpeed + ((currentRound - 1) * spawnSpeedPerRound),
            maxSpawnSpeed
        );

        if (spawner != null)
            spawner.SetSpawnSpeedMultiplier(spawnSpeed);

        UpdateRoundUI();

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);

            countdownText.text = "3";
            yield return new WaitForSeconds(countdownStepTime);

            countdownText.text = "2";
            yield return new WaitForSeconds(countdownStepTime);

            countdownText.text = "1";
            yield return new WaitForSeconds(countdownStepTime);

            countdownText.text = "SLICE!";
            yield return new WaitForSeconds(0.35f);

            countdownText.gameObject.SetActive(false);
        }

        if (spawner != null)
            spawner.StartSpawning(spawnCount);
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

        if (pendingLevelUps > 0)
        {
            if (UpgradeManager.Instance != null)
                UpgradeManager.Instance.StartUpgradeSequence(currentRound, pendingLevelUps);
            else
                Debug.LogError("UpgradeManager.Instance is NULL");
        }
        else
        {
            OpenShopAfterRound();
        }
    }

    private void OpenShopAfterRound()
    {
        if (UpgradeManager.Instance != null)
            UpgradeManager.Instance.OpenShopOnly();
        else
            Debug.LogError("UpgradeManager.Instance is NULL");
    }

    public void BeginNextRoundAfterShop()
    {
        if (PlayerInventory.Instance != null)
            PlayerInventory.Instance.DecreaseCooldown();

        GameManager.Instance.SetXPMultiplier(1f);
        GameManager.Instance.SetGoldMultiplier(1f);

        pendingLevelUps = 0;
        StartRound();
    }

    private void LoseGame()
    {
        Debug.Log("GAME OVER");

        Time.timeScale = 0f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void AddXP(int amount)
    {
        float experiencePercent = 0f;

        if (PlayerStats.Instance != null)
            experiencePercent = PlayerStats.Instance.Experience;

        float statMultiplier = 1f + experiencePercent / 100f;

        float roundMultiplier = Mathf.Min(
            1f + ((currentRound - 1) * xpPerRound),
            maxXpMultiplier
        );

        float multiplier = statMultiplier * roundMultiplier;
        int finalAmount = Mathf.RoundToInt(amount * multiplier);

        currentXP += finalAmount;

        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            LevelUp();
        }

        UpdateXPUI();
    }

    public void AddFruitRewards(int coinAmount, int xpAmount)
    {
        bool isCrit = false;

        if (PlayerStats.Instance != null)
            isCrit = PlayerStats.Instance.RollCrit();

        int finalCoins = coinAmount;
        int finalXP = xpAmount;

        if (PlayerStats.Instance != null)
            finalCoins = PlayerStats.Instance.ApplyIncomeBonus(finalCoins);

        if (isCrit)
        {
            finalCoins *= 2;
            finalXP *= 2;
            Debug.Log("CRITICAL SLICE! x2 coins and XP");
        }

        currentCoins += finalCoins;
        AddXP(finalXP);

        UpdateCoinsUI();
    }

    public void AddCoins(int amount)
    {
        float incomePercent = 0f;
        if (PlayerStats.Instance != null)
            incomePercent = PlayerStats.Instance.Income;

        float statMultiplier = 1f + incomePercent / 100f;

        float roundMultiplier = Mathf.Min(
            1f + ((currentRound - 1) * goldPerRound),
            maxGoldMultiplier
        );

        float multiplier = statMultiplier * roundMultiplier;
        int finalAmount = Mathf.RoundToInt(amount * multiplier);

        if (finalAmount < 0) finalAmount = 0;

        currentCoins += finalAmount;
        UpdateCoinsUI();
    }

    public void AddFruitCoins(int amount)
    {
        AddCoins(Mathf.RoundToInt(amount));
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
        if (bombInProgress) return;
        bombInProgress = true;
        AddPenalty(penalty);
        PlaySliceSound();
        if (bombRoutine != null) StopCoroutine(bombRoutine);
        bombRoutine = StartCoroutine(BombRoutine());
    }

    private IEnumerator BombRoutine()
    {
        if (spawner != null) spawner.PauseSpawning();
        ClearScene();

        yield return new WaitForSeconds(bombPauseDuration);

        if (spawner != null) spawner.ResumeSpawning();

        bombRoutine = null;
        bombInProgress = false;
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

        float volume = levelUpVolume;

        if (SettingsManager.Instance != null)
            volume *= SettingsManager.Instance.SfxVolume;

        audioSource.PlayOneShot(levelUpSound, volume);
    }

    private void LevelUp()
    {
        currentLevel++;
        xpToNextLevel += 25;
        pendingLevelUps++;
        PlayLevelUpSound();
        ShowLevelUpMessage();
        Debug.Log("LEVEL UP QUEUED. Pending = " + pendingLevelUps);
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

        float volume = sliceVolume;

        if (SettingsManager.Instance != null)
            volume *= SettingsManager.Instance.SfxVolume;

        audioSource.PlayOneShot(clip, volume);
    }

    public void ReturnPineappleCamera(Camera pineappleCamera, Image darkOverlay, Image spotlightCircle, Vector3 originalPos, float originalSize, float speed)
    {
        StartCoroutine(ReturnPineapleCameraRoutine(pineappleCamera, darkOverlay, spotlightCircle, originalPos, originalSize, speed));
    }

    private IEnumerator ReturnPineapleCameraRoutine(Camera pineappleCam, Image overlay, Image spotlight, Vector3 originalPos, float originalSize, float speed)
    {
        Camera main = Camera.main;
        float t = 0f;
        Vector3 startPos = pineappleCam.transform.position;
        float startSize = pineappleCam.orthographicSize;

        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            pineappleCam.orthographicSize = Mathf.Lerp(startSize, originalSize, t);
            pineappleCam.transform.position = Vector3.Lerp(startPos, originalPos, t);

            if (overlay != null)
            {
                Color c = overlay.color;
                c.a = Mathf.Lerp(0.85f, 0f, t);
                overlay.color = c;
            }

            if (spotlight != null)
            {
                Color c = spotlight.color;
                c.a = Mathf.Lerp(1f, 0f, t);
                spotlight.color = c;
            }

            yield return null;
        }

        pineappleCam.orthographicSize = originalSize;
        pineappleCam.transform.position = originalPos;
        pineappleCam.gameObject.SetActive(false);

        if (overlay != null)
        {
            Color c = overlay.color;
            c.a = 0f;
            overlay.color = c;
            overlay.gameObject.SetActive(false);
        }

        if (spotlight != null)
        {
            Color c = spotlight.color;
            c.a = 0f;
            spotlight.color = c;
            spotlight.gameObject.SetActive(false);
        }

        main.depth = -1;

        if (spawner != null) spawner.ResumeSpawning();
    }
}