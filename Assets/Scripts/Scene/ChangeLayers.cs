using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChangeLayers : MonoBehaviour
{
    [SerializeField] private float yRangeLow;
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        if (Player.Instance.transform.position.y <= yRangeLow)
        {
            // below player
            if (GetComponent<SpriteRenderer>())
            {
                GetComponent<SpriteRenderer>().sortingOrder = 0;
            }
            if (GetComponent<TilemapRenderer>())
            {
                GetComponent<TilemapRenderer>().sortingOrder = 0;
            }

        }
        else
        {
            // above player
            if (GetComponent<SpriteRenderer>())
            { 
                GetComponent<SpriteRenderer>().sortingOrder = 1;
            }
            if (GetComponent<TilemapRenderer>())
            {
                GetComponent<TilemapRenderer>().sortingOrder = 1;
            }
        }
    }
}
