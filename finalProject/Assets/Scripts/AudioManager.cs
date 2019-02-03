using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

    public Sound[] sounds;
    [Range(0f, 1f)]
    public float curVol;

    public static AudioManager instance;

    void Awake ()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        for (int i = 0; i < sounds.Length; i++)
        {
            sounds[i].source = gameObject.AddComponent<AudioSource>();

            sounds[i].source.clip = sounds[i].clip;
            sounds[i].source.volume = sounds[i].volume;
            sounds[i].source.pitch = sounds[i].pitch;
            sounds[i].source.loop = sounds[i].loop;
        }
	}

    private Sound FindSound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found.");
        }
        return s;
    }

    public void Play(string name)
    {
        Sound s = FindSound(name);
        if (s == null) return;
        s.source.Play();
        s.source.time = 1.5f;
    }

    public void Stop(string name)
    {
        Sound s = FindSound(name);
        if (s == null) return;
        s.source.Stop();
    }

    public void StopAll()
    {
        foreach(Sound s in sounds)
        {
            s.source.Stop();
        }
    }

    public void SetVolume(float vol)
    {
        curVol = vol;
        for (int i = 0; i < sounds.Length; i++)
        {
            sounds[i].source.volume = sounds[i].volume * curVol;
        }
    }

    // takes float between 0 and 1
    public void SetVolPercent(float volPercent)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            sounds[i].source.volume *= volPercent;
        }
    }
}
