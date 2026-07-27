using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class AudioManager : MonoBehaviour
{
    [Header("Sound References")]
    public AudioClip[] tracks;
    public AudioClip gameOverClip;
    public AudioClip buttonPointSound;
    public AudioClip buttonClickSound;

    [Header("References")]
    public Slider musicVolumeSlider;
    public TextMeshProUGUI musicVolumeText;
    public AudioMixer mixer;
    public Slider masterVolumeSlider;
    public TextMeshProUGUI masterVolumeText;
    public Slider sfxVolumeSlider;
    public TextMeshProUGUI sfxVolumeText;

    [Header("Settings")]
    [Range(0f, 100f)] public float startVolume = 20f;

    private AudioSource mainSource;
    private AudioSource sfxSource;
    private int currentTrackIndex = -1;

    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        mainSource = GetComponent<AudioSource>();
        mainSource.playOnAwake = false;
        mainSource.loop = false;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.outputAudioMixerGroup = GameManager.Instance.sfxGroup;
    }

    private void Start()
    {
        SliderInitializer();

        if (tracks.Length > 0) PlayNext();
    }

    private void Update()
    {
        if (GameManager.Instance.gameOver) return;

        if (!mainSource.isPlaying && mainSource.clip != null)
        {
            mainSource.clip = null;
            PlayNext();
        }

        if (Keyboard.current.numpad1Key.wasPressedThisFrame) PlayNext();
    }

    public void PlayNext()
    {
        if (tracks.Length == 0) return;

        currentTrackIndex = (currentTrackIndex + 1) % tracks.Length;
        mainSource.clip = tracks[currentTrackIndex];
        mainSource.Play();
    }

    private void SliderInitializer()
    {
        Slider[] sliders = { masterVolumeSlider, musicVolumeSlider, sfxVolumeSlider };
        string[] mixerParams = { "MasterVolume", "MusicVolume", "SFXVolume" };
        TextMeshProUGUI[] texts = { masterVolumeText, musicVolumeText, sfxVolumeText };

        for (int i = 0; i < sliders.Length; i++)
        {
            int index = i;

            var slider = sliders[i];
            slider.minValue = 0;
            slider.maxValue = 100;
            slider.value = startVolume;

            slider.onValueChanged.AddListener(
                (value) => OnAllVolumeChanged(value, mixerParams[index], texts[index])
                );
            OnAllVolumeChanged(startVolume, mixerParams[index], texts[index]);
        }
    }

    private void OnAllVolumeChanged(float value, string mixerText, TextMeshProUGUI volumeText)
    {
        float normalized = value / 100f;
        if (normalized <= 0.0001f) normalized = 0.0001f;
        float dbValue = Mathf.Log10(normalized) * 20f;
        mixer.SetFloat(mixerText, dbValue);

        volumeText.text = $"{Mathf.RoundToInt(value)}%";
    }

    public void PlayGameOverSound()
    {
        /*mainSource.clip = gameOverClip;
        mainSource.Play();*/
        mainSource.Stop();
        sfxSource.PlayOneShot(gameOverClip);
    }

    public void OnButtonPoint() => sfxSource.PlayOneShot(buttonPointSound);
    public void OnButtonClick() => sfxSource.PlayOneShot(buttonClickSound);
}
