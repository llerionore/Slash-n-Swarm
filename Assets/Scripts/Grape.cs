using UnityEngine;

public class Grape : Fruit
{
    [Header("Grape Drop")]
    [SerializeField] private GameObject grapePiecePrefab;
    [SerializeField] private int minGrapes = 3;
    [SerializeField] private int maxGrapes = 6;
    [SerializeField] private float spawnRadius = 0.35f;
    [SerializeField] private float minPieceForce = 1.5f;
    [SerializeField] private float maxPieceForce = 3.5f;
    [SerializeField] private float upwardBonus = 1.5f;
    [SerializeField] private float grapeLifetime = 4f;

    public override void Slice(Vector3 direction, Vector3 position, float force)
    {
        if (hasBeenSliced) return;
        hasBeenSliced = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlaySliceSound();
            GameManager.Instance.AddFruitXP();
            GameManager.Instance.AddCoins(coinReward);
        }

        if (juiceEffectPrefab != null)
        {
            Quaternion effectRotation = direction.sqrMagnitude > 0.001f
                ? Quaternion.FromToRotation(Vector3.up, direction.normalized)
                : Quaternion.identity;

            GameObject effect = Instantiate(juiceEffectPrefab, transform.position, effectRotation);
            Destroy(effect, juiceEffectLifetime);
        }

        if (slicedPrefab != null)
        {
            GameObject slicedObject = Instantiate(slicedPrefab, transform.position, transform.rotation);

            Rigidbody[] slices = slicedObject.GetComponentsInChildren<Rigidbody>();

            foreach (Rigidbody slice in slices)
            {
                if (fruitRigidbody != null)
                {
                    slice.velocity = fruitRigidbody.velocity;
                }

                slice.AddForceAtPosition(direction * force, position, ForceMode.Impulse);
            }

            Destroy(slicedObject, slicedLifetime);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnFruitSliced();
        }

        SpawnGrapes(direction);

        Destroy(gameObject);
    }

    private void SpawnGrapes(Vector3 sliceDirection)
    {
        if (grapePiecePrefab == null) return;

        int count = Random.Range(minGrapes, maxGrapes + 1);

        for (int i = 0; i < count; i++)
        {
            Vector3 offset = Random.insideUnitSphere * spawnRadius;
            offset.z = 0f;

            Vector3 spawnPosition = transform.position + offset;

            GameObject grape = Instantiate(
                grapePiecePrefab,
                spawnPosition,
                Random.rotation
            );

            Rigidbody rb = grape.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 randomDirection = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(0.2f, 1f),
                    0f
                ).normalized;

                Vector3 finalDirection = randomDirection;

                if (sliceDirection.sqrMagnitude > 0.001f)
                {
                    finalDirection += sliceDirection.normalized * 0.6f;
                    finalDirection.Normalize();
                }

                float pieceForce = Random.Range(minPieceForce, maxPieceForce);

                if (fruitRigidbody != null)
                {
                    rb.velocity = fruitRigidbody.velocity * 0.6f;
                }

                rb.AddForce(finalDirection * pieceForce + Vector3.up * upwardBonus, ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * 8f, ForceMode.Impulse);
            }

            Destroy(grape, grapeLifetime);
        }
    }
}