using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// adds rigidbody and boxcollider to object if it doesnt have it yet
[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class ExitDoorway : MonoBehaviour
{
    // Reset happens after Awake() but before Start()
    private void Reset()
    {
        GetComponent<Rigidbody2D>().isKinematic = true;
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        box.size = Vector2.one * 0.2f;
        box.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
