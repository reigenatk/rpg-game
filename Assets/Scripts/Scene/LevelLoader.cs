using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.Playables;

using System;
using static GameState;
using Yarn.Unity;
using System.Threading;
using static TimeManager;

public class LevelLoader : Singleton<LevelLoader>
{
    public Vector3 defaultSceneLocation = new Vector3(-69.0f, -69.0f, 0.0f);

    [SerializeField] private bool spawnWherePlaced = false; // spawn the player where he is in the scene? Debug only
    private bool isFirstLoad = true; // is this the first scene we load? Used to keep track of if spawnWherePlaced should take effect (should only spawn when placed on first scene)

    private bool isFading;
    public bool isFadingScenes; // are we in the process of switching scenes?
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private CanvasGroup faderCanvasGroup = null;
    [SerializeField] private GameObject vmCams;
    [SerializeField] private Image faderImage = null;
    [SerializeField] List<SceneName> scenes;
    [SerializeField] List<float> orthoSizes;


    // names of cutscenes that are waiting to be played after this one
    public Stack<string> cutsceneQueue;

    [SerializeField] public GameObject nonnpcs;
    private TimeManager timeManager;
    [SerializeField] public GameObject npcs;


    // for my ease of use- when debugging I dont wanna see cutscenes sometimes
    public bool cutscenesEnabled = true;
    public bool dialoguesEnabled = true;

    // make dictionaries manually bc unity editor doesn't support them for whatever reason
    private Dictionary<SceneName, float> sceneToStartingOrthoSize;
    private Dictionary<SceneName, List<CutsceneCondtional>> CutscenesDict;

    // manually keep track of which scene we on
    public SceneName curScene;

    // this helps us to keep track of which scene we should load after the cutscene finishes (for a cutscene + new scene pattern)
    SceneName nextSceneToLoad;

    public bool firstSceneLoaded; // set true when first scene is done loadin

    // cutscene type
    [System.Serializable]
    public class CutsceneCondtional
    {
        // which cutscene it is
        public PlayableDirector cutsceneToPlay;
        // the day it must be. Set to -1 if it can play any day
        public int dayToPlay;
        // the days >= this day will work as well
        public int dayToPlayGEQ;
        // the scene this will play on
        public SceneName scene;
        // will this cutscene be triggered via Yarn, or checked each time we load a scene?
        public bool isTriggeredCutscene;
        // some extra conditions that must be either true or false
        public List<GameVariablePair> extraConditions;

        public int priority = 0; // higher = more prio

        // this is for editor debugging purposes so I can set the isTriggered stuff properly and then use this bool to turn on and off
        public bool shouldActivate;
        public string custsceneDescription; // for my own use

        // valid times (if empty ignore)
        [SerializeField] List<ChunkOfTime> validTimes;
        [SerializeField] List<YarnVariablePair> YarnVariables;
        public bool shouldPlayCutscene(SceneName scene)
        {
            if (!shouldActivate) return false;

            GameState gameState = FindObjectOfType<GameState>();
            // skip cutscenes that we will only trigger via Yarn + playCutscene 
            if (isTriggeredCutscene == true)
            {
                return false;
            }
            Debug.Log("Consindering " + this.cutsceneToPlay.ToString());

            int theDay = gameState.getGameDay();
            // here are both fail cases... either
            // dayToPlay isnt -1, and the day doesnt match, and there is no GEQ day (as indicated by it being 0)
            // or it doesnt match days, ok, then we check the GEQ instance (which isn't 0), and it still doesnt work, then it also fails.
            // otherwise its ok
            if (((theDay != dayToPlay) && (dayToPlay != -1) && (dayToPlayGEQ == 0)))
            {
                Debug.Log("[LevelLoader] Game day didn't match, value was " + gameState.getGameDay());
                return false;
            }

            if ((theDay < dayToPlayGEQ) && (dayToPlayGEQ != 0))
            {
                Debug.Log("[LevelLoader] Game day GEQ didn't match, value was " + gameState.getGameDay());
                return false;
            }

            if (validTimes.Count != 0)
            {
                foreach (ChunkOfTime c in validTimes)
                {
                    // must find at least one valid chunk of time. If none we will return false
                    if (c.isInChunk(TimeManager.Instance.gt))
                    {
                        goto foundValidTime;
                    }
                }
                return false;
            }

            foundValidTime:
            // check if each condition is met. if not, set isPlaying to false
            foreach (GameVariablePair gv in extraConditions)
            {
                if (gameState.getGameVariableEnum(gv.variable) != gv.desiredValue)
                {
                    Debug.Log("[LevelLoader] Broken on " + gv.variable.ToString());
                    return false;
                }
            }
            foreach (YarnVariablePair yv in YarnVariables)
            {
                if (gameState.getYarnVariable(yv.YarnVariable) != yv.desiredValue)
                {
                    Debug.Log("[LevelLoader] Broken on yarn variable " + yv.YarnVariable.ToString() + " value was " + gameState.getYarnVariable(yv.YarnVariable) + " expected value is " + yv.desiredValue);
                    return false;
                }
            }
            Debug.Log("Cutscene " + cutsceneToPlay.ToString() + "works..");
            return true;
        }
    }
    [SerializeField] List<CutsceneCondtional> cutscenes;
    [SerializeField] GameState gameState;

    [System.Serializable]
    public class SceneStartingPositions {
        public SceneName scene;
        public Vector3 startingPosition;
    }
    [SerializeField] List<SceneStartingPositions> sceneStartingPositions;

    public Vector3 getSceneStartingLocation(SceneName s)
    {
        foreach (SceneStartingPositions ss in sceneStartingPositions)
        {
            if (ss.scene == s)
            {

                return ss.startingPosition;
            }
        }
        // shouldn't get to this point, since in the editor we should give each scene its own starting default spot
        return Vector3.zero;
    }

    // https://answers.unity.com/questions/1305859/unload-all-scenes-except-one.html
    IEnumerator UnloadAllScenesExcept(string sceneName)
    {
        int c = SceneManager.sceneCount;
        print("There are " + c + " scenes loaded");
        List<Scene> scenesToUnload = new List<Scene>();
        for (int i = 0; i < c; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            print("Scene name is " + scene.name);
            if (scene.name != sceneName)
            {
                print("Unloading " + scene.name);
                scenesToUnload.Add(scene);
            }
        }

        foreach (Scene s in scenesToUnload)
        {
            yield return SceneManager.UnloadSceneAsync(s);
        }
    }

    // only difference between Start() and FadeAndLoadScene() is that, Start doesn't have to unload anything
    // and Start also has to initialize the dictionaries. Other than that they're essentially identical
    // (at least the loading scene part) so I put that into one function called CreateScene
    private IEnumerator Start()
    {
        timeManager = FindObjectOfType<TimeManager>();
        sceneToStartingOrthoSize = new Dictionary<SceneName, float>();
        CutscenesDict = new Dictionary<SceneName, List<CutsceneCondtional>>();

        // get the default starting position for this scene
        SceneName startingSceneName = FindObjectOfType<GameState>().startingScene;
        Vector3 startingPosition = getSceneStartingLocation(startingSceneName);

        // make the dictionary because untiy can't make dictionaries
        for (int i = 0; i < scenes.Count; i++)
        {

            sceneToStartingOrthoSize.Add(scenes[i], orthoSizes[i]);
            CutscenesDict.Add(scenes[i], new List<CutsceneCondtional>());
        }

        // distribute each cutscene into the scene in which it should play
        foreach (CutsceneCondtional c in cutscenes)
        {
            Debug.Log("Adding cutscene with name " + c.cutsceneToPlay.name + " to scene with key " + c.scene.ToString());
            CutscenesDict[c.scene].Add(c);
        }

        // Set the initial alpha to start off with a black screen.
        faderImage.color = new Color(0f, 0f, 0f, 1f);
        faderCanvasGroup.alpha = 1f;

        // Unload the scene we're gonna load in, if its already loaded
        Debug.Log("Starting scene to load is " + startingSceneName.ToString());
        StartCoroutine(UnloadAllScenesExcept("PersistantScene"));

        curScene = startingSceneName;

        yield return StartCoroutine(CreateScene(startingSceneName, startingPosition, 1.0f));
        firstSceneLoaded = true;
        FindObjectOfType<MapBounds>().DoSwitchBoundingShape(); // do this manually cuz its not working for wahtever reason
    }

    // call this when player faints. More or less just goToSleep() but with less stuff
    public void faint()
    {
        Debug.Log("Player fainted");

        advanceDay();
        endOfDayCutsceneFinished();
    }

    public void advanceDay()
    {
        gameState.gameStateAdvanceDay();

        FindObjectOfType<GameState>().resetDailyYarnVariables();
        // pause the clock so we don't continue to leech energy, contentedness, etc.
        timeManager.gameClockPaused = true;

        gameState.numSecondsAwake = 0; // back to 0

        // disable UI
        GameUI gameUI = FindObjectOfType<GameUI>();
        gameUI.disableUI();
    }

    // so this acts as a "new day" kinda thing. Also doubles as our "faint" animation
    [YarnCommand("goToSleep")]
    public void goToSleep()
    {
        Debug.Log("Going to sleep");

        advanceDay();


        // calculate time for new day and new energy/contentedness levels
        float sleepPenalty = timeManager.gt.gameHour * 10;
        Moods mood = gameState.calculateMood(-1.0f * sleepPenalty);
        gameState.numSecondsAwake = 0; // back to 0 sec awake
        gameState.setGameVariableEnum(GameVariable.canPlayerSleep, false);  
        switch (mood)
        {
            case Moods.Suicidal:
                timeManager.gt = new GameTime(13, 0, 0); // 1pm wake
                gameState.setPlayerScore(PlayerScore.energy, 60.0f);
                gameState.changePlayerScore(PlayerScore.contentedness, -5.0f);
                break;
            case Moods.Depressed:
                timeManager.gt = new GameTime(12, 0, 0); // 12pm wake
                gameState.setPlayerScore(PlayerScore.energy, 65.0f);
                gameState.changePlayerScore(PlayerScore.contentedness, -2.5f);
                break;
            case Moods.Unhinged:
                timeManager.gt = new GameTime(11, 0, 0); // 11am wake
                gameState.setPlayerScore(PlayerScore.energy, 70.0f);
                gameState.changePlayerScore(PlayerScore.contentedness, 0.0f);
                break;
            case Moods.Average:
                timeManager.gt = new GameTime(10, 0, 0); // 10am wake
                gameState.setPlayerScore(PlayerScore.energy, 80.0f);
                gameState.changePlayerScore(PlayerScore.contentedness, 2.5f);
                break;
            case Moods.Good:
                timeManager.gt = new GameTime(9, 0, 0); // 10am wake
                gameState.setPlayerScore(PlayerScore.energy, 85.0f);
                gameState.changePlayerScore(PlayerScore.contentedness, 5.0f);
                break;
            case Moods.LovingLife:
                timeManager.gt = new GameTime(8, 0, 0); // 10am wake
                gameState.setPlayerScore(PlayerScore.energy, 90.0f);
                gameState.changePlayerScore(PlayerScore.contentedness, 7.5f);
                break;
        }


        // change the bedsheets to be on top of player
        GameObject.Find("Bedsheets").GetComponent<SpriteRenderer>().sortingOrder = 1;

        // if the lamp is on, play a cutscene where he closes it first then goes to sleep
        bool isLampOn = gameState.getYarnVariable("$isBedroomLampOn");
        PlayableDirector endOfDayCutscene = null;
        if (isLampOn)
        {
            Debug.Log("Bedroom lamp is on, closing it " + isLampOn);
            endOfDayCutscene = playCutscene("CloseLampLieInBed");
        }
        else
        {
            endOfDayCutscene = playCutscene("LieInBed");
        }

        // endOfDayCutscene.stopped += endOfDayCutsceneFinished;
    }

    public void endOfDayCutsceneFinished()
    {
        Debug.Log("[endOfDayCutsceneFinished]");
        if (SceneManager.GetActiveScene().name == "Bedroom")
            GameObject.Find("Bedsheets").GetComponent<SpriteRenderer>().sortingOrder = 0;

        if (gameState.gameDay == 1)
        {
            // if day 0 going to day 1
            Debug.Log("Going to Dream0");
            FadeAndLoadScene(SceneName.DreamDay0, defaultSceneLocation);
        }
        else if (gameState.gameDay >= 2 && gameState.gameDay <= 7)
        {
            // day 1->2 is dream 1
            // 2->3 is dream 2
            // 3->4 is dream 3
            // 4->5 is dream 4
            // 5->6 is dream 5
            // 6->7 is go to dream world, talk to reaper, and then wakeup again
            Debug.Log("Going to DarkScene");
            // otherwise go to dark scene if we between days 2-7 (aka when the reaper hasnt finished his dreams)
            FadeAndLoadScene(SceneName.DarkScene, defaultSceneLocation);
        }
        else
        {
            Debug.Log("[SleepAndThenWakeup]");
            // otherwise, play some sleep sounds and then wakeup
            StartCoroutine(SleepAndThenWakeup());
        }

    }

    // a simple sequence to just play snoring sounds then wakeup (use when done with all dreams)
    public IEnumerator SleepAndThenWakeup()
    {
        Fade(1.0f, 1.0f);
        yield return new WaitForSeconds(2.0f);
        AudioManager.Instance.PlaySound(SoundItem.SoundName.SnoringSounds);
        yield return new WaitForSeconds(5.0f);
        Fade(0.0f, 1.0f);
        yield return new WaitForSeconds(2.0f);
        wakeUp();
    }

    /*    [YarnCommand("leaveHouse")] 
        public void leaveHouse()
        {
            PlayableDirector endOfDayCutscene = playCutscene("D2_Commons_OpenDoor");

        }*/

    [YarnCommand("playCutsceneAndFade")]
    public void playCutsceneAndFade(string cutsceneName, string sceneToFadeTo)
    {
        Debug.Log("PlayCutsceneAndFade to play " + cutsceneName + " received");
        // end whatever dialogue is currently playing (if any), since we could've came from a playCutscene call in Yarn
        FindObjectOfType<DialogueManager>().DialogueNodeFinishedPlaying();

        PlayableDirector cutsceneObj = playCutscene(cutsceneName);
        cutsceneObj.stopped += fadeAfterCutsceneFinishes;
        nextSceneToLoad = (SceneName)Enum.Parse(typeof(SceneName), sceneToFadeTo);
    }

    public void fadeAfterCutsceneFinishes(PlayableDirector stoppedDirector)
    {
        FadeAndLoadScene(nextSceneToLoad, defaultSceneLocation);
    }

    [YarnCommand("WakeUp")]
    public void wakeUp()
    {

        Debug.Log("Waking up");
        timeManager.gameClockPaused = false;
        GameUI gameUI = FindObjectOfType<GameUI>();
        gameUI.enableUI();
        FindObjectOfType<Player>().setAnimationState("Base Layer.Idle.IdleDown");

        FadeAndLoadScene(SceneName.Bedroom, defaultSceneLocation, 2.0f);
        playCutscene("WakeUp");
    }



    // helper method for when there's a new day and we gotta reset all game variables for the next day
    // this doesnt impact all variables- only the ones that are day dependent.
    public void gameVariablesReset()
    {
        // reset game variables too
        List<GameVariable> variablesToChangeToFalse = new List<GameVariable>();
        foreach (KeyValuePair<GameVariable, bool> gv in gameState.gameVariables)
        {

            if (gv.Key.ToString().Substring(0, 3) == "has" || gv.Key.ToString() == "canPlayerSleep")
            {
                variablesToChangeToFalse.Add(gv.Key);
            }
        }
        // set all the hasEntered gamevariables to false again
        // since its a new day, none of the rooms has been visited for this day.
        foreach (GameVariable gv in variablesToChangeToFalse)
        {
            gameState.gameVariables[gv] = false;
        }

        // change all necessary variables inside of yarn 

        // teeth back to being unbrushed for next day
        gameState.yarnVariables.SetValue("$teethBrushed", false);
    }

    // this just plays a cutscene (WITHOUT CHECKING ANYTHING like day and such), useful if we wanna do this from Yarn
    // also each time a scene is loaded it will check if any cutscenes need to be played
    // and it will go thru this as well.
    [YarnCommand("playCutscene")]
    public PlayableDirector playCutscene(string cutscene)
    {

        Debug.Log("Request received to play cutscene named " + cutscene);
        foreach (CutsceneCondtional c in cutscenes)
        {
            if (c.cutsceneToPlay.name == cutscene)
            {
                // play it, also store that playing cutscene in gameState
                return playCutsceneInternal(c.cutsceneToPlay);
            }
        }


        // else ok there really is no cutscene with this name
        return null;
    }

    public IEnumerator playCutsceneWithDelay(string cutscene)
    {
        Debug.Log("[playCutsceneWithDelay] named " + cutscene);
        foreach (CutsceneCondtional c in cutscenes)
        {
            if (c.cutsceneToPlay.name == cutscene)
            {
                // wait till whatever is playing is done, then call the internal version which will kill any currently running cutscene and play our one
                if (gameState.cutscenePlaying != null)
                {
                    yield return 0; // wait a frame
                    // yield return new WaitForSeconds(1);
                }
                playCutsceneInternal(c.cutsceneToPlay);
                yield break;
            }
        }


        // else ok there really is no cutscene with this name
        yield break;
    }

    // helper for above function
    public PlayableDirector playCutsceneInternal(PlayableDirector cutscene)
    {


        Debug.Log("[playCutsceneInternal] " + cutscene.name);
        if (gameState.cutscenePlaying == cutscene)
        {
            // to stop this weird bug where a cutscene runs twice of the same name???
            return null;
        }
        // pause time (OK EDIT IDK WHY TF THIS ISNT WORKING so I just made it check in TimeManager instead)
        // TimeManager.Instance.gameClockPaused = true;

        // play it, also store that playing cutscene in gameState
        gameState.setIsCutscenePlaying(true);

        GameUI.Instance.disableUI();

        Debug.Log("Disabling sprite renderers on NPC");
        // disable all NPC characters
        foreach (SpriteRenderer sr in npcs.GetComponentsInChildren<SpriteRenderer>())
        {
            // Debug.Log("Disabling sprite renderer on NPC " + sr.name);
            sr.enabled = false;
            // Debug.Log(sr.enabled);
        }
        Debug.Log("Enabling sprite renderers on non NPC");
        // enable all Non NPC characters
        foreach (SpriteRenderer sr2 in nonnpcs.GetComponentsInChildren<SpriteRenderer>())
        {
            // Debug.Log("Enabling sprite renderer on non-NPC " + sr2.name);
            sr2.enabled = true;
            // Debug.Log(sr2.enabled);
        }

        // disable audio coming from the NPCs
        foreach (AudioSource asr in npcs.GetComponentsInChildren<AudioSource>())
        {
            asr.enabled = false;
        }

        // stop any already playing cutscenes!
        if (gameState.cutscenePlaying != null)
        {
            Debug.Log("Cutscene was already playing called " + gameState.cutscenePlaying + ", stopping it");
            gameState.cutscenePlaying.Stop();

        }

        // set the current cutscene playing to the new one
        gameState.cutscenePlaying = cutscene;

        // make sure player sprite renderer enabled. Could be disabled from things if it was played via the CreateScene() call
        // since that call disables the player sprite renderer if a cutscene is gonna play right when the scene first loads.
        Player.Instance.GetComponent<SpriteRenderer>().enabled = true;
        FindObjectOfType<Player>().GetComponent<BoxCollider2D>().enabled = false;

        cutscene.Play();
        return cutscene;
    }

    public IEnumerator Fade(float finalAlpha, float time = 1.0f)
    {
        // StopCoroutine("Fade"); // stop any other instances of Fade to prevent infinite loops

        Debug.Log("Fade called to alpha " + finalAlpha + " over time " + time);
        // Set the fading flag to true so the FadeAndSwitchScenes coroutine won't be called again.
        isFading = true;
        faderImage.color = new Color(0f, 0f, 0f, 1f);

        // Make sure the CanvasGroup blocks raycasts into the scene so no more input can be accepted.
        faderCanvasGroup.blocksRaycasts = true;

        // Calculate how fast the CanvasGroup should fade based on it's current alpha, it's final alpha and how long it has to change between the two.
        float fadeSpeed = Mathf.Abs(faderCanvasGroup.alpha - finalAlpha) / fadeDuration;

        // While the CanvasGroup hasn't reached the final alpha yet...
        while (!Mathf.Approximately(faderCanvasGroup.alpha, finalAlpha))
        {
            // ... move the alpha towards it's target alpha.
            faderCanvasGroup.alpha = Mathf.MoveTowards(faderCanvasGroup.alpha, finalAlpha,
                fadeSpeed * Time.deltaTime);

            // Wait for a frame then continue.
            yield return null;
        }

        // Set the flag to false since the fade has finished.
        isFading = false;

        // Stop the CanvasGroup from blocking raycasts so input is no longer ignored.
        faderCanvasGroup.blocksRaycasts = false;
    }

    [YarnCommand("FadeIn")]
    public void FadeIn(float time = 1.0f)
    {

        Debug.Log("Fading In " + time + " seconds");
        StartCoroutine(Fade(1.0f, time));
    }

    [YarnCommand("FadeToValue")]
    public void FadeToValue(float opacity)
    {
        Debug.Log("Fading to opacity " + opacity);
        StartCoroutine(Fade(opacity, 1.0f));
    }

    [YarnCommand("FadeOut")]
    public void FadeOut(float time = 1.0f)
    {
        Debug.Log("Fading Out" + time + " seconds");
        StartCoroutine(Fade(0.0f, time));
    }

    private IEnumerator LoadSceneAndSetActive(SceneName sceneName)
    {

        // Allow the given scene to load over several frames and add it to the already loaded scenes (just the Persistent scene at this point).
        yield return SceneManager.LoadSceneAsync(sceneName.ToString(), LoadSceneMode.Additive);

        // Find the scene that was most recently loaded (the one at the last index of the loaded scenes).
        Scene newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

        // Set the newly loaded scene as the active scene (this marks it as the one to be unloaded next).
        SceneManager.SetActiveScene(newlyLoadedScene);
    }

    private string areWePlayingCutscene(SceneName sceneName)
    {
        Debug.Log("Checking if we should play any cutscenes...");
        if (cutscenesEnabled == false) return null;
        Debug.Log("Cutscenes are enabled, keep looking. The cutscene dict for scene " + sceneName + " is of size " + CutscenesDict[sceneName].Count);
        int curPriority = -2000;
        string cutsceneToPlay = null;
        foreach (CutsceneCondtional c in CutscenesDict[sceneName])
        {
            if (c.shouldPlayCutscene(sceneName))
            {
                
                if (c.priority > curPriority)
                {
                    Debug.Log("Found a cutscene to play with prio " + c.priority + " called " + c.cutsceneToPlay.name);
                    curPriority = c.priority;
                    cutsceneToPlay = c.cutsceneToPlay.name;
                }
            }
        }
        // if we can't find any cutscenes with all conditions met, then we're playing nothing
        // just return null
        
        if (cutsceneToPlay == null)
        {
            Debug.Log("No cutscenes necessary to play");
            return null;
        }
        else
        {
            Debug.Log("Playing cutscene " + cutsceneToPlay + " with prio " + curPriority);
            return cutsceneToPlay;
        }

    }

    // This is the main function that you should call to switch scene. It calls a bunch of helpers internally
    private IEnumerator FadeAndSwitchScenes(SceneName sceneName, Vector3 spawnPosition, float delay, AudioClip clipToPlay = null)
    {
        isFadingScenes = true;
        // stop game time!
        FindObjectOfType<TimeManager>().gameClockPaused = true;

        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        // stop player from moving once we hit scene switch point
        player.DisableMovementAndAnimations();

        // Call before scene unload fade out event
        EventHandler.CallBeforeSceneUnloadFadeOutEvent();

        // unrender player sprite (after fade finishes!)
        // there are exceptions, for instance, in the bus cutscene we dont want player to immediately show up
        player.GetComponent<SpriteRenderer>().enabled = false;


        // Start fading to black and wait for it to finish before continuing.
        yield return StartCoroutine(Fade(1.0f, 0.1f));

        Debug.Log("After Fade");
        // play optional sound!
        if (clipToPlay != null)
        {
            Debug.Log("Playing sound effect between scenes: " + clipToPlay.name);
            GameObject.Find("Audio").GetComponent<AudioSource>().PlayOneShot(clipToPlay);

        }


        //  Call before scene unload event.
        EventHandler.CallBeforeSceneUnloadEvent();

        // Unload the current active scene.
        Debug.Log("Unload the current active scene");

        yield return UnloadAllScenesExcept("PersistantScene");

        Debug.Log("Loading in scene " + sceneName);





        yield return StartCoroutine(CreateScene(sceneName, spawnPosition, delay));

    }

    public bool shouldHaveTime()
    {
        switch (curScene)
        {
            case SceneName.DreamDay0:
            case SceneName.DreamDay1:
            case SceneName.DreamDay2:
            case SceneName.DreamDay3:
            case SceneName.DreamDay4:
            case SceneName.DreamDay5:
            case SceneName.DreamRoom:
            case SceneName.DreamHome:
            case SceneName.DarkScene:
            case SceneName.ActualDarkScene:
            case SceneName.Menu:
                return false;
            default:
                return true;
        }
    }

    public bool isDreamScene()
    {
        switch (curScene)
        {
            case SceneName.ActualDarkScene:
                return true;
            default:
                return false;
        }
    }

    // a helper function to instantiate a scene
    public IEnumerator CreateScene(SceneName sceneName, Vector3 spawnPosition, float delay)
    {
        Debug.Log("[CreateScene] " + sceneName);
        // Start loading the given scene and wait for it to finish.
        yield return StartCoroutine(LoadSceneAndSetActive(sceneName));

        // Call after scene load event
        EventHandler.CallAfterSceneLoadEvent();

        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        string cutsceneToPlayOnLoad = areWePlayingCutscene(sceneName);

        // Set player position if no cutscene to play, (because if cutscene then it will
        // decide the starting location for us anyways)
        // also we make sure to put this BEFORE setting the gameVariable as sometimes cutscenes will depend
        // on whether or not we have entered a room already

        if (spawnWherePlaced && isFirstLoad == true)
        {
            // keep player where he's at
            isFirstLoad = false;

        }
        else
        {
            Debug.Log("Spawning player in Position " + spawnPosition);
            Player.Instance.gameObject.transform.position = spawnPosition;
        }


        // make player invis if cutscene is gonna play
        if (cutsceneToPlayOnLoad == null)
        {
            Player.Instance.GetComponent<SpriteRenderer>().enabled = false;
        }

            // if it's our first time entering the scene, mark it as visited
            if (gameState.getGameVariable("hasEntered" + sceneName.ToString()) == false)
        {
            gameState.setGameVariable("hasEntered" + sceneName.ToString(), true);
        }

        // alter all cameras to have the specified ortho size and position
        CinemachineVirtualCamera[] allCams = vmCams.GetComponentsInChildren<CinemachineVirtualCamera>();
        foreach (CinemachineVirtualCamera cam in allCams)
        {
            // Debug.Log("Starting position: " + sceneToStartingCamPos[sceneName]);
            cam.transform.position = Player.Instance.gameObject.transform.position;

            // Debug.Log("Setting Ortho Size: " + sceneToStartingOrthoSize[sceneName]);
            cam.m_Lens.OrthographicSize = sceneToStartingOrthoSize[sceneName];
        }

        // SCENE SPECIFIC CHECKS

        // add an artifical delay of extra time if desired
        if (delay != 0.0f)
        {
            Debug.Log("Delaying an artificial " + delay + "seconds");
            yield return new WaitForSeconds(delay);
        }

        Debug.Log("[CreateScene] 2");


        // Call after scene load fade in event
        EventHandler.CallAfterSceneLoadFadeInEvent();

        // wait till fade finishes completely before enabling momvements and showing player again
        while (true)
        {
            if (isFading == false)
            {
                player.EnableMovementAndAnimations();
                break;
            }
        }

        player.GetComponent<SpriteRenderer>().enabled = true;
        player.EnableMovementAndAnimations();

        // set the current scene equal to the new one
        curScene = sceneName;

        // Start fading back in and wait for it to finish before exiting the function.
        // dont fade in if its the darkscene since we want the darkscene to stay dark.
        // We can then fade in at will using the yarn commands
        // DEPENDS ON LINE ABOVE!!
        if (!isDreamScene())
        {
            yield return StartCoroutine(Fade(0f));
        }

        // play some music if the scene demands it
        yield return StartCoroutine(FindObjectOfType<MusicManager>().playMusic(gameState.getGameDay(), sceneName));

        // play the starting cutscene if there is one
        if (cutsceneToPlayOnLoad != null)
        {
            playCutscene(cutsceneToPlayOnLoad);
        }

        // start game time!
        FindObjectOfType<TimeManager>().gameClockPaused = false;
        isFadingScenes = false;
    }

    public SceneName getCurScene()
    {
        return (SceneName)System.Enum.Parse(typeof(SceneName), SceneManager.GetActiveScene().name);
    }

/*    private void LoadPlayers(SceneName sceneName)
    {
        // for each player, check if we should load it into this scene
        foreach (PlayerLoad pl in nonnpcs.gameObject.transform.GetComponentsInChildren<PlayerLoad>())
        {
            // Debug.Log("Considering to add player " + player.name);
            if (pl.checkAddPlayer(sceneName))
            {
                Debug.Log("Adding player " + pl.gameObject);
                // if we should add player to this scene, then instantiate a copy of it and move it to that scene
                // this would assume the scene (to put the player in) has already been loaded
                GameObject newPlayer = Instantiate(pl.player);

                // keep the same name, no (Clone) business
                newPlayer.name = pl.player.name;
                SceneManager.MoveGameObjectToScene(newPlayer, SceneManager.GetSceneByName(sceneName.ToString()));
                GameObject playersObject = GameObject.Find("Players");
                newPlayer.transform.parent = playersObject.transform;
                newPlayer.SetActive(true);
            }
        }
    }
*/

    // This is the main external point of contact and influence from the rest of the project.
    // This will be called when the player wants to switch scenes.
    // the "delay" btw is some extra time that I will add when the first scene has unloaded but the second scene has not yet loaded
    // so its like how long there's just a black screen. I'm adding this for some extra effect, for example when he walks between important scenes
    // we can make it stay on the black screen a bit longer. By default this value is zero
    public void FadeAndLoadScene(SceneName sceneName, Vector3 spawnPosition, float delay = 0.0f, AudioClip clipToPlay = null)
    {
        // If a fade isn't happening then start fading and switching scenes.
        if (!isFadingScenes)
        {
            Debug.Log("Loading new scene " + sceneName);
            if (spawnPosition == defaultSceneLocation)
            {
                // then use the default scene spawn point
                spawnPosition = getSceneStartingLocation(sceneName);
                Debug.Log("Spawn position is now " + spawnPosition);
            }
            StartCoroutine(FadeAndSwitchScenes(sceneName, spawnPosition, delay, clipToPlay));
            return;
        }
    }

    // used to go from black scene with just dialogue, to bedroom
    // also if we don't specify a specific location it will assume to use the default location
    [YarnCommand("yarnLoadScene")]
    public void YarnLoadScene(string sceneName, float delay, float playerX = -69.0f, float playerY = -69.0f)
    {
        Vector3 playerSpawn = new Vector3(playerX, playerY, 0);
        FadeAndLoadScene((SceneName)System.Enum.Parse(typeof(SceneName), sceneName), playerSpawn, delay);
    }

    public bool isNPCMovementScene()
    {
        switch (curScene)
        {
            case SceneName.DreamDay0:
            case SceneName.DreamDay1:
            case SceneName.DreamDay2:
            case SceneName.DreamDay3:
            case SceneName.DreamDay4:
            case SceneName.DreamDay5:
            case SceneName.DarkScene:
            case SceneName.ActualDarkScene:
            case SceneName.Menu:
                return false;
            default:
                return true;

        }
    }
    public void NewGame()
    {

        FadeAndLoadScene(SceneName.ActualDarkScene, defaultSceneLocation);
    }
}
