using UnityEngine;

public class GlowPulse : MonoBehaviour
{
    [SerializeField] private Renderer glowRenderer;
    [SerializeField] private Color glowColor = Color.red;
    [SerializeField] private float minIntensity = 1f;
    [SerializeField] private float maxIntensity = 3f;
    [SerializeField] private float speed = 2f;

    private Material mat;

    private void Awake()
    {
        if (glowRenderer != null)
            mat = glowRenderer.material;
    }

    private void Update()
    {
        if (mat == null) return;
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, (Mathf.Sin(Time.time * speed) + 1f) / 2f);
        mat.SetColor("_EmissionColor", glowColor * intensity);
    }
}