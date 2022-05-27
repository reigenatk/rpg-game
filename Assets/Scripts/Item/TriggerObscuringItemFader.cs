using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerObscuringItemFader : MonoBehaviour
{
    // when player collides with something that has the fading object script,
    // trigger the fade
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // look for a obscuringitemfader script (actually can be more than 1)
        ObscuringItemFader[] obscuringItems = collision.gameObject.GetComponentsInChildren<ObscuringItemFader>();

        if (obscuringItems.Length > 0)
        {
            for (int i = 0; i < obscuringItems.Length; i++)
            {
                obscuringItems[i].FadeOut();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // look for a obscuringitemfader script (actually can be more than 1)
        ObscuringItemFader[] obscuringItems = collision.gameObject.GetComponentsInChildren<ObscuringItemFader>();

        if (obscuringItems.Length > 0)
        {
            for (int i = 0; i < obscuringItems.Length; i++)
            {
                obscuringItems[i].FadeIn();
            }
        }
    }
}
