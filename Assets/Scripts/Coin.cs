using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Reward")]
    [SerializeField] private int goldReward = 15;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 180f;

    [Header("Effect")]
    [SerializeField] private GameObject collectEffect;
    [SerializeField] private float effectLifetime = 1f;

    [Header("Sound")]
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private float collectVolume = 1f;

    private bool collected = false;

    private void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        Blade blade = other.GetComponent<Blade>();
        if (blade == null) return;

        collected = true;

        if (collectSound != null)
            AudioSource.PlayClipAtPoint(collectSound, transform.position, collectVolume);

        if (GameManager.Instance != null)
            GameManager.Instance.AddCoins(goldReward);

        if (collectEffect != null)
        {
            GameObject effect = Instantiate(collectEffect, transform.position, Quaternion.identity);
            Destroy(effect, effectLifetime);
        }

        Destroy(gameObject);
    }
}