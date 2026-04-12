using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Bomb")]
    [SerializeField] private int penaltyFruits = 3;
    [SerializeField] private ParticleSystem explosionEffect;

    private bool hasBeenSliced = false;

    public void Slice(Vector3 direction, Vector3 position, float force)
    {
        if (hasBeenSliced) return;
        hasBeenSliced = true;

        if (explosionEffect != null)
        {
            ParticleSystem effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.BombExploded(penaltyFruits);
        }

        Destroy(gameObject);
    }
}