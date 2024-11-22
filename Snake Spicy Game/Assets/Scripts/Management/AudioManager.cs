using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip pepperSFX;
    public AudioClip bananaSFX;
    public AudioClip flyingSound;
    public AudioClip gameMusic;
    public static AudioManager instance;
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioSource musicAudioSource;

    void Awake()
    {
        instance = this; 
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        musicAudioSource.clip = gameMusic;
        musicAudioSource.loop = true;
        musicAudioSource.Play();
    }

    public void PlayBananaSFX()
    {
        sfxAudioSource.PlayOneShot(bananaSFX);
    }
    public void PlayPepperSFX()
    {
        sfxAudioSource.PlayOneShot(pepperSFX);
    }
    public void PlayFlyingSound()
    {
        sfxAudioSource.clip = flyingSound;
        sfxAudioSource.loop = true;
        sfxAudioSource.Play();
    }
    public void StopFlyingSound()
    {
        sfxAudioSource.loop = false;
        sfxAudioSource.Stop();
    }
}
