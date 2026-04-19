using UnityEngine;
using UnityEngine.UI;

public class TestArmorReduction : MonoBehaviour
{
    [SerializeField] private Button button;

    private void Start()
    {
        if (button != null)
            button.onClick.AddListener(Upgrade);
    }

    private void Upgrade()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.UpgradeArmorReduction();

        Debug.Log("Armor Reduction Level: " + GameManager.Instance.GetArmorReduction());
    }
}