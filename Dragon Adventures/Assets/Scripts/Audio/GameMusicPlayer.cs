using System;
using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("Dragon Adventure/Audio/Game Music Player")]
[RequireComponent(typeof(AudioSource))]
public class GameMusicPlayer : MonoBehaviour
{
    private const string MusicResourcesPath = "Music";
    private const string DefaultStartupMusicResourcePath = "Music/Soundtrack derg game";

    public static GameMusicPlayer Instance { get; private set; }

    [Header("Startup Music")]
    public AudioClip startupMusic;
    [Tooltip("If Startup Music is empty, play the first AudioClip found in Assets/Resources/Music.")]
    public bool loadFirstResourcesClip = true;
    public bool playOnStart = true;

    [Header("Playback")]
    [Range(0f, 1f)] public float volume = 0.6f;
    public bool loop = true;
    public bool persistBetweenScenes = true;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (persistBetweenScenes)
            DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = loop;
        audioSource.volume = volume;
        audioSource.spatialBlend = 0f;
    }

    private void Start()
    {
        if (playOnStart)
            PlayStartupMusic();
    }

    private void OnValidate()
    {
        AudioSource source = GetComponent<AudioSource>();
        if (source == null) return;

        source.playOnAwake = false;
        source.loop = loop;
        source.volume = volume;
        source.spatialBlend = 0f;
    }

    public void PlayStartupMusic()
    {
        AudioClip clip = startupMusic != null
            ? startupMusic
            : loadFirstResourcesClip ? LoadDefaultMusicClip() : null;

        if (clip == null)
        {
            Debug.LogWarning("No startup music found. Add an AudioClip to Assets/Resources/Music or assign Startup Music on GameMusicPlayer.");
            return;
        }

        Play(clip);
    }

    public void Play(AudioClip clip, bool restartIfAlreadyPlaying = false)
    {
        if (clip == null) return;

        if (audioSource.clip == clip && audioSource.isPlaying && !restartIfAlreadyPlaying)
            return;

        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.volume = volume;
        audioSource.spatialBlend = 0f;
        audioSource.Play();

        Debug.Log($"Playing music: {clip.name}");
    }

    public void Stop()
    {
        audioSource.Stop();
    }

    private static AudioClip LoadDefaultMusicClip()
    {
        AudioClip defaultClip = Resources.Load<AudioClip>(DefaultStartupMusicResourcePath);
        if (defaultClip != null)
            return defaultClip;

        AudioClip[] clips = Resources.LoadAll<AudioClip>(MusicResourcesPath);
        if (clips == null || clips.Length == 0)
            return null;

        Array.Sort(clips, (left, right) => string.CompareOrdinal(left.name, right.name));
        return clips[0];
    }
}