using UnityEngine;

public class Walnut : Fruit
{
    [Header("Walnut")]
    [SerializeField] private GameObject shellBrokenPrefab;
    [SerializeField] private float secondHitDelay = 0.5f;

    private int hitCount = 0;
    private float lastHitTime = -999f;

    public override void Slice(Vector3 direction, Vector3 position, float force)
    {
        if (hasBeenSliced) return;

        if (Time.time - lastHitTime < secondHitDelay) return;
        lastHitTime = Time.time;

        hitCount++;

        if (hitCount == 1)
        {
            if (shellBrokenPrefab != null)
            {
                GameObject broken = Instantiate(shellBrokenPrefab, transform.position, transform.rotation);

                // Корневой rigidbody — передаём скорость
                Rigidbody rootRb = broken.GetComponent<Rigidbody>();
                if (rootRb != null && fruitRigidbody != null)
                    rootRb.velocity = fruitRigidbody.velocity;

                // Разлетаем дочерние куски скорлупы
                Rigidbody[] pieces = broken.GetComponentsInChildren<Rigidbody>();
                Vector3[] directions = {
                    new Vector3(-1f, 1f, 0f).normalized,
                    new Vector3(1f, 1f, 0f).normalized,
                    new Vector3(0f, 0.5f, 0f).normalized  // косточка — чуть вверх
                };

                for (int i = 0; i < pieces.Length; i++)
                {
                    if (fruitRigidbody != null)
                        pieces[i].velocity = fruitRigidbody.velocity;

                    Vector3 flyDir = i < directions.Length ? directions[i] : Vector3.up;
                    pieces[i].AddForce(flyDir * force * 1.5f, ForceMode.Impulse);
                }
            }
            Destroy(gameObject);
        }
        else
        {
            base.Slice(direction, position, force);
        }
    }
}