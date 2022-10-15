using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Sound : MonoBehaviour
{
    private AudioSource audioSource;
    public SoundItem soundItem;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SetSound(SoundItem soundItem)
    {
        this.soundItem = soundItem;
        audioSource.pitch = Random.Range(soundItem.soundPitchRandomVariationMin, soundItem.soundPitchRandomVariationMax);
        audioSource.volume = soundItem.soundVolume;
        float randomSound = Random.Range(0, soundItem.soundClips.Length);
        int idx = Mathf.FloorToInt(randomSound);
        audioSource.clip = soundItem.soundClips[idx];
    }



    private void OnEnable()
    {
        if (audioSource.clip != null)
        {
            // Debug.Log(gameObject.name + " set active, playing");
            audioSource.Play();
        }
    }

    private void OnDisable()
    {
        audioSource.Stop();
    }
}