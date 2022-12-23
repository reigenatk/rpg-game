using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static TimeManager;

public class Discoball : MonoBehaviour
{
    List<Color> colorsToLerpBetween;
    [SerializeField] GameObject lightsObject;

    [SerializeField] float rotationSpeed = 150.0f;
    Coroutine colorRoutine;
    public bool isRunning = false;

    public void startDiscoball()
    {
        gameObject.transform.position = new Vector3(-25.2f, 42f, 0);
        foreach (Light2D light in lightsObject.GetComponentsInChildren<Light2D>())
        {
            light.intensity = 1.0f;
        }

        isRunning = true;
        colorRoutine = StartCoroutine(colorStuff());
        GetComponent<Animator>().SetBool("Active", true);
    }

    private void Update()
    {
        if (isRunning)
        {

            lightsObject.transform.Rotate(new Vector3(0, 0, Time.deltaTime * rotationSpeed));

        }
    }

    IEnumerator colorStuff()
    {
        while (true)
        {
            foreach (Light2D light in lightsObject.GetComponentsInChildren<Light2D>())
            {
                light.color = new Color(Random.Range(0, 1.0f), Random.Range(0, 1.0f), Random.Range(0, 1.0f));
                light.intensity = Random.Range(0.8f, 2.2f);
            }


            rotationSpeed = Random.Range(120.0f, 250.0f);
            yield return new WaitForSeconds(2.0f);
        }
    }

    public void stopDiscoball()
    {

        gameObject.transform.position = new Vector3(-25.03f, 44.6f, 0);
        isRunning = false;
        StopCoroutine(colorRoutine);

        // turn off all lights
        foreach (Light2D light in lightsObject.GetComponentsInChildren<Light2D>())
        {
            light.intensity = 0.0f;
        }
        GetComponent<Animator>().SetBool("Active", false);
    }
}
