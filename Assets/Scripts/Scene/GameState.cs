using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] TextMeshProUGUI energyText;
    [SerializeField] TextMeshProUGUI socialText;
    [SerializeField] TextMeshProUGUI contentednessText;
    [SerializeField] TextMeshProUGUI entertainmentText;
    [SerializeField] TimeManager timeManager;
    public Moods playerMood;
    private int gameDay = 1;

    // Start is called before the first frame update
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
                playerScore.Add(foo, 35.0f);
            }
            else
            {
                playerScore.Add(foo, 100.0f);
            }
        }
    }
    public float getPlayerScoreSum()
    {
        float ret = 0.0f;
        ret += playerScore[PlayerScore.contentedness];
        ret += playerScore[PlayerScore.entertained];
        ret += playerScore[PlayerScore.social];
        return ret;
    }

    public void setCutscenePlaying(bool value)
    {
        gameVariables[GameVariable.isCutscenePlaying] = value;
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

        updateScoreUI(ps, percentage);
    }

    // changes player score by "delta" amount
    public void changePlayerScore(PlayerScore ps, float delta)
    {
        float newVal = playerScore[ps] + delta;
        
        if (newVal < 0.0f)
        {
            LevelLoader levelLoader = FindObjectOfType<LevelLoader>();
            // trigger some kind of cutscene
            switch (ps)
            {
                case PlayerScore.energy:
                    levelLoader.playCutscene("EnergyZero");
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
            float percentage = newVal / 100.0f;
            /*        string sformatted = String.Format("{0:0.##\\%}", percentage*100.0f); // "44.36%"
                    Debug.Log("percentage is " + percentage);*/

            updateScoreUI(ps, percentage);
        }
    }

    private void updateScoreUI(PlayerScore scoreToUpdate, float percentage)
    {
        switch (scoreToUpdate)
        {
            case PlayerScore.energy:
                energyBar.fillAmount = percentage;
                // energyText.text = sformatted;
                break;
            case PlayerScore.social:
                socialBar.fillAmount = percentage;
                // socialText.text = sformatted;
                break;
            case PlayerScore.entertained:
                entertainmentBar.fillAmount = percentage;
                // entertainmentText.text = sformatted;
                break;
            case PlayerScore.contentedness:
                contentednessBar.fillAmount = percentage;
                // contentednessText.text = sformatted;
                break;
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
    public void setGameVariable(string variableName, bool val)
    {
        GameVariable gv = gameVariableToEnum(variableName);
        if (gameVariables.ContainsKey(gv))
        {
            gameVariables[gv] = val;
        }
        else {
            gameVariables.Add(gv, val);
        }
        Debug.Log("Set gamevariable " + variableName + " to " + val);
    }

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
