using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource normalSource;
    public AudioSource muffledSource;

    [Header("Tracks")]
    public AudioClip[] normalTracks;
    public AudioClip[] muffledTracks;

    [Header("UI Sounds")]
    [SerializeField] private AudioSource uiSource;

    [SerializeField] private AudioClip menuHoverSound;
    [SerializeField] private AudioClip menuClickSound;
    [SerializeField] private AudioClip itemHoverSound;
    [SerializeField] private AudioClip itemClickSound;
    [SerializeField] private AudioClip sellActiveItemSound;

    private int currentTrackIndex = 0;
    private bool isMuffled = false;

    private float musicVolume = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);

        if (normalTracks.Length > 0 && muffledTracks.Length > 0)
            PlayTrack(0);
    }

    private void Update()
    {
        if (normalSource != null && !normalSource.isPlaying)
            NextTrack();
    }

    private void PlayTrack(int index)
    {
        currentTrackIndex = index;

        normalSource.clip = normalTracks[index];
        muffledSource.clip = muffledTracks[index];

        normalSource.time = 0f;
        muffledSource.time = 0f;

        normalSource.Play();
        muffledSource.Play();

        UpdateVolumes();
    }

    private void NextTrack()
    {
        int next = (currentTrackIndex + 1) % normalTracks.Length;
        PlayTrack(next);
    }

    private void UpdateVolumes()
    {
        if (normalSource == null || muffledSource == null) return;

        if (isMuffled)
        {
            normalSource.volume = 0f;
            muffledSource.volume = musicVolume;
        }
        else
        {
            normalSource.volume = musicVolume;
            muffledSource.volume = 0f;
        }
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();

        UpdateVolumes();
    }

    public void SetMuffled(bool value)
    {
        isMuffled = value;
        UpdateVolumes();
    }

    private float GetSfxVolume()
    {
        if (SettingsManager.Instance != null)
            return SettingsManager.Instance.SfxVolume;

        return 1f;
    }

    private void PlayUI(AudioClip clip)
    {
        if (uiSource == null) return;
        if (clip == null) return;

        uiSource.PlayOneShot(clip, GetSfxVolume());
    }

    public void PlayMenuHover()
    {
        PlayUI(menuHoverSound);
    }

    public void PlayMenuClick()
    {
        PlayUI(menuClickSound);
    }

    public void PlayItemHover()
    {
        PlayUI(itemHoverSound);
    }

    public void PlayItemClick()
    {
        PlayUI(itemClickSound);
    }

    public void PlaySellActiveItem()
    {
        PlayUI(sellActiveItemSound);
    }
}