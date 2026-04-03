using UnityEngine;

public class Blade : MonoBehaviour
{
    public float sliceForce = 7f;
    public float minSliceVelocity = 0.01f;

    [Header("Slice Detection")]
    public float sliceRadius = 0.6f;

    [Header("Skins")]
    [SerializeField] private Material[] skinMaterials;
    [SerializeField] private ParticleSystem[] skinParticles;
    [SerializeField] private LineTextureMode[] skinTextureModes;
    [SerializeField] private float[] skinWidths;
    private int currentSkin = 0;
    private ParticleSystem currentParticles;

    private Camera mainCamera;
    private Collider sliceCollider;
    private TrailRenderer sliceTrail;
    public Vector3 direction { get; private set; }
    public bool slicing { get; private set; }
    private Plane gamePlane;
    private Vector3 previousPosition;

    private void Awake()
    {
        mainCamera = Camera.main;
        sliceCollider = GetComponent<Collider>();
        sliceTrail = GetComponentInChildren<TrailRenderer>();
        gamePlane = new Plane(Vector3.forward, Vector3.zero);
    }

    private void Start()
    {
        int saved = PlayerPrefs.GetInt("skin", 0);
        SetSkin(saved);
    }

    private void OnEnable()
    {
        StopSlice();
    }

    private void OnDisable()
    {
        StopSlice();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (GameManager.Instance == null || GameManager.Instance.HasStamina())
            {
                StartSlice();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopSlice();
        }
        else if (slicing)
        {
            ContinueSlice();
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (gamePlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }
        return transform.position;
    }

    private void StartSlice()
    {
        Vector3 position = GetMouseWorldPosition();
        transform.position = position;
        previousPosition = position;
        slicing = true;
        sliceCollider.enabled = true;
        sliceTrail.enabled = true;
        sliceTrail.Clear();
        if (currentParticles != null) currentParticles.Play();
    }

    private void StopSlice()
    {
        slicing = false;
        sliceCollider.enabled = false;
        sliceTrail.enabled = false;
        if (currentParticles != null) currentParticles.Stop();
    }

    private void ContinueSlice()
    {
        if (GameManager.Instance != null)
        {
            bool canUse = GameManager.Instance.TryUseStamina(Time.deltaTime);
            if (!canUse)
            {
                StopSlice();
                return;
            }
        }

        Vector3 newPosition = GetMouseWorldPosition();
        direction = newPosition - previousPosition;
        float velocity = direction.magnitude / Time.deltaTime;
        sliceCollider.enabled = velocity > minSliceVelocity;

        if (sliceCollider.enabled)
        {
            CheckSlice(previousPosition, newPosition);
        }

        transform.position = newPosition;
        previousPosition = newPosition;
    }

    private void CheckSlice(Vector3 start, Vector3 end)
    {
        Vector3 capsuleDirection = end - start;
        float distance = capsuleDirection.magnitude;
        if (distance <= 0f) return;
        Vector3 dir = capsuleDirection.normalized;

        Collider[] hits = Physics.OverlapCapsule(start, end, sliceRadius);
        for (int i = 0; i < hits.Length; i++)
        {
            Fruit fruit = hits[i].GetComponent<Fruit>();
            if (fruit != null)
            {
                fruit.Slice(dir, transform.position, sliceForce);
            }
        }
    }

    public void SetSkin(int index)
    {
        if (skinMaterials == null || index < 0 || index >= skinMaterials.Length) return;

        if (currentParticles != null) currentParticles.Stop();

        currentSkin = index;

        Material[] mats = new Material[] { skinMaterials[index] };
        sliceTrail.materials = mats;

        if (skinTextureModes != null && index < skinTextureModes.Length)
            sliceTrail.textureMode = skinTextureModes[index];

        if (skinWidths != null && index < skinWidths.Length)
            sliceTrail.startWidth = skinWidths[index];

        if (skinParticles != null && index < skinParticles.Length)
            currentParticles = skinParticles[index];
        else
            currentParticles = null;

        PlayerPrefs.SetInt("skin", index);
    }
}