using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using UnityEngine.InputSystem;
using WeaponSystem;

[SelectionBase, RequireComponent(typeof(PlayerInput), typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement related:")]
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float maxXVelocity = 12f;
    private Vector2 movement = Vector2.zero;
    [Header("Jump related:")]
    [SerializeField] float jumpForceUpMultiplier = 10f;
    [SerializeField] AnimationCurve jumpCurve;
    [SerializeField] int maxNumberOfJumps = 1;
    [SerializeField] float fallSpeedMultiplier = 5f;
    [SerializeField] float jumpTime = .5f;
    bool fallFaster;
    int currentNumberOfJumps;
    bool canJump = true;

    [Header("Dash related:")]
    [SerializeField] float dashDuration = 1f;
    [SerializeField] float dashForce = 10f;
    [SerializeField] AnimationCurve dashCurve;
    [SerializeField] float dashCooldown = 1f;
    bool canDash = true;

    // Make it an event.
    public event Action InteractionHandler;


    private InputActionAsset inputAsset;
    InputActionMap actionMap;
    public InputAction moveAction;
    public InputAction jumpAction;
    public InputAction swapAction;
    public InputAction interactAction;
    public InputAction dashAction;

    public event Action<bool> OnSwappingCall;
    public bool CanSwap { get; set; } = true;
    public Rigidbody Rb { get => rb; set => rb = value; }

    bool wantsToSwap;
    [Header("Particle Effects related:")]
    [SerializeField] GameObject dustLandPrefab;
    [SerializeField] ParticleSystem dustRunningEffect;
    [SerializeField] ParticleSystem dashEffect;
    [SerializeField] GameObject swapDesireEffect;
    [SerializeField] GameObject swapEffect;
    
    [SerializeField] SoundFXRequestCollection sfx;
    [SerializeField] AudioEvent landingSound;
    [SerializeField] AudioEvent walkingSound;
    [SerializeField] AudioEvent jumpSound;
    [SerializeField] AudioEvent switchSound;
    

    [SerializeField] PhysicMaterial physicMaterial;
    Rigidbody rb;
    Collider col;

    float moveCheckTimer = 4f;

    public float CurrentMovementSpeed => Rb.velocity.sqrMagnitude;

    public bool WantsToSwap { get => wantsToSwap; set => wantsToSwap = value; }

    public bool IsJumping;
    public bool IsDashing;
    
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
        moveAction.performed += (context) =>
        {
            movement.x = context.ReadValue<Vector2>().x;
            // Check if rotation really works...
            if (movement.x > 0) transform.rotation = Quaternion.Euler(0f, 90f, 0f);
            else if (movement.x < 0)
                transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        };
        moveAction.canceled += (context) =>
        {
            // Check if rotation really works...
            movement = Vector2.zero;
        };
        moveAction.Enable();

        jumpAction = actionMap.FindAction("Jump");
        jumpAction.performed += OnJump;
        jumpAction.Enable();

        swapAction = actionMap.FindAction("Swap");
        swapAction.started += (context) => { if (swapDesireEffect != null) swapDesireEffect.SetActive(true); };
        swapAction.performed += (context) =>{
            wantsToSwap = true;      
        };
        swapAction.canceled += (context) =>{
            wantsToSwap = false;
            if (swapDesireEffect != null)
                swapDesireEffect.SetActive(false);
        };
        swapAction.Enable();

        interactAction = actionMap.FindAction("Interact");
        interactAction.performed += OnInteract;
        interactAction.Enable();

        dashAction = actionMap.FindAction("Dash");
        dashAction.performed += OnDash;
        dashAction.Enable();
        
        GetComponent<PlayerInput>().DeactivateInput();
    }

    private Vector2 lastLookAtVeloctiy;

    private void Update()
    {
        if (Rb.velocity.x > 0) transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        else if (Rb.velocity.x < 0)
            transform.rotation = Quaternion.Euler(0f, -90f, 0f);

        if (movement != Vector2.zero) lastLookAtVeloctiy = movement;
        
        if (lastLookAtVeloctiy.x > 0) transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        else if (lastLookAtVeloctiy.x < 0)
            transform.rotation = Quaternion.Euler(0f, -90f, 0f);

        if (rb.velocity.x > .2f && dustRunningEffect != null)
            dustRunningEffect.Play();
        else
            dustRunningEffect.Stop();

        // Delete later:
        CheckPlayerMovement();
    }

    void CheckPlayerMovement()
    {
        if(rb.velocity.magnitude == 0)
        {
            moveCheckTimer -= Time.deltaTime;
            if(moveCheckTimer < 0)
            {
                rb.AddForce(Vector2.up * 15f,ForceMode.Impulse);
                moveCheckTimer = 4f;
            }
        }
        else
        {
            moveCheckTimer = 4f;
        }
    }

    private void FixedUpdate()
    {
        // Move the player.
        if(Mathf.Abs(rb.velocity.x) < maxXVelocity)
            Rb.AddForce(movement * movementSpeed);
        if (fallFaster)
        {
            Rb.AddForce(Vector2.down * fallSpeedMultiplier);
        }
        
        //var tmp = rb.velocity.normalized;
        // rb.velocity = Vector3.Min(rb.velocity, tmp * maxVelocity);
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
            IsDashing = true;
            IsJumping = false;
            if (dashEffect != null)
                dashEffect.Play();
        }
    }

    IEnumerator StartDashTimer(float dashDuration, bool jumpStore)
    {
        float dashTimer = 0f;
        while (dashTimer < dashDuration)
        {
            dashTimer += Time.deltaTime;
            Rb.velocity = new Vector2(dashCurve.Evaluate(dashTimer / dashDuration) * dashForce * (transform.rotation == Quaternion.Euler(0f, 90f, 0f) ? 1f : -1f), 0f);
            //dashDuration -= Time.deltaTime;
            yield return null;
        }

        Rb.useGravity = true;
        canJump = jumpStore;
        StartCoroutine(StartDashCooldown(dashCooldown));
    }

    IEnumerator StartDashCooldown(float dashCooldown)
    {
        IsDashing = false;
        
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
            StartCoroutine(StartJumpSpeedUp(jumpTime));
            currentNumberOfJumps--;
            //canJump = currentNumberOfJumps <= 0 ? false : true;

            sfx.Add(AudioSFX.Request(jumpSound));
            
            IsJumping = true;
        }
    }

    IEnumerator StartJumpSpeedUp(float jumpTime)
    {
        float timer = 0f;
        while (timer < jumpTime)
        {
            timer += Time.deltaTime;
            rb.velocity += new Vector3(0f, jumpCurve.Evaluate(timer / jumpTime) * jumpForceUpMultiplier * Time.deltaTime,0f);
            //rb.AddForce(jumpCurve.Evaluate(timer / jumpTime) * jumpForceUpMultiplier * Vector2.up);
            yield return null;
        }
        fallFaster = true;
    }

    public void ResetJumps()
    {
        canJump = true;
        currentNumberOfJumps = maxNumberOfJumps;
        col.material = null;
        fallFaster = false;
        Instantiate(dustLandPrefab, transform.position, Quaternion.identity);
        
        sfx.Add(AudioSFX.Request(landingSound));
        IsJumping = false;
    }

    public void LeftGround()
    {
        col.material = physicMaterial;
    }

    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.TryGetComponent(out Diamond controller))
        {
            controller.Explode(10, collision.GetContact(0).point, 0.6f, 2f);
            CameraShaker.Instance.ShakeCamera(2f, 0.5f);
        }
    }

    public void TeleportPlayer(Vector3 newPosition)
    {
        transform.position = newPosition;
        if(swapEffect != null)
        {
            Instantiate(swapEffect, transform.position + new Vector3(0f,.5f,-.2f), Quaternion.identity);
        }
        
        sfx.Add(AudioSFX.Request(switchSound));
    }
}
