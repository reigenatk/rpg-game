using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoffeeShopDoors : MonoBehaviour
{
    Player player;
    SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.y < gameObject.transform.position.y)
        {
            spriteRenderer.sortingOrder = 0;
        }
        else
        {
            spriteRenderer.sortingOrder = 1;
        }
    }
}
