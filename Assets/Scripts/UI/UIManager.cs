using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class UIManager : Singleton<UIManager>
{
    // start game with pause off of course
    private bool _pauseMenuOn = false;


    [SerializeField] private GameObject pauseMenu = null;

    [SerializeField] private Button[] menuButtons = null;

    private GameState gameState;

    public bool PauseMenuOn { get => _pauseMenuOn; set => _pauseMenuOn = value; }

    protected override void Awake()
    {
        base.Awake();

        pauseMenu.SetActive(false);
        gameState = FindObjectOfType<GameState>();
    }

    // Update is called once per frame
    private void Update()
    {
        PauseMenu();
    }

    private void PauseMenu()
    {
        // Toggle pause menu if escape is pressed

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // if theres any dialogue or cutscenes playing, dont let player save
/*            if (gameState.cutscenePlaying != null || (gameState.currentRunningDialogueNode != "" && gameState.currentRunningDialogueNode != null))
            {

            }*/

            if (PauseMenuOn)
            {
                DisablePauseMenu();
            }
            else
            {
                EnablePauseMenu();
            }
        }

    }

    private void EnablePauseMenu()
    {


        PauseMenuOn = true;
        Player.Instance.disableMovement = true;
        TimeManager.Instance.pauseTime = true; // no time run when in menu

        pauseMenu.SetActive(true);

        // Trigger garbage collector
        System.GC.Collect();

    }

    public void DisablePauseMenu()
    {

        PauseMenuOn = false;
        Player.Instance.disableMovement = false;
        TimeManager.Instance.pauseTime = false; // unpause
        pauseMenu.SetActive(false);
    }


    private void SetButtonColorToActive(Button button)
    {
        ColorBlock colors = button.colors;

        colors.normalColor = colors.pressedColor;

        button.colors = colors;

    }

    private void SetButtonColorToInactive(Button button)
    {
        ColorBlock colors = button.colors;

        colors.normalColor = colors.disabledColor;

        button.colors = colors;

    }

    [YarnCommand("QuitGame")]
    public void QuitGame()
    {
        Application.Quit();
    }

}
