using TMPro;
using UnityEngine;


public class GameClock : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText = null;
    [SerializeField] private TextMeshProUGUI dateText = null;



    private void OnEnable()
    {
        EventHandler.AdvanceGameMinuteEvent += UpdateGameTime;
    }

    private void OnDisable()
    {
        EventHandler.AdvanceGameMinuteEvent -= UpdateGameTime;
    }

    private void UpdateGameTime(string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        // Update time in increments of 10 minutes

        gameMinute = gameMinute - (gameMinute % 10);

        string ampm = "";
        string minute;

        if (gameHour >= 12)
        {
            ampm = " pm";
        }
        else
        {
            ampm = " am";
        }

        if (gameHour >= 13)
        {
            gameHour -= 12;
        }

        if (gameMinute < 10)
        {
            minute = "0" + gameMinute.ToString();
        }
        else
        {
            minute = gameMinute.ToString();
        }
        string gameHourString = gameHour.ToString();
        if (gameHour == 0)
        {
            gameHourString = "12";
        }
        string time = gameHourString + " : " + minute + ampm;


        timeText.SetText(time);
        dateText.SetText(gameDayOfWeek + ". " + FindObjectOfType<GameState>().getGameDay());
    }

}
