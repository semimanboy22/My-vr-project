using UnityEngine;

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AmbientSound
{
    public AudioClip clip;
    public float startTime = 0f;
    public bool loop = false;
}

public class ambienthighwaysounds : MonoBehaviour
{
    [SerializeField] private List<AmbientSound> soundsList = new List<AmbientSound>();
    [SerializeField] private bool playOnAwake = true;
    [SerializeField] private float volume = 1f;

    private AudioSource audioSource;
    private List<float> soundStartTimes = new List<float>();
    private float cycleStartTime;
    private float totalCycleDuration;
    private bool isPlaying = false;

    void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        audioSource.volume = volume;
    }

    void Start()
    {
        if (playOnAwake && soundsList.Count > 0)
        {
            Play();
        }
    }

    void Update()
    {
        if (isPlaying && audioSource.isPlaying == false && soundsList.Count > 0)
        {
            float elapsedTime = Time.time - cycleStartTime;
            if (elapsedTime >= totalCycleDuration)
            {
                Play();
            }
        }
    }

    public void Play()
    {
        if (soundsList.Count == 0)
        {
            Debug.LogWarning("No sounds in the ambienthighwaysounds list!");
            return;
        }

        cycleStartTime = Time.time;
        CalculateTotalDuration();
        isPlaying = true;

        PlayNextSound(0);
    }

    public void Stop()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
        isPlaying = false;
    }

    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }

    public void AddSound(AudioClip clip, float startTime = 0f, bool loop = false)
    {
        if (clip == null)
        {
            Debug.LogWarning("Cannot add null AudioClip!");
            return;
        }

        soundsList.Add(new AmbientSound { clip = clip, startTime = startTime, loop = loop });
        CalculateTotalDuration();
    }

    public void RemoveSound(int index)
    {
        if (index >= 0 && index < soundsList.Count)
        {
            soundsList.RemoveAt(index);
            CalculateTotalDuration();
        }
    }

    public void ClearSounds()
    {
        soundsList.Clear();
        Stop();
    }

    private void CalculateTotalDuration()
    {
        totalCycleDuration = 0f;

        foreach (AmbientSound sound in soundsList)
        {
            if (sound.clip != null)
            {
                float soundEndTime = sound.startTime + sound.clip.length;
                if (sound.loop)
                {
                    soundEndTime = sound.startTime + (sound.clip.length * 2);
                }

                totalCycleDuration = Mathf.Max(totalCycleDuration, soundEndTime);
            }
        }
    }

    private void PlayNextSound(int index)
    {
        if (index >= soundsList.Count || soundsList.Count == 0)
        {
            isPlaying = false;
            return;
        }

        AmbientSound ambientSound = soundsList[index];
        if (ambientSound.clip == null)
        {
            PlayNextSound(index + 1);
            return;
        }

        audioSource.clip = ambientSound.clip;
        audioSource.loop = ambientSound.loop;
        audioSource.PlayDelayed(ambientSound.startTime);

        if (index + 1 < soundsList.Count)
        {
            Invoke(nameof(PlayNextSoundHelper), ambientSound.startTime + (ambientSound.loop ? ambientSound.clip.length * 2 : ambientSound.clip.length));
        }
    }

    private void PlayNextSoundHelper()
    {
        if (isPlaying)
        {
            int nextIndex = GetNextSoundIndex();
            if (nextIndex != -1)
            {
                PlayNextSound(nextIndex);
            }
        }
    }

    private int GetNextSoundIndex()
    {
        for (int i = 0; i < soundsList.Count; i++)
        {
            if (soundsList[i].clip != null)
            {
                return i;
            }
        }
        return -1;
    }
}
