using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
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

    private AudioSource audioSource;
    private int currentMusicIndex = -1;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.volume = 0;
        audioSource.Play();
    }

    void Update()
    {
        int newMusicIndex = -1;

        // Determine which music clip should be playing based on height
        for (int i = 0; i < heightMusicPairs.Count; i++)
        {
            if (transform.position.y >= heightMusicPairs[i].heightThreshold)
            {
                newMusicIndex = i;
            }
        }

        // If we need to switch to a new track, start the fade coroutine
        if (newMusicIndex != currentMusicIndex)
        {
            currentMusicIndex = newMusicIndex;

            if (currentMusicIndex != -1)
            {
                StartCoroutine(FadeToNewTrack(heightMusicPairs[currentMusicIndex].musicClip));
            }
            else
            {
                StartCoroutine(FadeOutCurrentTrack());
            }
        }
    }

    private System.Collections.IEnumerator FadeToNewTrack(AudioClip newClip)
    {
        float startVolume = audioSource.volume;

        // Fade out the current clip
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }
        audioSource.volume = 0;
        audioSource.clip = newClip;
        audioSource.Play();

        // Fade in the new clip
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }
        audioSource.volume = 1;
    }

    private System.Collections.IEnumerator FadeOutCurrentTrack()
    {
        float startVolume = audioSource.volume;

        // Fade out the current track
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        audioSource.Stop();
    }
}
