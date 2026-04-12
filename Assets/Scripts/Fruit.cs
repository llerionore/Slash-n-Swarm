using UnityEngine;
using System.Collections;

public class Fruit : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] protected GameObject slicedPrefab;
    [SerializeField] protected GameObject juiceEffectPrefab;
    [SerializeField] protected float slicedLifetime = 3f;
    [SerializeField] protected float juiceEffectLifetime = 2f;

    [Header("Rewards")]
    [SerializeField] protected int coinReward = 5;

    [Header("Freeze")]
    [SerializeField] private bool canBeFrozen = false;
    [SerializeField] private float freezeChance = 0.2f;
    [SerializeField] private Color frozenColor = new Color(0.5f, 0.8f, 1f, 1f);

    protected Rigidbody fruitRigidbody;
    protected bool hasBeenSliced;
    private bool isFrozen = false;

    protected virtual void Awake()
    {
        fruitRigidbody = GetComponent<Rigidbody>();

        if (canBeFrozen && Random.value < freezeChance)
        {
            isFrozen = true;
            ApplyFrozenColor();
        }
    }

    private void ApplyFrozenColor()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            foreach (Material mat in r.materials)
            {
                mat.color = frozenColor;
            }
        }
    }

    public virtual void Slice(Vector3 direction, Vector3 position, float force)
    {
        if (hasBeenSliced) return;
        hasBeenSliced = true;

        if (isFrozen && GameManager.Instance != null)
        {
            GameManager.Instance.StartFreezeEffect();
        }

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

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnFruitSliced();
        }

        if (slicedPrefab != null)
        {
            GameObject slicedObject = Instantiate(slicedPrefab, transform.position, transform.rotation);
            Rigidbody[] slices = slicedObject.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody slice in slices)
            {
                if (fruitRigidbody != null) slice.velocity = fruitRigidbody.velocity;
                slice.AddForceAtPosition(direction * force, position, ForceMode.Impulse);
            }
            Destroy(slicedObject, slicedLifetime);
        }

        Destroy(gameObject);
    }
}