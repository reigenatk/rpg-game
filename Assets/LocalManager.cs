using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalManager : MonoBehaviour
{
    Player p;

    public void Start()
    {
        p = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    public void stopAllMovements()
    {
        p.DisableMovementAndAnimations();
    }
    public void enableAllMovements()
    {
        p.EnableMovementAndAnimations();
    }

}
