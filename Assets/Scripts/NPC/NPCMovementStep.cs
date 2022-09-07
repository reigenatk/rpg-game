
using UnityEngine;

// this class represents a single step to take after A* has finished finding the ideal path
public class NPCMovementStep
{
    public SceneName sceneName;
    // the hour min sec here means that this NPC must be at X grid coordinate at Y time
    public int hour;
    public int minute;
    public int second;
    public Vector2Int gridCoordinate;
}
