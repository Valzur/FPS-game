using UnityEngine.Audio;
using System;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]

public class AudioManager : MonoBehaviour
{
    #region Initializations
    public Sound[] sounds;
    private AudioSource audioSource;
    #endregion

    // Start is called before the first frame update
    void Start(){
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    public void PlayOneShot(string Name){
        Sound s = Array.Find(sounds,sound => sound.name == Name);
        audioSource.PlayOneShot(s.clip);
    }
    public void PlayOneShot(AudioClip clip){
        audioSource.PlayOneShot(clip);
    }
    public void Play(string Name){
        Sound s = Array.Find(sounds,sound => sound.name == Name);
        audioSource.clip = s.clip;
        audioSource.pitch = s.pitch;
        audioSource.volume = s.volume;
        audioSource.spatialBlend = s.spatialBlend;
        audioSource.loop = s.loop;
        audioSource.Play();
    }
    public void Stop(string Name){
        audioSource.Stop();
    }
}