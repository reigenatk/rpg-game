using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameObject gameClock;
    [SerializeField] private GameObject playerBars;
    // Start is called before the first frame update

    [YarnCommand("enableGameClock")]
    public void enableGameClock()
    {
        gameClock.SetActive(true);
    }
    [YarnCommand("enablePlayerBars")]
    public void enablePlayerBars()
    {
        playerBars.SetActive(true);
    }
}
