using UnityEngine;

public class SetActiveTarget : MonoBehaviour
{
    [SerializeField] private GameObject target;

    public void Show()
    {
        SetActive(true);
    }

    public void Hide()
    {
        SetActive(false);
    }

    public void Toggle()
    {
        if (target != null)
            target.SetActive(!target.activeSelf);
    }

    // Old method name kept for animation events/buttons that already call Hide_Scroll.
    public void Hide_Scroll()
    {
        Hide();
    }

    private void SetActive(bool value)
    {
        if (target != null)
            target.SetActive(value);
    }
}
