using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GodStaminaGadget : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float duration = 5f;
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
        StartCoroutine(GodStaminaRoutine());
    }

    private IEnumerator GodStaminaRoutine()
    {
        isActive = true;
        onCooldown = true;

        // Активируем бесконечную стамину
        GameManager.Instance.SetInfiniteStamina(true);

        // Таймер активности
        float t = duration;
        while (t > 0f)
        {
            t -= Time.deltaTime;
            yield return null;
        }

        // Выключаем
        GameManager.Instance.SetInfiniteStamina(false);
        isActive = false;

        // Откат
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