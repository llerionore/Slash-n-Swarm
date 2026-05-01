using UnityEngine;

public class ArmoredFruit : Fruit
{
    [Header("Armor")]
    [SerializeField] private int armorHP = 2;
    [SerializeField] private float hitDelay = 0.3f;

    [Header("Crack Effect")]
    [SerializeField] private Material[] crackMaterials;
    [SerializeField] private Renderer fruitRenderer;

    [Header("Sound")]
    [SerializeField] private AudioClip hitSound;

    private AudioSource audioSource;
    private int currentArmorHP;
    private float lastHitTime = -999f;

    protected override void Awake()
    {
        base.Awake();

        currentArmorHP = armorHP;
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public override void Slice(Vector3 direction, Vector3 position, float force)
    {
        if (hasBeenSliced) return;
        if (Time.time - lastHitTime < hitDelay) return;

        lastHitTime = Time.time;

        int playerStrength = 1;

        if (PlayerStats.Instance != null)
            playerStrength = Mathf.RoundToInt(PlayerStats.Instance.Strength);

        playerStrength = Mathf.Max(1, playerStrength);

        currentArmorHP -= GetArmorDamage();

        if (currentArmorHP > 0)
        {
            ShowCrack();

            if (hitSound != null && audioSource != null)
                audioSource.PlayOneShot(hitSound);

            return;
        }

        base.Slice(direction, position, force);
    }

    private int GetArmorDamage()
    {
        int strength = 1;

        if (PlayerStats.Instance != null)
            strength = Mathf.RoundToInt(PlayerStats.Instance.Strength);

        if (strength >= 13) return 5;
        if (strength >= 9) return 4;
        if (strength >= 6) return 3;
        if (strength >= 3) return 2;

        return 1;
    }

    public void ForceSlice(Vector3 direction, Vector3 position, float force)
    {
        currentArmorHP = 0;
        base.Slice(direction, position, force);
    }

    private void ShowCrack()
    {
        if (fruitRenderer == null || crackMaterials == null || crackMaterials.Length == 0)
            return;

        float armorPercent = (float)currentArmorHP / armorHP;

        int index = Mathf.FloorToInt((1f - armorPercent) * crackMaterials.Length);
        index = Mathf.Clamp(index, 0, crackMaterials.Length - 1);

        Material[] mats = fruitRenderer.materials;

        Material[] newMats = new Material[mats.Length + 1];

        for (int i = 0; i < mats.Length; i++)
            newMats[i] = mats[i];

        newMats[newMats.Length - 1] = crackMaterials[index];

        fruitRenderer.materials = newMats;
    }
}