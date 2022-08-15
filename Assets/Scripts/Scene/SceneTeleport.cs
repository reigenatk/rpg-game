using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTeleport : MonoBehaviour
{
    [SerializeField] private SceneName sceneNameGoto;
    [SerializeField] private Vector3 scenePositionGoto = new Vector3();
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player != null)
        {
            //  Calculate players new position
            //  if new positions are specified for x or y, then use the current value

            float xPosition = Mathf.Approximately(scenePositionGoto.x, 0f) ? player.transform.position.x : scenePositionGoto.x;

            float yPosition = Mathf.Approximately(scenePositionGoto.y, 0f) ? player.transform.position.y : scenePositionGoto.y;

            float zPosition = 0f;

            // Teleport to new scene
            LevelLoader.Instance.FadeAndLoadScene(sceneNameGoto, new Vector3(xPosition, yPosition, zPosition));

        }

    }
}
