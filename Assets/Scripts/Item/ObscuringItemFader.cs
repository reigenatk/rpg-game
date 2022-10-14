using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]

public class ObscuringItemFader : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FadeOutRoutine()
    {
        float currentAlpha = spriteRenderer.color.a;
        float distance = currentAlpha - Settings.targetAlpha; // positive distance, will fade
        while (currentAlpha - Settings.targetAlpha > 0.01f)
        {
            currentAlpha = currentAlpha - (distance / Settings.fadeOutSeconds) * Time.deltaTime;
            spriteRenderer.color = new Color(1f, 1f, 1f, currentAlpha);
            yield return null; // next frame will carry on and evaluate the while again
        }
        if (GetComponent<Animator>() != null)
        {
            GetComponent<Animator>().enabled = false;
        }

        // at the end just set it equal to target alpha
        spriteRenderer.color = new Color(1f, 1f, 1f, Settings.targetAlpha);

    }

    public void FadeIn()
    {
        StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        float currentAlpha = spriteRenderer.color.a;
        float distance = 1.0f - currentAlpha; // positive distance, will fade
        while (1.0f - currentAlpha > 0.01f)
        {
            currentAlpha = currentAlpha + (distance / Settings.fadeOutSeconds) * Time.deltaTime;
            spriteRenderer.color = new Color(1f, 1f, 1f, currentAlpha);
            yield return null; // next frame will carry on and evaluate the while again
        }

        if (GetComponent<Animator>() != null)
        {
            GetComponent<Animator>().enabled = true;
        }


        // at the end just set it equal to target alpha
        spriteRenderer.color = new Color(1f, 1f, 1f, 1f);

    }
}
