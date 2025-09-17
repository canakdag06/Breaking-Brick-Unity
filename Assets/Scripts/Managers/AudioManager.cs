using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioMixer mixer;
    public SoundData soundData;
    public AudioSource musicSource;
    public AudioSource loopSfxSource;

    [SerializeField] private int sfxPoolSize = 10;
    private List<AudioSource> sfxSources;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            sfxSources = new List<AudioSource>();
            for (int i = 0; i < sfxPoolSize; i++)
            {
                var obj = new GameObject("SFXSource_" + i);
                obj.transform.SetParent(transform);
                var source = obj.AddComponent<AudioSource>();
                sfxSources.Add(source);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(SoundType type)
    {
        var entry = soundData.Get(type);
        if (entry == null) return;

        AudioSource src = null;
        for (int i = 0; i < sfxSources.Count; i++)
        {
            if (!sfxSources[i].isPlaying)
            {
                src = sfxSources[i];
                break;
            }
        }
        if (src == null) src = sfxSources[0];

        src.PlayOneShot(entry.GetRandomClip(), entry.volume);
        Debug.Log(src);
    }

    public void PlayMusic(SoundType type)
    {
        var entry = soundData.Get(type);
        if (entry != null)
        {
            musicSource.clip = entry.GetRandomClip();
            musicSource.volume = entry.volume;
            musicSource.Play();
        }
        else
        {
            Debug.Log($"Music not found: {type}");
        }
    }

    public void PlayLoopSound(SoundType type)
    {
        var entry = soundData.Get(type);
        if (entry != null && !loopSfxSource.isPlaying)
        {
            loopSfxSource.clip = entry.GetRandomClip();
            loopSfxSource.volume = entry.volume;
            loopSfxSource.loop = true;
            loopSfxSource.Play();
        }
    }

    public void StopLoopSound()
    {
        if (loopSfxSource.isPlaying)
        {
            loopSfxSource.Stop();
            loopSfxSource.clip = null;
        }
    }

}
