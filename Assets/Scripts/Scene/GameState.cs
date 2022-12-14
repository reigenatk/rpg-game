using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Yarn;
using Yarn.Unity;

public class GameState : Singleton<GameState>, ISaveable
{

    [System.Serializable]
    public class GameVariablePair
    {
        public GameVariable variable;
        public bool desiredValue;
    }

    [System.Serializable]
    public class YarnVariablePair
    {
        public string YarnVariable;
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
    [SerializeField] GameObject npcs;
    [SerializeField] GameObject nonnpcs;
    [SerializeField] string currentScene;
    public SceneName startingScene;
    public int totalNumFriends;

    [SerializeField] TimeManager timeManager;
    [SerializeField] public InMemoryVariableStorage yarnVariables;

    // basically specify which variables are 1 time events (in entire game, only happens once) vs variables that should be reset daily
    [SerializeField] List<GameVariable> variablesToNotResetEachDay;
    [SerializeField] List<GameVariablePair> initialGameState; // sets some game variables for debugging purposes

    // not working idk why
    // [SerializeField] List<YarnVariablePair> initialYarnGameState; // sets some game variables for debugging purposes


    public Moods playerMood;
    public int gameDay = 0;
    public int numSecondsAwake = 0;
    public Weather currentWeather = Weather.None; // never ended up using this shit

    // saving some objects as well
    public PlayableDirector cutscenePlaying = null;
    public SceneTeleport curSceneTeleport = null;
    public string currentRunningDialogueNode = null;

    // need this to basically tell the NPC when its done being talked to
    public NPCMovement currentNPCBeingTalkedTo = null;


    // save stuff
    private string iSaveableUniqueID;
    public string ISaveableUniqueID { get => iSaveableUniqueID; set => iSaveableUniqueID = value; }
    private GameObjectSave gameObjectSave;
    public GameObjectSave GameObjectSave { get => gameObjectSave; set => gameObjectSave = value; }

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

        // starting player scores, change this at will
        foreach (PlayerScore foo in Enum.GetValues(typeof(PlayerScore)))
        {
            if (foo == PlayerScore.energy)
            {
                playerScore.Add(foo, UnityEngine.Random.Range(80, 100));
            }
            else if (foo == PlayerScore.contentedness)
            {
                playerScore.Add(foo, UnityEngine.Random.Range(80, 100));
            }
            else if (foo == PlayerScore.social)
            {
                playerScore.Add(foo, UnityEngine.Random.Range(80, 100));
            }
            else if (foo == PlayerScore.entertained)
            {
                playerScore.Add(foo, UnityEngine.Random.Range(80, 100));
            }
        }

        resetGameState();

        // set OUR variables
        foreach (GameVariablePair gvp in initialGameState)
        {
            setGameVariable(gvp.variable.ToString(), gvp.desiredValue);
        }


        // start with all non npcs disabled
        foreach (SpriteRenderer sr in nonnpcs.GetComponentsInChildren<SpriteRenderer>())
        {
            sr.enabled = false;
        }

        // save stuff
        iSaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        gameObjectSave = new GameObjectSave();
    }

    public void resetGameState()
    {


        // tutorial variables
        setYarnVariable("$hasEnteredMainQuadBefore", false);
        setYarnVariable("$hasEnteredInsideBuildingBefore", false);
        setYarnVariable("$hasEnteredOutsideHouseBefore", false);
        setYarnVariable("$hasEnteredCampusBefore", false);
        setYarnVariable("$hasEnteredClassroomBefore", false);

        // start at day 1
        setYarnVariable("$day", 0);
        numSecondsAwake = 45900;
        TimeManager.Instance.gt = new TimeManager.GameTime(20, 45, 0); // 8:45 PM start

        // DEBUG ONLY. Once final game is done done set all values to right value (usually false)
        // these are to trigger the teleporters in the dream scenes. When compiling final game just comment all this out.

        // dream day 1
        setYarnVariable("$finishedMeetJefferyScene", false);

        // dream day 2
        setYarnVariable("$finishedEatingFoodScene", false); // the dream version when the kids laugh at your weird food
        setYarnVariable("$hasFinishedLecture", false);
        /*        setYarnVariable("$hasEnteredInsideBuildingBefore", false);
                setYarnVariable("$hasEnteredCampusBefore", false);
                setYarnVariable("$hasEnteredCommonsBefore", false);*/
        setYarnVariable("$hasStacySuprisedUs", false);
        setYarnVariable("$hasDoneDay2Eating", false);

        // day 3
        setYarnVariable("$didCoomerCutscene", false);
        setYarnVariable("$hasDoneShopping", false);

        // day 4
        setYarnVariable("$didGraveyardScene", false);



        // misc/events
        setYarnVariable("$hasMadeFriend", false);
        setYarnVariable("$hasTurnedDownMusic", false);
        setYarnVariable("$hasMadeFriend", false);
        setYarnVariable("$didGamingCutscene", false);
        setYarnVariable("$hasHeardReaperTerms", false);
        setYarnVariable("$isGroupMeetingOn", false);
        setYarnVariable("$hasDoneFirstFriend", false); // toggles the cutscene where it says you can text ur friends now
        setYarnVariable("$hasBoated", false);
        setYarnVariable("$hasDoneGroupMeeting", false);
        setYarnVariable("$finishedEatingFoodSceneDay3", false);

        // set some dialogue values for our playthrough
        setYarnVariable("$isKYS", false);

        // kabowski 1
        setYarnVariable("$hasMetKabowski", false);
        setYarnVariable("$didKabowskiDialogue1", false);
        setYarnVariable("$didKabowskiLifting", false);
        setYarnVariable("$didKabowskiGirls", false);
        setYarnVariable("$kabowskiFriendProgress", 0);

        // nikolai 2
        setYarnVariable("$hasMetNikolai", false);
        setYarnVariable("$didNikolaiDialogue1", false);
        setYarnVariable("$didNikolaiDialogue2", false);
        setYarnVariable("$didNikolaiRussia", false);
        setYarnVariable("$didNikolaiEnglish", false);
        setYarnVariable("$nikolaiFriendProgress", 0);


        // stacy 3
        setYarnVariable("$hasMetStacy", false);
        setYarnVariable("$didStacyLaura", false);
        setYarnVariable("$didStacyHome", false);
        setYarnVariable("$stacyFriendProgress", 0);
        setYarnVariable("$didStacyDialogue1", false);

        // laura 4
        setYarnVariable("$hasMetDoomerGirl", false);
        setYarnVariable("$didDGStacyDialogue", false);
        setYarnVariable("$didDGMusicDialogue", false);
        setYarnVariable("$doomerGirlFriendProgress", 0);
        setYarnVariable("$didDoomerGirlDialogue1", false);
        setYarnVariable("$didDoomerGirlDialogue2", false);
        setYarnVariable("$gotLauraNumber", false);
        setYarnVariable("$hasTextedLaura", false);

        // boomer 5
        setYarnVariable("$hasMetBoomer", false);
        setYarnVariable("$hasAcceptedFootballGame", false);
        setYarnVariable("$didBoomerMusic", false);
        setYarnVariable("$didBoomerFootball", false);
        setYarnVariable("$didBoomerDialogue1", false);
        setYarnVariable("$didBoomerDialogue2", false);
        setYarnVariable("$boomerFriendProgress", 0);

        // zoomer 6
        setYarnVariable("$hasMetZoomer", false);
        setYarnVariable("$didZoomerMusic", false);
        setYarnVariable("$didZoomerFashion", false);
        setYarnVariable("$didZoomerDialogue1", false);
        setYarnVariable("$didZoomerDialogue2", false);
        setYarnVariable("$zoomerFriendProgress", 0);

        // coomer 7
        setYarnVariable("$hasMetCoomer", false);
        setYarnVariable("$didCoomerHair", false);
        setYarnVariable("$didCoomerBiceps", false);
        setYarnVariable("$didCoomerDialogue1", false);
        setYarnVariable("$coomerFriendProgress", 0);

        // doomer 8
        setYarnVariable("$hasMetDoomer", false);
        setYarnVariable("$gotDoomerNumber", false);
        setYarnVariable("$didDoomerDialogue1", false);
        setYarnVariable("$didDoomerDialogue2", false);
        setYarnVariable("$hasTextedDoomer", false);
        setYarnVariable("$doomerFriendProgress", 0);

        // pepe 9
        setYarnVariable("$hasMetPepe", false);
        setYarnVariable("$hasPepeNumber", false);
        setYarnVariable("$hasTextedPepe", false);
        setYarnVariable("$pepeFriendProgress", 0);
        setYarnVariable("$didPepeDialogue1", false);


        // Becky 10
        setYarnVariable("$hasMetBecky", false);
        setYarnVariable("$didBeckyStacy", false);
        setYarnVariable("$didBeckyKabowski", false);
        setYarnVariable("$beckyFriendProgress", 0);


        // discord 11 
        setYarnVariable("$hasMetDiscord", false);
        setYarnVariable("$didDiscordDialogue1", false);
        setYarnVariable("$didDiscordDialogue2", false);
        setYarnVariable("$DiscordFriendProgress", 0);

        // reddit 12
        setYarnVariable("$hasMetReddit", false);
        setYarnVariable("$didRedditDialogue1", false);
        setYarnVariable("$didRedditDialogue2", false);
        setYarnVariable("$RedditFriendProgress", 0);

        // brain 13
        setYarnVariable("$hasMetBrain", false);
        setYarnVariable("$brainFriendProgress", 0);
        setYarnVariable("$didBrainDialogue1", false);
        setYarnVariable("$didBrainDialogue2", false);
        setYarnVariable("$didBrainRoomates", false);
        setYarnVariable("$didBrainScience", false);


        // bloomer 14
        setYarnVariable("$hasMetBloomer", false);
        setYarnVariable("$bloomerFriendProgress", 0);
        setYarnVariable("$didBloomerDialogue1", false);
        setYarnVariable("$didBloomerDialogue2", false);
        setYarnVariable("$didBloomerDialogue3", false);
        setYarnVariable("$didBloomerHair", false);
        setYarnVariable("$didBloomerGuitar", false);
        setYarnVariable("$didBloomerSmiling", false);

        resetDailyYarnVariables();
    }

    private void OnEnable()
    {
        ISaveableRegister();


    }

    private void OnDisable()
    {
        ISaveableDeregister();


    }

    private void Update()
    {

        // we want to tell yarn when nikolai is in his room and music is blasting, so we can create some custom dialogue
        // that lets the player ask Nikolai to turn down his music.
        if (getCurrentSceneEnum() == SceneName.LancelotRoom && FindObjectOfType<Subwoofer>().isSubwooferPlaying)
        {
            setYarnVariable("$isInNikolaiRoom", true);
        }
        else
        {
            setYarnVariable("$isInNikolaiRoom", false);
        }

        if (getCurrentSceneEnum() == SceneName.Classroom2)
        {
            setYarnVariable("$isInClassroom2", true);
        }
        else
        {
            setYarnVariable("$isInClassroom2", false);
        }

        // just for sanity
        currentScene = getCurrentSceneEnum().ToString();

        setYarnVariable("$gameHour", TimeManager.Instance.gt.gameHour);

        // condition A: made super close friends with at least 2 of the 3 options
        bool conditionA = getYarnVariable("$hasTextedDoomer") && getYarnVariable("$hasTextedLaura") && getYarnVariable("$hasTextedPepe") == true;

        // condition B: Just befriend at least 10 of the 14 total people
        totalNumFriends = getYarnVariableInt("$kabowskiFriendProgress") + getYarnVariableInt("$nikolaiFriendProgress") + getYarnVariableInt("$brainFriendProgress")
            + getYarnVariableInt("$stacyFriendProgress") + getYarnVariableInt("$beckyFriendProgress") + getYarnVariableInt("$doomerFriendProgress")
            + getYarnVariableInt("$doomerGirlFriendProgress") + getYarnVariableInt("$pepeFriendProgress") + getYarnVariableInt("$boomerFriendProgress")
            + getYarnVariableInt("$bloomerFriendProgress") + getYarnVariableInt("$coomerFriendProgress") + getYarnVariableInt("$zoomerFriendProgress")
            + getYarnVariableInt("$DiscordFriendProgress") + getYarnVariableInt("$RedditFriendProgress");
        bool conditionB = (totalNumFriends > 10); 

        // conditonal on whether we can trigger the cutscene to beat the game
        if ((conditionA || conditionB) && getYarnVariable("$hasDoneGroupMeeting") == false && getYarnVariableInt("$day") > 5)
        {
            setYarnVariable("$canBeatGame", true);
        }
        else
        {
            setYarnVariable("$canBeatGame", false);
        }

        // check if lost the game
        if (playerScore[PlayerScore.contentedness] + playerScore[PlayerScore.social] + playerScore[PlayerScore.energy] + playerScore[PlayerScore.entertained] < 30.0f)
        {
            if (getYarnVariable("$hasPlayedKYSMonologue") == false)
            {
                LevelLoader.Instance.playCutscene("KYS");
            }
            
        }
    }

    public void resetDailyYarnVariables()
    {
        setYarnVariable("$teethBrushed", false);
        setYarnVariable("$isBedroomLampOn", false);
        setYarnVariable("$numTimesNutted", 0);

        if (getYarnVariable("$wasNiceToRoomates") == true)
        {
            setYarnVariable("$numTimesNiceToRoomates", getYarnVariableInt("$numTimesNiceToRoomates") + 1);
        }
        setYarnVariable("$wasNiceToRoomates", false);

        // day was already advanced, tell yarn about that
        setYarnVariable("$gameDay", gameDay);

        // reset dialogue varialbes
        canTalkToAllNPCsAgain();

        setYarnVariable("$alreadyCookedToday", false);
        setYarnVariable("$hasEatenSnack", false);
        setYarnVariable("$hasTalkedToReaper", false);
        setYarnVariable("$hasRanToday", false);
        setYarnVariable("$hasMicrowavedToday", false);
        setYarnVariable("$hasShitted", false);
        setYarnVariable("$hasBoated", false);
        
    }

    public void setYarnVariable(string name, bool val)
    {
        // Debug.Log("[Set Yarn Variable] " + name + " to val " + val);
        yarnVariables.SetValue(name, val);
    }
    public void setYarnVariable(string name, int val)
    {
        // Debug.Log("[Set Yarn Variable] " + name + " to INT val " + val);
        yarnVariables.SetValue(name, val);
    }
    public bool getYarnVariable(string name)
    {
        if (yarnVariables.Contains(name))
        {
            yarnVariables.TryGetValue<bool>(name, out bool result);
            return result;
        }
        else
        {
            Debug.Log("Doesn't contain yarn variable of name " + name);
            return false;
        }

    }



    public int getYarnVariableInt(string name)
    {
        if (yarnVariables.Contains(name))
        {
            yarnVariables.TryGetValue<float>(name, out float result);
            return Convert.ToInt32(result);
        }
        else
        {
            Debug.Log("Doesn't contain yarn variable of name " + name);
            return -1;
        }
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

    // stop any running cutscene
    [YarnCommand("stopRunningCutscene")]
    public void stopRunningCutscene()
    {
        if (cutscenePlaying != null)
        {
            cutscenePlaying.Stop();
            cutsceneFinishedPlaying();
        }
    }

   
    public void canTalkToAllNPCsAgain()
    {
        Debug.Log("Can Talk to All NPCs again");
        setYarnVariable("$talkedToNikolaiAlready", false);
        setYarnVariable("$talkedToKabowskiAlready", false);
        setYarnVariable("$talkedToBrainAlready", false);
        setYarnVariable("$talkedToStacyAlready", false);
        setYarnVariable("$talkedToBeckyAlready", false);
        setYarnVariable("$talkedToDoomerAlready", false);
        setYarnVariable("$talkedToDoomerGirlAlready", false);
        setYarnVariable("$talkedToPepeAlready", false);
        setYarnVariable("$talkedToBoomerAlready", false);
        setYarnVariable("$talkedToBloomerAlready", false);
        setYarnVariable("$talkedToCoomerAlready", false);
        setYarnVariable("$talkedToRedditAlready", false);
        setYarnVariable("$talkedToDiscordAlready", false);
        setYarnVariable("$talkedToZoomerAlready", false);
    }

    // this is called via SIGNAL from each timeline's signal emitters
    public void cutsceneFinishedPlaying()
    {
        Debug.Log("cutscene finished playing");
        // enable all NPC characters
        Debug.Log("Enabling all NPC chars sprite renderers");
        foreach (SpriteRenderer sr in npcs.GetComponentsInChildren<SpriteRenderer>())
        {
            sr.enabled = true;
        }
        Debug.Log("Enabling all Audio on npcs");
        foreach (AudioSource asr in npcs.GetComponentsInChildren<AudioSource>())
        {
            asr.enabled = true;
        }
        // disable all Non NPC characters
        Debug.Log("Disabling all Non-NPC chars sprite renderers");
        foreach (SpriteRenderer sr in nonnpcs.GetComponentsInChildren<SpriteRenderer>())
        {
            sr.enabled = false;
        }
/*        foreach (Animator a in npcs.GetComponentsInChildren<Animator>())
        {
            a.enabled = true;
        }*/

        GameUI.Instance.enableUI();
        FindObjectOfType<Player>().GetComponent<BoxCollider2D>().enabled = true;
        setIsCutscenePlaying(false);
        cutscenePlaying = null;

        // fade in again (if faded out)
        LevelLoader.Instance.FadeOut(1.0f);

        // start up game clock again
        FindObjectOfType<TimeManager>().gameClockPaused = false;

/*        if (LevelLoader.Instance.cutsceneQueue.Count != 0)
        {
            PlayableDirector nextCutscene = LevelLoader.Instance.cutsceneQueue.Pop();
            LevelLoader.Instance.playCutscene(nextCutscene.name);
        }*/
    }

    // basically when a bus cutscene plays, we switch scenes right? But the new scene needs to know that the bus is still driving 
    // so it will check if isBusCutscenePlaying is true, and if it is, it will play the bus arrival cutscene right when the scene loads.
    public void setIsBusCutscenePlaying(bool value)
    {
        gameVariables[GameVariable.isBusCutscenePlaying] = value;
    }

    public SceneName getCurrentSceneEnum()
    {
        return (SceneName)Enum.Parse(typeof(SceneName), SceneManager.GetActiveScene().name);
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
        Debug.Log("Changing player score " + ps + " by value " + delta);
        changePlayerScore(Settings.ParseEnum<PlayerScore>(ps), delta);
    }

    // changes player score by "delta" amount
    public void changePlayerScore(PlayerScore ps, float delta)
    {
        float newVal = playerScore[ps] + delta;
        LevelLoader levelLoader = FindObjectOfType<LevelLoader>();

        // only play the warning scenes once through the entire game...
        // most of them would just be dialogues, anyways...
        if (newVal < 20.0f)
        {
            switch (ps)
            {
                case PlayerScore.energy:
                    if (!getGameVariable("EnergyLowWarningPlayed"))
                    {
                        setGameVariable("EnergyLowWarningPlayed", true);
                        StartCoroutine(levelLoader.playCutsceneWithDelay("EnergyLow"));
                    }
                    break;
                case PlayerScore.contentedness:
                    if (!getGameVariable("ContentLowPlayed"))
                    {
                        setGameVariable("ContentLowPlayed", true);
                        StartCoroutine(levelLoader.playCutsceneWithDelay("ContentLow"));
                    }
                    break;
                case PlayerScore.social:
                    if (!getGameVariable("SocialLowPlayed"))
                    {
                        setGameVariable("SocialLowPlayed", true);
                        StartCoroutine(levelLoader.playCutsceneWithDelay("ContentLow"));
                    }
                    break;
                case PlayerScore.entertained:
                    if (!getGameVariable("EntertainLowPlayed"))
                    {
                        setGameVariable("EntertainLowPlayed", true);
                        StartCoroutine(levelLoader.playCutsceneWithDelay("EntertainLow"));
                    }
                    break;
            }
        }

        if (newVal < 0.0f)
        {
            newVal = 0.0f;
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
    [YarnCommand("SetGameVariable")]
    public void setGameVariable(string variableName, bool val)
    {
        Debug.Log("Set the game variable " + variableName + " to " + val);
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
        if (gameVariables != null)
        {
            return gameVariables[gv];
        }
        else
        {
            Debug.Log("[ERROR] gameVariables is null!");
            return false;
        }
        
    }

    public GameVariable gameVariableToEnum(string name)
    {
        return (GameVariable)System.Enum.Parse(typeof(GameVariable), name);
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public GameObjectSave ISaveableSave()
    {
        // Delete existing scene save if exists
        GameObjectSave.sceneData.Remove(Settings.PersistentScene);

        // Create new scene save
        SceneSave sceneSave = new SceneSave();

        // Create new int dictionary
        sceneSave.intDictionary = new Dictionary<string, int>();
        sceneSave.intDictionary.Add("numSecondsAwake", numSecondsAwake);

        sceneSave.yarnfloatDictionary = new Dictionary<string, float>();

        sceneSave.yarnboolDictionary = new Dictionary<string, bool>();
        InMemoryVariableStorage imvs = FindObjectOfType<InMemoryVariableStorage>();

        Debug.Log("Saving Yarn Variables...");
        // https://discord.com/channels/754171172693868585/754171643990900776/1010222548849537024
        (var floats, var strings, var bools) = imvs.GetAllVariables();
        foreach (KeyValuePair<string, float> f in floats)
        {
            Debug.Log("[Yarn Float] " + f.Key + " " + f.Value);
            sceneSave.yarnfloatDictionary.Add(f.Key, f.Value);
        }

        // not being used
/*        foreach (KeyValuePair<string, string> s in strings)
        {
            Debug.Log("[Yarn String] " + s.Key + " " + s.Value);
            sceneSave.stringDictionary.Add(s.Key, s.Value);
        }*/
        foreach (KeyValuePair<string, bool> b in bools)
        {
            Debug.Log("[Yarn bool] " + b.Key + " " + b.Value);
            sceneSave.yarnboolDictionary.Add(b.Key, b.Value);
        }

        Debug.Log("Saving Game Variables...");
        sceneSave.gamevariables = new Dictionary<GameVariable, bool>();
        foreach (GameVariable gv in Enum.GetValues(typeof(GameVariable)))
        {
            sceneSave.gamevariables.Add(gv, gameVariables[gv]);
        }

        Debug.Log("Saving Player scores");
        sceneSave.playerscores = new Dictionary<PlayerScore, float>();
        foreach (PlayerScore ps in Enum.GetValues(typeof(PlayerScore)))
        {
            sceneSave.playerscores.Add(ps, playerScore[ps]);
        }

        // Save all this under persistent scene
        GameObjectSave.sceneData.Add(Settings.PersistentScene, sceneSave);

        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        Debug.Log("[ISaveableLoad GameState]");
        // Get saved gameobject from gameSave data
        if (gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;
            if (GameObjectSave.sceneData.TryGetValue(Settings.PersistentScene, out SceneSave sceneSave))
            {
                Debug.Log("Restoring Yarn Variables");
                // YARN VARIABLE RESOTRE
                InMemoryVariableStorage imvs = FindObjectOfType<InMemoryVariableStorage>();

                // take whatever they were previously from the save file and RESTORE it into the in memory storage of yarn!
                Dictionary<string, bool> yarnbools = sceneSave.yarnboolDictionary;
                Dictionary<string, float> yarnfloats = sceneSave.yarnfloatDictionary;
                Dictionary<string, string> strings = new Dictionary<string, string>();

                imvs.SetAllVariables(yarnfloats, strings, yarnbools);

                // GAME VARIABLE RESOTRE

                Debug.Log("Restoring Game Variables");
                foreach (KeyValuePair<GameVariable, bool> kvp in sceneSave.gamevariables)
                {
                    // just restore the values we have into the real dictionary
                    gameVariables[kvp.Key] = kvp.Value;
                }

                // PLAYER SCORE RESTORE
                foreach (KeyValuePair<PlayerScore, float> kvp in sceneSave.playerscores)
                {
                    // just restore the values we have into the real dictionary
                    playerScore[kvp.Key] = kvp.Value;
                }

                foreach (ScoreCategoryUI s in FindObjectsOfType<ScoreCategoryUI>())
                {
                    s.refreshScoreUI();
                }

                numSecondsAwake = sceneSave.intDictionary["numSecondsAwake"];
            }
        }

    }

    public void ISaveableStoreScene(string sceneName)
    {
        throw new NotImplementedException();
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        throw new NotImplementedException();
    }
}
