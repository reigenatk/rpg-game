using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    [System.Serializable]
    public class GameVariablePair
    {
        public GameVariable variable;
        public bool desiredValue;
    }

    Dictionary<GameVariable, bool> gameVariables;
    // Start is called before the first frame update
    void Start()
    {
        gameVariables = new Dictionary<GameVariable, bool>();

        // start everything false first
        foreach (GameVariable foo in Enum.GetValues(typeof(GameVariable)))
        {
            gameVariables.Add(foo, false);
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
