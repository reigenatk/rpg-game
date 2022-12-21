using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour
{
    Animator animator;
    AudioSource asrc;
    private void Start()
    {
        animator = GetComponent<Animator>();
        asrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        int gameHour = TimeManager.Instance.gt.gameHour;
        if ((gameHour < 6 && gameHour > 0) || (gameHour > 20 && gameHour < 24))
        {
            animator.SetBool("open", false);
            asrc.enabled = false;
        }
        else
        {
            animator.SetBool("open", true);
            asrc.enabled = true;
        }
    }
}
