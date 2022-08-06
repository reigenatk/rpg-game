using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(GetCoordinatesOfMouse))]
class SceneViewMouseEditor : Editor
{

    void OnSceneGUI()
    {
        GetCoordinatesOfMouse obj = (GetCoordinatesOfMouse) target;

        Vector3 mousepos = Event.current.mousePosition;

        mousepos = SceneView.lastActiveSceneView.camera.ScreenToWorldPoint(mousepos);
        mousepos.y = -mousepos.y;


        Debug.Log("mousepos: " + mousepos);
        if (Event.current.type == EventType.MouseDown)
        {


            Debug.Log("click");

        }
    }
}