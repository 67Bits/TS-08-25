using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class VolumeValueChanger : MonoBehaviour
{
    [Required, SerializeField] private VolumeType _volumeType;
    [Required, SerializeField] private Slider _slider;
    private float _maxVolume = 1;

    public void SetVolume(float value)
    {
        AudioManager.Instance?.SetVolume(_volumeType.ToString(), value * _maxVolume);
        AudioManager.Instance?.SetVolume(_volumeType.ToString(), value * _maxVolume);
        PlayerPrefs.SetFloat(_volumeType.ToString(), value);
    }
    public void LoadVolume()
    {
        var volume = PlayerPrefs.GetFloat(_volumeType.ToString(), 1);
        SetVolume(volume);
        _slider.value = volume;
    }
    private void OnEnable() => LoadVolume();
    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    //public static void LoadAll()
    //{
    //    AudioManager.Instance.SetVolume(VolumeType.MusicVolume.ToString(),PlayerPrefs.GetFloat(VolumeType.MusicVolume.ToString(), 1));
    //    AudioManager.Instance.SetVolume(VolumeType.SfxVolume.ToString(), PlayerPrefs.GetFloat(VolumeType.SfxVolume.ToString(), 1));
    //}
    public enum VolumeType
    {
        MusicVolume,
        SfxVolume
    }
}
