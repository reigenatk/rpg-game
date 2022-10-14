using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


// this is a CROSS SCENE OBJECT (meaning, we will put this in many scenes) that exposes a bunch of functions on objects that are in other scenes.
// Because untiy doesnt let you reference objects cross scene, unfortunately
// All of these functions are called VIA SIGNALS
public class CrossSceneObjects : MonoBehaviour
{
    
    // helper to call the toggleLight on the PropsToggle.cs function (across scenes)
    public void turnOffLamp()
    {
        GameObject.Find("Lamp").GetComponent<PropsToggle>().toggleLight(false);
    }

    public void openDoorAnim()
    {
        GameObject.Find("Door").GetComponent<Animator>().SetBool("Opening", true);
        GameObject.Find("Door").GetComponent<Animator>().SetBool("Closing", false);
    }
    public void closeDoorAnim()
    {
        GameObject.Find("Door").GetComponent<Animator>().SetBool("Opening", false);
        GameObject.Find("Door").GetComponent<Animator>().SetBool("Closing", true);
    }


    public void turnOffPlayerSprite()
    {
        GameObject.Find("Player").GetComponent<SpriteRenderer>().enabled = false;
    }
    public void turnOnPlayerSprite()
    {
        GameObject.Find("Player").GetComponent<SpriteRenderer>().enabled = true;
    }

    public void cutsceneFinishedPlaying()
    {
        FindObjectOfType<GameState>().cutsceneFinishedPlaying();
    }

    public void fadeInCross()
    {
        FindObjectOfType<LevelLoader>().FadeIn();
    }

    public void openLectureHallDoors()
    {
        // this should trigger the opening animation
        GameObject.Find("LectureHallDoors").GetComponent<Animator>().SetBool("Opening", true);
        GameObject.Find("LectureHallDoors").GetComponent<Animator>().SetBool("Closing", false);
    }
    public void closeLectureHallDoors()
    {
        // this should trigger the closing animation
        GameObject.Find("LectureHallDoors").GetComponent<Animator>().SetBool("Closing", true);
        GameObject.Find("LectureHallDoors").GetComponent<Animator>().SetBool("Opening", false);
    }


    public void facePlayerDirection(string direction)
    {
        switch (direction)
        {
            case "Down":
                FindObjectOfType<Player>().setToIdleAnimation();
                break;
        }
    }
}
