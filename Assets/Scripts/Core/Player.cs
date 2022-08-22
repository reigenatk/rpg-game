using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class Player : Singleton<Player>
{


    private Camera mainCamera;

    // make something called Interactable that can be get and setted.
    [SerializeField]  public List<Interactable> interactables { get; set; }

    public float RunningSpeed, WalkingSpeed, speed;
    public float xInput;
    public float yInput;

    Vector2 targetPos;
    LayerMask obstacleMask;
    bool isMoving;
    private bool isWalking, isRunning, isIdle;
    private Animator animator;
    public bool disableMovement = false;
    public string lastFacedDirection;
    public int sanity;
    SoundManager sm;
    [SerializeField] GameObject gameUI;
    [SerializeField] GameState gameState;
    protected override void Awake()
    {
        base.Awake();

        interactables = new List<Interactable>();
        // get the camera on the player like so
        mainCamera = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        // check for player collisions against these layers
        obstacleMask = LayerMask.GetMask("Enemy");
        sm = FindObjectOfType<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // can't move if talking to someone or if in cutscene
        if (disableMovement || gameState.getGameVariableEnum(GameVariable.isCutscenePlaying))
        {
            return;
        }
        Move();
        PlayerRunInput();
        OtherInput();


        // Send event to any listeners for player movement input
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
    }

    public void playStepSound()
    {
        if (sm == null)
        {
            sm = FindObjectOfType<SoundManager>();
        }

        if (disableMovement || gameState.getGameVariableEnum(GameVariable.isCutscenePlaying)) return; // obviously no playing moving sounds if player can't move.

        // if a cutscene is currently playing, don't let us make noises.
        // this is to avoid us making footstep noises when animator is moving around?

        /*if (FindObjectOfType<GameState>().getGameVariableEnum(GameVariable.isCutscenePlaying) == true) return;*/
        sm.playSoundOneShot(SoundManager.Sound.WalkingSound);
    }

    public void setAnimationState(string animationName)
    {
        animator.Play(animationName);
    }

    public string getPlayerDirection()
    {
        AnimatorStateInfo animationState = GameObject.FindGameObjectWithTag("Player").
    GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);

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
        this.transform.position = t;
    }

    [YarnCommand("deductSanity")]
    public void deductSanity(int amount)
    {
        sanity -= amount;
        Debug.Log("Sanity deducted");
    }

    public void DisableMovementAndAnimations()
    {
        GetComponent<Animator>().enabled = false;
        // Debug.Log("Disabling movements");
        disableMovement = true;
        ResetTriggers();

        // setAnimationState("Base Layer.Idle.IdleDown");
    }

    public void EnableMovementAndAnimations()
    {
        GetComponent<Animator>().enabled = true;
        // Debug.Log("Enabling movements");
        disableMovement = false;
    }

    public void ResetTriggers()
    {
        isRunning = false;
        isWalking = false;
        xInput = 0.0f;
        yInput = 0.0f;
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
}

