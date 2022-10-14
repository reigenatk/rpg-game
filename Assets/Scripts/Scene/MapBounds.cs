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
    void DoSwitchBoundingShape()
    {
        GameObject p = GameObject.FindGameObjectWithTag(Tags.BoundsConfiner);
        if (p != null)
        {
            PolygonCollider2D pc2d = p.GetComponent<PolygonCollider2D>();
            CinemachineConfiner c = GetComponent<CinemachineConfiner>();
            c.m_BoundingShape2D = pc2d;

            // clear cache
            c.InvalidatePathCache();
        }
       
    }
}
