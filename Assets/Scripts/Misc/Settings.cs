
using System;
using UnityEngine;

public static class Settings
{
    // Scenes
    public const string PersistentScene = "PersistantScene";

    // fade in fade out
    public const float fadeInSeconds = 0.24f;
    public const float fadeOutSeconds = 0.35f;
    public const float targetAlpha = 0.45f; // the value of the alpha in the sprite rendered for faded objects

    // Player Animation Parameters
    public static int xInput;
    public static int yInput;
    public static int isWalking;
    public static int isRunning;
    public static int idleLeft;
    public static int idleRight;
    public static int idleUp;
    public static int idleDown;



    // Inventory
    public static int playerInitialInventoryCapacity = 24;
    public static int playerMaximumInventoryCapacity = 48;

    // for NPC cross scene movemenet
    public const int maxGridWidth = 99999;
    public const int maxGridHeight = 99999;

    // 1 min in game = 1 sec irl (1/60)
    public const float secondsPerGameSecond = 0.0166667f;
    public const int numSecondsAwakeMandatory = 14 * 3600;

    // Tilemap
    public const float gridCellSize = 1f; // grid cell size in unity units
    public const float gridCellDiagonalSize = 1.41f; // diagonal distance between unity cell centres
    public static Vector2 cursorSize = Vector2.one;

    //NPC Movement
    public static float pixelSize = 0.0625f;

    // NPC Animation Parameters
    public static int walkUp;
    public static int walkDown;
    public static int walkLeft;
    public static int walkRight;
    public static int eventAnimation;

    public static float defaultCarSpeed = 7.0f; // 7 unity units per second 

    public static T ParseEnum<T>(string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }

    // static constructor
    static Settings()
    {
        // Player Animation Parameters
        xInput = Animator.StringToHash("xInput");
        yInput = Animator.StringToHash("yInput");
        isWalking = Animator.StringToHash("isWalking");
        isRunning = Animator.StringToHash("isRunning");

        // NPC Animation parameters
        walkUp = Animator.StringToHash("walkUp");
        walkDown = Animator.StringToHash("walkDown");
        walkLeft = Animator.StringToHash("walkLeft");
        walkRight = Animator.StringToHash("walkRight");
        eventAnimation = Animator.StringToHash("eventAnimation");


        //toolEffect = Animator.StringToHash("toolEffect");
        //isUsingToolRight = Animator.StringToHash("isUsingToolRight");
        //isUsingToolLeft = Animator.StringToHash("isUsingToolLeft");
        //isUsingToolUp = Animator.StringToHash("isUsingToolUp");
        //isUsingToolDown = Animator.StringToHash("isUsingToolDown");
        //isLiftingToolRight = Animator.StringToHash("isLiftingToolRight");
        //isLiftingToolLeft = Animator.StringToHash("isLiftingToolLeft");
        //isLiftingToolUp = Animator.StringToHash("isLiftingToolUp");
        //isLiftingToolDown = Animator.StringToHash("isLiftingToolDown");
        //isSwingingToolRight = Animator.StringToHash("isSwingingToolRight");
        //isSwingingToolLeft = Animator.StringToHash("isSwingingToolLeft");
        //isSwingingToolUp = Animator.StringToHash("isSwingingToolUp");
        //isSwingingToolDown = Animator.StringToHash("isSwingingToolDown");
        //isPickingRight = Animator.StringToHash("isPickingRight");
        //isPickingLeft = Animator.StringToHash("isPickingLeft");
        //isPickingUp = Animator.StringToHash("isPickingUp");
        //isPickingDown = Animator.StringToHash("isPickingDown");

        //// Shared Animation parameters
        idleUp = Animator.StringToHash("idleUp");
        idleDown = Animator.StringToHash("idleDown");
        idleLeft = Animator.StringToHash("idleLeft");
        idleRight = Animator.StringToHash("idleRight");
    }
}
