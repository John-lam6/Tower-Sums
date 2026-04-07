using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    void Start()
    {
        if(audioSource)
        {
            DontDestroyOnLoad(audioSource);
        }
    }
    public void ChangeVolume(float value)
    {
        audioSource.volume = value;
    }
}
