using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using DG.Tweening;
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
    bool doGroundCheck = true;
    bool inAir;
    public bool IsJumping;
    bool playJumpFeedback = true;

    [Header("Dash related:")]
    [SerializeField] float dashDuration = 1f;
    [SerializeField] float dashForce = 10f;
    [SerializeField] AnimationCurve dashCurve;
    [SerializeField] float dashCooldown = 1f;
    bool canDash = true;
    public bool IsDashing;


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

    [Header("Tweening Feedback related:")]
    [SerializeField] bool useTweenFeedback = true;
    [SerializeField] Transform tweenTransform;
    Tween landingTween;
    Tween scalingXTween;
    Tween scalingYTween;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        col.material = basePhysicsMaterial;

        // Get the input action.
        inputAsset = GetComponent<PlayerInput>().actions;
        actionMap = inputAsset.FindActionMap("Player");
        actionMap.Enable();

        //landingTween = transform.DOShakeScale(.5f);
        if(tweenTransform == null)
        {
            useTweenFeedback = false;
        }
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
            bool jumpStore = canJump;
            canJump = false;
            StartCoroutine(StartDashTimer(dashDuration,jumpStore));
            if (useTweenFeedback)
            {
                tweenTransform.DOScaleX(1.75f, dashDuration).SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo).SetLink(transform.gameObject);
                tweenTransform.DOScaleY(.65f, dashDuration).SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo).SetLink(transform.gameObject);
            }
                
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
            Rb.velocity = new Vector2((transform.rotation == Quaternion.Euler(0f, 90f, 0f) ? 1f : -1f) * dashForce * dashCurve.Evaluate(dashTimer / dashDuration),0f);
            yield return null;
        }
        canJump = jumpStore;
        // Eventually erease this, but sometimes no doing the ground check will prevent from jumpiing again.
        if(!inAir && !canJump)
            doGroundCheck = true;
        IsDashing = false;
        Rb.useGravity = true;
        StartCoroutine(StartDashCooldown(dashCooldown));
        tweenTransform.DORewind();
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
        if (canJump)
        {
            StartCoroutine(StartJump(jumpTime));
            StartCoroutine(StartGroundCheck());
            sfx.Add(AudioSFX.Request(jumpSound));                     
        }
    }

    IEnumerator StartJump(float jumpTime)
    {
        canJump = false;
        col.material = jumpPhysicsMaterial;
        IsJumping = true;
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
        doGroundCheck = true;      
    }

    public void ResetJumps()
    {
        canJump = true;
        IsJumping = false;
        col.material = basePhysicsMaterial;
        doGroundCheck = false;
        
        if(playJumpFeedback)
        {
            StartCoroutine(FeedbackResetTimer(0.3f));
            Instantiate(dustLandPrefab, transform.position, Quaternion.identity);
            sfx.Add(AudioSFX.Request(landingSound));
            if(useTweenFeedback)
                tweenTransform.DOScale(.7f, .1f).SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo).SetLink(transform.gameObject);
        }
        else
        {
            tweenTransform.DORewind();
        }     
    }

    void GroundCheck()
    {
        if (Physics.Raycast(groundCheckPosition.position, Vector3.down, out RaycastHit hitInfo, 3f, groundLayer))
        {
            inAir = hitInfo.distance > 0.25f ? true : false;
            if (inAir) doGroundCheck = true;
        }
        if(doGroundCheck && Physics.Raycast(groundCheckPosition.position, Vector3.down, 0.1f, groundLayer))
            ResetJumps() ;
    }

    IEnumerator FeedbackResetTimer(float timer)
    {
        playJumpFeedback = false;
        while (timer > 0f)
        {
            timer -= Time.deltaTime; yield return null;
        }
        playJumpFeedback = true;
    }

    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        if(isFuturePlayer && IsDashing && collision.transform.TryGetComponent(out IHittable controller))
        {
            //if(collision.transform.TryGetComponent(out Diamond diamond))
            //    diamond.Explode(10, collision.GetContact(0).point, 0.6f, 2f);
                
            controller.OnHit(2);
            CameraShaker.Instance.ShakeCamera(2f, 0.5f);
        }else if(!isFuturePlayer && IsDashing && (collision.transform.CompareTag("Crate") || collision.transform.CompareTag("Collectable")) && collision.transform.TryGetComponent(out IHittable target))
        {
            target.OnHit(1);
            CameraShaker.Instance.ShakeCamera(2f, 0.5f);
        }else if(collision.transform.TryGetComponent(out Diamond diamond))
        {
            diamond.transform.GetComponent<IHittable>().OnHit(1);
        }
    }

    public void TeleportPlayer(Vector3 newPosition)
    {
        transform.position = newPosition;
        if(swapEffect != null)
        {
            var effect = Instantiate(swapEffect, transform.position, Quaternion.identity);
            effect.transform.DOScale(1.2f, .4f).SetEase(Ease.Linear).SetLink(transform.gameObject);
            StartCoroutine(UpdateTeleportEffect(effect.transform));
        }
        //+new Vector3(0f, .5f, -.2f)
        sfx.Add(AudioSFX.Request(switchSound));

        if (useTweenFeedback)
            tweenTransform.DOScale(.2f, .2f).SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo).SetLink(transform.gameObject);
    }

    IEnumerator UpdateTeleportEffect(Transform teleportTransform)
    {
        float timer = 0.8f;
        while (timer > 0f)
        {
            timer -=Time.deltaTime;
            teleportTransform.position = transform.position;
            yield return null;
        }
        Destroy(teleportTransform.gameObject);
    }

    private void OnDestroy()
    {
        //scalingXTween.Kill();
        //scalingYTween.Kill();
        //landingTween.Kill();
    }
}
