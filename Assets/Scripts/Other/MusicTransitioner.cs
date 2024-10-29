using System.Collections.Generic;
using UnityEngine;

public class HeightBasedMusicChanger : MonoBehaviour
{
    [System.Serializable]
    public class HeightMusicPair
    {
        public float heightThreshold;
        public AudioClip musicClip;
    }

    public List<HeightMusicPair> heightMusicPairs; // List of height thresholds and corresponding music clips
    public float fadeDuration = 1.0f; // Time to fade in/out between tracks
    public float maxVolume = 1.0f; // Maximum volume for each track

    private List<AudioSource> audioSources = new List<AudioSource>();
    private int currentMusicIndex = -1;

    void Start()
    {
        // Initialize an AudioSource for each HeightMusicPair
        foreach (var pair in heightMusicPairs)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = pair.musicClip;
            source.loop = true;
            source.volume = 0;
            source.Play();
            audioSources.Add(source);
        }
    }

    void Update()
    {
        int newMusicIndex = -1;

        // Determine which music clip should be the primary one based on height
        for (int i = 0; i < heightMusicPairs.Count; i++)
        {
            if (transform.position.y >= heightMusicPairs[i].heightThreshold)
            {
                newMusicIndex = i;
            }
        }

        // If the desired track is different, start fading between tracks
        if (newMusicIndex != currentMusicIndex)
        {
            currentMusicIndex = newMusicIndex;
            StartCoroutine(FadeTracks(newMusicIndex));
        }
    }

    private System.Collections.IEnumerator FadeTracks(int newMusicIndex)
    {
        float fadeSpeed = 1.0f / fadeDuration;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            // Gradually fade in the new track and fade out others
            for (int i = 0; i < audioSources.Count; i++)
            {
                float targetVolume = (i == newMusicIndex) ? maxVolume : 0;
                audioSources[i].volume = Mathf.Lerp(audioSources[i].volume, targetVolume, t * fadeSpeed);
            }
            yield return null;
        }

        // Ensure volumes are set correctly at the end of the fade
        for (int i = 0; i < audioSources.Count; i++)
        {
            audioSources[i].volume = (i == newMusicIndex) ? maxVolume : 0;
        }
    }
}
