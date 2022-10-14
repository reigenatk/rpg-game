using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A script to "nudge" objects when player walks past them
// basically just waits for a trigger enter by the player, and then when that happens
// it will run a coroutine that rotates the sprite a bit
public class ItemNudge : MonoBehaviour
{
    private WaitForSeconds pause;
    private bool isAnimating = false;
    private void Awake()
    {
        pause = new WaitForSeconds(0.04f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isAnimating == false)
        {
            if (gameObject.transform.position.x < collision.gameObject.transform.position.x)
            {
                StartCoroutine(RotateAntiClock());
            }
            else
            {
                StartCoroutine(RotateClock());
            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isAnimating == false)
        {
            if (gameObject.transform.position.x > collision.gameObject.transform.position.x)
            {
                StartCoroutine(RotateAntiClock());
            }
            else
            {
                StartCoroutine(RotateClock());
            }

        }
    }

    private IEnumerator RotateAntiClock()
    {
        isAnimating = true;

        for (int i = 0; i < 4; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, 2f);

            yield return pause;
        }

        for (int i = 0; i < 5; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, -2f);

            yield return pause;
        }

        gameObject.transform.GetChild(0).Rotate(0f, 0f, 2f);

        yield return pause;

        isAnimating = false;
    }

    private IEnumerator RotateClock()
    {
        isAnimating = true;

        for (int i = 0; i < 4; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, -2f);

            yield return pause;
        }

        for (int i = 0; i < 5; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, 2f);

            yield return pause;
        }

        gameObject.transform.GetChild(0).Rotate(0f, 0f, -2f);

        yield return pause;

        isAnimating = false;
    }
}
