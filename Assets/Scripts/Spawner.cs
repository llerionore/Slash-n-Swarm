using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Spawner : MonoBehaviour
{
    private Collider spawnArea;

    [Header("Prefabs")]
    public GameObject[] fruitPrefabs;

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

        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
        }

        spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(1f);

        while (spawnedCount < fruitsToSpawn)
        {
            SpawnFruit();
            spawnedCount++;

            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
        }

        spawnRoutine = null;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnSpawnFinished();
        }
    }

    private void SpawnFruit()
    {
        if (fruitPrefabs == null || fruitPrefabs.Length == 0) return;
        if (spawnArea == null) return;

        GameObject prefab = fruitPrefabs[Random.Range(0, fruitPrefabs.Length)];

        Vector3 position = new Vector3(
            Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
            Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y),
            0f
        );

        Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(minAngle, maxAngle));

        GameObject fruit = Instantiate(prefab, position, rotation);

        Rigidbody rb = fruit.GetComponent<Rigidbody>();
        if (rb != null)
        {
            float force = Random.Range(minForce, maxForce);
            rb.AddForce(fruit.transform.up * force, ForceMode.Impulse);

            Vector3 randomTorque = new Vector3(
                Random.Range(minTorque.x, maxTorque.x),
                Random.Range(minTorque.y, maxTorque.y),
                Random.Range(minTorque.z, maxTorque.z)
            );

            rb.AddTorque(randomTorque, ForceMode.Impulse);
        }
    }
}