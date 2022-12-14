using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class GameUI : Singleton<GameUI>
{
    [SerializeField] private GameObject gameClock;
    [SerializeField] private GameObject playerBars;
    GameState gameState;
    // Start is called before the first frame update
    public bool UIDisabled = false;


    private void Start()
    {
        gameState = FindObjectOfType<GameState>();

    }

    private void Update()
    {
        // if in dark scene, NO enabling game UI bars.
        if (LevelLoader.Instance.shouldHaveTime() == false)
        {
            if (!UIDisabled)
            {
                UIDisabled = true;
                disableUI();
            }
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            if (UIDisabled)
            {
                UIDisabled = false;
                enableUI();
            }
            else {
                UIDisabled = true;
                disableUI();
            }

        }
    }

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
    
    [YarnCommand("disableUI")]
    public void disableUI()
    {
        UIDisabled = true;
        gameClock.SetActive(false);
        playerBars.SetActive(false);
    }
    [YarnCommand("enableUI")]
    public void enableUI()
    {
        UIDisabled = false;
        gameClock.SetActive(true);
        playerBars.SetActive(true);
    }
}
