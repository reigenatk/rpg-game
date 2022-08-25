using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TimeManager : Singleton<TimeManager>
{
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
    

    private int totalGameSeconds = 0;
    private string gameDayOfWeek = "Mon";
    public bool gameClockPaused = false;
    private float gameTick = 0f;

    // how many in game seconds correspond to one decayed percent of each category
    [SerializeField] private GameState gameState;
    [SerializeField] private int energyDecayRate;
    [SerializeField] private int socialDecayRate;
    [SerializeField] private int contentednessDecayRate;
    [SerializeField] private int entertainmentDecayRate;
    [SerializeField] Light2D globalLight;
    [SerializeField] List<ColorPoint> globalLightColors;


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
        gt = new GameTime(16, 30, 0);
        EventHandler.CallAdvanceGameMinuteEvent(gameDayOfWeek, gt.gameHour, gt.gameMinute, gt.gameSecond);
    }

    private void Update()
    {
        if (!gameClockPaused)
        {
            GameTick();
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


    private void UpdateGameSecond()
    {
        gt.gameSecond++;
        totalGameSeconds++;
        gameState.numSecondsAwake++;
        // Debug.Log(gameState.getGameVariable("canPlayerSleep"));
        if (gameState.numSecondsAwake > Settings.numSecondsAwakeMandatory && gameState.getGameVariable("canPlayerSleep") == false)
        {
            gameState.setGameVariable("canPlayerSleep", true);
        }

        gameState.playerMood = gameState.calculateMood();

        if (gt.gameSecond > 59)
        {
            gt.gameSecond = 0;
            gt.gameMinute++;


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

        lightingTick();

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
}
