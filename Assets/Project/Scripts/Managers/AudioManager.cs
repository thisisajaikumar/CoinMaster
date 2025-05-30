using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SFXType
{
    CoinCollect,
    ButtonClick,
}

public enum BGMType
{
    MainMenu,
    Gameplay,
    GameOver
}

[System.Serializable]
public class SFXClip
{
    public SFXType type;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.5f, 2f)] public float pitch = 1f;
}

[System.Serializable]
public class BGMClip
{
    public BGMType type;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 0.7f;
    public bool loop = true;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource uiSfxSource; // Separate source for UI sounds

    [Header("Audio Clips")]
    [SerializeField] private SFXClip[] sfxClips;
    [SerializeField] private BGMClip[] bgmClips;

    [Header("Settings")]
    [SerializeField] private float masterVolume = 1f;
    [SerializeField] private float bgmVolume = 0.7f;
    [SerializeField] private float sfxVolume = 1f;
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    // Dictionaries for fast lookup
    private Dictionary<SFXType, SFXClip> sfxDictionary;
    private Dictionary<BGMType, BGMClip> bgmDictionary;

    // State management
    private bool isMuted = false;
    private bool isPaused = false;
    private BGMType currentBGMType;
    private Coroutine fadeCoroutine;

    // Performance optimization
    private WaitForSeconds shortDelay = new WaitForSeconds(0.01f);

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSystem()
    {
        // Create audio sources if not assigned
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }

        if (uiSfxSource == null)
        {
            uiSfxSource = gameObject.AddComponent<AudioSource>();
            uiSfxSource.playOnAwake = false;
        }

        // Initialize dictionaries
        InitializeDictionaries();

        // Load saved settings
        LoadAudioSettings();
    }

    private void InitializeDictionaries()
    {
        sfxDictionary = new Dictionary<SFXType, SFXClip>();
        bgmDictionary = new Dictionary<BGMType, BGMClip>();

        foreach (var sfx in sfxClips)
        {
            if (!sfxDictionary.ContainsKey(sfx.type))
                sfxDictionary.Add(sfx.type, sfx);
        }

        foreach (var bgm in bgmClips)
        {
            if (!bgmDictionary.ContainsKey(bgm.type))
                bgmDictionary.Add(bgm.type, bgm);
        }
    }


    public void PlaySFX(SFXType type, float volumeMultiplier = 1f)
    {
        if (isMuted) return;

        if (sfxDictionary.TryGetValue(type, out SFXClip sfxClip) && sfxClip.clip != null)
        {
            AudioSource source = (type == SFXType.ButtonClick) ? uiSfxSource : sfxSource;

            source.pitch = sfxClip.pitch;
            source.PlayOneShot(sfxClip.clip, sfxClip.volume * sfxVolume * masterVolume * volumeMultiplier);
        }
        else
        {
            Debug.LogWarning($"SFX clip not found for type: {type}");
        }
    }

    public void PlayBGM(BGMType type, bool forceRestart = false)
    {
        if (currentBGMType == type && bgmSource.isPlaying && !forceRestart) return;

        if (bgmDictionary.TryGetValue(type, out BGMClip bgmClip) && bgmClip.clip != null)
        {
            currentBGMType = type;

            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            fadeCoroutine = StartCoroutine(FadeToBGM(bgmClip));
        }
        else
        {
            Debug.LogWarning($"BGM clip not found for type: {type}");
        }
    }

    public void StopBGM(bool fade = true)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        if (fade)
            fadeCoroutine = StartCoroutine(FadeOutBGM());
        else
            bgmSource.Stop();
    }

    public void PauseBGM()
    {
        if (!isPaused && bgmSource.isPlaying)
        {
            bgmSource.Pause();
            isPaused = true;
        }
    }

    public void ResumeBGM()
    {
        if (isPaused)
        {
            bgmSource.UnPause();
            isPaused = false;
        }
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateAllVolumes();
        SaveAudioSettings();
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        UpdateBGMVolume();
        SaveAudioSettings();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        SaveAudioSettings();
    }

    public void ToggleMute()
    {
        SetMute(!isMuted);
    }

    public void SetMute(bool mute)
    {
        isMuted = mute;
        bgmSource.mute = mute;
        sfxSource.mute = mute;
        uiSfxSource.mute = mute;
        SaveAudioSettings();
    }


    private IEnumerator FadeToBGM(BGMClip newBGM)
    {
        // Fade out current BGM
        if (bgmSource.isPlaying)
        {
            yield return StartCoroutine(FadeOutBGM(false));
        }

        // Set new BGM
        bgmSource.clip = newBGM.clip;
        bgmSource.loop = newBGM.loop;
        bgmSource.volume = 0f;
        bgmSource.Play();

        // Fade in new BGM
        yield return StartCoroutine(FadeInBGM(newBGM));
    }

    private IEnumerator FadeInBGM(BGMClip bgmClip)
    {
        float targetVolume = bgmClip.volume * bgmVolume * masterVolume;
        float currentTime = 0f;

        while (currentTime < fadeInDuration)
        {
            currentTime += Time.deltaTime;
            float normalizedTime = currentTime / fadeInDuration;
            bgmSource.volume = Mathf.Lerp(0f, targetVolume, normalizedTime);
            yield return shortDelay;
        }

        bgmSource.volume = targetVolume;
    }

    private IEnumerator FadeOutBGM(bool stopAfterFade = true)
    {
        float startVolume = bgmSource.volume;
        float currentTime = 0f;

        while (currentTime < fadeOutDuration)
        {
            currentTime += Time.deltaTime;
            float normalizedTime = currentTime / fadeOutDuration;
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, normalizedTime);
            yield return shortDelay;
        }

        bgmSource.volume = 0f;
        if (stopAfterFade)
            bgmSource.Stop();
    }

    private void UpdateAllVolumes()
    {
        UpdateBGMVolume();
        // SFX volume is applied per-clip in PlaySFX
    }

    private void UpdateBGMVolume()
    {
        if (bgmDictionary.TryGetValue(currentBGMType, out BGMClip bgmClip))
        {
            bgmSource.volume = bgmClip.volume * bgmVolume * masterVolume;
        }
    }

    private void SaveAudioSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadAudioSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 0.7f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;

        SetMute(isMuted);
        UpdateAllVolumes();
    }


    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            PauseBGM();
        else
            ResumeBGM();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            PauseBGM();
        else
            ResumeBGM();
    }

    private void OnDestroy()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
    }


    public bool IsMuted => isMuted;
    public bool IsPaused => isPaused;
    public float MasterVolume => masterVolume;
    public float BGMVolume => bgmVolume;
    public float SFXVolume => sfxVolume;
    public BGMType CurrentBGMType => currentBGMType;
}