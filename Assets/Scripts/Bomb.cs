using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Bomb")]
    [SerializeField] private int penaltyFruits = 3;
    [SerializeField] private ParticleSystem explosionEffect;

    [Header("Sound")]
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private float explosionVolume = 1f;

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

        if (bombColliders != null)
        {
            for (int i = 0; i < bombColliders.Length; i++)
            {
                if (bombColliders[i] != null)
                    bombColliders[i].enabled = false;
            }
        }

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (explosionSound != null)
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, explosionVolume);

        if (explosionEffect != null)
        {
            // Спавним только один эффект
            ParticleSystem effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            effect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            effect.Play();
            Destroy(effect.gameObject, effect.main.duration);
        }

        if (GameManager.Instance != null)
            GameManager.Instance.BombExploded(penaltyFruits);

        Destroy(gameObject);
    }
}