using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    public float MusicVolume { get; private set; }
    public float SfxVolume { get; private set; }
    public KeyCode SliceKey { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadSettings()
    {
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        SfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1f);

        string key = PlayerPrefs.GetString("SliceKey", KeyCode.Mouse0.ToString());
        SliceKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), key);

        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMusicVolume(MusicVolume);
    }

    public void SetMusicVolume(float value)
    {
        MusicVolume = value;
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();

        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMusicVolume(value);
    }

    public void SetSfxVolume(float value)
    {
        SfxVolume = value;
        PlayerPrefs.SetFloat("SfxVolume", value);
        PlayerPrefs.Save();
    }

    public void SetSliceKey(KeyCode key)
    {
        SliceKey = key;
        PlayerPrefs.SetString("SliceKey", key.ToString());
        PlayerPrefs.Save();
    }
}