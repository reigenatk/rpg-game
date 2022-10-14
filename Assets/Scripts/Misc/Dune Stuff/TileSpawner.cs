using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DuneStuff
{
    public class TileSpawner : MonoBehaviour
    {
        DungeonManager dm;

        // awake called before start
        private void Awake()
        {
            // put down floor
            dm = FindObjectOfType<DungeonManager>();
            GameObject gameObjectFloor = Instantiate(dm.floorPrefab, transform.position, Quaternion.identity) as GameObject;
            gameObjectFloor.name = dm.floorPrefab.name;

            // dungeonmanager sits on top of environment gameobject
            // make the new floor a child of the environment object by setting its
            // parent equal to the environment object.
            gameObjectFloor.transform.SetParent(dm.transform);

            if (transform.position.x < dm.minX)
            {
                dm.minX = transform.position.x;
            }
            if (transform.position.x > dm.maxX)
            {
                dm.maxX = transform.position.x;
            }

            if (transform.position.y < dm.minY)
            {
                dm.minY = transform.position.y;
            }
            if (transform.position.y > dm.maxY)
            {
                dm.maxY = transform.position.y;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            Vector2 hitSize = Vector2.one * 0.8f;
            LayerMask envMask = LayerMask.GetMask("Wall", "Floor");
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector2 targetPos = new Vector2(transform.position.x + x, transform.position.y + y);
                    Collider2D hit = Physics2D.OverlapBox(targetPos, hitSize, 0, envMask);

                    // if no wall or floor yet defined
                    if (!hit)
                    {
                        GameObject gameObjectWall = Instantiate(dm.wallPrefab, targetPos, Quaternion.identity) as GameObject;
                        gameObjectWall.name = dm.wallPrefab.name;
                        gameObjectWall.transform.SetParent(dm.transform);
                    }
                }
            }


            // destory tilespawner object
            Destroy(gameObject);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawCube(transform.position, Vector3.one);
        }
    }
}