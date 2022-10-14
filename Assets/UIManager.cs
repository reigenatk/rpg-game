using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    // start game with pause off of course
    private bool _pauseMenuOn = false;


    [SerializeField] private GameObject pauseMenu = null;

    [SerializeField] private Button[] menuButtons = null;

    public bool PauseMenuOn { get => _pauseMenuOn; set => _pauseMenuOn = value; }

    protected override void Awake()
    {
        base.Awake();

        pauseMenu.SetActive(false);
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
        Time.timeScale = 0;
        pauseMenu.SetActive(true);

        // Trigger garbage collector
        System.GC.Collect();

    }

    public void DisablePauseMenu()
    {

        PauseMenuOn = false;
        Player.Instance.disableMovement = false;
        Time.timeScale = 1;
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

    public void QuitGame()
    {
        Application.Quit();
    }

}
