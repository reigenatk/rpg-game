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
    Up,
    Down,
    Left,
    Right,
    none
}


// You must add new entires at the very bottom, otherwise it changes the pre-existing values in the editor! Very annoying.
[System.Serializable]
public enum GameVariable
{
    // this is just "hasEntered" + the name of the scene
    // make sure it matches the name of the scene in the editor, else it will fail!
    // the hasEntered variables are just useful for first-time scene triggers.
    // Like for instance they help us detect our 
    // first time upon entering commons for each day
    // which then tells us that we should trigger a cutscene, etc.
    hasEnteredDarkScene,
    hasEnteredBedroom,
    hasEnteredCommons,
    hasEnteredKabowskiRoom,
    hasEnteredBrainsRoom,
    hasEnteredBathroomWTF,
    hasEnteredLancelotRoom,

    // use this to know whether to enable player anims or not
    isCutscenePlaying,
    isDialoguePlaying,
    // has the player stayed up long enough so that sleeping is available?
    canPlayerSleep, 
    
    // cutscene related logic
    hasEnergyLowWarningPlayed,
    hasEnteredOutsideHouse,
    hasEnteredCampus,
    hasEnteredInsideBuilding,
    hasEnteredLectureHall,
    hasFinishedLecture,
    
    // each day there will be a new scene when the player arrives home. This is set to true after that scene has concluded
    hasFinishedHomeScene, 

    isBusCutscenePlaying,
    hasEnteredMainQuad,
    hasEnteredClassroom,
}


public enum PlayerScore
{
    entertained,
    contentedness,
    social,
    energy,
}

public enum GridBoolProperty
{
    isPath,
    isNPCObstacle,
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
    DarkScene,
    OutsideHouse,
    Campus,
    InsideBuilding,
    LectureHall,
    MainQuad,
    Classroom
}

public enum Moods
{
    Suicidal,
    Unhinged,
    Depressed,
    Average,
    Good,
    LovingLife,
}

public enum Weather
{
    None,
    Overcast,
    Raining,
    Thunderstorm,
}