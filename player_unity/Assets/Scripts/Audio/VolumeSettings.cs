using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider volumeSlider;

    private const string MasterVolume = "MasterVolume";

    private void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);

        volumeSlider.value = savedVolume;

        SetVolume(savedVolume);
    }

    public void SetVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);

        float decibels = Mathf.Log10(volume) * 20f;

        audioMixer.SetFloat(MasterVolume, decibels);

        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
    }
}