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

    public int PlayerAmount = 0;

    public event Action<int> OnNextPlayerJoined;
         
    private void Awake()
    {
        inputManager = GetComponent<PlayerInputManager>();
   
    }

    private void Update()
    {
        if (players != null && players.Length == 2 && players[0] != null && players[1] != null)
        {
            targetGroupHelper.position = 
                new Vector2((players[0].transform.position.x + players[1].transform.position.x) * 0.5f,targetGroupHelper.position.y);
        }
        if (players == null || players.Length != 2) return;

        // Check for players swapping:
        if (justSwapped == false && 
            players[0].WantsToSwap && players[1].WantsToSwap &&
            players[0].CanSwap && players[1].CanSwap)
        {
            // position change:
            Vector3 tmpPos = players[0].transform.position;
            players[0].TeleportPlayer(players[1].transform.position);
            players[0].Rb.position = players[1].transform.position;
            players[1].TeleportPlayer(tmpPos);
            players[1].Rb.position = tmpPos;

            // velo change:
            Vector2 tmpVelocity = players[0].Rb.velocity;
            players[0].Rb.velocity = players[1].Rb.velocity;
            players[1].Rb.velocity = tmpVelocity;

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
            players = FindObjectsOfType<PlayerController>();
            if (players.Length == 2)
            {
                OnNextPlayerJoined?.Invoke(2);
                targetGroup.m_Targets[0].target = players[0].transform;
                targetGroup.m_Targets[1].target = players[1].transform;
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
        if (players.Length == 2)
        {
            if (isTop)
            {
                if (!isSwapped) return players[0];
                else return players[1];
            }
            else
            {
                if (!isSwapped) return players[1];
                else return players[0];
            }
        }
        else
        {
            Debug.Log("Two players needs to be joined");
            return null;
        }
    }
        
}
