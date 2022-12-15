using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour
{
    Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        int gameHour = TimeManager.Instance.gt.gameHour;
        if ((gameHour < 6 && gameHour > 0) || (gameHour > 20 && gameHour < 24))
        {
            animator.SetBool("open", false);
        }
        else
        {
            animator.SetBool("open", true);
        }
    }
}
