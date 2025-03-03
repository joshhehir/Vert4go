using UnityEngine;

public class BackgroundAudioController : MonoBehaviour
{
    public AudioSource audioSource;

    // Define the height ranges and corresponding audio clips
    [System.Serializable]
    public struct HeightAudioClip
    {
        public float minHeight;
        public float maxHeight;
        public AudioClip audioClip;
    }

    public HeightAudioClip[] heightAudioClips;

    void Update()
    {
        // Assuming worldHeight is a variable that represents the current height in your world
        float worldHeight = transform.position.y;

        // Find the appropriate audio clip based on the current worldHeight
        AudioClip selectedClip = FindSelectedClip(worldHeight);

        // Check if the current clip is different from the selected one
        if (audioSource.clip != selectedClip)
        {
            // Fade out current clip and fade in the selected clip
            audioSource.volume = Mathf.Lerp(audioSource.volume, 0f, Time.deltaTime);
            if (audioSource.volume <= 0.01f)
            {
                audioSource.clip = selectedClip;
                audioSource.Play();
            }
        }
        else
        {
            // Fade in/out volume smoothly
            audioSource.volume = Mathf.Lerp(audioSource.volume, 1f, Time.deltaTime);
        }
        Debug.Log("World Height: " + worldHeight);
    }

    // Find the appropriate audio clip based on the current worldHeight
    AudioClip FindSelectedClip(float worldHeight)
    {
        foreach (HeightAudioClip heightAudioClip in heightAudioClips)
        {
            if (worldHeight >= heightAudioClip.minHeight && worldHeight <= heightAudioClip.maxHeight)
            {
                return heightAudioClip.audioClip;
            }
        }
        return null;
    }
}