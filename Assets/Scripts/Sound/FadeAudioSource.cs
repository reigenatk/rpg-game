using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class FadeAudioSource
{
    // https://johnleonardfrench.com/how-to-fade-audio-in-unity-i-tested-every-method-this-ones-the-best/#:~:text=You%20can%20fade%20audio%20in,script%20will%20do%20the%20rest.
    public static IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume)
    {
        // Debug.Log("Playing song " + audioSource.gameObject.name + " at pitch " + pitch);
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            
            yield return null;
        }
        yield break;
    }
}