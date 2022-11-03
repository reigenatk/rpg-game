using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TimeManager;

public class Building : MonoBehaviour
{
    [SerializeField] GameTime OpenTime;
    [SerializeField] GameTime CloseTime;

    // ignore if it isn't a bar
    [SerializeField] bool isBar;
    [SerializeField] AudioSource daytimeMusic;
    [SerializeField] AudioSource nightclubMusic;
    Animator a;
    AudioSource asource;

    // Start is called before the first frame update
    void Start()
    {
        a = GetComponent<Animator>();
        if (!isBar)
        {
            // get the one and only audio source
            asource = GetComponent<AudioSource>();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (TimeManager.Instance.gt.compareTimes(OpenTime) == true)
        {
            // current time is BEFORE open time. So do not open it.
            if (isBar)
            {
                if (daytimeMusic.isPlaying)
                {
                    daytimeMusic.Pause();
                }
                if (!nightclubMusic.isPlaying)
                {
                    nightclubMusic.Play();
                }
            }
            else
            {
                if (asource.isPlaying)
                {
                    asource.Pause();
                }
                
            }
            a.SetBool("open", false);
            a.SetBool("closed", true);
        }
        else if (TimeManager.Instance.gt.compareTimes(OpenTime) == false && TimeManager.Instance.gt.compareTimes(CloseTime) == true)
        {
            // then its open
            if (isBar)
            {
                if (!daytimeMusic.isPlaying)
                {
                    daytimeMusic.Play();
                }
                if (nightclubMusic.isPlaying)
                {
                    nightclubMusic.Pause();
                }
            }
            else
            {
                if (!asource.isPlaying)
                {
                    asource.Play();
                }
            }
            a.SetBool("closed", false);
            a.SetBool("open", true);
        }
        else if (TimeManager.Instance.gt.compareTimes(OpenTime) == false && TimeManager.Instance.gt.compareTimes(CloseTime) == false)
        {
            // greater than both open + close times- means its past the close time hence its closed
            if (isBar)
            {
                if (daytimeMusic.isPlaying)
                {
                    daytimeMusic.Pause();
                }
                if (!nightclubMusic.isPlaying)
                {
                    nightclubMusic.Play();
                }
            }
            else
            {
                if (asource.isPlaying)
                {
                    asource.Pause();
                }
            }
            a.SetBool("open", false);
            a.SetBool("closed", true);
        }
    }
}
