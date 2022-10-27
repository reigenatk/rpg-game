using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class StreetLight : MonoBehaviour
{
    TimeManager tm;
    private Light2D spotLight;
    // Start is called before the first frame update
    void Start()
    {
        tm = FindObjectOfType<TimeManager>();
        spotLight = GetComponentInChildren<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // lets do lights turn on at 7pm, turn off at 5am, peaks in intensity at 
        if (tm.gt.gameHour >= 19 || tm.gt.gameHour <= 5)
        {
            spotLight.intensity = 1.0f;
        }
        else
        {
            spotLight.intensity = 0.0f;
        }
    }
}
