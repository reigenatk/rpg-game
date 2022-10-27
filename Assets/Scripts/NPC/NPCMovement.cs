using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NPCPath))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class NPCMovement : MonoBehaviour
{
    // set this to the starting scene that the NPC should be in
    [SerializeField] public SceneName npcCurrentScene;


    [HideInInspector] public SceneName npcTargetScene;
    [HideInInspector] public Vector3Int npcCurrentGridPosition;
    [HideInInspector] public Vector3Int npcTargetGridPosition;
    [HideInInspector] public Vector3 npcTargetWorldPosition;
    [HideInInspector] public Direction npcFacingDirectionAtDestination;

    private SceneName npcPreviousMovementStepScene;
    private Vector3Int npcNextGridPosition;
    private Vector3 npcNextWorldPosition;

    [Header("NPC Movement")]
    public float npcNormalSpeed = 2.0f;

    [SerializeField] private float npcMinSpeed = 0.1f;
    [SerializeField] private float npcMaxSpeed = 5.0f;
    private bool npcIsMoving = false;

    [HideInInspector] public AnimationClip npcTargetAnimationClip;

    [Header("NPC Animation")]
    [SerializeField] private AnimationClip blankAnimation = null;

    private Grid grid;
    private Rigidbody2D rigidBody2D;
    private BoxCollider2D boxCollider2D;
    private WaitForFixedUpdate waitForFixedUpdate;
    private Animator animator;

    // we will use animation override controller to OVERRIDE the BlankAnimation on the AnimationEvent state in the NPC animator
    private AnimatorOverrideController animatorOverrideController;
    private int lastMoveAnimationParameter;
    private NPCPath npcPath;
    private bool npcInitialised = false;
    private SpriteRenderer spriteRenderer;
    [SerializeField] public bool npcActiveInScene = false;

    // audio source on the NPC
    AudioSource audioSource;
    AudioClip npcTargetAudioClip; // audio to play once movement complete

    private bool sceneLoaded = false;

    private Coroutine moveToGridPositionRoutine;

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += AfterSceneLoad;
        EventHandler.BeforeSceneUnloadEvent += BeforeSceneUnloaded;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoad;
        EventHandler.BeforeSceneUnloadEvent -= BeforeSceneUnloaded;
    }

    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        npcPath = GetComponent<NPCPath>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;

        // Initialise target world position, target grid position & target scene to current
        npcTargetScene = npcCurrentScene;
        npcTargetGridPosition = npcCurrentGridPosition;
        npcTargetWorldPosition = transform.position;
    }

    // Start is called before the first frame update
    private void Start()
    {
        waitForFixedUpdate = new WaitForFixedUpdate();

        SetIdleAnimation();
    }

    private void FixedUpdate()
    {
        if (sceneLoaded)
        {
            
            if (npcIsMoving == false)
            {
                // set npc current and next grid position - to take into account the npc might be animating
                npcCurrentGridPosition = GetGridPosition(transform.position);
                npcNextGridPosition = npcCurrentGridPosition;

                if (npcPath.npcMovementStepStack.Count > 0)
                {
                    NPCMovementStep npcMovementStep = npcPath.npcMovementStepStack.Peek();

                   
                    npcCurrentScene = npcMovementStep.sceneName;
                    // Debug.Log("Setting npcCurrentScene for " + gameObject.name + " to " + npcCurrentScene);

                    // If NPC is about the move to a new scene reset position to starting point in new scene and update the step times
                    if (npcCurrentScene != npcPreviousMovementStepScene)
                    {
                        
                        npcCurrentGridPosition = (Vector3Int)npcMovementStep.gridCoordinate;
                        npcNextGridPosition = npcCurrentGridPosition;
                        transform.position = GetWorldPosition(npcCurrentGridPosition, npcCurrentScene);
                        npcPreviousMovementStepScene = npcCurrentScene;
                        npcPath.UpdateTimesOnPath();
                        Debug.Log("Moving scenes, setting NPC " + gameObject.name + "'s position to grid position " + npcCurrentGridPosition + " world position " + GetWorldPosition(npcCurrentGridPosition, npcCurrentScene));
                    }


                    // If NPC is in current scene then set NPC to active to make visible, pop the movement step off the stack and then call method to move NPC
                    if (npcCurrentScene.ToString() == SceneManager.GetActiveScene().name)
                    {
                        SetNPCActiveInScene();

                        npcMovementStep = npcPath.npcMovementStepStack.Pop();

                        npcNextGridPosition = (Vector3Int)npcMovementStep.gridCoordinate;
                        Debug.Log("Next grid position for " + gameObject.name + npcNextGridPosition);

                        TimeSpan npcMovementStepTime = new TimeSpan(npcMovementStep.hour, npcMovementStep.minute, npcMovementStep.second);

                        MoveToGridPosition(npcNextGridPosition, npcMovementStepTime, TimeManager.Instance.GetGameTime());
                    }

                    // else if NPC is not in current scene then set NPC to inactive to make invisible
                    // - once the movement step time is less than game time (in the past) then pop movement step off the stack and set NPC position to movement step position
                    else
                    {
                        SetNPCInactiveInScene();

                        npcCurrentGridPosition = (Vector3Int)npcMovementStep.gridCoordinate;
                        npcNextGridPosition = npcCurrentGridPosition;
                        transform.position = GetWorldPosition(npcCurrentGridPosition, npcCurrentScene);

                        TimeSpan npcMovementStepTime = new TimeSpan(npcMovementStep.hour, npcMovementStep.minute, npcMovementStep.second);

                        TimeSpan gameTime = TimeManager.Instance.GetGameTime();

                        // if the time is later than what it should be, instantly move the NPC to catch up.
                        if (npcMovementStepTime < gameTime)
                        {
                            npcMovementStep = npcPath.npcMovementStepStack.Pop();

                            npcCurrentGridPosition = (Vector3Int)npcMovementStep.gridCoordinate;
                            npcNextGridPosition = npcCurrentGridPosition;
                            transform.position = GetWorldPosition(npcCurrentGridPosition, npcCurrentScene);
                            Debug.Log("NPC not in scene, set world position to " + transform.position);
                        }
                    }

                }
                // else if no more NPC movement steps. This means we have finished our movement and now we wanna 
                // play the appropriate custom anim and also audio if desired.
                else
                {
                    ResetMoveAnimation();

                    SetNPCFacingDirection();

                    SetNPCEventAnimation();

                    SetNPCAudio();
                }
            }
        }
    }


    public void SetScheduleEventDetails(NPCScheduleEvent npcScheduleEvent)
    {
        npcTargetScene = npcScheduleEvent.toSceneName;
        npcTargetGridPosition = (Vector3Int)npcScheduleEvent.toGridCoordinate;
        npcTargetWorldPosition = GetWorldPosition(npcTargetGridPosition, npcTargetScene);
        npcFacingDirectionAtDestination = npcScheduleEvent.npcFacingDirectionAtDestination;
        npcTargetAnimationClip = npcScheduleEvent.animationAtDestination;
        npcTargetAudioClip = npcScheduleEvent.audioToPlay;
        ClearNPCEventAnimation();
    }

    // this is how we run an animation after the movement, basically the override controller overrides 
    // blankAnimation and then triggers eventAnimation bool
    private void SetNPCEventAnimation()
    {
        if (npcTargetAnimationClip != null)
        {
            ResetIdleAnimation();
            animatorOverrideController[blankAnimation] = npcTargetAnimationClip;
            animator.SetBool(Settings.eventAnimation, true);
        }
        else
        {
            animatorOverrideController[blankAnimation] = blankAnimation;
            animator.SetBool(Settings.eventAnimation, false);
        }
    }

    private void SetNPCAudio()
    {
        // if we wanna play audio once we get there, then start playing it now.
        if (npcTargetAudioClip != null && audioSource.clip == null)
        {
            Debug.Log(gameObject.name + " Is playing audioclip " + npcTargetAudioClip + " after reaching destination ");
            audioSource.clip = npcTargetAudioClip;
            audioSource.Play();
        }
    }

    // this is called before any movement occurs? So we do things here like 
    public void ClearNPCEventAnimation()
    {
        animatorOverrideController[blankAnimation] = blankAnimation;
        animator.SetBool(Settings.eventAnimation, false);

        // Clear any rotation on npc
        transform.rotation = Quaternion.identity;
    }

    private void SetNPCFacingDirection()
    {
        ResetIdleAnimation();

        switch (npcFacingDirectionAtDestination)
        {
            case Direction.Up:
                animator.SetBool(Settings.idleUp, true);
                break;

            case Direction.Down:
                animator.SetBool(Settings.idleDown, true);
                break;

            case Direction.Left:
                animator.SetBool(Settings.idleLeft, true);
                break;

            case Direction.Right:
                animator.SetBool(Settings.idleRight, true);
                break;

            case Direction.none:
                break;

            default:
                break;
        }
    }

    public void SetNPCActiveInScene()
    {
        // Debug.Log("Set NPC " + gameObject.name + "Active");
        spriteRenderer.enabled = true;
        boxCollider2D.enabled = true;
        npcActiveInScene = true;
    }

    public void SetNPCInactiveInScene()
    {
        // Debug.Log("Set NPC" + gameObject.name + " Inactive");
        spriteRenderer.enabled = false;
        boxCollider2D.enabled = false;
        npcActiveInScene = false;
    }

    private void AfterSceneLoad()
    {
        // this should grab the current (and only) grid in the scene
        grid = FindObjectOfType<Grid>();
        Debug.Log("grid is " + grid);

        if (!npcInitialised)
        {
            InitialiseNPC();
            npcInitialised = true;
        }

        sceneLoaded = true;
    }

    private void BeforeSceneUnloaded()
    {
        sceneLoaded = false;
    }

    /// <summary>
    /// returns the grid position given the worldPosition
    /// </summary>
    private Vector3Int GetGridPosition(Vector3 worldPosition)
    {
        if (grid != null)
        {
            // use this builtin Unity function to translate the position to a cell position in the grid
            return grid.WorldToCell(worldPosition);
        }
        else
        {
            return Vector3Int.zero;
        }
    }

    /// <summary>
    ///  returns the world position (centre of grid square) from gridPosition
    /// </summary>
    public Vector3 GetWorldPosition(Vector3Int gridPosition, SceneName nameOfScene)
    {
        Vector3 worldPosition = grid.CellToWorld(gridPosition);
        worldPosition -= grid.transform.position;
        worldPosition += NPCManager.Instance.getTilemapOffset(nameOfScene);

        // Get centre of grid square
        return new Vector3(worldPosition.x + Settings.gridCellSize / 2.0f, worldPosition.y + Settings.gridCellSize / 2.0f, worldPosition.z);
    }

    private void InitialiseNPC()
    {
       
        // Active in scene
        if (npcCurrentScene.ToString() == SceneManager.GetActiveScene().name)
        {
            // Debug.Log("Active in scene");
            SetNPCActiveInScene();
        }
        else
        {
            SetNPCInactiveInScene();
        }

        npcPreviousMovementStepScene = npcCurrentScene;
        // Get NPC Current Grid Position
        npcCurrentGridPosition = GetGridPosition(transform.position);

        // Set Next Grid Position and Target Grid Position to current Grid Position
        npcNextGridPosition = npcCurrentGridPosition;
        npcTargetGridPosition = npcCurrentGridPosition;
        npcTargetWorldPosition = GetWorldPosition(npcTargetGridPosition, npcCurrentScene);

        // Get NPC WorldPosition
        npcNextWorldPosition = GetWorldPosition(npcCurrentGridPosition, npcCurrentScene);
        // Debug.Log("transform position " + transform.position + " grid position " + npcCurrentGridPosition + " target position " + npcTargetWorldPosition);
    }

    // this actually moves the NPC, it just starts a Coroutine. We need coroutine because we want it to smoothly move
    private void MoveToGridPosition(Vector3Int gridPosition, TimeSpan npcMovementStepTime, TimeSpan gameTime)
    {
        moveToGridPositionRoutine = StartCoroutine(MoveToGridPositionRoutine(gridPosition, npcMovementStepTime, gameTime));
    }

    private IEnumerator MoveToGridPositionRoutine(Vector3Int gridPosition, TimeSpan npcMovementStepTime, TimeSpan gameTime)
    {
        // Debug.Log("MoveToGridPosition running");
        npcIsMoving = true;
        if (audioSource.isPlaying)
        {
            // turn off the audio from a previous state (if there was one)
            audioSource.clip = null;
            audioSource.Stop();
        }

        // change the animation to appropriate value
        SetMoveAnimation(gridPosition);

        npcNextWorldPosition = GetWorldPosition(gridPosition, npcCurrentScene);

        Debug.Log("npcMovementStepTime is " + npcMovementStepTime.ToString() + " gameTime is " + gameTime.ToString() + " npcTime > gamemTime is " + (npcMovementStepTime > gameTime));

        // If movement step time is in the future then smoothly move the NPC using waitForFixedUpdate, otherwise skip and move NPC immediately to position (for some reason if it takes too long)
        if (npcMovementStepTime > gameTime)
        {
            //calculate time difference in seconds
            float timeToMove = (float)(npcMovementStepTime.TotalSeconds - gameTime.TotalSeconds);

            // Calculate speed
            float npcCalculatedSpeed = Vector3.Distance(transform.position, npcNextWorldPosition) / timeToMove / Settings.secondsPerGameSecond;
            Debug.Log("timeToMove " + timeToMove + " npcCalculatedSpeed " + npcCalculatedSpeed + " >= minspeed " + (npcCalculatedSpeed >= npcMinSpeed) + " <= maxspeed " + (npcCalculatedSpeed <= npcMaxSpeed));
            //// If speed is at least npc min speed and less than npc max speed  then process, otherwise skip and move NPC immediately to position
            if (npcCalculatedSpeed >= npcMinSpeed && npcCalculatedSpeed <= npcMaxSpeed)
            {
                Debug.Log("Distance is " + Vector3.Distance(transform.position, npcNextWorldPosition) + " is it less than Settings.pixelSize? " + (Vector3.Distance(transform.position, npcNextWorldPosition) > Settings.pixelSize));
                // while we aren't there yet (we know we're there if the distance is small enough)
                while (Vector3.Distance(transform.position, npcNextWorldPosition) > Settings.pixelSize)
                {
                    // create a unitvector and multiply it by the distance needed to move
                    Vector3 unitVector = Vector3.Normalize(npcNextWorldPosition - transform.position);
                    Vector2 move = new Vector2(unitVector.x * npcCalculatedSpeed * Time.fixedDeltaTime, unitVector.y * npcCalculatedSpeed * Time.fixedDeltaTime);
                    Debug.Log("[MovePosition running for " + gameObject.name+ "] distance " + move + " unit vector " + unitVector);
                    rigidBody2D.MovePosition(rigidBody2D.position + move);

                    yield return waitForFixedUpdate;
                }
            }
        }
        Debug.Log("MoveToGridPosition finished for " + gameObject.name);
        rigidBody2D.position = npcNextWorldPosition;
        npcCurrentGridPosition = gridPosition;
        npcNextGridPosition = npcCurrentGridPosition;
        npcIsMoving = false;
    }

    // figure out which direction NPC should face based on where they are currently vs where they want to go.
    private void SetMoveAnimation(Vector3Int gridPosition)
    {
        // Reset idle animation
        ResetIdleAnimation();

        // Reset move animation
        ResetMoveAnimation();

        // get world position
        Vector3 toWorldPosition = GetWorldPosition(gridPosition, npcCurrentScene);


        // get vector
        Vector3 directionVector = toWorldPosition - transform.position;
        Debug.Log("toWorldPosition " + toWorldPosition + "transform.position " + transform.position + " directionVector " + directionVector);

        if (Mathf.Abs(directionVector.x) >= Mathf.Abs(directionVector.y))
        {
            // Use left/right animation
            if (directionVector.x > 0)
            {
                Debug.Log("walkRight");
                animator.SetBool(Settings.walkRight, true);
            }
            else
            {
                Debug.Log("walkLeft");
                animator.SetBool(Settings.walkLeft, true);
            }
        }
        else
        {
            //Use up/down animation
            if (directionVector.y > 0)
            {
                Debug.Log("walkUp");
                animator.SetBool(Settings.walkUp, true);
            }
            else
            {
                Debug.Log("walkDown");
                animator.SetBool(Settings.walkDown, true);
            }
        }
    }

    // stops all NPC movement and resets their position, this is for when we wanna save
    public void CancelNPCMovement()
    {
        npcPath.ClearPath();
        npcNextGridPosition = Vector3Int.zero;
        npcNextWorldPosition = Vector3.zero;
        npcIsMoving = false;

        if (moveToGridPositionRoutine != null)
        {
            StopCoroutine(moveToGridPositionRoutine);
        }

        // Reset move animation
        ResetMoveAnimation();

        // Clear event animation
        ClearNPCEventAnimation();
        npcTargetAnimationClip = null;

        // Reset idle animation
        ResetIdleAnimation();

        // Set idle animation
        SetIdleAnimation();
    }

    private void SetIdleAnimation()
    {
        animator.SetBool(Settings.idleDown, true);
    }

    private void ResetMoveAnimation()
    {
        animator.SetBool(Settings.walkRight, false);
        animator.SetBool(Settings.walkLeft, false);
        animator.SetBool(Settings.walkUp, false);
        animator.SetBool(Settings.walkDown, false);
    }

    private void ResetIdleAnimation()
    {
        animator.SetBool(Settings.idleRight, false);
        animator.SetBool(Settings.idleLeft, false);
        animator.SetBool(Settings.idleUp, false);
        animator.SetBool(Settings.idleDown, false);
    }
}