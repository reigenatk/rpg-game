using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zoomControl : MonoBehaviour
{

    public Camera mainCamera; //Uses the camera. Made public for testing purposes//


    void Start()
    {
        mainCamera = GetComponent<Camera>();  //Grabs the camera size for zooming
                                              //
    }


    void Update()
    {
        //Camera Zoom Function
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (mainCamera.orthographicSize > 3)
            {
                mainCamera.orthographicSize -= 1;
            }

        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (mainCamera.orthographicSize < 13)
            {
                mainCamera.orthographicSize += 1;
            }
        }
    }
}
