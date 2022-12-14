using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MapBounds : MonoBehaviour
{
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += DoSwitchBoundingShape;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= DoSwitchBoundingShape;
    }

    /// <summary>
    /// move collider to cinemachine confiner to make camera stop at boundaries
    /// </summary>
    public void DoSwitchBoundingShape()
    {

        GameObject p = GameObject.FindGameObjectWithTag(Tags.BoundsConfiner);
        Debug.Log("Bounds confiner is " + p.name);
        if (p != null)
        {
            PolygonCollider2D pc2d = p.GetComponent<PolygonCollider2D>();
            Debug.Log("PolygonCollider2D is " + pc2d);
            CinemachineConfiner c = GetComponent<CinemachineConfiner>();
            c.m_BoundingShape2D = pc2d;

            // clear cache
            c.InvalidatePathCache();
        }
       
    }
}
