using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelUI : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button bindButton;
    [SerializeField] private TextMeshProUGUI bindButtonText;

    private bool waitingForKey;

    private void Start()
    {
        if (SettingsManager.Instance == null) return;

        if (musicSlider != null)
        {
            musicSlider.value = SettingsManager.Instance.MusicVolume;
            musicSlider.onValueChanged.AddListener(SettingsManager.Instance.SetMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = SettingsManager.Instance.SfxVolume;
            sfxSlider.onValueChanged.AddListener(SettingsManager.Instance.SetSfxVolume);
        }

        if (bindButton != null)
            bindButton.onClick.AddListener(StartBind);

        UpdateBindText();
    }

    private void Update()
    {
        if (!waitingForKey) return;

        if (Input.anyKeyDown)
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    SettingsManager.Instance.SetSliceKey(key);
                    waitingForKey = false;
                    UpdateBindText();
                    break;
                }
            }
        }
    }

    private void StartBind()
    {
        waitingForKey = true;
        bindButtonText.text = "Press key...";
    }

    private void UpdateBindText()
    {
        if (bindButtonText == null) return;
        if (SettingsManager.Instance == null) return;

        bindButtonText.text = SettingsManager.Instance.SliceKey.ToString();
    }
}