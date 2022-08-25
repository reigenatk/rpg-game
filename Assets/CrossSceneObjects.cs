using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CrossSceneObjects : MonoBehaviour
{
    
    // helper to call the toggleLight on the PropsToggle.cs function (across scenes)
    public void turnOffLamp()
    {
        GameObject.Find("Lamp").GetComponent<PropsToggle>().toggleLight(false);
    }

    public void openDoorAnim()
    {
        GameObject.Find("DoorOpen").GetComponent<PlayableDirector>().Play();
    }
    public void closeDoorAnim()
    {
        GameObject.Find("DoorClose").GetComponent<PlayableDirector>().Play();
    }
}
