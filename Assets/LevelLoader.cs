using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime;
    public void FadeScreenOut()
    {
        StartCoroutine(FadeScreen());
    }

    IEnumerator FadeScreen()
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        transition.SetTrigger("End");
    }
}
