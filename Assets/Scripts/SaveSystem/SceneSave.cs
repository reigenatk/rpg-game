using System.Collections.Generic;

[System.Serializable]
public class SceneSave
{
    // create dictionaries that will let us store any possible type of data we may come across
    public Dictionary<string, int> intDictionary;
    public Dictionary<string, float> floatDictionary;
    public Dictionary<string, float> yarnfloatDictionary; // use only for saving yarn floats
    public Dictionary<string, bool> boolDictionary;    // string key is an identifier name we choose for this list
    public Dictionary<string, bool> yarnboolDictionary; // use only for saving yarn bools
    public Dictionary<string, string> stringDictionary;
    public Dictionary<string, Vector3Serializable> vector3Dictionary;
    public Dictionary<string, int[]> intArrayDictionary;
    public Dictionary<GameVariable, bool> gamevariables; // for saving gamevariables (should've just used yarn from the start, but aint no way im doing that tranisiton)

    public Dictionary<PlayerScore, float> playerscores;

    public Dictionary<string, GridPropertyDetails> gridPropertyDetailsDictionary;

}
