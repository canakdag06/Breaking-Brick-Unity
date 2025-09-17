using UnityEngine;

[System.Serializable]
public class SoundEntry
{
    public SoundType type;
    public AudioClip[] clips;
    public float volume = 1f;

    public AudioClip GetRandomClip()
    {
        if (clips == null || clips.Length == 0) return null;
        if (clips.Length == 1) return clips[0];

        int index = Random.Range(0, clips.Length);
        return clips[index];
    }
}