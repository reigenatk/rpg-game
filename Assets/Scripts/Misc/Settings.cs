
using UnityEngine;

public static class Settings
{
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

    // 1 min in game = 1 sec irl (1/60)
    public const float secondsPerGameSecond = 0.0166667f;

    // Inventory
    public static int playerInitialInventoryCapacity = 24;
    public static int playerMaximumInventoryCapacity = 48;

    // static constructor
    static Settings()
    {
        // Player Animation Parameters
        xInput = Animator.StringToHash("xInput");
        yInput = Animator.StringToHash("yInput");
        isWalking = Animator.StringToHash("isWalking");
        isRunning = Animator.StringToHash("isRunning");


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
