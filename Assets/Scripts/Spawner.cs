using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Spawner : MonoBehaviour
{
    private Collider spawnArea;

    [Header("Prefabs")]
    [Header("Prefabs")]
    public GameObject[] fruitPrefabs;

    [Header("Armored Prefabs")]
    public GameObject[] armorTier1Prefabs;
    public GameObject[] armorTier2Prefabs;
    public GameObject[] armorTier3Prefabs;

    [Header("Armor Spawn Balance")]
    [SerializeField] private int armorStartRound = 4;

    [SerializeField] private float tier1ChanceRound4 = 0.20f;
    [SerializeField] private float tier1ChancePerRound = 0.04f;
    [SerializeField] private float tier1MaxChance = 0.45f;

    [SerializeField] private int tier2StartRound = 6;
    [SerializeField] private float tier2ChanceRound6 = 0.10f;
    [SerializeField] private float tier2ChancePerRound = 0.03f;
    [SerializeField] private float tier2MaxChance = 0.25f;

    [SerializeField] private int tier3StartRound = 8;
    [SerializeField] private float tier3ChanceRound8 = 0.05f;
    [SerializeField] private float tier3ChancePerRound = 0.02f;
    [SerializeField] private float tier3MaxChance = 0.12f;

    [Header("Coins")]
    public GameObject coinPrefab;
    [Range(0f, 1f)]
    public float coinSpawnChance = 0.1f;

    [Header("Bomb")]
    public GameObject bombPrefab;
    [SerializeField] private int bombStartRound = 3;
    [SerializeField] private float bombChanceRound3 = 0.05f;
    [SerializeField] private float bombChancePerRound = 0.015f;
    [SerializeField] private float bombMaxChance = 0.16f;

    [Header("Pineapple")]
    public GameObject pineapplePrefab;
    [Range(0f, 1f)]
    public float pineappleSpawnChance = 0.05f;

    [Header("Spawn Timing")]
    public float minSpawnDelay = 0.25f;
    public float maxSpawnDelay = 1f;

    [Header("Spawn Angle")]
    public float minAngle = -15f;
    public float maxAngle = 15f;

    [Header("Spawn Force")]
    public float minForce = 18f;
    public float maxForce = 22f;

    [Header("Spin")]
    public Vector3 minTorque = new Vector3(-6f, -6f, -15f);
    public Vector3 maxTorque = new Vector3(6f, 6f, 15f);

    private int fruitsToSpawn;
    private int spawnedCount;
    private Coroutine spawnRoutine;
    private float spawnSpeedMultiplier = 1f;

    private void Awake()
    {
        spawnArea = GetComponent<Collider>();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        spawnRoutine = null;
    }

    public void StartSpawning(int count)
    {
        fruitsToSpawn = count;
        spawnedCount = 0;
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    public void SetSpawnSpeedMultiplier(float multiplier)
    {
        spawnSpeedMultiplier = multiplier;
    }

    public void PauseSpawning()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    public void ResumeSpawning()
    {
        if (fruitsToSpawn - spawnedCount > 0)
            spawnRoutine = StartCoroutine(SpawnRoutine(true));
    }

    private IEnumerator SpawnRoutine(bool skipDelay = false)
    {
        if (!skipDelay) yield return null;

        while (spawnedCount < fruitsToSpawn)
        {
            SpawnFruit();
            spawnedCount++;

            float delay = Random.Range(minSpawnDelay, maxSpawnDelay) / spawnSpeedMultiplier;
            yield return new WaitForSeconds(delay);
        }

        spawnRoutine = null;
        if (GameManager.Instance != null)
            GameManager.Instance.OnSpawnFinished();
    }

    private void SpawnFruit()
    {
        if (fruitPrefabs == null || fruitPrefabs.Length == 0) return;
        if (spawnArea == null) return;

        if (pineapplePrefab != null && Random.value < pineappleSpawnChance)
        {
            SpawnObject(pineapplePrefab);
            return;
        }

        GameObject prefab = ChooseFruitPrefab();
        if (prefab != null)
            SpawnObject(prefab);

        if (coinPrefab != null && Random.value < coinSpawnChance)
        {
            Vector3 coinPosition = new Vector3(
                Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
                Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y),
                0f
            );
            Quaternion coinRotation = Quaternion.Euler(0f, 0f, Random.Range(minAngle, maxAngle));
            GameObject coin = Instantiate(coinPrefab, coinPosition, coinRotation);
            Rigidbody coinRb = coin.GetComponent<Rigidbody>();
            if (coinRb != null)
            {
                float force = Random.Range(minForce, maxForce);
                coinRb.AddForce(coin.transform.up * force, ForceMode.Impulse);
            }
        }

        int round = 1;

        if (GameManager.Instance != null)
            round = GameManager.Instance.CurrentRound;

        float bombChance = GetBombChance(round);

        if (bombPrefab != null && Random.value < bombChance)
        {
            SpawnObject(bombPrefab);
            return;
        }
    }

    private float GetBombChance(int round)
    {
        if (round < bombStartRound) return 0f;

        return Mathf.Min(
            bombChanceRound3 + ((round - bombStartRound) * bombChancePerRound),
            bombMaxChance
        );
    }

    private GameObject ChooseFruitPrefab()
    {
        int round = 1;

        if (GameManager.Instance != null)
            round = GameManager.Instance.CurrentRound;

        float tier1Chance = GetTier1Chance(round);
        float tier2Chance = GetTier2Chance(round);
        float tier3Chance = GetTier3Chance(round);

        float roll = Random.value;

        if (roll < tier3Chance && HasPrefabs(armorTier3Prefabs))
            return GetRandomPrefab(armorTier3Prefabs);

        roll -= tier3Chance;

        if (roll < tier2Chance && HasPrefabs(armorTier2Prefabs))
            return GetRandomPrefab(armorTier2Prefabs);

        roll -= tier2Chance;

        if (roll < tier1Chance && HasPrefabs(armorTier1Prefabs))
            return GetRandomPrefab(armorTier1Prefabs);

        return GetRandomPrefab(fruitPrefabs);
    }

    private float GetTier1Chance(int round)
    {
        if (round < armorStartRound) return 0f;

        return Mathf.Min(
            tier1ChanceRound4 + ((round - armorStartRound) * tier1ChancePerRound),
            tier1MaxChance
        );
    }

    private float GetTier2Chance(int round)
    {
        if (round < tier2StartRound) return 0f;

        return Mathf.Min(
            tier2ChanceRound6 + ((round - tier2StartRound) * tier2ChancePerRound),
            tier2MaxChance
        );
    }

    private float GetTier3Chance(int round)
    {
        if (round < tier3StartRound) return 0f;

        return Mathf.Min(
            tier3ChanceRound8 + ((round - tier3StartRound) * tier3ChancePerRound),
            tier3MaxChance
        );
    }

    private bool HasPrefabs(GameObject[] prefabs)
    {
        return prefabs != null && prefabs.Length > 0;
    }

    private GameObject GetRandomPrefab(GameObject[] prefabs)
    {
        if (!HasPrefabs(prefabs)) return null;
        return prefabs[Random.Range(0, prefabs.Length)];
    }

    private void SpawnObject(GameObject prefab)
    {
        Vector3 position = new Vector3(
            Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
            Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y),
            0f
        );

        Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(minAngle, maxAngle));
        GameObject obj = Instantiate(prefab, position, rotation);

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            float force = Random.Range(minForce, maxForce);
            rb.AddForce(obj.transform.up * force, ForceMode.Impulse);

            Vector3 randomTorque = new Vector3(
                Random.Range(minTorque.x, maxTorque.x),
                Random.Range(minTorque.y, maxTorque.y),
                Random.Range(minTorque.z, maxTorque.z)
            );
            rb.AddTorque(randomTorque, ForceMode.Impulse);
        }
    }
}