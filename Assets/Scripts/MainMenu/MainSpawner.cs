using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MenuSpawner : MonoBehaviour
{
    private Collider spawnArea;
    private Coroutine spawnRoutine;

    [Header("Prefabs")]
    [SerializeField] private GameObject[] spawnPrefabs;

    [Header("Spawn Timing")]
    [SerializeField] private float startDelay = 0.5f;
    [SerializeField] private float minSpawnDelay = 0.6f;
    [SerializeField] private float maxSpawnDelay = 1.4f;

    [Header("Spawn Force")]
    [SerializeField] private float minForce = 14f;
    [SerializeField] private float maxForce = 20f;

    [Header("Spawn Angle")]
    [SerializeField] private float minAngle = -20f;
    [SerializeField] private float maxAngle = 20f;

    [Header("Spin")]
    [SerializeField] private Vector3 minTorque = new Vector3(-4f, -4f, -10f);
    [SerializeField] private Vector3 maxTorque = new Vector3(4f, 4f, 10f);

    [Header("Lifetime")]
    [SerializeField] private float objectLifetime = 6f;

    private void Awake()
    {
        spawnArea = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        StartSpawning();
    }

    private void OnDisable()
    {
        StopSpawning();
    }

    public void StartSpawning()
    {
        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);

        spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    public void StopSpawning()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    private IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(startDelay);

        while (true)
        {
            SpawnObject();

            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);
        }
    }

    private void SpawnObject()
    {
        if (spawnPrefabs == null || spawnPrefabs.Length == 0) return;
        if (spawnArea == null) return;

        GameObject prefab = spawnPrefabs[Random.Range(0, spawnPrefabs.Length)];

        Vector3 spawnPosition = new Vector3(
            Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
            Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y),
            0f
        );

        Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(minAngle, maxAngle));
        GameObject spawnedObject = Instantiate(prefab, spawnPosition, rotation);

        Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            float force = Random.Range(minForce, maxForce);
            rb.AddForce(spawnedObject.transform.up * force, ForceMode.Impulse);

            Vector3 randomTorque = new Vector3(
                Random.Range(minTorque.x, maxTorque.x),
                Random.Range(minTorque.y, maxTorque.y),
                Random.Range(minTorque.z, maxTorque.z)
            );

            rb.AddTorque(randomTorque, ForceMode.Impulse);
        }

        Destroy(spawnedObject, objectLifetime);
    }
}