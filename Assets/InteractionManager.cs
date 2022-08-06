using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class InteractionManager : MonoBehaviour
{
    [System.Serializable]
    public class Interaction
    {
        [SerializeField] private Vector2 bottomLeft;
        [SerializeField] private Vector2 topRight;
        [SerializeField] public string direction;
        [SerializeField] public string dialogueToRun;


        public bool withinBounds(float x, float y)
        {
            if (x < topRight.x && x > bottomLeft.x && y < topRight.y && y > topRight.x)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [SerializeField] private Interaction[] interactions;
    DialogueRunner r;

    // Start is called before the first frame update
    void Start()
    {
        r = GameObject.FindGameObjectWithTag("DialogueSystem").GetComponent<Yarn.Unity.DialogueRunner>();
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorStateInfo animationState = GameObject.FindGameObjectWithTag("Player").
            GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);

        string playerDirection = "";
        if (animationState.IsName("IdleUp"))
        {
            playerDirection = "Up";
        }
        else if (animationState.IsName("IdleDown"))
        {
            playerDirection = "Down";
        }
        else if (animationState.IsName("IdleLeft"))
        {
            playerDirection = "Left";
        }
        else if (animationState.IsName("IdleRight"))
        {
            playerDirection = "Right";
        }

        Vector3 p = GameObject.FindGameObjectWithTag("Player").transform.position;
        // Debug.Log(playerDirection);
        foreach (Interaction i in interactions)
        {
            // if within bounds, facing right direction, and we pressed interact key
            if (i.withinBounds(p.x, p.y) && playerDirection == i.direction && Input.GetKeyDown(KeyCode.E)) {
                // then trigger the dialogue
                r.Stop();
                r.StartDialogue(i.dialogueToRun);
                break;
            }
        }
    }
}
