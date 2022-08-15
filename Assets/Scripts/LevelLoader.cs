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

public class LevelLoader : Singleton<LevelLoader>
{
    private bool isFading;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private CanvasGroup faderCanvasGroup = null;
    [SerializeField] private CinemachineVirtualCamera cam;
    [SerializeField] private Image faderImage = null;
    [SerializeField] List<SceneName> scenes;

    [SerializeField] List<float> orthoSizes;

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
    }
    [SerializeField] List<CutsceneCondtional> cutscenes;
    [SerializeField] GameState gameState;
    [SerializeField] Vector3 playerStartingPosition;

    private IEnumerator Start()
    {

        sceneToStartingOrthoSize = new Dictionary<SceneName, float>();
        CutscenesDict = new Dictionary<SceneName, List<CutsceneCondtional>>();

        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.transform.position = playerStartingPosition;

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

        // check if we will play any cutscenes for the starting scene
        CutsceneCondtional cutsceneToPlayOnLoad = areWePlayingCutscene(startingSceneName);

        // Start the first scene loading and wait for it to finish.
        yield return StartCoroutine(LoadSceneAndSetActive(startingSceneName));

        // If this event has any subscribers, call it.
        EventHandler.CallAfterSceneLoadEvent();

        // change camera to right place

        if (GameObject.FindGameObjectWithTag("MainCamera") == null)
        {
            Debug.Log("Maincamera null");
        }
        if (cam == null)
        {
            Debug.Log("Cam null");
        }


        Debug.Log("Starting Ortho Size: " + sceneToStartingOrthoSize[startingSceneName]);
        cam.m_Lens.OrthographicSize = sceneToStartingOrthoSize[startingSceneName];


        // Once the scene is finished loading, start fading in.
        StartCoroutine(Fade(0f));

        // play the starting cutscene if there is one
        if (cutsceneToPlayOnLoad != null)
        {
            cutsceneToPlayOnLoad.cutsceneToPlay.Play();
        }
    }

    // so this acts as a "new day" kinda thing.
    [YarnCommand("goToSleep")]
    public void goToSleep()
    {
        Debug.Log("Go to sleep");
    }

    // this just plays a cutscene, useful if we wanna do this from Yarn
    // also each time a scene is loaded it will check if any cutscenes need to be played
    // and it will go thru this as well.
    [YarnCommand("playCutscene")]
    public void playCutscene(string cutscene)
    {
        foreach (CutsceneCondtional c in cutscenes)
        {
            if (c.cutsceneToPlay.name == cutscene)
            {
                c.cutsceneToPlay.Play();
                break;
            }
        }
    }

    [YarnCommand("Fade")]
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

    private IEnumerator LoadSceneAndSetActive(SceneName sceneName)
    {

        // Allow the given scene to load over several frames and add it to the already loaded scenes (just the Persistent scene at this point).
        yield return SceneManager.LoadSceneAsync(sceneName.ToString(), LoadSceneMode.Additive);

        // Find the scene that was most recently loaded (the one at the last index of the loaded scenes).
        Scene newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

        // Set the newly loaded scene as the active scene (this marks it as the one to be unloaded next).
        SceneManager.SetActiveScene(newlyLoadedScene);

        // if it's our first time entering the scene, mark it as visited
        if (gameState.getGameVariable("hasEntered" + sceneName.ToString()) == false)
        {
            gameState.setGameVariable("hasEntered" + sceneName.ToString(), true);
        }
    }

    private CutsceneCondtional areWePlayingCutscene(SceneName sceneName)
    {
        foreach (CutsceneCondtional c in CutscenesDict[sceneName])
        {
            // skip cutscenes that we will only trigger via Yarn + playCutscene 
            if (c.isTriggeredCutscene == true)
            {
                continue;
            }
            // Debug.Log("Consindering " + c.cutsceneToPlay.ToString());
            bool isPlaying = true;
            if (gameState.getGameDay() != c.dayToPlay)
            {
                Debug.Log("Game day didn't match, value was " + gameState.getGameDay());
                isPlaying = false;
            }

            // check if each condition is met. if not, set isPlaying to false
            foreach (GameVariablePair gv in c.extraConditions)
            {
                if (gameState.getGameVariableEnum(gv.variable) != gv.desiredValue)
                {
                    Debug.Log("Broken on " + gv.variable.ToString());
                    isPlaying = false;
                    break;
                }
            }

            // if no conditions were broken, we are playing this cutscene. Return it.
            if (isPlaying == true)
            {
                return c;
            }
        }
        // if we can't find any cutscenes with all conditions met, then we're playing nothing
        // just return null
        return null;
    }

    // This is the coroutine where the 'building blocks' of the script are put together.
    private IEnumerator FadeAndSwitchScenes(SceneName sceneName, Vector3 spawnPosition)
    {
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        // stop player from moving once we hit scene switch point
        player.DisableMovementAndAnimations();

        player.GetComponent<SpriteRenderer>().enabled = false;

        // Call before scene unload fade out event
        EventHandler.CallBeforeSceneUnloadFadeOutEvent();

        // Start fading to black and wait for it to finish before continuing.
        yield return StartCoroutine(Fade(1.0f));

        CutsceneCondtional cutsceneToPlayOnLoad = areWePlayingCutscene(sceneName);

        // Set player position if no cutscene to play, (because if cutscene then it will
        // decide the starting location for us anyways)
        if (cutsceneToPlayOnLoad == null)
        {
            Player.Instance.gameObject.transform.position = spawnPosition;
        }

        //  Call before scene unload event.
        EventHandler.CallBeforeSceneUnloadEvent();

        // Unload the current active scene.
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);


        // Start loading the given scene and wait for it to finish.
        yield return StartCoroutine(LoadSceneAndSetActive(sceneName));

        // Call after scene load event
        EventHandler.CallAfterSceneLoadEvent();


        // Debug.Log("Starting position: " + sceneToStartingCamPos[sceneName]);
        cam.transform.position = Player.Instance.gameObject.transform.position;

        // Debug.Log("Starting Ortho Size: " + sceneToStartingOrthoSize[sceneName]);
        cam.m_Lens.OrthographicSize = sceneToStartingOrthoSize[sceneName];


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

        // play the starting cutscene if there is one
        if (cutsceneToPlayOnLoad != null)
        {
            cutsceneToPlayOnLoad.cutsceneToPlay.Play();
        }

    }


    // This is the main external point of contact and influence from the rest of the project.
    // This will be called when the player wants to switch scenes.
    public void FadeAndLoadScene(SceneName sceneName, Vector3 spawnPosition)
    {
        // If a fade isn't happening then start fading and switching scenes.
        if (!isFading)
        {
            Debug.Log("Loading new scene" + sceneName);
            StartCoroutine(FadeAndSwitchScenes(sceneName, spawnPosition));
        }
    }

    [YarnCommand("yarnLoadScene")]
    public void YarnLoadScene(string sceneName, float playerX, float playerY)
    {
        Vector3 playerSpawn = new Vector3(playerX, playerY, 0);
        FadeAndLoadScene((SceneName)System.Enum.Parse(typeof(SceneName), sceneName), playerSpawn);
    }

}
