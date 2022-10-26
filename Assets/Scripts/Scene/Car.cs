using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    [SerializeField] List<AudioClip> drivingSounds;
    float t;
    Vector3 startPosition;
    Vector3 target;
    [SerializeField] float speed; 
    bool isDriving;
    string nameOfAnimationToPlay;
    float timeToReachTarget;

    // Start is called before the first frame update
    void Start()
    {
        int randomIdx = Random.Range(0, drivingSounds.Count);
        GetComponent<AudioSource>().clip = drivingSounds[randomIdx];

        // start the song at a random point in time to make it more interesting!
        GetComponent<AudioSource>().time = Random.value * drivingSounds[randomIdx].length;
        Debug.Log("Song started at time " + GetComponent<AudioSource>().time + " out of total time " + GetComponent<AudioSource>().clip.length);
    }


    public void Drive(Vector3 start, Vector3 finish, string nameOfAnimationToPlay)
    {
        Debug.Log("Spawned a " + this.name + " at " + start + " triggering animation bool with name of " + nameOfAnimationToPlay + "audio is playing? " + GetComponent<AudioSource>().isPlaying);
        this.nameOfAnimationToPlay = nameOfAnimationToPlay;
        GetComponent<Animator>().SetBool(nameOfAnimationToPlay, true);
        StartCoroutine(delayStuff(start, finish));
    }

    // do a slight delay since if we don't we get taht weird bug where the audio for the car refuses to play
    IEnumerator delayStuff(Vector3 start, Vector3 finish)
    {
        yield return new WaitForSeconds(1.0f);
        isDriving = true;
        // Debug.Log("isDriving set to true");
        t = 0;
        startPosition = start;
        timeToReachTarget = Vector3.Distance(start, finish) / (Settings.defaultCarSpeed * speed);
        target = finish;
        GetComponent<AudioSource>().Play();
    }

    void Update()
    {
        if (isDriving)
        {
            t += Time.deltaTime / timeToReachTarget;
            transform.position = Vector3.Lerp(startPosition, target, t);
        }
    
        if (t >= 1.0)
        {
            // then stop, we are there
            // isDriving = false;
            // GetComponent<Animator>().SetBool(nameOfAnimationToPlay, false);
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<AudioSource>().Pause();

            Debug.Log("Destory object " + this.name + ", t is " + t);
            Destroy(gameObject);
            
        }
    }
}
