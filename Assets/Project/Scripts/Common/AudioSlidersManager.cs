using UnityEngine;
using UnityEngine.UI;

public class AudioSlidersManager : MonoBehaviour
{
    [SerializeField]
    private AudioService _audioService;

    [SerializeField]
    private Slider _masterSlider;

    [SerializeField]
    private Slider _musicSlider;

    [SerializeField]
    private Slider _soundSlider;

    private void Start()
    {
        InitializeSliders();
    }

    private void OnEnable()
    {
        if (_masterSlider != null)
        {
            _masterSlider.onValueChanged.AddListener(SetMasterVolume);
        }

        if (_musicSlider != null)
        {
            _musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (_soundSlider != null)
        {
            _soundSlider.onValueChanged.AddListener(SetSoundVolume);
        }
    }

    private void OnDisable()
    {
        if (_masterSlider != null)
        {
            _masterSlider.onValueChanged.RemoveListener(SetMasterVolume);
        }

        if (_musicSlider != null)
        {
            _musicSlider.onValueChanged.RemoveListener(SetMusicVolume);
        }

        if (_soundSlider != null)
        {
            _soundSlider.onValueChanged.RemoveListener(SetSoundVolume);
        }
    }

    private void InitializeSliders()
    {
        if (_audioService == null)
        {
            return;
        }

        if (_masterSlider != null)
        {
            _masterSlider.SetValueWithoutNotify(_audioService.MasterVolume);
        }

        if (_musicSlider != null)
        {
            _musicSlider.SetValueWithoutNotify(_audioService.MusicVolume);
        }

        if (_soundSlider != null)
        {
            _soundSlider.SetValueWithoutNotify(_audioService.SoundVolume);
        }
    }

    private void SetMasterVolume(float value)
    {
        if (_audioService != null)
        {
            _audioService.SetMasterVolume(value);
        }
    }

    private void SetMusicVolume(float value)
    {
        if (_audioService != null)
        {
            _audioService.SetMusicVolume(value);
        }
    }

    private void SetSoundVolume(float value)
    {
        if (_audioService != null)
        {
            _audioService.SetSoundVolume(value);
        }
    }
}
