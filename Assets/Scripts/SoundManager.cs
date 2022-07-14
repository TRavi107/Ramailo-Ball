using RamailoGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum SoundType
{
    none,
    mainMenuSound,
    backgroundSound,
    uiSound,
    pauseSound,
    popSound,
    warningSound,
    powerUps,
}


public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public List<AudioClip> mainMenuSound;
    public List<AudioClip> backGroundSound;
    public List<AudioClip> popSound;
    public List<AudioClip> warningSound;
    public List<AudioClip> uiSounds;
    public List<AudioClip> PowerUpSounds;
    public AudioClip pauseResumeSound;

    [HideInInspector]public float backGroundAudioVolume;
    [HideInInspector]public float soundeffectVolume; 
    public float OriginalbackGroundAudioVolume;
    public float OriginalsoundeffectVolume;

    private AudioSource backGroundAudioSource;
    private AudioSource WarningAudioSource;

    private bool warningSoundPlaying;
    private void Awake()
    {
        if (instance)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }
    void Start()
    {
        PlaySound(SoundType.backgroundSound);

        MusicVolumeChanged(OriginalbackGroundAudioVolume);
        SoundVolumeChanged(OriginalsoundeffectVolume);
        ScoreAPI.GameStart((bool s) => {
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (backGroundAudioSource != null)
        {
            if(backGroundAudioSource.isPlaying == false)
            {
                switch (SceneManager.GetActiveScene().buildIndex)
                {
                    
                    case 1:
                        PlaySound(SoundType.mainMenuSound);
                        break;
                    case 2:
                        PlaySound(SoundType.backgroundSound);
                        break;
                    
                }
                MusicVolumeChanged(backGroundAudioVolume);
                SoundVolumeChanged(soundeffectVolume);
            }
        }
    }

    public void PlaySound(SoundType soundType)
    {
        GameObject audioSource=new();
        AudioSource source= audioSource.AddComponent<AudioSource>();
        
        AudioClip clip=null;
        AudioClip bgClip;
        float volume=soundeffectVolume;
        int soundIndex;
        switch (soundType)
        {
            case SoundType.mainMenuSound:
                soundIndex = Random.Range(0, mainMenuSound.Count);
                bgClip = mainMenuSound[soundIndex];
                if (backGroundAudioSource == null)
                {
                    backGroundAudioSource = gameObject.AddComponent<AudioSource>();
                }
                backGroundAudioSource.clip = bgClip;
                backGroundAudioSource.loop = false;
                backGroundAudioSource.volume = backGroundAudioVolume;
                backGroundAudioSource.Play();
                break;
            case SoundType.backgroundSound:
                soundIndex = Random.Range(0, backGroundSound.Count);
                bgClip = backGroundSound[soundIndex];
                if (backGroundAudioSource == null)
                {
                    backGroundAudioSource = gameObject.AddComponent<AudioSource>();
                }
                backGroundAudioSource.clip = bgClip;
                backGroundAudioSource.loop = false;
                backGroundAudioSource.volume = backGroundAudioVolume;
                backGroundAudioSource.Play();
                break;

            case SoundType.warningSound:
                if (warningSoundPlaying)
                    break;
                soundIndex = Random.Range(0, warningSound.Count);
                bgClip = warningSound[soundIndex];
                if (WarningAudioSource == null)
                {
                    WarningAudioSource = gameObject.AddComponent<AudioSource>();
                }
                WarningAudioSource.clip = bgClip;
                WarningAudioSource.loop = false;
                WarningAudioSource.volume = soundeffectVolume;
                WarningAudioSource.Play();
                warningSoundPlaying = true;
                break;

            case SoundType.uiSound:
                soundIndex = Random.Range(0, uiSounds.Count);
                clip = uiSounds[soundIndex];
                
                break;
            case SoundType.pauseSound:
                clip = pauseResumeSound;
                break;

            case SoundType.popSound:
                soundIndex = Random.Range(0, popSound.Count);
                clip = popSound[soundIndex];

                break;
            case SoundType.powerUps:
                soundIndex = Random.Range(0, PowerUpSounds.Count);
                clip = PowerUpSounds[soundIndex];
                volume = soundeffectVolume / 3;
                break;
            default:
                break;
        }
        if (clip != null)
        {
            source.clip = clip;
            source.loop = false;
            source.volume = volume;
            source.Play();
            Destroy(audioSource, clip.length + 0.1f);
            return;
        }
        Destroy(audioSource);
    }
    public void StopSound(SoundType type)
    {
        if (type == SoundType.warningSound)
        {
            if (WarningAudioSource != null)
            {
                WarningAudioSource.Stop();
                warningSoundPlaying = false;
            }
        }
    }
    public void MusicVolumeChanged(float volume)
    {
        if(backGroundAudioSource != null)
        {
            backGroundAudioSource.volume = volume;
            SaveMusicVoulme(volume);
        }
    }
    public void SoundVolumeChanged(float volume)
    {
        SaveSoundVoulme(volume);
    }

    public void SaveMusicVoulme(float volume)
    {
        backGroundAudioVolume = volume;
    }
    public void SaveSoundVoulme(float volume)
    {
        soundeffectVolume = volume;
    }
}
