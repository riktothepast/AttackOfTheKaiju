using System;
using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public AudioSource _soundSource;
    public AudioSource _musicSource;
    string _currentMusicPath = "";
    public bool _playMusicOnLoaded = false;
    private Dictionary<string, AudioClip> _soundClips = new Dictionary<string, AudioClip>();
    private AudioClip _currentMusicClip = null;

    private static AudioManager _instance;
    public static AudioManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<AudioManager>();

                //Tell unity not to destroy this object when loading a new scene!
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            //If I am the first instance, make me the Singleton
            _instance = this;
            DontDestroyOnLoad(this);

            if (GameObject.FindGameObjectsWithTag("AudioManager").Length > 1)
            {
                Destroy(this.gameObject);
                return;
            }

        }
        else
        {
            //If a Singleton already exists and you find
            //another reference in scene, destroy it!
            if (this != _instance)
                Destroy(this.gameObject);
        }
    }


    public void PlayMusic(AudioClip musicClip, float volume, bool shouldRestartIfSameSongIsAlreadyPlaying)
    {

        if (_currentMusicClip != null) //we only want to have one music clip in memory at a time
        {
            if (_currentMusicClip == musicClip) //we're already playing this music, just restart it!
            {
                Debug.Log("same clip");
                if (shouldRestartIfSameSongIsAlreadyPlaying)
                {
                    Debug.Log("restarting music");
                    _musicSource.Stop();
                    _musicSource.volume = volume;
                    _musicSource.loop = true;
                    _musicSource.Play();
                }
                return;
            }
            else //unload the old music
            {
                Debug.Log("must play new music music");
                _musicSource.Stop();
                Resources.UnloadAsset(_currentMusicClip);
                _currentMusicClip = null;
                _currentMusicPath = "";
            }
        }

        _currentMusicClip = musicClip;

        if (_currentMusicClip == null)
        {
            Debug.Log("Error! Couldn't find music clip " + musicClip);
        }
        else
        {
            Debug.Log("playing music");
            _musicSource.clip = _currentMusicClip;
            _musicSource.volume = volume;
            _musicSource.loop = true;
            _musicSource.Play();
        }
    }

    public void PlayMusic(AudioClip musicClip)
    {
        PlayMusic(musicClip, 1, false);
    }

    public void StopMusic()
    {
        if (_musicSource != null)
        {
            _musicSource.Stop();
        }
    }

    public void PlaySound(AudioClip audioClip, float volume) //it is not necessary to preload sounds in order to play them
    {

        AudioClip soundClip;
        if (audioClip == null)
            return;
        if (_soundClips.ContainsKey(audioClip.name))
        {
            soundClip = _soundClips[audioClip.name];
        }
        else
        {
            soundClip = audioClip;

            if (soundClip == null)
            {
                return; //can't play the sound because we can't find it!
            }
            else
            {
                _soundClips[audioClip.name] = soundClip;
            }
        }

        _soundSource.PlayOneShot(soundClip, volume);
    }

    public void PlaySound(AudioClip soundClip)
    {
        PlaySound(soundClip, 1.0f);
    }

    public void PreloadSound(AudioClip resourceName)
    {
        if (_soundClips.ContainsKey(resourceName.name))
        {
            return; //we already have it, no need to preload it again!
        }
        else
        {
            AudioClip soundClip = resourceName;

            if (soundClip == null)
            {
                Debug.Log("Couldn't find sound at: " + resourceName);
            }
            else
            {
                _soundClips[resourceName.name] = soundClip;
            }
        }
    }

}

