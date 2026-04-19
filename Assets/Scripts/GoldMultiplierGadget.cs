using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GoldMultiplierGadget : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float multiplier = 2f;
    [SerializeField] private float duration = 10f;
    [SerializeField] private float cooldown = 30f;

    [Header("UI")]
    [SerializeField] private Button button;
    [SerializeField] private Image cooldownFill;

    private bool isActive = false;
    private bool onCooldown = false;

    private void Start()
    {
        if (button != null)
            button.onClick.AddListener(Activate);
    }

    private void Activate()
    {
        if (isActive || onCooldown) return;
        StartCoroutine(GoldMultiplierRoutine());
    }

    private IEnumerator GoldMultiplierRoutine()
    {
        isActive = true;
        onCooldown = true;

        GameManager.Instance.SetGoldMultiplier(multiplier);

        float t = duration;
        while (t > 0f)
        {
            t -= Time.deltaTime;
            yield return null;
        }

        GameManager.Instance.SetGoldMultiplier(1f);
        isActive = false;

        float c = cooldown;
        while (c > 0f)
        {
            c -= Time.deltaTime;
            if (cooldownFill != null)
                cooldownFill.fillAmount = c / cooldown;
            yield return null;
        }

        if (cooldownFill != null)
            cooldownFill.fillAmount = 0f;

        onCooldown = false;
    }
}