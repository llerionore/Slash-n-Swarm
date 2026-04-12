using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Frost")]
public class FrostEffect : MonoBehaviour
{
    public float FrostAmount = 0f;
    public float EdgeSharpness = 1;
    public float minFrost = 0;
    public float maxFrost = 1;
    public float seethroughness = 0.2f;
    public float distortion = 0.1f;
    public Texture2D Frost;
    public Texture2D FrostNormals;
    public Shader Shader;

    private Material material;

    private void Awake()
    {
        material = new Material(Shader);
        material.SetTexture("_BlendTex", Frost);
        material.SetTexture("_BumpMap", FrostNormals);
    }

    private void OnEnable()
    {
        if (material == null && Shader != null)
        {
            material = new Material(Shader);
            material.SetTexture("_BlendTex", Frost);
            material.SetTexture("_BumpMap", FrostNormals);
        }
        FrostAmount = 0f;
    }

    private void OnDisable()
    {
        FrostAmount = 0f;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (material == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        if (!Application.isPlaying)
        {
            material.SetTexture("_BlendTex", Frost);
            material.SetTexture("_BumpMap", FrostNormals);
            EdgeSharpness = Mathf.Max(1, EdgeSharpness);
        }

        material.SetFloat("_BlendAmount", Mathf.Clamp01(Mathf.Clamp01(FrostAmount) * (maxFrost - minFrost) + minFrost));
        material.SetFloat("_EdgeSharpness", EdgeSharpness);
        material.SetFloat("_SeeThroughness", seethroughness);
        material.SetFloat("_Distortion", distortion);
        Graphics.Blit(source, destination, material);
    }
}