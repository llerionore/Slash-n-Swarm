using UnityEngine;

public class HideScroll : MonoBehaviour
{
    [SerializeField] private GameObject target;

    public void Hide_Scroll()
    {
        target.SetActive(false);
    }
}