using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Yarn.Unity;


// this is a CROSS SCENE OBJECT (meaning, we will put this in many scenes) that exposes a bunch of functions on objects that are in other scenes.
// Because untiy doesnt let you reference objects cross scene, unfortunately
// All of these functions are called VIA SIGNALS
public class CrossSceneObjects : MonoBehaviour
{
    [SerializeField] SpriteRenderer CoomerHead;
    [SerializeField] Animator CoomerNPCAnimator;
    
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

    public void fadeInValue(float time)
    {
        FindObjectOfType<LevelLoader>().FadeIn(time);
    }

    public void fadeOutValue(float time)
    {
        FindObjectOfType<LevelLoader>().FadeOut(time);
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

    [YarnCommand("turnOnLectureHallStudents")]
    public void turnOnLectureHallStudents()
    {
        AudioSource lectureHallTalking = GameObject.Find("LectureHallLedge").GetComponent<AudioSource>();
        lectureHallTalking.time = Random.value * lectureHallTalking.clip.length;
        StartCoroutine(FadeAudioSource.StartFade(lectureHallTalking, 3.0f, 1.0f));
        lectureHallTalking.Play();
    }

    // K I just hardcoded the ledge in the middle to have the audio source of the students.
    [YarnCommand("shutUpLectureHallStudents")]
    public void shutUpLectureHallStudents()
    {
        Debug.Log("Trying to shutup students");
        AudioSource lectureHallTalking = GameObject.Find("LectureHallLedge").GetComponent<AudioSource>();
        lectureHallTalking.time = Random.value * lectureHallTalking.clip.length;
        StartCoroutine(FadeAudioSource.StartFade(lectureHallTalking, 3.0f, 0.0f));
    }

    [YarnCommand("FaceCoomerDown")]
    public void faceCoomerDown()
    {
        CoomerNPCAnimator.SetBool("idleUp", false);
        CoomerNPCAnimator.SetBool("idleDown", true);
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

    public void enablePlates()
    {
        GameObject plates = GameObject.Find("Plates");
        if (plates)
        {
            foreach (SpriteRenderer sr in plates.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.enabled = true;
            }
        }
    }
    public void disablePlates()
    {
        GameObject plates = GameObject.Find("Plates");
        if (plates)
        {
            foreach (SpriteRenderer sr in plates.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.enabled = false;
            }
        }
    }

    public void enableCoomer()
    {
        CoomerHead.enabled = true;
    }

    public void disableCoomer()
    {
        CoomerHead.enabled = false;
    }
}
