using UnityEngine;

public class HideScroll : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private ScrollAnimationUI scrollAnimationUI;

    public void HideTopScroll()
    {
        if (target != null)
            target.SetActive(false);

        if (scrollAnimationUI != null)
            scrollAnimationUI.PlayIdle();
    }

    public void Hide_Scroll()
    {
        HideTopScroll();
    }
}