﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{
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
        
    }
    public GameTime gt;
    

    private int totalGameSeconds = 0;
    private string gameDayOfWeek = "Mon";
    private bool gameClockPaused = false;
    private float gameTick = 0f;

    // how many in game seconds correspond to one decayed percent of each category
    [SerializeField] private GameState gameState;
    [SerializeField] private int energyDecayRate = 100;
    [SerializeField] private int socialDecayRate = 100;
    [SerializeField] private int contentednessDecayRate = 100;
    [SerializeField] private int entertainmentDecayRate = 100;
    private void Start()
    {
        gt = new GameTime(19, 30, 0);
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

        if (gt.gameSecond > 59)
        {
            gt.gameSecond = 0;
            gt.gameMinute++;


            if (gt.gameMinute > 59)
            {
                gt.gameMinute = 0;
                gt.gameHour++;

                if (gt.gameHour > 24)
                {
                    gt.gameHour = 0;
                    

                    gameDayOfWeek = GetDayOfWeek();
                    EventHandler.CallAdvanceGameDayEvent(gameDayOfWeek, gt.gameHour, gt.gameMinute, gt.gameSecond);
                }

                EventHandler.CallAdvanceGameHourEvent(gameDayOfWeek, gt.gameHour, gt.gameMinute, gt.gameSecond);
            }

            EventHandler.CallAdvanceGameMinuteEvent(gameDayOfWeek, gt.gameHour, gt.gameMinute, gt.gameSecond);

        }

        if (totalGameSeconds % energyDecayRate == 0)
        {
            float curScore = gameState.getPlayerScore(PlayerScore.energy);
            gameState.setPlayerScore(PlayerScore.energy, curScore - 1.0f);
            // Debug.Log("New energy is " + gameState.getPlayerScore(PlayerScore.energy));
        }
        if (totalGameSeconds % socialDecayRate == 0)
        {
            float curScore = gameState.getPlayerScore(PlayerScore.social);
            gameState.setPlayerScore(PlayerScore.social, curScore - 1.0f);
        }
        if (totalGameSeconds % contentednessDecayRate == 0)
        {
            float curScore = gameState.getPlayerScore(PlayerScore.contentedness);
            gameState.setPlayerScore(PlayerScore.contentedness, curScore - 1.0f);
        }
        if (totalGameSeconds % entertainmentDecayRate == 0)
        {
            float curScore = gameState.getPlayerScore(PlayerScore.entertained);
            gameState.setPlayerScore(PlayerScore.entertained, curScore - 1.0f);
        }


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
