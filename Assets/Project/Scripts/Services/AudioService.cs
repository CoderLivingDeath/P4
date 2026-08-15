using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[DefaultExecutionOrder(-100)]
public class AudioService : MonoBehaviour
{
    private const string MasterVolumeKey = "MasterVolume";
    private const string MusicVolumeKey = "MusicVolume";
    private const string SoundVolumeKey = "SoundVolume";

    [SerializeField]
    private AudioMixerGroup _masterMixerGroup;

    [SerializeField]
    private AudioMixerGroup _musicMixerGroup;

    [SerializeField]
    private AudioMixerGroup _soundMixerGroup;

    [SerializeField]
    private AudioSource _musicSource;

    [SerializeField]
    private Transform _soundPoolRoot;

    [SerializeField]
    private int _soundPoolSize = 16;

    private readonly List<AudioSource> _soundPool = new List<AudioSource>();
    private readonly Queue<AudioSource> _availableSounds = new Queue<AudioSource>();
    private float _masterVolume = 1f;
    private float _musicVolume = 1f;
    private float _soundVolume = 1f;

    public float MasterVolume => _masterVolume;
    public float MusicVolume => _musicVolume;
    public float SoundVolume => _soundVolume;

    private void Awake()
    {
        CreateSoundPool();
    }

    private void Start()
    {
        LoadVolumesFromPrefs();
    }

    private void LoadVolumesFromPrefs()
    {
        _masterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
        _musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        _soundVolume = PlayerPrefs.GetFloat(SoundVolumeKey, 1f);

        ApplyVolumes();
    }

    private void CreateSoundPool()
    {
        if (_soundPoolRoot == null)
        {
            GameObject root = new GameObject("SoundPoolRoot");
            root.transform.SetParent(transform);
            _soundPoolRoot = root.transform;
        }

        for (int i = 0; i < _soundPoolSize; i++)
        {
            GameObject go = new GameObject($"SoundSource_{i}");
            go.transform.SetParent(_soundPoolRoot);

            AudioSource source = go.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = _soundMixerGroup;
            source.playOnAwake = false;
            _soundPool.Add(source);
            _availableSounds.Enqueue(source);
        }
    }

    private AudioSource GetAvailableSource()
    {
        if (_availableSounds.Count == 0)
        {
            return _soundPool[0];
        }

        return _availableSounds.Dequeue();
    }

    private void OnSoundFinished(AudioSource source)
    {
        if (_availableSounds.Contains(source))
        {
            return;
        }

        _availableSounds.Enqueue(source);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        _musicSource.clip = clip;
        _musicSource.loop = true;
        _musicSource.Play();
    }

    public void StopMusic()
    {
        _musicSource.Stop();
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        AudioSource source = GetAvailableSource();
        source.PlayOneShot(clip);
        OnSoundFinished(source);
    }

    public void PlaySoundAtPoint(AudioClip clip, Vector3 position)
    {
        if (clip == null)
        {
            return;
        }

        AudioSource.PlayClipAtPoint(clip, position);
    }

    public void SetMasterVolume(float volume)
    {
        _masterVolume = Mathf.Clamp01(volume);
        ApplyVolumes();
    }

    public void SetMusicVolume(float volume)
    {
        _musicVolume = Mathf.Clamp01(volume);
        ApplyVolumes();
    }

    public void SetSoundVolume(float volume)
    {
        _soundVolume = Mathf.Clamp01(volume);
        ApplyVolumes();
    }

    private void ApplyVolumes()
    {
        if (_masterMixerGroup != null)
        {
            _masterMixerGroup.audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Max(_masterVolume, 0.0001f)) * 20f);
        }

        if (_musicMixerGroup != null)
        {
            _musicMixerGroup.audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Max(_musicVolume, 0.0001f)) * 20f);
        }

        if (_soundMixerGroup != null)
        {
            _soundMixerGroup.audioMixer.SetFloat("SoundVolume", Mathf.Log10(Mathf.Max(_soundVolume, 0.0001f)) * 20f);
        }

        PlayerPrefs.SetFloat(MasterVolumeKey, _masterVolume);
        PlayerPrefs.SetFloat(MusicVolumeKey, _musicVolume);
        PlayerPrefs.SetFloat(SoundVolumeKey, _soundVolume);
        PlayerPrefs.Save();
    }
}
