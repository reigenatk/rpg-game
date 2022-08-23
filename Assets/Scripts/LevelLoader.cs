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

public class LevelLoader : Singleton<LevelLoader>
{
    public Vector3 defaultSceneLocation = new Vector3(-69.0f, -69.0f, 0.0f);
    private bool isFading;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private CanvasGroup faderCanvasGroup = null;
    [SerializeField] private CinemachineVirtualCamera cam;
    [SerializeField] private Image faderImage = null;
    [SerializeField] List<SceneName> scenes;
    [SerializeField] List<float> orthoSizes;
    [SerializeField] private GameObject players;
    [SerializeField] private TimeManager timeManager;

    // make dictionaries manually bc unity editor doesn't support them for whatever reason
    private Dictionary<SceneName, float> sceneToStartingOrthoSize;
    private Dictionary<SceneName, List<CutsceneCondtional>> CutscenesDict;

    public SceneName startingSceneName;


    // cutscene type
    [System.Serializable]
    public class CutsceneCondtional
    {
        // which cutscene it is
        public PlayableDirector cutsceneToPlay;
        // the day it must be
        public int dayToPlay;
        // the scene this will play on
        public SceneName scene;
        // will this cutscene be triggered via Yarn, or checked each time we load a scene?
        public bool isTriggeredCutscene;
        // some extra conditions that must be either true or false
        public List<GameVariablePair> extraConditions;

        public bool shouldPlayCutscene(SceneName scene)
        {
            GameState gameState = FindObjectOfType<GameState>();
            // skip cutscenes that we will only trigger via Yarn + playCutscene 
            if (isTriggeredCutscene == true)
            {
                return false;
            }
            // Debug.Log("Consindering " + c.cutsceneToPlay.ToString());

            bool isPlaying = true;
            if (gameState.getGameDay() != dayToPlay)
            {
                // Debug.Log("Game day didn't match, value was " + gameState.getGameDay());
                return false;
            }

            // check if each condition is met. if not, set isPlaying to false
            foreach (GameVariablePair gv in extraConditions)
            {
                if (gameState.getGameVariableEnum(gv.variable) != gv.desiredValue)
                {
                    // Debug.Log("Broken on " + gv.variable.ToString());
                    return false;
                }
            }
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

    // only difference between Start() and FadeAndLoadScene() is that, Start doesn't have to unload anything
    // and Start also has to initialize the dictionaries. Other than that they're essentially identical
    // (at least the loading scene part) so I put that into one function called CreateScene
    private IEnumerator Start()
    {

        sceneToStartingOrthoSize = new Dictionary<SceneName, float>();
        CutscenesDict = new Dictionary<SceneName, List<CutsceneCondtional>>();

        // get the default starting position for this scene
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
            CutscenesDict[c.scene].Add(c);
        }

        // Set the initial alpha to start off with a black screen.
        faderImage.color = new Color(0f, 0f, 0f, 1f);
        faderCanvasGroup.alpha = 1f;

        yield return StartCoroutine(CreateScene(startingSceneName, startingPosition));
    }

    // so this acts as a "new day" kinda thing.
    [YarnCommand("goToSleep")]
    public void goToSleep()
    {
        Debug.Log("Going to sleep");
        int curDay = gameState.getGameDay();
        gameState.setGameDay(curDay + 1);

        // notify yarn of this day change too, since which sleep dialogue we play depends on what day it is
        gameState.setYarnVariable("$day", curDay + 1);

        // pause the clock so we don't continue to leech energy, contentedness, etc.
        timeManager.gameClockPaused = true;

        // disable UI
        GameUI gameUI = FindObjectOfType<GameUI>();
        gameUI.disableUI();

        // set player animation state to face down
        FindObjectOfType<Player>().setAnimationState("Base Layer.Idle.IdleDown");

        // calculate time for new day and new energy/contentedness levels
        float sleepPenalty = timeManager.gt.gameHour * 10;
        Moods mood = gameState.calculateMood(-1.0f * sleepPenalty);
        switch (mood)
        {
            case Moods.Suicidal:
                timeManager.gt.advanceTime(10, 30);
                gameState.setPlayerScore(PlayerScore.energy, 60.0f);
                gameState.changePlayerScore(PlayerScore.contentedness, -5.0f);
                break;
            case Moods.Depressed:
                timeManager.gt.advanceTime(10, 0);
                gameState.setPlayerScore(PlayerScore.energy, 65.0f);
                gameState.changePlayerScore(PlayerScore.contentedness, -2.5f);
                break;
            case Moods.Unhinged:
                timeManager.gt.advanceTime(9, 30);
                gameState.setPlayerScore(PlayerScore.energy, 70.0f);
                gameState.changePlayerScore(PlayerScore.contentedness, 0.0f);
                break;
            case Moods.Average:
                timeManager.gt.advanceTime(9, 0);
                gameState.setPlayerScore(PlayerScore.energy, 80.0f);
                gameState.changePlayerScore(PlayerScore.contentedness, 2.5f);
                break;
            case Moods.Good:
                timeManager.gt.advanceTime(8, 30);
                gameState.setPlayerScore(PlayerScore.energy, 85.0f);
                gameState.changePlayerScore(PlayerScore.contentedness, 5.0f);
                break;
            case Moods.LovingLife:
                timeManager.gt.advanceTime(8, 0);
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

        endOfDayCutscene.stopped += cutsceneFinished;
    }

    private void cutsceneFinished(PlayableDirector stoppedDirector)
    {
        GameObject.Find("Bedsheets").GetComponent<SpriteRenderer>().sortingOrder = 0;
        FadeAndLoadScene(SceneName.DarkScene, defaultSceneLocation);
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
        foreach (CutsceneCondtional c in cutscenes)
        {
            if (c.cutsceneToPlay.name == cutscene)
            {
                // play it, also store that playing cutscene in gameState
                gameState.setCutscenePlaying(true);
                gameState.cutscenePlaying = c.cutsceneToPlay;
                FindObjectOfType<Player>().GetComponent<BoxCollider2D>().enabled = false;
                Debug.Log("Playing cutscene " + c.cutsceneToPlay.name);
                c.cutsceneToPlay.Play();
                return c.cutsceneToPlay;
            }
        }
        return null;
    }

    public IEnumerator Fade(float finalAlpha)
    {
        // Set the fading flag to true so the FadeAndSwitchScenes coroutine won't be called again.
        isFading = true;

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
    public void yarnFadeIn()
    {
        StartCoroutine(Fade(1.0f));
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
        foreach (CutsceneCondtional c in CutscenesDict[sceneName])
        {
            if (c.shouldPlayCutscene(sceneName))
            {
                return c.cutsceneToPlay.name;
            }
        }
        // if we can't find any cutscenes with all conditions met, then we're playing nothing
        // just return null
        return null;
    }

    // This is the main function that you should call to switch scene. It calls a bunch of helpers internally
    private IEnumerator FadeAndSwitchScenes(SceneName sceneName, Vector3 spawnPosition)
    {
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        // stop player from moving once we hit scene switch point
        player.DisableMovementAndAnimations();

        // Call before scene unload fade out event
        EventHandler.CallBeforeSceneUnloadFadeOutEvent();

        // unrender player sprite (after fade finishes!)
        player.GetComponent<SpriteRenderer>().enabled = false;

        // Start fading to black and wait for it to finish before continuing.
        yield return StartCoroutine(Fade(1.0f));

        //  Call before scene unload event.
        EventHandler.CallBeforeSceneUnloadEvent();

        // Unload the current active scene.
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        yield return StartCoroutine(CreateScene(sceneName, spawnPosition));

    }

    // a helper function to instantiate a scene
    public IEnumerator CreateScene(SceneName sceneName, Vector3 spawnPosition)
    {
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
        if (cutsceneToPlayOnLoad == null)
        {
            Player.Instance.gameObject.transform.position = spawnPosition;
        }

        // if it's our first time entering the scene, mark it as visited
        if (gameState.getGameVariable("hasEntered" + sceneName.ToString()) == false)
        {
            gameState.setGameVariable("hasEntered" + sceneName.ToString(), true);
        }

        // Debug.Log("Starting position: " + sceneToStartingCamPos[sceneName]);
        cam.transform.position = Player.Instance.gameObject.transform.position;

        // Debug.Log("Starting Ortho Size: " + sceneToStartingOrthoSize[sceneName]);
        cam.m_Lens.OrthographicSize = sceneToStartingOrthoSize[sceneName];


        // check if we need to load in any players, and if so load them in
        // if a cutscene is gonna play though, don't load them since the cutscene will do that
        if (cutsceneToPlayOnLoad == null)
        {
            LoadPlayers(sceneName);
        }

        // Start fading back in and wait for it to finish before exiting the function.
        yield return StartCoroutine(Fade(0f));

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


        // play some music if the scene demands it
        yield return StartCoroutine(FindObjectOfType<MusicManager>().playMusic(gameState.getGameDay(), sceneName));

        // play the starting cutscene if there is one
        if (cutsceneToPlayOnLoad != null)
        {
            playCutscene(cutsceneToPlayOnLoad);
        }
    }

    private void LoadPlayers(SceneName sceneName)
    {
        // for each player, check if we should load it into this scene
        foreach (Transform player in players.transform)
        {
            // Debug.Log("Considering to add player " + player.name);
            PlayerLoad pl = player.gameObject.GetComponent<PlayerLoad>();
            if (pl.checkAddPlayer(sceneName))
            {
                Debug.Log("Adding player " + player.name);
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


    // This is the main external point of contact and influence from the rest of the project.
    // This will be called when the player wants to switch scenes.
    public void FadeAndLoadScene(SceneName sceneName, Vector3 spawnPosition)
    {
        // If a fade isn't happening then start fading and switching scenes.
        if (!isFading)
        {
            Debug.Log("Loading new scene " + sceneName);
            if (spawnPosition == defaultSceneLocation)
            {
                // then use the default scene spawn point
                spawnPosition = getSceneStartingLocation(sceneName);
                Debug.Log("Spawn position is now " + spawnPosition);
            }
            StartCoroutine(FadeAndSwitchScenes(sceneName, spawnPosition));
            return;
        }
    }

    // used to go from black scene with just dialogue, to bedroom
    // also if we don't specify a specific location it will assume to use the default location
    [YarnCommand("yarnLoadScene")]
    public void YarnLoadScene(string sceneName, float playerX = -69.0f, float playerY = -69.0f)
    {
        Vector3 playerSpawn = new Vector3(playerX, playerY, 0);
        FadeAndLoadScene((SceneName)System.Enum.Parse(typeof(SceneName), sceneName), playerSpawn);
    }

}
