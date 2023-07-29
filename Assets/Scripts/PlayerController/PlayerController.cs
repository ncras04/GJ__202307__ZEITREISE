using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[SelectionBase, RequireComponent(typeof(PlayerInput), typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement related:")]
    [SerializeField] float movementSpeed = 5f;
    public Vector2 movement = Vector2.zero;
    [Header("Jump related:")]
    [SerializeField] float jumpForce = 10f;
    [SerializeField] int maxNumberOfJumps = 1;
    [SerializeField] float fallSpeedMultiplier = 5f;
    int currentNumberOfJumps;
    bool canJump = true;
    [Header("Dash related:")]
    [SerializeField] float dashDuration = 1f;
    [SerializeField] float dashForce = 10f;
    [SerializeField] AnimationCurve dashCurve;
    [SerializeField] float dashCooldown = 1f;
    float dashTimer;
    bool canDash = true;
    public Vector2 dashDirection = Vector2.zero;

    // Make it an event.
    public Action InteractionHandler;

    InputActionAsset inputAsset;
    InputActionMap actionMap;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction swapAction;
    InputAction interactAction;
    InputAction dashAction;

    public event Action<bool> OnSwappingCall;
    public bool CanSwap { get; set; } = true;
    public Rigidbody Rb { get => rb; set => rb = value; }

    bool wantsToSwap;


    [SerializeField] PhysicMaterial physicMaterial;
    Rigidbody rb;  
    Collider col;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        // Get the input action.
        //controls = new PlayerControls();
        //actionMap = controls.Player;
        inputAsset = GetComponent<PlayerInput>().actions;
        actionMap = inputAsset.FindActionMap("Player");
        actionMap.Enable();
    }  

    private void OnEnable()
    {
        moveAction = actionMap.FindAction("Move");
        moveAction.performed += (context) => { 
            movement.x = context.ReadValue<Vector2>().x;
            dashDirection = movement; 
        };
        moveAction.canceled += (context) => { movement = Vector2.zero; };
        moveAction.Enable();

        jumpAction = actionMap.FindAction("Jump");
        jumpAction.performed += OnJump;
        jumpAction.Enable();

        swapAction = actionMap.FindAction("Swap");
        swapAction.performed += (context) => {
            wantsToSwap = true;
            OnSwappingCall?.Invoke(wantsToSwap);
            Debug.Log("context.duration");
        };
        swapAction.canceled += (context) => {
            wantsToSwap = false;
            OnSwappingCall?.Invoke(wantsToSwap);
        };
        swapAction.Enable();

        interactAction = actionMap.FindAction("Interact");
        interactAction.performed += OnInteract;
        interactAction.Enable();

        dashAction = actionMap.FindAction("Dash");
        dashAction.performed += OnDash;
        dashAction.Enable();

    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        // Move the player.
        Rb.AddForce(movement * movementSpeed);
        if (Rb.velocity.y < 0f)
        {
            Rb.AddForce(Vector2.down * fallSpeedMultiplier);
        }
    }

    #region Interaction
    private void OnInteract(InputAction.CallbackContext context)
    {
        InteractionHandler?.Invoke();
    }
    #endregion

    #region Dash
    private void OnDash(InputAction.CallbackContext context)
    {
        if (canDash)
        {
            canDash = false;
            Rb.useGravity = false;
            bool storeJump = canJump;
            canJump = false;
            //rb.AddForce(dashDirection * dashForce, ForceMode.Impulse);
            // Zero out y velocity.
            //Vector2 velo = rb.velocity;
            //velo.y = 0f;
            //rb.velocity = velo;
            StartCoroutine(StartDashTimer(dashDuration, storeJump));
        }
    }

    IEnumerator StartDashTimer(float dashDuration, bool jumpStore)
    {
        float dashTimer = 0f;
        while (dashTimer < dashDuration)
        {
            dashTimer += Time.deltaTime;
            Rb.velocity = new Vector2(dashCurve.Evaluate(dashTimer/dashDuration)*dashForce*dashDirection.x, 0f);
            //dashDuration -= Time.deltaTime;
            yield return null;
        }
        Rb.useGravity = true;
        canJump = jumpStore;
        StartCoroutine(StartDashCooldown(dashCooldown));
    }

    IEnumerator StartDashCooldown(float dashCooldown)
    {
        while (dashCooldown > 0)
        {
            dashCooldown -= Time.deltaTime;
            yield return null;
        }
        canDash = true;
    }
    #endregion

    #region Jump
    private void OnJump(InputAction.CallbackContext context)
    {
        if (canJump && currentNumberOfJumps > 0)
        {
            Jump();
            currentNumberOfJumps--;
            //canJump = currentNumberOfJumps <= 0 ? false : true;
        }
    }
    private void Jump()
    {
        Rb.AddForce(Vector2.up * jumpForce, ForceMode.Impulse);
    }

    public void ResetJumps()
    {
        canJump = true;
        currentNumberOfJumps = maxNumberOfJumps;
        col.material = null;
    }

    public void LeftGround()
    {
        col.material = physicMaterial;
    }
    #endregion
}
