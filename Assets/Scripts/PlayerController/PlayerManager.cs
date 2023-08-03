using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
    private PlayerController[] players;

    bool isSwapped = false;
    bool justSwapped = false;

    public PlayerInputManager inputManager;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private CinemachineTargetGroup targetGroup;
    [SerializeField] private Transform targetGroupHelper;

    public bool IsSwapped { get => isSwapped; set => isSwapped = value; }
    public PlayerController[] Players { get => players; set => players = value; }
    public bool PlayersReady { get; private set; }

    public int PlayerAmount = 0;

    public event Action<int> OnNextPlayerJoined;
         
    private void Awake()
    {
        inputManager = GetComponent<PlayerInputManager>();
   
    }

    private void Update()
    {
        //if (Players != null && Players.Length == 2 && Players[0] != null && Players[1] != null)
        //{
        //    targetGroupHelper.position = 
        //        new Vector2((Players[0].transform.position.x + Players[1].transform.position.x) * 0.5f,targetGroupHelper.position.y);
        //}
        if (Players == null || Players.Length != 2) return;

        // Check for players swapping:
        if (justSwapped == false && 
            Players[0].WantsToSwap && Players[1].WantsToSwap &&
            Players[0].CanSwap && Players[1].CanSwap)
        {
            // position change:
            Vector3 tmpPos = Players[0].transform.position;
            Players[0].TeleportPlayer(Players[1].transform.position);
            Players[0].Rb.position = Players[1].transform.position;
            Players[1].TeleportPlayer(tmpPos);
            Players[1].Rb.position = tmpPos;

            // velo change:
            Vector2 tmpVelocity = Players[0].Rb.velocity;
            Players[0].Rb.velocity = Players[1].Rb.velocity;
            Players[1].Rb.velocity = tmpVelocity;

            isSwapped = !isSwapped;
            justSwapped = true;

            StartCoroutine(StartSwapCooldown(3f));
        }
    }

    IEnumerator StartSwapCooldown(float duration)
    {
        float timer = 0f;
        while (timer <= duration) { 
            timer += Time.deltaTime;
            yield return null;
        }
        justSwapped = false;
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        if (inputManager.playerCount == 1)
        {
            inputManager.playerPrefab = playerPrefab;
            OnNextPlayerJoined?.Invoke(1);
        }
        else if (inputManager.playerCount == 2)
        {
            Players = FindObjectsOfType<PlayerController>();
            if (Players.Length == 2)
            {
                OnNextPlayerJoined?.Invoke(2);
                targetGroup.m_Targets[0].target = Players[0].transform;
                targetGroup.m_Targets[1].target = Players[1].transform;
                PlayersReady = true;
            }
            else
            {
                Debug.LogWarningFormat("not enough players.");
            }
        }
    }

    /// <summary>
    /// Gives u the top or bottom player.
    /// </summary>
    /// <param name="isTop">True if u want the top player. False for bottom player.</param>
    /// <returns>The top or bottom player.</returns>
    public PlayerController GetTopPlayer(bool isTop = true)
    {
        if (Players.Length == 2)
        {
            if (isTop)
            {
                if (!isSwapped) return Players[0];
                else return Players[1];
            }
            else
            {
                if (!isSwapped) return Players[1];
                else return Players[0];
            }
        }
        else
        {
            Debug.Log("Two players needs to be joined");
            return null;
        }
    }
        
}
