using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPCPath))]
public class NPCSchedule : MonoBehaviour
{
    [SerializeField] private SO_NPCScheduleEventList so_NPCScheduleEventList = null;

    // all the NPC Schedules, SortedSet will automatically order them using our comparator, NPCScheduleEventSort
    private SortedSet<NPCScheduleEvent> npcScheduleEventSet;
    private NPCPath npcPath;

    private void Awake()
    {
        // Load NPC schedule event list into a sorted set
        npcScheduleEventSet = new SortedSet<NPCScheduleEvent>(new NPCScheduleEventSort());

        foreach (NPCScheduleEvent npcScheduleEvent in so_NPCScheduleEventList.npcScheduleEventList)
        {
            npcScheduleEventSet.Add(npcScheduleEvent);
        }

        // Get NPC Path Component
        npcPath = GetComponent<NPCPath>();

    }

    private void OnEnable()
    {
        EventHandler.AdvanceGameMinuteEvent += GameTimeSystem_AdvanceMinute;
    }

    private void OnDisable()
    {
        EventHandler.AdvanceGameMinuteEvent -= GameTimeSystem_AdvanceMinute;
    }

    private void GameTimeSystem_AdvanceMinute(string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        int time = (gameHour * 100) + gameMinute;

        //Attempt to get matching schedule

        NPCScheduleEvent matchingNPCScheduleEvent = null;

        int gameDay = FindObjectOfType<GameState>().getGameDay();

        foreach (NPCScheduleEvent npcScheduleEvent in npcScheduleEventSet)
        {
            // schedules work by matching the time
            if (npcScheduleEvent.Time == time)
            {
                // Time matches, now check if parameters match
                if (npcScheduleEvent.day != 0 && npcScheduleEvent.day != gameDay)
                    continue;
                /*
                if (npcScheduleEvent.season != Season.none && npcScheduleEvent.season != gameSeason)
                    continue;*/

                if (npcScheduleEvent.weather != FindObjectOfType<GameState>().currentWeather)
                    continue;

                //Schdule matches
                Debug.Log("Schedule Matches! " + npcScheduleEvent);
                matchingNPCScheduleEvent = npcScheduleEvent;
                break;
            }
            else if (npcScheduleEvent.Time > time)
            {
                break;
            }
        }

        // Now test is matchingSchedule!=null and do something;
        if (matchingNPCScheduleEvent != null)
        {
            // Build path for matching schedule (just like we did in AStarTest)
            npcPath.BuildPath(matchingNPCScheduleEvent);
        }
    }
}