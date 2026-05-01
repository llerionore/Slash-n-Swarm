using UnityEngine;

public class ExplosiveFruit : Fruit
{
    [Header("Explosion")]
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private float explosionVolume = 1f;

    public override void Slice(Vector3 direction, Vector3 position, float force)
    {
        if (hasBeenSliced) return;
        hasBeenSliced = true;

        if (explosionSound != null)
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, explosionVolume);

        if (explosionEffect != null)
        {
            GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        if (CameraShake.Instance != null)
            CameraShake.Instance.Shake(0.3f, 0.2f);

        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            Fruit nearbyFruit = hit.GetComponent<Fruit>();
            if (nearbyFruit != null)
            {
                Vector3 dir = (hit.transform.position - transform.position).normalized;

                ArmoredFruit armoredFruit = nearbyFruit as ArmoredFruit;
                if (armoredFruit != null)
                    armoredFruit.ForceSlice(dir, transform.position, force);
                else
                    nearbyFruit.Slice(dir, transform.position, force);
            }
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlaySliceSound();
            GameManager.Instance.AddFruitRewards(0, XPReward);
            GameManager.Instance.AddFruitCoins(coinReward);
            GameManager.Instance.OnFruitSliced();
        }

        if (juiceEffectPrefab != null)
        {
            GameObject effect = Instantiate(juiceEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, juiceEffectLifetime);
        }

        Destroy(gameObject);
    }
}