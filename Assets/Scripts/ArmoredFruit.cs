using UnityEngine;

public class ArmoredFruit : Fruit
{
    [Header("Armor")]
    [SerializeField] private int hitsRequired = 2;
    [SerializeField] private float hitDelay = 0.3f;

    [Header("Crack Effect")]
    [SerializeField] private Material[] crackMaterials;
    [SerializeField] private Renderer fruitRenderer;

    [Header("Sound")]
    [SerializeField] private AudioClip hitSound;
    private AudioSource audioSource;

    private int hitCount = 0;
    private float lastHitTime = -999f;

    protected override void Awake()
    {
        base.Awake();
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public override void Slice(Vector3 direction, Vector3 position, float force)
    {
        if (hasBeenSliced) return;

        if (Time.time - lastHitTime < hitDelay) return;
        lastHitTime = Time.time;

        hitCount++;

        if (hitCount < hitsRequired)
        {
            ShowCrack(hitCount - 1);
            if (hitSound != null) audioSource.PlayOneShot(hitSound);
            return;
        }

        base.Slice(direction, position, force);
    }

    private void ShowCrack(int index)
    {
        if (fruitRenderer == null || crackMaterials == null) return;
        if (index >= crackMaterials.Length) return;

        Material[] mats = fruitRenderer.materials;
        Material[] newMats = new Material[mats.Length + 1];
        for (int i = 0; i < mats.Length; i++) newMats[i] = mats[i];
        newMats[newMats.Length - 1] = crackMaterials[index];
        fruitRenderer.materials = newMats;
    }
}