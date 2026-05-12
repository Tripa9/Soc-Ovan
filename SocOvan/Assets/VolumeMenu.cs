using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI; 

public class VolumeMenu : MonoBehaviour
{
    [Header("Configuración")]
    public AudioMixer mainMixer;
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        float savedMusic = PlayerPrefs.GetFloat("MusicVol", 1f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVol", 1f);

        
        musicSlider.value = savedMusic;
        sfxSlider.value = savedSFX;

    
        SetMusicVolume(savedMusic);
        SetSFXVolume(savedSFX);
    }

 
    public void SetMusicVolume(float sliderValue)
    {

        mainMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20f);
        PlayerPrefs.SetFloat("MusicVol", sliderValue); 
    }

    public void SetSFXVolume(float sliderValue)
    {
        mainMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20f);
        PlayerPrefs.SetFloat("SFXVol", sliderValue);
    }
}