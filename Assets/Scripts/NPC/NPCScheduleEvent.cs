using UnityEngine;

// a schedule object to decide whether or not an NPC should move around in a scene
[System.Serializable]
public class NPCScheduleEvent
{
    // hour min second is when the NPC shoudl START moving, not when itll arrive.
    public int hour;
    public int minute;
    public int priority;
    public int day;
    public Weather weather;
    public SceneName toSceneName;
    public GridCoordinate toGridCoordinate;
    public Direction npcFacingDirectionAtDestination = Direction.none;
    public AnimationClip animationAtDestination;
    public string description; // for me

    public int Time
    {
        get
        {
            return (hour * 100) + minute;
        }
    }

    public NPCScheduleEvent(int hour, int minute, int priority, int day, SceneName toSceneName, GridCoordinate toGridCoordinate, AnimationClip animationAtDestination)
    {
        this.hour = hour;
        this.minute = minute;
        this.priority = priority;
        this.day = day;
        this.toSceneName = toSceneName;
        this.toGridCoordinate = toGridCoordinate;
        this.animationAtDestination = animationAtDestination;
    }

    public NPCScheduleEvent()
    {

    }

    public override string ToString()
    {
        return $"Time: {Time}, Priority: {priority}, Day: {day}";
    }
}
