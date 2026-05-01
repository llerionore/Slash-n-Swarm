using UnityEngine;
using UnityEngine.EventSystems;

public enum UIButtonSoundType
{
    Menu,
    GameUI
}

public class UIButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private UIButtonSoundType soundType = UIButtonSoundType.Menu;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (AudioManager.Instance == null) return;

        if (soundType == UIButtonSoundType.Menu)
            AudioManager.Instance.PlayMenuHover();
        else
            AudioManager.Instance.PlayItemHover();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (AudioManager.Instance == null) return;

        if (soundType == UIButtonSoundType.Menu)
            AudioManager.Instance.PlayMenuClick();
        else
            AudioManager.Instance.PlayItemClick();
    }
}