using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using Yarn;
using Yarn.Unity;

public class GameState : MonoBehaviour
{
    [System.Serializable]
    public class GameVariablePair
    {
        public GameVariable variable;
        public bool desiredValue;
    }

    public Dictionary<GameVariable, bool> gameVariables;

    Dictionary<PlayerScore, float> playerScore;
    [SerializeField] Image energyBar;
    [SerializeField] Image socialBar;
    [SerializeField] Image contentednessBar;
    [SerializeField] Image entertainmentBar;
    [SerializeField] ScoreCategoryUI energy;
    [SerializeField] ScoreCategoryUI social;
    [SerializeField] ScoreCategoryUI contentedness;
    [SerializeField] ScoreCategoryUI entertainment;



    [SerializeField] TimeManager timeManager;
    [SerializeField] public InMemoryVariableStorage yarnVariables;

    // basically specify which variables are 1 time events (in entire game, only happens once) vs variables that should be reset daily
    [SerializeField] List<GameVariable> variablesToNotResetEachDay;

    public Moods playerMood;
    public int gameDay = 1;
    public int numSecondsAwake = 0;
    public Weather currentWeather = Weather.None;

    // saving some objects as well
    public PlayableDirector cutscenePlaying;
    public SceneTeleport curSceneTeleport;
    public string currentRunningDialogueNode;

    // This is the function that INITIALIZES ALL GAME VARIABLES
    void Awake()
    {
        gameVariables = new Dictionary<GameVariable, bool>();
        playerScore = new Dictionary<PlayerScore, float>();

        // add all the gamevariables, start everything false first
        foreach (GameVariable foo in Enum.GetValues(typeof(GameVariable)))
        {
            gameVariables.Add(foo, false);
        }
        foreach (PlayerScore foo in Enum.GetValues(typeof(PlayerScore)))
        {
            if (foo == PlayerScore.energy)
            {
                playerScore.Add(foo, 170.0f);
            }
            else
            {
                playerScore.Add(foo, 100.0f);
            }
        }

        // for Day 1, we don't wanna wakeup, so let's set that we have entered the bedroom already
        // cuz otherwise it will play wakeup cutscene
        setGameVariableEnum(GameVariable.hasEnteredBedroom, true);

        // variables that live in yarn
        setYarnVariable("$teethBrushed", false);
        setYarnVariable("$isBedroomLampOn", false);

        // start at day 1
        setYarnVariable("$day", gameDay);

        // an event during Day 2, where Brain smashes some glass if you knock his door
        setYarnVariable("$hasBrainSmashedGlassYet", false);
    }

    public void setYarnVariable(string name, bool val)
    {
        yarnVariables.SetValue(name, val);
    }
    public void setYarnVariable(string name, int val)
    {
        yarnVariables.SetValue(name, val);
    }
    public bool getYarnVariable(string name)
    {
        yarnVariables.TryGetValue<bool>(name, out bool result);
        return result;
    }

    // this method resets a list of variables that we specify in the interface.
    // because, some game variables need to be reset everyday, while others are 1-day events
    public void gameStateAdvanceDay()
    {
        gameDay++;
        foreach (GameVariable foo in Enum.GetValues(typeof(GameVariable)))
        {
            bool shouldReset = true;
            foreach (GameVariable gv in variablesToNotResetEachDay)
            {
                if (foo == gv)
                {
                    shouldReset = false;
                    break;
                }
            }
            if (shouldReset) gameVariables[foo] = false;
        }

        // notify yarn of this day change too, since which sleep dialogue we play depends on what day it is
        setYarnVariable("$day", gameDay);
    }

    // helper
    public float getPlayerScoreSum()
    {
        float ret = 0.0f;
        ret += playerScore[PlayerScore.contentedness];
        ret += playerScore[PlayerScore.entertained];
        ret += playerScore[PlayerScore.social];
        return ret;
    }

    public void setIsCutscenePlaying(bool value)
    {
        gameVariables[GameVariable.isCutscenePlaying] = value;
    }
    // this is called via SIGNAL from each timeline's signal emitters
    public void cutsceneFinishedPlaying()
    {
        
        FindObjectOfType<Player>().GetComponent<BoxCollider2D>().enabled = true;
        setIsCutscenePlaying(false);
        cutscenePlaying = null;

        // start up game clock again
        FindObjectOfType<TimeManager>().gameClockPaused = false;
    }

    // basically when a bus cutscene plays, we switch scenes right? But the new scene needs to know that the bus is still driving 
    // so it will check if isBusCutscenePlaying is true, and if it is, it will play the bus arrival cutscene right when the scene loads.
    public void setIsBusCutscenePlaying(bool value)
    {
        gameVariables[GameVariable.isBusCutscenePlaying] = value;
    }



    // this is only here cuz we need the Manager object to distribute this to the actual SceneTeleport being referenced
    [YarnCommand("KnockOnDoor")]
    public void KnockOnDoor()
    {
        if (curSceneTeleport != null)
        {
            curSceneTeleport.KnockOnDoor();
        }
    }

    public void PlayKnockOnDoorDialogue()
    {
        if (curSceneTeleport != null)
        {
            curSceneTeleport.playKnockDialogue();
        }
    }


    public string getScore(PlayerScore ps)
    {
        float val = playerScore[ps];
        float percentage = val / 100.0f;
        string sformatted = String.Format("{0:0.##\\%}", percentage * 100.0f); // "44.36%"
        return sformatted;
    }

    // changes player score (exact value), also updates the UI bars accordingly
    public void setPlayerScore(PlayerScore ps, float val)
    {

        playerScore[ps] = val;
        float percentage = val / 100.0f;
        /*        string sformatted = String.Format("{0:0.##\\%}", percentage*100.0f); // "44.36%"
                Debug.Log("percentage is " + percentage);*/

        switch (ps)
        {
            case PlayerScore.energy:
                energy.UpdateScoreUI(ps, percentage, 0.0f);
                break;
            case PlayerScore.social:
                social.UpdateScoreUI(ps, percentage, 0.0f);
                break;
            case PlayerScore.entertained:
                entertainment.UpdateScoreUI(ps, percentage, 0.0f);
                break;
            case PlayerScore.contentedness:
                contentedness.UpdateScoreUI(ps, percentage, 0.0f);
                break;
        }
    }

    // literally same as below except you can pass string (so Yarn can use it)
    [YarnCommand("changePlayerScoreString")]
    public void changePlayerScoreString(string ps, float delta)
    {
        changePlayerScore(Settings.ParseEnum<PlayerScore>(ps), delta);
    }

    // changes player score by "delta" amount
    public void changePlayerScore(PlayerScore ps, float delta)
    {
        float newVal = playerScore[ps] + delta;
        LevelLoader levelLoader = FindObjectOfType<LevelLoader>();

        // only play the warning scenes once (per day). So say, user eats and energy goes back up.
        // when it goes below 10 again we shouldn't run it again, that might get redundant.
        if (newVal < 10.0f)
        {
            switch (ps)
            {
                case PlayerScore.energy:
                    if (!getGameVariable("hasEnergyLowWarningPlayed"))
                    {
                        setGameVariable("hasEnergyLowWarningPlayed", true);
                        levelLoader.playCutscene("EnergyLow");
                    }
                    break;
                case PlayerScore.contentedness:
                    levelLoader.playCutscene("ContentednessZero");
                    break;
                case PlayerScore.social:
                    levelLoader.playCutscene("SocialZero");
                    break;
                case PlayerScore.entertained:
                    levelLoader.playCutscene("EntertainedZero");
                    break;
            }
        }

        if (newVal < 0.0f)
        {
            Debug.Log("Zero " + ps.ToString());

            // stop the time since player will either faint or enter cutscene
            timeManager.gameClockPaused = true;
            DialogueManager dm = FindObjectOfType<DialogueManager>();
            // trigger some kind of cutscene
            switch (ps)
            {
                case PlayerScore.energy:
                    dm.StartDialogueString("OutOfEnergy");
                    break;
                case PlayerScore.contentedness:
                    levelLoader.playCutscene("ContentednessZero");
                    break;
                case PlayerScore.social:
                    levelLoader.playCutscene("SocialZero");
                    break;
                case PlayerScore.entertained:
                    levelLoader.playCutscene("EntertainedZero");
                    break;
            }
        }
        else
        {
            playerScore[ps] = newVal;
            /*        string sformatted = String.Format("{0:0.##\\%}", percentage*100.0f); // "44.36%"
                    Debug.Log("percentage is " + percentage);*/

            switch (ps)
            {
                case PlayerScore.energy:
                    energy.UpdateScoreUI(ps, newVal, delta);
                    break;
                case PlayerScore.social:
                    social.UpdateScoreUI(ps, newVal, delta);
                    break;
                case PlayerScore.entertained:
                    entertainment.UpdateScoreUI(ps, newVal, delta);
                    break;
                case PlayerScore.contentedness:
                    contentedness.UpdateScoreUI(ps, newVal, delta);
                    break;
            }

        }
    }

   



    public int getGameDay()
    {
        return gameDay;
    }

    public void setGameDay(int val)
    {
        Debug.Log("setting gameday to " + val);
        gameDay = val;
    }

    public float getPlayerScore(PlayerScore ps)
    {
        return playerScore[ps];
    }

    // delta is some value to add. Default is just zero
    public Moods calculateMood(float delta = 0.0f)
    {
        float playerScore = getPlayerScoreSum() + delta;
        if (playerScore < 50.0f)
        {
            return Moods.Suicidal;
        }
        else if (playerScore < 100.0f)
        {
            return Moods.Depressed;
        }
        else if (playerScore < 150.0f)
        {
            return Moods.Unhinged;
        }
        else if (playerScore < 200.0f)
        {
            return Moods.Average;
        }
        else if (playerScore < 250.0f)
        {
            return Moods.Good;
        }
        else if (playerScore < 300.0f)
        {
            return Moods.LovingLife;
        }
        else
        {
            return Moods.LovingLife;
        }
    }

    // Update is called once per frame
    [YarnFunction("SetGameVariable")]
    public void setGameVariable(string variableName, bool val)
    {
        GameVariable gv = gameVariableToEnum(variableName);
        setGameVariableEnum(gv, val);
    }

    public void setGameVariableEnum(GameVariable gv, bool val)
    {
        if (gameVariables.ContainsKey(gv))
        {
            gameVariables[gv] = val;
        }
        else
        {
            gameVariables.Add(gv, val);
        }
        Debug.Log("Set gamevariable " + gv.ToString() + " to " + val);
    }

    [YarnFunction("GetGameVariable")]
    public bool getGameVariable(string variableName)
    {
        GameVariable gv = gameVariableToEnum(variableName);
        return gameVariables[gv];
    }

    public bool getGameVariableEnum(GameVariable gv)
    {

        return gameVariables[gv];
    }

    public GameVariable gameVariableToEnum(string name)
    {
        return (GameVariable)System.Enum.Parse(typeof(GameVariable), name);
    }

}
