public enum ToolEffect
{
    none,
    watering
}

public enum InventoryLocation
{
    player, 
    chest, 
    count, // neat way to get how many enums there are
}

public enum Direction
{
    up,
    down,
    left,
    right,
    none
}


[System.Serializable]
public enum GameVariable
{
    // this is just "hasEntered" + the name of the scene
    // make sure it matches the name of the scene in the editor, else it will fail!
    // the hasEntered variables are just useful for first-time scene triggers. Like for instance
    // first time upon entering commons, we should trigger a cutscene, etc.
    hasEnteredIntroScene,
    hasEnteredBedroom,
    hasEnteredCommons,
    hasEnteredKabowskiRoom,
    hasEnteredBrainsRoom,
    hasEnteredBathroomWTF,
    hasEnteredLancelotRoom,
    isCutscenePlaying,
}

public enum PlayerScore
{
    entertained,
    contentedness,
    social,
    energy,
}

public enum ItemType
{
    Seed,
    Commodity,
    Watering_Tool,
    Hoeing_Tool,
    Chopping_Tool,
    Breaking_Tool,
    Reaping_Tool,
    Collecting_Tool,
    Reapable_Scenery,
    Furniture,
    none,
    count
}

// must be same as the name of scene in Unity editor! We serialize the enum directly to string
public enum SceneName
{
    Bedroom,
    Commons,
    BathroomWTF,
    KabowskiRoom,
    BrainsRoom,
    LancelotRoom,
    IntroScene,
}