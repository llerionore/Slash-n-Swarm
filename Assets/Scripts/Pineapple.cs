using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Pineapple : Fruit
{
    [Header("Pineapple")]
    [SerializeField] private float sliceWindow = 5f;
    [SerializeField] private int coinsPerSlice = 10;
    [SerializeField] private GameObject sliceParticle;
    [SerializeField] private GameObject finalExplosion;

    [Header("Camera")]
    [SerializeField] private float zoomSpeed = 3f;
    [SerializeField] private float zoomAmount = 0.4f;

    [Header("Explosion")]
    [SerializeField] private float explosionRadius = 5f;

    [Header("Slow Motion")]
    [SerializeField] private float slowTimeScale = 0.3f;

    private bool isActive = false;
    private Camera mainCamera;
    private Camera pineappleCamera;
    private Image darkOverlay;
    private Image spotlightCircle;
    private float lastSliceTime = -999f;
    private float hitDelay = 0.2f;

    protected override void Awake()
    {
        base.Awake();
        mainCamera = Camera.main;

        Camera[] allCameras = Resources.FindObjectsOfTypeAll<Camera>();
        foreach (Camera cam in allCameras)
        {
            if (cam.CompareTag("PineappleCamera"))
            {
                pineappleCamera = cam;
                break;
            }
        }

        Image[] allImages = FindObjectsOfType<Image>(true);
        foreach (Image img in allImages)
        {
            if (img.CompareTag("DarkOverlay"))
                darkOverlay = img;
            else if (img.CompareTag("PineappleSpotlight"))
                spotlightCircle = img;
        }
    }

    public override void Slice(Vector3 direction, Vector3 position, float force)
    {
        if (!isActive && !hasBeenSliced)
        {
            isActive = true;
            hasBeenSliced = true;
            StartCoroutine(PineappleRoutine());
            return;
        }

        if (!isActive) return;

        if (Time.time - lastSliceTime < hitDelay) return;
        lastSliceTime = Time.time;

        if (sliceParticle != null)
        {
            GameObject p = Instantiate(sliceParticle, transform.position, Quaternion.identity);
            Destroy(p, 1f);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlaySliceSound();
            GameManager.Instance.AddCoins(coinsPerSlice);
        }

        if (!isActive && !hasBeenSliced)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.PlaySliceSound();

            isActive = true;
            hasBeenSliced = true;
            StartCoroutine(PineappleRoutine());
            return;
        }
    }

    private IEnumerator PineappleRoutine()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.GetSpawner().PauseSpawning();

        Fruit[] allFruits = FindObjectsOfType<Fruit>();
        foreach (Fruit f in allFruits)
        {
            if (f.gameObject != gameObject)
                Destroy(f.gameObject);
        }

        float mainSize = mainCamera.orthographicSize;
        float targetSize = mainSize * zoomAmount;
        Vector3 mainPos = mainCamera.transform.position;

        if (pineappleCamera != null)
        {
            pineappleCamera.gameObject.SetActive(true);
            pineappleCamera.orthographicSize = mainSize;
            pineappleCamera.transform.position = mainPos;
        }

        mainCamera.depth = -10;

        if (darkOverlay != null)
        {
            darkOverlay.gameObject.SetActive(true);
            Color c = darkOverlay.color;
            c.a = 0f;
            darkOverlay.color = c;
        }

        if (spotlightCircle != null)
        {
            spotlightCircle.gameObject.SetActive(true);
            Color c = spotlightCircle.color;
            c.a = 0f;
            spotlightCircle.color = c;
        }

        float t = 0f;
        Vector3 targetPos = new Vector3(
            transform.position.x,
            transform.position.y,
            mainPos.z
        );

        while (t < 1f)
        {
            t += Time.deltaTime * zoomSpeed;

            if (pineappleCamera != null)
            {
                pineappleCamera.orthographicSize = Mathf.Lerp(mainSize, targetSize, t);
                pineappleCamera.transform.position = Vector3.Lerp(mainPos, targetPos, t);
            }

            if (darkOverlay != null)
            {
                Color c = darkOverlay.color;
                c.a = Mathf.Lerp(0f, 0.85f, t);
                darkOverlay.color = c;
            }

            if (spotlightCircle != null)
            {
                Color c = spotlightCircle.color;
                c.a = Mathf.Lerp(0f, 1f, t);
                spotlightCircle.color = c;
            }

            yield return null;
        }

        Time.timeScale = slowTimeScale;
        Time.fixedDeltaTime = 0.02f * slowTimeScale;

        float timer = sliceWindow;
        while (timer > 0f)
        {
            timer -= Time.unscaledDeltaTime;

            if (pineappleCamera != null)
            {
                pineappleCamera.transform.position = new Vector3(
                    transform.position.x,
                    transform.position.y,
                    mainPos.z
                );
            }

            if (spotlightCircle != null && pineappleCamera != null)
            {
                Vector3 screenPos = pineappleCamera.WorldToScreenPoint(transform.position);
                spotlightCircle.transform.position = screenPos;
            }

            yield return null;
        }

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject) continue;
            Fruit nearbyFruit = hit.GetComponent<Fruit>();
            if (nearbyFruit != null)
            {
                Vector3 dir = (hit.transform.position - transform.position).normalized;
                ArmoredFruit armored = nearbyFruit as ArmoredFruit;
                if (armored != null)
                    armored.ForceSlice(dir, transform.position, 5f);
                else
                    nearbyFruit.Slice(dir, transform.position, 5f);
            }
        }

        if (finalExplosion != null)
        {
            GameObject exp = Instantiate(finalExplosion, transform.position, Quaternion.identity);
            Destroy(exp, 2f);
        }

        if (CameraShake.Instance != null)
            CameraShake.Instance.Shake(0.5f, 0.4f);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddFruitRewards(0, XPReward);
            GameManager.Instance.OnFruitSliced();
            GameManager.Instance.ReturnPineappleCamera(pineappleCamera, darkOverlay, spotlightCircle, mainPos, mainSize, zoomSpeed);
        }

        Destroy(gameObject);
    }
}