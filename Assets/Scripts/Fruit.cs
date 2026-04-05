using UnityEngine;

public class Fruit : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] protected GameObject slicedPrefab;
    [SerializeField] protected GameObject juiceEffectPrefab;
    [SerializeField] protected float slicedLifetime = 3f;
    [SerializeField] protected float juiceEffectLifetime = 2f;

    [Header("Rewards")]
    [SerializeField] protected int coinReward = 5;

    protected Rigidbody fruitRigidbody;
    protected bool hasBeenSliced;

    protected virtual void Awake()
    {
        fruitRigidbody = GetComponent<Rigidbody>();
    }

    public virtual void Slice(Vector3 direction, Vector3 position, float force)
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
                if (fruitRigidbody != null)
                {
                    slice.velocity = fruitRigidbody.velocity;
                }

                slice.AddForceAtPosition(direction * force, position, ForceMode.Impulse);
            }

            Destroy(slicedObject, slicedLifetime);
        }

        Destroy(gameObject);
    }
}