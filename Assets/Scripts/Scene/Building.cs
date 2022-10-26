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

    // Start is called before the first frame update
    void Start()
    {
        a = GetComponent<Animator>(); 
    }

    // Update is called once per frame
    void Update()
    {
        if (TimeManager.Instance.gt.compareTimes(OpenTime) == true)
        {
            // current time is BEFORE open time. So do not open it.
            if (isBar)
            {
                daytimeMusic.enabled = false;
                nightclubMusic.enabled = true;
            }

            a.SetBool("closed", true);
        }
        else if (TimeManager.Instance.gt.compareTimes(OpenTime) == false && TimeManager.Instance.gt.compareTimes(CloseTime) == true)
        {
            // then its open
            if (isBar)
            {
                daytimeMusic.enabled = true;
                nightclubMusic.enabled = false;
            }
            a.SetBool("closed", false);
            a.SetBool("open", true);
        }
        else if (TimeManager.Instance.gt.compareTimes(OpenTime) == false && TimeManager.Instance.gt.compareTimes(CloseTime) == false)
        {
            // greater than both open + close times- means its past the close time hence its closed
            if (isBar)
            {
                daytimeMusic.enabled = false;
                nightclubMusic.enabled = true;
            }
            a.SetBool("closed", true);
        }
    }
}
