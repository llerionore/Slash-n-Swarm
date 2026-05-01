using UnityEngine;

public class ScrollAnimationUI : MonoBehaviour
{
    [Header("Animators")]
    [SerializeField] private Animator bottomScrollAnimator;
    [SerializeField] private Animator topScrollAnimator;

    [Header("Top Scroll Object")]
    [SerializeField] private GameObject topScrollObject;

    [Header("Animation Names")]
    [SerializeField] private string bottomOpeningAnimation = "scrollOpening";
    [SerializeField] private string topOpeningAnimation = "scrollSides";
    [SerializeField] private string idleAnimation = "scrollIdle";

    public void PlayOpenAnimation()
    {
        if (topScrollObject != null)
            topScrollObject.SetActive(true);

        if (bottomScrollAnimator != null)
        {
            bottomScrollAnimator.Play(bottomOpeningAnimation, 0, 0f);
        }

        if (topScrollAnimator != null)
        {
            topScrollAnimator.Play(topOpeningAnimation, 0, 0f);
        }
    }

    public void PlayIdle()
    {
        if (bottomScrollAnimator != null)
        {
            bottomScrollAnimator.Play(idleAnimation, 0, 0f);
        }
    }

    public void HideTopScroll()
    {
        if (topScrollObject != null)
            topScrollObject.SetActive(false);

        PlayIdle();
    }
}