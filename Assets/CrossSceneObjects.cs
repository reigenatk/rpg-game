using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossSceneObjects : MonoBehaviour
{
    
    // helper to call the toggleLight on the PropsToggle.cs function (across scenes)
    public void turnOffLamp()
    {
        GameObject.Find("Lamp").GetComponent<PropsToggle>().toggleLight(false);
    }
}
