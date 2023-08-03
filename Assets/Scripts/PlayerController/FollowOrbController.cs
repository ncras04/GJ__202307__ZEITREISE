using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowOrbController : MonoBehaviour
{
    // Targetgroup and zoom:
    [SerializeField] CinemachineTargetGroup targetGroup;
    [SerializeField] float maxZoomLevel = 15f;
    [SerializeField] float minZoomLevel = 5f;
    [SerializeField] AnimationCurve zoomCurve;
    [SerializeField] float autoZoomSpeed = 3f;
    float currentZoom;
    bool useAutoZoom = true;
    // Navigation:
    [SerializeField] bool useNavigationCommands = true;
    [SerializeField, Tooltip("Range within current destination is reached.")] float stopRange;
    [SerializeField, Tooltip("Start with a specific command."), Min(0)] int currentCommandIndex;
    [SerializeField] List<NavigationCommand> navigationCommands;
    NavigationCommand currentCommand;

    // Connection to orb displayed:
    [SerializeField] ParticleSystem connectionPlayer1Start;
    [SerializeField] ParticleSystem connectionPlayer1End;
    [SerializeField] ParticleSystem connectionPlayer2Start;
    [SerializeField] ParticleSystem connectionPlayer2End;


    PlayerManager playerManager;
    Coroutine currentRoutine;


    private void Awake()
    {
        playerManager = GameObject.FindObjectOfType<PlayerManager>();

        currentZoom = targetGroup != null ? targetGroup.m_Targets[2].radius : 7.5f;
        currentZoom = Mathf.Clamp(currentZoom, minZoomLevel, maxZoomLevel);
    }

    // Start is called before the first frame update
    void Start()
    {
        stopRange *= stopRange;
        currentCommandIndex = currentCommandIndex < navigationCommands.Count ? currentCommandIndex : 0;
        if(navigationCommands != null && navigationCommands.Count > 0)
        {
            currentCommand = navigationCommands[currentCommandIndex];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(useNavigationCommands  && currentCommand != null)
        {
            // Zoom in on current command position.
            if (!currentCommand.ZoomExecuted)
            {
                StartCoroutine(ZoomAtTarget(currentCommand.zoomDuration));
            }
            if (!currentCommand.ReachedDestination)
            {
                ReachedDestinationCheck();
                if(Vector3.SqrMagnitude(currentCommand.destination.position - transform.position) > stopRange)
                    transform.Translate((currentCommand.destination.position - transform.position).normalized * currentCommand.travelSpeed * Time.deltaTime);
            }else if(!currentCommand.IsFinished)
            {
                if(currentRoutine == null)
                    currentRoutine = StartCoroutine(IdleAtTarget(currentCommand.targetIdleDuration));
                
            }
            else if(currentCommand.IsFinished)
            {
                SetNextCommand();
            }   
        }
        else
        {
            // Update transform based on player positions.
            if (playerManager != null && playerManager.PlayersReady && playerManager.Players[0] != null && playerManager.Players[1] != null)
            {
                transform.position =
                    new Vector3((playerManager.Players[0].transform.position.x + playerManager.Players[1].transform.position.x) * 0.5f, transform.position.y,transform.position.z);
            }
        }

        if (playerManager == null || !playerManager.PlayersReady || playerManager.Players[0] == null || playerManager.Players[1] == null) return;

        // Update zoomlevel:
        if (useAutoZoom)
        {
            // some remapping from player distance to zoomlevel.
            float playerDistance = Mathf.Abs(playerManager.Players[0].transform.position.x - playerManager.Players[1].transform.position.x);
            playerDistance = Mathf.Clamp(playerDistance, 7f, 15f);
            playerDistance -= 7f;
            if(currentZoom < 7.5f)
            {
                currentZoom += Time.deltaTime * autoZoomSpeed;
            }else if(currentZoom > 10f)
            {
                currentZoom -= Time.deltaTime * autoZoomSpeed;
            }
            else
            {
                currentZoom = Mathf.Lerp(7.5f, 10f, zoomCurve.Evaluate(playerDistance / 8f));
            }
            
            targetGroup.m_Targets[2].radius = currentZoom;
        }

        // Update player connections:
        UpdatePlayerConnection(playerManager.Players[0].transform, connectionPlayer1Start, connectionPlayer1End);
        UpdatePlayerConnection(playerManager.Players[1].transform, connectionPlayer2Start, connectionPlayer2End);        
    }

    private void UpdatePlayerConnection(Transform player, ParticleSystem startPS, ParticleSystem endPS)
    {
        float distanceToOrb = Mathf.Abs(player.position.x - transform.position.x);
        if (distanceToOrb > 8.5f)
        {

            startPS.transform.up = (player.position - transform.position);
            if (!startPS.isPlaying)
                startPS.Play();
            endPS.transform.position = player.position + new Vector3(0f, 0.5f, -0.2f);
            endPS.transform.up = startPS.transform.up * -1;
            if (!endPS.isPlaying)
                endPS.Play();
        }
        else
        {
            if (startPS.isPlaying)
            {
                startPS.Stop();
                endPS.Stop();
            }
        }
    }

    void SetNextCommand()
    {
        currentCommandIndex++;
        if (currentCommandIndex >= navigationCommands.Count) {
            currentCommand = null;
            Debug.Log("No movement commands left");
        }
        else
        {
            currentCommand = navigationCommands[currentCommandIndex];
        }
    }

    public void ExecuteNavigationCommand(Transform target, bool followTarget, float travelSpeed, float targetIdleDuration, float zoomLevel, float zoomSpeed)
    {
        useNavigationCommands = true;
        currentCommand = new NavigationCommand(target, followTarget, travelSpeed, targetIdleDuration, zoomLevel, zoomSpeed);
    }

    IEnumerator IdleAtTarget(float duration)
    {
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            yield return null;
        }
        currentCommand.IsFinished = true;
        currentRoutine = null;
    }

    IEnumerator ZoomAtTarget(float zoomDuration)
    {
        currentCommand.ZoomExecuted = true;
        if(currentCommand.zoomDuration == 0)
            yield break;
        useAutoZoom = false;
        currentCommand.zoomLevel = Mathf.Clamp(currentCommand.zoomLevel, minZoomLevel, maxZoomLevel);

        float zoomTimer = 0f;
        currentZoom = targetGroup.m_Targets[2].radius;

        while (zoomTimer < zoomDuration)
        {
            zoomTimer += Time.deltaTime;
            if(targetGroup != null)
            {
                currentZoom = Mathf.Lerp(currentZoom, currentCommand.zoomLevel, zoomCurve.Evaluate(zoomTimer / zoomDuration));
                targetGroup.m_Targets[2].radius = currentZoom;
            }
            yield return null;
        }
        useAutoZoom = true;
    }

    public void ReachedDestinationCheck()
    {
        if (currentCommand.ReachedDestination || currentCommand.followDestination) return;
        currentCommand.ReachedDestination = Vector3.SqrMagnitude(currentCommand.destination.position - transform.position) <= stopRange;

    }


    [System.Serializable]
    class NavigationCommand
    {
        public Transform destination;
        public bool followDestination;
        private bool reachedDestination;
        public float travelSpeed = 3f;
        public float targetIdleDuration = 5f;
        private bool isFinished;
        public float zoomLevel = 7.5f;
        [Min(0)]public float zoomDuration = 3f;

        public bool IsFinished { get => isFinished; set => isFinished = value; }
        public bool ZoomExecuted { get; set ; }
        public bool ReachedDestination { get => reachedDestination; set => reachedDestination = value; }

        public NavigationCommand(Transform target, bool followTarget, float travelSpeed, float targetIdleDuration, float zoomLevel, float zoomSpeed)
        {
            destination = target;
            followDestination = followTarget;
            this.travelSpeed = travelSpeed;
            this.targetIdleDuration = targetIdleDuration;
            this.zoomLevel = zoomLevel;
            this.zoomDuration = zoomSpeed;
        }
    }
}
