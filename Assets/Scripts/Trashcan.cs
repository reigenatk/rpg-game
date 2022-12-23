using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class Trashcan : MonoBehaviour
{
    public bool isDirty = true; // start it dirty
    Animator a;
    GameState g;
    AudioSource asrc;

    private void Start()
    {
        a = GetComponent<Animator>();
        g = FindObjectOfType<GameState>();
        asrc = FindObjectOfType<AudioSource>();
        g.setYarnVariable("$isTrashDirty", true); // start trash dirty
    }

    public void setDirty()
    {
        isDirty = true;
        a.SetBool("dirty", true);
        asrc.volume = 0.3f;
        g.setYarnVariable("$isTrashDirty", true);
    }

    [YarnCommand("SetClean")]
    public void setClean()
    {
        isDirty = false;
        asrc.volume = 0.0f;
        a.SetBool("dirty", false);
        g.setYarnVariable("$isTrashDirty", false);
    }
}
