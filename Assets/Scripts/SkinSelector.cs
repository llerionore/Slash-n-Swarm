using UnityEngine;
using UnityEngine.UI;

public class SkinSelector : MonoBehaviour
{
    [SerializeField] private Blade blade;
    [SerializeField] private Button[] skinButtons;
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color normalColor = Color.white;

    private void Start()
    {
        for (int i = 0; i < skinButtons.Length; i++)
        {
            int index = i;
            skinButtons[i].onClick.AddListener(() => SelectSkin(index));
        }

        // Подсветить текущий скин
        HighlightButton(PlayerPrefs.GetInt("skin", 0));
    }

    private void SelectSkin(int index)
    {
        blade.SetSkin(index);
        HighlightButton(index);
    }

    private void HighlightButton(int index)
    {
        for (int i = 0; i < skinButtons.Length; i++)
        {
            ColorBlock cb = skinButtons[i].colors;
            cb.normalColor = i == index ? selectedColor : normalColor;
            skinButtons[i].colors = cb;
        }
    }
}