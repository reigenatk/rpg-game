using System.Collections.Generic;

[System.Serializable]
public class GameObjectSave
{
    // string key = scene name
    // we have one instance for all the data in the game here, called sceneData, which is a dict from sceneName to SceneSave
    public Dictionary<string, SceneSave> sceneData;

    public GameObjectSave()
    {
        sceneData = new Dictionary<string, SceneSave>();
    }

    public GameObjectSave(Dictionary<string, SceneSave> sceneData)
    {
        this.sceneData = sceneData;
    }
}
