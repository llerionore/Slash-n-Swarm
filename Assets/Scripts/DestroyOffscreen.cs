using UnityEngine;

public class DestroyOffscreen : MonoBehaviour
{
    [SerializeField] private float extraOffset = 2f;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (mainCamera == null) return;

        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);

        bool outside =
            viewportPos.y < -extraOffset ||
            viewportPos.x < -extraOffset ||
            viewportPos.x > 1f + extraOffset;

        if (outside)
        {
            Destroy(gameObject);
        }
    }
}