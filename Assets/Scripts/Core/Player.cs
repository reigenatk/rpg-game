using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;
using static SoundItem;

public class Player : Singleton<Player>, ISaveable
{
    [SerializeField]
    private LayerMask groundLayerMask; // for footsteps

    private Camera mainCamera;

    // make something called Interactable that can be get and setted.
    [SerializeField]  public List<Interactable> interactables { get; set; }

    // save stuff
    private string iSaveableUniqueID;
    public string ISaveableUniqueID { get => iSaveableUniqueID; set => iSaveableUniqueID = value; }
    private GameObjectSave gameObjectSave;
    public GameObjectSave GameObjectSave { get => gameObjectSave; set => gameObjectSave = value; }

    public float RunningSpeed, WalkingSpeed, speed;
    public float xInput;
    public float yInput;

    Vector2 targetPos;
    LayerMask obstacleMask;
    bool isMoving;
    private bool isWalking, isRunning, isIdle;
    private Animator animator;
    public string lastFacedDirection;
    public int sanity;
    [SerializeField] GameObject gameUI;
    [SerializeField] GameState gameState;

    // just a general purpose variable for disabling movements
    public bool disableMovement;

    protected override void Awake()
    {
        base.Awake();

        interactables = new List<Interactable>();
        // get the camera on the player like so
        mainCamera = Camera.main;

        iSaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        gameObjectSave = new GameObjectSave();
    }

    // just put this here cuz he did too lol
    private void OnDisable()
    {
        ISaveableDeregister();
    }

    private void OnEnable()
    {
        ISaveableRegister();
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        // check for player collisions against these layers
        obstacleMask = LayerMask.GetMask("Enemy");
    }

    // Update is called once per frame
    void Update()
    {

        // can't move or enter input, if talking to someone or if in cutscene, or if transitioning between scenes (scene teleport)
        if (gameState.getGameVariableEnum(GameVariable.isDialoguePlaying) || gameState.getGameVariableEnum(GameVariable.isCutscenePlaying) || !GetComponent<Animator>().enabled || disableMovement)
        {
            return;
        }
        Move();
        PlayerRunInput();
        OtherInput();


        // Send event to any listeners for player movement input. This is the key line of code that communicates the input to the animator!
        EventHandler.CallMovementEvent(xInput, yInput, isWalking, isRunning, isIdle, false, false, false, false);

        if (Input.GetKeyDown(KeyCode.E))
        {
            // if there's nothing to interact with, ignore this command
            if (interactables.Count != 0)
            {
                // go thru all possible interactables 
                // we prioritize players first, then items
                foreach (Interactable i in interactables)
                {
                    if (i.isAnotherPlayer())
                    {
                        // prioritize it
                        i.Interact(this);
                    }
                }
                interactables[0].Interact(this);
            }
        }
    }

    private void OtherInput()
    {
        // Trigger Advance Time
        if (Input.GetKey(KeyCode.T))
        {
            TimeManager.Instance.TestAdvanceGameMinute();
        }

        // Trigger Advance Day
        if (Input.GetKeyDown(KeyCode.G))
        {
            TimeManager.Instance.TestAdvanceGameDay();
        }

        // toggle UI
        if (Input.GetKeyDown(KeyCode.U))
        {
            gameUI.SetActive(!gameUI.activeSelf);
        }

        // fully refresh all health bars
        if (Input.GetKeyDown(KeyCode.F))
        {
            foreach (ScoreCategoryUI scui in FindObjectsOfType<ScoreCategoryUI>())
            {
                // set all scores to 100
                scui.UpdateScoreUI(scui.playerScore, 100.0f, 0.0f);
            }
        }
        if (Input.GetKey(KeyCode.Q))
        {
            // enabled all dialogues again
            gameState.canTalkToAllNPCsAgain();
        }
    }

    public void playLowBattery()
    {
        AudioManager.Instance.PlaySound(SoundName.LowBattery);
    }

    public void playStepSound()
    {


        if (gameState.getGameVariableEnum(GameVariable.isDialoguePlaying)) return; // obviously no playing moving sounds if player can't move.

        // if a cutscene is currently playing, don't let us make noises.
        // this is to avoid us making footstep noises when animator is moving around?

        Collider2D collider = Physics2D.OverlapPoint(transform.position, groundLayerMask);
        if (collider != null)
        {
            // Debug.Log("Footstep collider called " + collider.gameObject.name);
            StepSoundFeedback data = collider.GetComponent<StepSoundFeedback>();
            AudioSource audioSource = collider.gameObject.GetComponent<AudioSource>();
            if (data != null && (audioSource.isPlaying == false || audioSource.clip != data.StepClip || audioSource.time / audioSource.clip.length > 0.75f))
            {
                audioSource.clip = data.StepClip;
                audioSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
                audioSource.Play();
            }

        }
        else
        {
            // if no special terrain sound, just play no sound lol 
            // AudioManager.Instance.PlaySound(SoundName.RobotWalk);
        }

    }

    public void setAnimationState(string animationName)
    {
        ResetTriggers();
        animator.Play(animationName);
    }

    public string getPlayerDirection()
    {
        AnimatorStateInfo animationState = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);

        string playerDirection = "";
        if (animationState.IsName("IdleUp") || animationState.IsName("WalkUp") || animationState.IsName("SprintUp"))
        {
            playerDirection = "Up";
        }
        else if (animationState.IsName("IdleDown") || animationState.IsName("WalkDown") || animationState.IsName("SprintDown"))
        {
            playerDirection = "Down";
        }
        else if (animationState.IsName("IdleLeft") || animationState.IsName("WalkLeft") || animationState.IsName("SprintLeft"))
        {
            playerDirection = "Left";
        }
        else if (animationState.IsName("IdleRight") || animationState.IsName("WalkRight") || animationState.IsName("SprintRight"))
        {
            playerDirection = "Right";
        }
        return playerDirection;
    }

    public void setPosition(Vector3 t)
    {
        transform.position = t;
    }

    [YarnCommand("deductSanity")]
    public void deductSanity(int amount)
    {
        sanity -= amount;
        Debug.Log("Sanity deducted");
    }

    public void DisableMovementAndAnimations()
    {
        Debug.Log("Disable momvents");
        // disable the animator
        GetComponent<Animator>().enabled = false;

        disableMovement = true;
        ResetTriggers();
        setToIdleAnimation();
    }

    public void EnableMovementAndAnimations()
    {
        Debug.Log("Enabling movements");
        GetComponent<Animator>().enabled = true;
        disableMovement = false;
    }

    public void setToIdleAnimation()
    {
        AnimatorStateInfo animationState = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
        if (animationState.IsName("WalkUp") || animationState.IsName("SprintUp"))
        {
            setAnimationState("Base Layer.Idle.IdleUp");

        }
        if (animationState.IsName("WalkDown") || animationState.IsName("SprintDown"))
        {
            setAnimationState("Base Layer.Idle.IdleDown");

        }
        if (animationState.IsName("WalkLeft") || animationState.IsName("SprintLeft"))
        {
            setAnimationState("Base Layer.Idle.IdleLeft");

        }
        if (animationState.IsName("WalkRight") || animationState.IsName("SprintRight"))
        {
            setAnimationState("Base Layer.Idle.IdleRight");
        }
    }


    public void ResetTriggers()
    {
        isRunning = false;
        isWalking = false;
        animator.SetBool("isWalking", false); 
        animator.SetBool("isRunning", false);
        xInput = 0.0f;
        yInput = 0.0f;
        animator.ResetTrigger("idleLeft");
        animator.ResetTrigger("idleRight");
        animator.ResetTrigger("idleDown");
        animator.ResetTrigger("idleUp");
    }

    public Vector3 GetPlayerViewportPosition()
    {
        // Vector3 viewport position for player ((0,0) viewport bottom left, (1,1) viewport top right
        return mainCamera.WorldToViewportPoint(transform.position);
    }

    private void Move()
    {
        yInput = Input.GetAxisRaw("Vertical");
        xInput = Input.GetAxisRaw("Horizontal");
        float horz = System.Math.Sign(xInput);
        float vert = System.Math.Sign(yInput);

        
        //Debug.LogFormat("{0} {1}", Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        // only process if something is being pushed down

        if (xInput != 0 || yInput != 0)
        {


            targetPos = new Vector2(transform.position.x + horz, transform.position.y + vert);
            transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

            isWalking = true;
            isRunning = false;
            isIdle = false;


        }
        else if (xInput == 0 && yInput == 0)
        {
            isRunning = false;
            isWalking = false;
            isIdle = true;
        }
    }
    private void PlayerRunInput()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            isRunning = true;
            isWalking = false;
            isIdle = false;
            speed = RunningSpeed;
        }
        else
        {
            isRunning = false;
            isWalking = true;
            isIdle = false;
            speed = WalkingSpeed;
        }
    }

    IEnumerator SmoothMove()
    {
        isMoving = true;
        while (Vector2.Distance(transform.position, targetPos) > 0.01f)
        {
            /*Debug.Log(transform.position);*/
            transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            yield return null;

        }
        // if we get here we reached target Position
        // put it precisely there since we have the chance that it goes negative from movetowards
        transform.position = targetPos;
        isMoving = false;
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public GameObjectSave ISaveableSave()
    {
        // Delete saveScene for game object if it already exists
        GameObjectSave.sceneData.Remove(Settings.PersistentScene);

        // Create saveScene for game object
        SceneSave sceneSave = new SceneSave();

        // Create Vector3 Dictionary
        sceneSave.vector3Dictionary = new Dictionary<string, Vector3Serializable>();

        //  Create String Dictionary
        sceneSave.stringDictionary = new Dictionary<string, string>();

        // Add Player position to Vector3 dictionary
        Vector3Serializable vector3Serializable = new Vector3Serializable(transform.position.x, transform.position.y, transform.position.z);
        Debug.Log("[Save]: current pos is " + vector3Serializable);
        sceneSave.vector3Dictionary.Add("playerPosition", vector3Serializable);

        // Add Current Scene Name to string dictionary
        Debug.Log("[Save]: current scene is " + SceneManager.GetActiveScene().name);
        sceneSave.stringDictionary.Add("currentScene", SceneManager.GetActiveScene().name);

        // Add Player Direction to string dictionary
        Debug.Log("[Save]: player direction is " + getPlayerDirection());
        sceneSave.stringDictionary.Add("playerDirection", getPlayerDirection());

        // Add sceneSave data for player game object
        GameObjectSave.sceneData.Add(Settings.PersistentScene, sceneSave);

        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        // try to find the GameObjectSave for this object, using the GUID as a key (which should be unique)
        if (gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            // Get save data dictionary for scene
            if (gameObjectSave.sceneData.TryGetValue(Settings.PersistentScene, out SceneSave sceneSave))
            {
                // Get player position
                if (sceneSave.vector3Dictionary != null && sceneSave.vector3Dictionary.TryGetValue("playerPosition", out Vector3Serializable playerPosition))
                {
                    Debug.Log("[ISaveableLoad Player] position to value" + playerPosition.x + " " + playerPosition.y + " " + playerPosition.z);
                    transform.position = new Vector3(playerPosition.x, playerPosition.y, playerPosition.z);
                }

                // Get String dictionary
                if (sceneSave.stringDictionary != null)
                {
                    // Get player scene
                    if (sceneSave.stringDictionary.TryGetValue("currentScene", out string currentScene))
                    {
                        
                        Enum.TryParse(currentScene, out SceneName sceneName);
                        Debug.Log("[ISaveableLoad Player] scene to value" + sceneName);
                        LevelLoader.Instance.FadeAndLoadScene(sceneName, transform.position);
                    }

                    // Get player direction
                    if (sceneSave.stringDictionary.TryGetValue("playerDirection", out string playerDir))
                    {
                        bool playerDirFound = Enum.TryParse(playerDir, true, out Direction direction);
                        Debug.Log("[ISaveableLoad Player] direction to value" + direction);
                        if (playerDirFound)
                        {
                            SetPlayerDirection(direction);
                        }
                    }
                }
            }
        }
        else
        {
            Debug.Log("[Player] No save found");
        }
    }

    private void SetPlayerDirection(Direction playerDirection)
    {
        switch (playerDirection)
        {
            case Direction.Up:
                // set idle up trigger
                EventHandler.CallMovementEvent(0f, 0f, false, false, true, false, false, true, false);
                break;

            case Direction.Down:
                // set idle down trigger
                EventHandler.CallMovementEvent(0f, 0f, false, false, true, false, false, false, true);
                break;

            case Direction.Left:
                EventHandler.CallMovementEvent(0f, 0f, false, false, true, true, false, false, false);
                break;

            case Direction.Right:
                EventHandler.CallMovementEvent(0f, 0f, false, false, true, false, true, false, false);
                break;

            default:
                // set idle down trigger
                EventHandler.CallMovementEvent(0f, 0f, false, false, true, false, false, false, true);
                break;
        }
    }

    public void ISaveableStoreScene(string sceneName)
    {
        // player is on persistant scene so we dont need to do anything here
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        // player is on persistant scene so we dont need to do anything here
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("Collision Exit with " + collision.gameObject.name);
        // check that its a player collision
        if (collision.gameObject.GetComponent<NPCMovement>() != null)
        {
            Debug.Log("Exiting Collision with an NPC");

            // STOP the rigidbody if we leave contact with an NPC 
            // if this isn't here we get that weird bug where player moves infinitely after hitting an NPC.
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }
}

