using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Bomb")]
    [SerializeField] private int penaltyFruits = 3;
    [SerializeField] private ParticleSystem explosionEffect;

    private bool hasBeenSliced = false;
    private Collider[] bombColliders;
    private Rigidbody rb;

    private void Awake()
    {
        bombColliders = GetComponentsInChildren<Collider>();
        rb = GetComponent<Rigidbody>();
    }

    public void Slice(Vector3 direction, Vector3 position, float force)
    {
        if (hasBeenSliced) return;
        hasBeenSliced = true;

        // Сразу отключаем все коллайдеры, чтобы клинок не задел повторно
        if (bombColliders != null)
        {
            for (int i = 0; i < bombColliders.Length; i++)
            {
                if (bombColliders[i] != null)
                    bombColliders[i].enabled = false;
            }
        }

        // Останавливаем физику
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (explosionEffect != null)
        {
            ParticleSystem effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            effect.Play();

            float lifetime = effect.main.duration + effect.main.startLifetime.constantMax;
            Destroy(effect.gameObject, lifetime);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.BombExploded(penaltyFruits);
        }

        Destroy(gameObject);
    }
}