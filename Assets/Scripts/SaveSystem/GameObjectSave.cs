using System.Collections.Generic;

[System.Serializable]
public class GameObjectSave
{
    // string key = scene name
    // we have one instance for all the data in the game here, called sceneData, which is a dict from sceneName to SceneSave
    // the number of entries in this dict = the number of scenes
    public Dictionary<string, SceneSave> sceneData;

    // so in other words, each gameobject can have a SceneSave object for each scene in the game? Hmmm

    public GameObjectSave()
    {
        sceneData = new Dictionary<string, SceneSave>();
    }

    public GameObjectSave(Dictionary<string, SceneSave> sceneData)
    {
        this.sceneData = sceneData;
    }
}
