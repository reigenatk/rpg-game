using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class TimeManager : Singleton<TimeManager>, ISaveable
{



    // save stuff
    private string iSaveableUniqueID;
    public string ISaveableUniqueID { get => iSaveableUniqueID; set => iSaveableUniqueID = value; }
    private GameObjectSave gameObjectSave;
    public GameObjectSave GameObjectSave { get => gameObjectSave; set => gameObjectSave = value; }


    protected override void Awake()
    {
        base.Awake();
        iSaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        gameObjectSave = new GameObjectSave();
    }

    private void OnEnable()
    {
        ISaveableRegister();

        // register some calls to our delegates
        EventHandler.BeforeSceneUnloadEvent += BeforeSceneUnloadFadeOut;
        EventHandler.AfterSceneLoadEvent += AfterSceneLoadFadeIn;
    }

    private void OnDisable()
    {
        ISaveableDeregister();

        EventHandler.BeforeSceneUnloadEvent -= BeforeSceneUnloadFadeOut;
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoadFadeIn;
    }

    // all these two methods do, is pause the gameclock when we are trying to load scenes. This makes sense cuz the game isn't running when its loading a new scene.
    private void BeforeSceneUnloadFadeOut()
    {
        gameClockPaused = true;
    }

    private void AfterSceneLoadFadeIn()
    {
        gameClockPaused = false;
    }

    [System.Serializable]
    public class ChunkOfTime
    {
        public GameTime startTime;
        public GameTime endTime;

        // is some time g within this chunk of time?
        public bool isInChunk(GameTime g)
        {
            return (startTime.compareTimes(g) == true) && (endTime.compareTimes(g) == false);
        }
    }
    [System.Serializable]
    public class GameTime
    {
        public int gameHour;
        public int gameMinute;
        public int gameSecond;

        public GameTime(int gameHour, int gameMinute, int gameSecond)
        {
            this.gameHour = gameHour;
            this.gameMinute = gameMinute;
            this.gameSecond = gameSecond;
        }

        public void advanceTime(int hours, int minutes)
        {
            gameMinute += minutes;
            if (gameMinute >= 60)
            {
                gameHour++;
                gameMinute %= 60;
            }
            gameHour += hours;
            if (gameHour >= 24)
            {
                gameHour %= 24;
            }
        }

        // returns result of < operation on the two times
        public bool compareTimes(GameTime b)
        {
            GameTime a = this;
            if (a.gameHour < b.gameHour)
            {
                return true;
            }
            else if (a.gameHour > b.gameHour)
            {
                return false;
            }
            else
            {
                if (a.gameMinute < b.gameMinute)
                {
                    return true;
                }
                else if (a.gameMinute > b.gameMinute)
                {
                    return false;
                }
                else
                {
                    if (a.gameSecond < b.gameSecond)
                    {
                        return true;
                    }
                    else if (a.gameSecond > b.gameSecond)
                    {
                        return false;
                    }
                    else
                    {
                        // they are equal :P but this will never happen hopefully
                        return false;
                    }
                }
            }
        }

        public int numericalTime()
        {
            return gameSecond + gameMinute * 60 + gameHour * 3600;
        }

        public int numSecondsFrom(GameTime b)
        {
            int curTimeNumerical = numericalTime();
            int bNumerical = b.numericalTime();
            if (bNumerical > curTimeNumerical) return bNumerical - curTimeNumerical;
            else
            {
                bNumerical += 24 * 3600; // 1 day
                return bNumerical - curTimeNumerical;
            }
        }
        
    }
    public GameTime gt;

    public TimeSpan GetGameTime()
    {
        TimeSpan gameTime = new TimeSpan(gt.gameHour, gt.gameMinute, gt.gameSecond);

        return gameTime;
    }


    // this is the number of seconds that have elapsed total in game (previous saves included)
    private int totalGameSeconds = 0;
    private string gameDayOfWeek = "Mon";

    // is paused?
    public bool gameClockPaused = false;

    // this is the internal time.Delta time clock for THIS RUN of the game
    private float gameTick = 0f;

    // how many in game seconds correspond to one decayed percent of each category
    [SerializeField] private GameState gameState;
    [SerializeField] private int energyDecayRate;
    [SerializeField] private int socialDecayRate;
    [SerializeField] private int contentednessDecayRate;
    [SerializeField] private int entertainmentDecayRate;
    [SerializeField] Light2D globalLight;
    [SerializeField] List<ColorPoint> globalLightColors;
    [SerializeField] List<SceneName> scenesThatShouldHaveColorChange; // which scenes should the global light changing effect be enabled for?

    // gameplay elements
    [SerializeField] int dialogueSlowdownRate; // slow time down by a certain factor when dialogue is playing
    [SerializeField] int dialogueSlowdownCounter = 0;
    [SerializeField] List<int> canTalkAgainTimes; // points in the game where we will reset all the "can talk to X NPC" related yarn variables. How about every 6 hours? so 0, 6, 12, 18 (6pm).
    [SerializeField] List<int> trashGoBadTimes; // times that the trashcan will revert back to being dirty

    bool doesThisSceneUseDynamicLights()
    {
        foreach (SceneName s in scenesThatShouldHaveColorChange)
        {
            if (SceneManager.GetActiveScene().name == s.ToString()) return true;
        }
        return false;
    }

    // a color point is just "turn the global light to certain color and intensity at hour x"
    [System.Serializable]
    public class ColorPoint
    {
        public Color color;
        public float intensity;
        public int gameHour;
    }

    private void Start()
    {
        // day 1 starts at 4:30 PM, we wanna enable sleeping at 11:00 PM which is 6:30,
        // so day 1 player has roughly 6 min 30 sec to explore.

        EventHandler.CallAdvanceGameMinuteEvent(gameDayOfWeek, gt.gameHour, gt.gameMinute, gt.gameSecond);
    }

    private void Update()
    {
        // filter out the cases where time SHOULDNT run first
        if (gameClockPaused == true)
        {
            // Debug.Log("Game clock is Paused");
        }
        else if (gameState.cutscenePlaying != null)
        {
            // cutscene is playing
            // Debug.Log("Cutscene is playing so pause time");
        }
        else if (LevelLoader.Instance.isDreamScene())
        {
            // time doesnt advance in dark scene lol

        }
        else if (gameState.currentRunningDialogueNode != null && gameState.currentRunningDialogueNode != "")
        {
            // idk why (for above) but checking against null isnt sufficient, we gotta check against empty string?
            // Dialogue is playing
            // Debug.Log("Dialogue is playing so pause time");

            // do a tick every (dialogueSlowdownRate) seconds
            if (dialogueSlowdownCounter == dialogueSlowdownRate)
            {
                dialogueSlowdownCounter = 0;
                GameTick();
            }
            else
            {
                dialogueSlowdownCounter++;
            }
        }
        else
        {
            GameTick();
        }



        CheckSpecialEvents();
    }

    [System.Serializable]
    public class GameEvent
    {
        public ChunkOfTime whenEventActive;
        public string yarnVariableName;
        public NPCMovement npcMovement; // (optional) so we can only set this variable true when the NPC stops moving (since it makes no sense to talk to someone on their way to somewhere and still have a special event trigger)
    }
    [SerializeField] List<GameEvent> specialEvents;
    public void CheckSpecialEvents()
    {
        foreach (GameEvent ge in specialEvents)
        {
            // only trigger the event if the player has stopped moving
            if (ge.whenEventActive.isInChunk(gt))
            {
                if (ge.npcMovement != null)
                {
                    if (ge.npcMovement.npcIsMoving) continue;
                }
                // tell yarn its active (so yarn knows to give special dialogues)
                gameState.setYarnVariable(ge.yarnVariableName, true);
            }
            else
            {
                // if NOT in event, set it false!
                gameState.setYarnVariable(ge.yarnVariableName, false);
            }
        }
    }

    private void GameTick()
    {
        gameTick += Time.deltaTime;

        if (gameTick >= Settings.secondsPerGameSecond)
        {
            gameTick -= Settings.secondsPerGameSecond;

            UpdateGameSecond();
        }
    }

    // the "tick" of the game.
    private void UpdateGameSecond()
    {
        gt.gameSecond++;
        totalGameSeconds++;
        gameState.numSecondsAwake++;
        // Debug.Log(gameState.getGameVariable("canPlayerSleep"));
        if (gameState.numSecondsAwake > Settings.numSecondsAwakeMandatory && gameState.getGameVariable("canPlayerSleep") == false)
        {
            gameState.setGameVariable("canPlayerSleep", true);
            LevelLoader.Instance.playCutscene("NotifySleep");
        }

        gameState.playerMood = gameState.calculateMood();

        if (gt.gameSecond > 59)
        {
            gt.gameSecond = 0;
            gt.gameMinute++;
            UpdateMinutesTillNextBus();

            if (gt.gameMinute > 59)
            {
                gt.gameMinute = 0;
                gt.gameHour++;

                if (gt.gameHour == 24)
                {
                    gt.gameHour = 0;
                    

                    gameDayOfWeek = GetDayOfWeek();
                    EventHandler.CallAdvanceGameDayEvent(gameDayOfWeek, gt.gameHour, gt.gameMinute, gt.gameSecond);
                }

                EventHandler.CallAdvanceGameHourEvent(gameDayOfWeek, gt.gameHour, gt.gameMinute, gt.gameSecond);
            }

            EventHandler.CallAdvanceGameMinuteEvent(gameDayOfWeek, gt.gameHour, gt.gameMinute, gt.gameSecond);

        }

        // reset NPC talkable variables
        foreach (int gameHour in canTalkAgainTimes)
        {
            if ((gt.gameHour == gameHour) && (gt.gameMinute == 0) && (gt.gameSecond == 0))
            {
                FindObjectOfType<GameState>().canTalkToAllNPCsAgain();
            }
        }

        // reset trashcan
        foreach (int gameHour in trashGoBadTimes)
        {
            if ((gt.gameHour == gameHour) && (gt.gameMinute == 0) && (gt.gameSecond == 0))
            {
                FindObjectOfType<Trashcan>().setDirty();
            }
        }


        if (doesThisSceneUseDynamicLights() == true) lightingTick();
        else
        {
            // keep global light at 1
            globalLight.intensity = 1.0f;
            globalLight.color = Color.white;
        }


        if (totalGameSeconds % energyDecayRate == 0)
        {
            gameState.changePlayerScore(PlayerScore.energy, -1.0f);
            // Debug.Log("New energy is " + gameState.getPlayerScore(PlayerScore.energy));
        }
        if (totalGameSeconds % socialDecayRate == 0)
        {
            gameState.changePlayerScore(PlayerScore.social, -1.0f);
        }
        if (totalGameSeconds % contentednessDecayRate == 0)
        {
            gameState.changePlayerScore(PlayerScore.contentedness, -1.0f);
        }
        if (totalGameSeconds % entertainmentDecayRate == 0)
        {
            gameState.changePlayerScore(PlayerScore.entertained, -1.0f);
        }


    }

    // this just updates a variable in Yarn which tells us when the next bus is
    public void UpdateMinutesTillNextBus()
    {
        GameState gs = FindObjectOfType<GameState>();
        gs.setYarnVariable("$minTilNextBus", Math.Min(Math.Abs(60 - gt.gameMinute), Math.Min(Math.Abs(30 - gt.gameMinute), Math.Abs(gt.gameMinute))));
    }

    private void lightingTick()
    {
        // change the color of the scene based on what time it is
        // make sure the list globalLightColors is sorted, with 3am first, 6am next, etc. Ending at 0am (aka midnight)
        // check which time it is, and therefore which color we are lerping from, and to
        // start from back since its sorted from lowest hour to highest, and we want first hour that it is greater than
        int hourIdx = -1;
        for (int i = globalLightColors.Count - 1; i >= 0; i--)
        {
            ColorPoint cp = globalLightColors[i];
            if (gt.gameHour >= cp.gameHour)
            {
                hourIdx = i;
                break;
            }
        }

        Color fromColor = globalLightColors[hourIdx].color;
        Color toColor = globalLightColors[(hourIdx + 1) % (globalLightColors.Count)].color;
        float fromIntesity = globalLightColors[hourIdx].intensity;
        float toIntensity = globalLightColors[(hourIdx + 1) % (globalLightColors.Count)].intensity;

        int toHour = globalLightColors[(hourIdx + 1) % (globalLightColors.Count)].gameHour;
        int numGameHoursBetween = toHour - globalLightColors[hourIdx].gameHour;
        if (numGameHoursBetween < 0) numGameHoursBetween += 24; // say we go from 21 to 1, (aka a new day), then its -22 +24 = 2 hours passed

        int numSecondsLeft = gt.numSecondsFrom(new GameTime(toHour, 0, 0));
        float percentComplete = 1.0f - ((numSecondsLeft * 1.0f) / (numGameHoursBetween * 1.0f * 60 * 60));
        /*        Debug.Log("num seconds left: " + numSecondsLeft + " going to hour " + toHour + " with percent complete: " + percentComplete);*/
        globalLight.color = Color.Lerp(fromColor, toColor, percentComplete);
        globalLight.intensity = Mathf.Lerp(fromIntesity, toIntensity, percentComplete);
    }

    private string GetDayOfWeek()
    {
        int dayOfWeek = FindObjectOfType<GameState>().getGameDay() % 7;

        switch (dayOfWeek)
        {
            case 1:
                return "Mon";

            case 2: 
                return "Tue";

            case 3:
                return "Wed";

            case 4:
                return "Thu";

            case 5:
                return "Fri";

            case 6:
                return "Sat";

            case 0:
                return "Sun";

            default:
                return "";
        }
    }
    [YarnCommand("AdvanceXMinutes")]
    public void AdvanceXMinutes(int numMinutes)
    {
        gt.advanceTime(0, numMinutes);
    }

    [YarnCommand("SetTime")]
    public void setTime(int hour, int min, int sec)
    {
        gt = new GameTime(hour, min, sec);
    }



    public void TestAdvanceGameMinute()
    {
        for (int i = 0; i < 60; i++)
        {
            UpdateGameSecond();
        }
    }

    //TODO:Remove
    /// <summary>
    /// Advance 1 day
    /// </summary>
    public void TestAdvanceGameDay()
    {
        for (int i = 0; i < 86400; i++)
        {
            UpdateGameSecond();
        }
    }

    public void ISaveableRegister()
    {

        // SaveLoadManager.Instance.iSaveableObjectList.Add(Instance);

    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(Instance);
    }


    public GameObjectSave ISaveableSave()
    {
        // Delete existing scene save if exists
        GameObjectSave.sceneData.Remove(Settings.PersistentScene);

        // Create new scene save
        SceneSave sceneSave = new SceneSave();

        // Create new int dictionary
        sceneSave.intDictionary = new Dictionary<string, int>();

        // Create new string dictionary
        sceneSave.stringDictionary = new Dictionary<string, string>();

        // Add values to the int dictionary
/*        sceneSave.intDictionary.Add("gameYear", gameYear);*/
        sceneSave.intDictionary.Add("gameDay", gameState.getGameDay());
        sceneSave.intDictionary.Add("gameHour", gt.gameHour);
        sceneSave.intDictionary.Add("gameMinute", gt.gameMinute);
        sceneSave.intDictionary.Add("gameSecond", gt.gameSecond);
        sceneSave.intDictionary.Add("numSecondsAwake", gameState.numSecondsAwake);
        sceneSave.intDictionary.Add("totalGameSeconds", totalGameSeconds);

        // save player score

        sceneSave.floatDictionary.Add("entertained", gameState.getPlayerScore(PlayerScore.entertained));
        sceneSave.floatDictionary.Add("contentedness", gameState.getPlayerScore(PlayerScore.contentedness));
        sceneSave.floatDictionary.Add("social", gameState.getPlayerScore(PlayerScore.social));
        sceneSave.floatDictionary.Add("energy", gameState.getPlayerScore(PlayerScore.energy));


        sceneSave.stringDictionary.Add("gameDayOfWeek", gameDayOfWeek);
/*        sceneSave.stringDictionary.Add("gameSeason", gameSeason.ToString());*/

        // Add scene save to game object for persistent scene
        GameObjectSave.sceneData.Add(Settings.PersistentScene, sceneSave);

        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        // Get saved gameobject from gameSave data
        if (gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            // Get savedscene data for gameObject
            if (GameObjectSave.sceneData.TryGetValue(Settings.PersistentScene, out SceneSave sceneSave))
            {
                // if int and string dictionaries are found
                if (sceneSave.intDictionary != null && sceneSave.stringDictionary != null)
                {
                    // populate saved int values


                    if (sceneSave.intDictionary.TryGetValue("gameDay", out int savedGameDay))
                        gameState.setGameDay(savedGameDay);

                    if (sceneSave.intDictionary.TryGetValue("gameHour", out int savedGameHour) && sceneSave.intDictionary.TryGetValue("gameMinute", out int savedGameMinute) && sceneSave.intDictionary.TryGetValue("gameSecond", out int savedGameSecond))
                    {
                        gt = new GameTime(savedGameHour, savedGameMinute, savedGameSecond);
                    }


                    // populate string saved values
                    if (sceneSave.stringDictionary.TryGetValue("gameDayOfWeek", out string savedGameDayOfWeek))
                        gameDayOfWeek = savedGameDayOfWeek;

                    if (sceneSave.intDictionary.TryGetValue("numSecondsAwake", out int savednumSecondsAwake))
                        gameState.numSecondsAwake = savednumSecondsAwake;

                    if (sceneSave.intDictionary.TryGetValue("totalGameSeconds", out int savedTotalGameSeconds))
                        totalGameSeconds = savedTotalGameSeconds;

                    // Player Score
                    if (sceneSave.floatDictionary.TryGetValue("entertained", out float savedEntertained))
                        gameState.setPlayerScore(PlayerScore.entertained, savedEntertained);
                    if (sceneSave.floatDictionary.TryGetValue("contentedness", out float savedContentedness))
                        gameState.setPlayerScore(PlayerScore.contentedness, savedContentedness);
                    if (sceneSave.floatDictionary.TryGetValue("social", out float savedSocial))
                        gameState.setPlayerScore(PlayerScore.social, savedSocial);
                    if (sceneSave.floatDictionary.TryGetValue("energy", out float savedEnergy))
                        gameState.setPlayerScore(PlayerScore.energy, savedEnergy);

                    // Zero gametick
                    gameTick = 0f;

                    // Trigger advance minute event
                    EventHandler.CallAdvanceGameMinuteEvent(gameDayOfWeek, gt.gameHour, gt.gameMinute, gt.gameSecond);

                    // Refresh game clock
                }
            }
        }
    }
    public void ISaveableStoreScene(string sceneName)
    {
        // Nothing required here since Time Manager is running on the persistent scene
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        // Nothing required here since Time Manager is running on the persistent scene
    }
}
