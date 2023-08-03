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
    [SerializeField] bool isFuturePlayer;
    [Header("Movement related:")]
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float maxXVelocity = 12f;
    private Vector2 movement = Vector2.zero;
    private Vector2 lastLookAtVeloctiy;
    [Header("Jump related:")]
    [SerializeField] float jumpTime = 0.5f;
    [SerializeField] float jumpForce = 10f;
    [SerializeField] AnimationCurve jumpCurve;
    bool canJump;
    [SerializeField] Transform groundCheckPosition;

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
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction swapAction;
    [HideInInspector]public InputAction interactAction;
    private InputAction dashAction;

    bool wantsToSwap;
    public bool CanSwap { get; set; } = true;
    public Rigidbody Rb { get => rb; private set => rb = value; }


    [Header("Particle Effects related:")]
    [SerializeField] GameObject dustLandPrefab;
    [SerializeField] ParticleSystem dustRunningEffect;
    [SerializeField] ParticleSystem dashEffect;
    [SerializeField] GameObject swapDesireEffect;
    [SerializeField] GameObject swapEffect;
    [Header("Sound related:")]
    [SerializeField] SoundFXRequestCollection sfx;
    [SerializeField] AudioEvent landingSound;
    [SerializeField] AudioEvent walkingSound;
    [SerializeField] AudioEvent jumpSound;
    [SerializeField] AudioEvent switchSound;
    
    Rigidbody rb;
    Collider col;
    [SerializeField] PhysicMaterial basePhysicsMaterial;
    [SerializeField] PhysicMaterial jumpPhysicsMaterial;
    [SerializeField] LayerMask groundLayer;

    public float CurrentMovementSpeed => Rb.velocity.sqrMagnitude;

    public bool WantsToSwap { get => wantsToSwap; set => wantsToSwap = value; }

    public bool IsJumping;
    public bool IsDashing;
    
    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        col.material = basePhysicsMaterial;

        // Get the input action.
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

    private void Update()
    {
        if (Rb.velocity.x > 0) transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        else if (Rb.velocity.x < 0)
            transform.rotation = Quaternion.Euler(0f, -90f, 0f);

        if (movement != Vector2.zero) lastLookAtVeloctiy = movement;

        if (lastLookAtVeloctiy.x > 0) transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        else if (lastLookAtVeloctiy.x < 0)
            transform.rotation = Quaternion.Euler(0f, -90f, 0f);

        if (Mathf.Abs(rb.velocity.x) > .1f && !dustRunningEffect.isPlaying)
            dustRunningEffect.Play();
        else if (Mathf.Abs(rb.velocity.x) < .05f && dustRunningEffect.isPlaying || dustRunningEffect.isPlaying && IsJumping)
            dustRunningEffect.Stop();

        GroundCheck();
    }

    private void FixedUpdate()
    {
        // Move the player.
        if(Mathf.Abs(rb.velocity.x) < maxXVelocity)
            Rb.AddForce(movement * movementSpeed);
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
            canJump = false;

            StartCoroutine(StartDashTimer(dashDuration));
            IsDashing = true;
            IsJumping = false;
            if (dashEffect != null)
                dashEffect.Play();
        }
    }

    IEnumerator StartDashTimer(float dashDuration)
    {
        float dashTimer = 0f;
        while (dashTimer < dashDuration)
        {
            dashTimer += Time.deltaTime;
            Rb.velocity = new Vector2((transform.rotation == Quaternion.Euler(0f, 90f, 0f) ? 1f : -1f) * dashForce * dashCurve.Evaluate(dashTimer / dashDuration),0f);
            yield return null;
        }

        Rb.useGravity = true;
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
        if (canJump)
        {
            StartCoroutine(StartJump(jumpTime));
            StartCoroutine(StartGroundCheck());
            sfx.Add(AudioSFX.Request(jumpSound));                     
        }
    }

    IEnumerator StartJump(float jumpTime)
    {      
        float timer = 0f;
        while (timer < jumpTime)
        {
            timer += Time.deltaTime;
            rb.velocity += new Vector3(0f, jumpCurve.Evaluate(timer / jumpTime) * jumpForce * Time.deltaTime,0f);
            yield return null;
        }
    }
    IEnumerator StartGroundCheck()
    {
        float timer = 0.2f;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        col.material = jumpPhysicsMaterial;
        IsJumping = true;
        canJump = false;
    }

    public void ResetJumps(bool playLandSound)
    {
        canJump = true;
        Instantiate(dustLandPrefab, transform.position, Quaternion.identity);   
        if(playLandSound)
            sfx.Add(AudioSFX.Request(landingSound));
        IsJumping = false;
        //Rb.drag = 0.3f;
        col.material = basePhysicsMaterial;
    }

    public void LeftGround()
    {
        Rb.drag = 0f;
    }

    void GroundCheck()
    {
        if (!canJump  && Physics.CheckSphere(groundCheckPosition.position, 0.05f, groundLayer))
        {
            if(!IsDashing)
                ResetJumps(true);
            else
                ResetJumps(false);
        }
    }

    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        if(isFuturePlayer && IsDashing && collision.transform.TryGetComponent(out IHittable controller))
        {
            //if(collision.transform.TryGetComponent(out Diamond diamond))
            //    diamond.Explode(10, collision.GetContact(0).point, 0.6f, 2f);
                
            controller.OnHit(1);
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
