using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    // NOTE: Use Audio Mixer in Unity Editor to set default volume values
    [Serializable]
    public class VolumeData
    {
        public float master_volume;
        public float ui_volume;
        public float sfx_volume;
        public float background_volume;
        public float music_volume;
    }

    [SerializeField]
    private AudioMixer mixer;
    [SerializeField]
    private Slider master_volume_slider;
    [SerializeField]
    private Slider ui_volume_slider;
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
        this.mixer.SetFloat("UIVolume",         this.volume_data.ui_volume);
        this.mixer.SetFloat("SFXVolume",        this.volume_data.sfx_volume);
        this.mixer.SetFloat("BackgroundVolume", this.volume_data.background_volume);
        this.mixer.SetFloat("MusicVolume",      this.volume_data.music_volume);
    }

    /**
     * @brief Updates the volume data from the mixer's current values 
     */
    private void UpdateVolumeDataFromMixer()
    {
        this.mixer.GetFloat("MasterVolume",     out this.volume_data.master_volume);
        this.mixer.GetFloat("UIVolume",         out this.volume_data.ui_volume);
        this.mixer.GetFloat("SFXVolume",        out this.volume_data.sfx_volume);
        this.mixer.GetFloat("BackgroundVolume", out this.volume_data.background_volume);
        this.mixer.GetFloat("MusicVolume",      out this.volume_data.music_volume);
    }

    /**
     * @brief Updates the volumes of the mixer with the current values given
     * by the sliders. Also, store new volume levels
     */
    public void UpdateVolume()
    {
        this.UpdateMasterVolume();
        this.UpdateUIVolume();
        this.UpdateSFXVolume();
        this.UpdateBackgroundVolume();
        this.UpdateMusicVolume();
    }

    public void UpdateMasterVolume()
    {
        this.volume_data.master_volume = this.master_volume_slider.value;
        this.UpdateMixer();
    }
    public void UpdateUIVolume()
    {
        this.volume_data.ui_volume = this.ui_volume_slider.value;
        this.UpdateMixer();
    }
    public void UpdateSFXVolume()
    {
        this.volume_data.sfx_volume = this.sfx_volume_slider.value;
        this.UpdateMixer();
    }
    public void UpdateBackgroundVolume()
    {
        this.volume_data.background_volume = this.background_volume_slider.value;
        this.UpdateMixer();
    }
    public void UpdateMusicVolume()
    {
        this.volume_data.music_volume = this.music_volume_slider.value;
        this.UpdateMixer();
    }
    
    /**
     * @brief Updates the sliders to reflect the current data 
     */
    private void UpdateSliders()
    {
        this.master_volume_slider.value     = this.volume_data.master_volume;
        this.ui_volume_slider.value         = this.volume_data.ui_volume;
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
        if (this.volume_data == null)
        {
            this.volume_data = new VolumeData();
            this.UpdateVolumeDataFromMixer();
        }
        else
        {
            UpdateMixer();
        }

        UpdateSliders();
    }

    public void Save()
    {
        this.UpdateVolume();
        SaveSystem.SaveVolumeData(this.volume_data);
    }
}
