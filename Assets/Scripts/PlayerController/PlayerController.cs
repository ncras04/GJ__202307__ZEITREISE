using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[SelectionBase, RequireComponent(typeof(PlayerInput), typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerController otherPlayer;
    [Header("Movement related:")]
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float jumpForce = 10f;
    [SerializeField] int maxNumberOfJumps = 1;
    int currentNumberOfJumps;
    bool canJump = true;
    [SerializeField] float dashDuration = 1f;
    [SerializeField] float dashForce = 10f;
    [SerializeField] float dashCooldown = 1f;
    bool canDash = true;

    [Header("Difference related:")]
    public Action InteractionHandler;
    Rigidbody rb;
    public Vector2 movement = Vector2.zero;
    public Vector2 dashDirection = Vector2.zero;
    

    InputActionAsset inputAsset;
    InputActionMap actionMap;

    InputAction moveAction;
    InputAction jumpAction;
    InputAction swapAction;
    InputAction interactAction;
    InputAction dashAction;

    public event Action<bool> OnSwappingCall;

    public bool CanSwap { get; set; } = true;
    bool wantsToSwap;

    [SerializeField] PhysicMaterial physicMaterial;
    Collider col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        // Get the input action.
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

    private void FixedUpdate()
    {
        // Move the player.
        rb.AddForce(movement * movementSpeed);
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
            rb.useGravity = false;
            bool storeJump = canJump;
            canJump = false;
            rb.AddForce(dashDirection * dashForce, ForceMode.Impulse);
            // Zero out y velocity.
            Vector2 velo = rb.velocity;
            velo.y = 0f;
            rb.velocity = velo;
            StartCoroutine(StartDashTimer(dashDuration, storeJump));
        }
    }

    IEnumerator StartDashTimer(float dashDuration, bool jumpStore)
    {
        while (dashDuration > 0)
        {
            dashDuration -= Time.deltaTime;
            yield return null;
        }
        rb.useGravity = true;
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
            canJump = currentNumberOfJumps <= 0 ? false : true;
        }
    }
    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode.Impulse);
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
