using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [Serializable]
    public class VolumeData
    {
        public float master_volume;
        public float sfx_volume;
        public float background_volume;
        public float music_volume;
    }

    [SerializeField]
    private AudioMixer mixer;
    [SerializeField]
    private Slider master_volume_slider;
    [SerializeField]
    private Slider sfx_volume_slider;
    [SerializeField]
    private Slider background_volume_slider;
    [SerializeField]
    private Slider music_volume_slider;

    private VolumeData volume_data;
    
    /**
     * @brief Updates the volumes of the mixer with the current values 
     */
    private void UpdateMixer()
    {
        this.mixer.SetFloat("MasterVolume",     this.volume_data.master_volume);
        this.mixer.SetFloat("SFXVolume",        this.volume_data.sfx_volume);
        this.mixer.SetFloat("BackgroundVolume", this.volume_data.background_volume);
        this.mixer.SetFloat("MusicVolume",      this.volume_data.music_volume);
    }

    /**
     * @brief Updates the volumes of the mixer with the current values given
     * by the sliders. Also, store new volume levels
     * @todo Can separate this into discrete functions
     */
    public void UpdateVolume()
    {
        this.volume_data.master_volume     = this.master_volume_slider.value;
        this.volume_data.sfx_volume        = this.sfx_volume_slider.value;
        this.volume_data.background_volume = this.background_volume_slider.value;
        this.volume_data.music_volume      = this.music_volume_slider.value;
        this.UpdateMixer();
    }
    
    /**
     * @brief Updates the sliders to reflect the current data 
     */
    private void UpdateSliders()
    {
        this.master_volume_slider.value     = this.volume_data.master_volume;
        this.sfx_volume_slider.value        = this.volume_data.sfx_volume;
        this.background_volume_slider.value = this.volume_data.background_volume;
        this.music_volume_slider.value      = this.volume_data.music_volume;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.Load();
    }

    void OnDestroy()
    {
        this.Save();    
    }

    public void Load()
    {
        this.volume_data = SaveSystem.LoadVolumeData();
        UpdateMixer();
        UpdateSliders();
    }

    public void Save()
    {
        SaveSystem.SaveVolumeData(this.volume_data);
    }
}
