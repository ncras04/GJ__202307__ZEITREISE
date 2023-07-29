using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private PlayerController[] players;
    bool swappingDesirePlayerOne;
    bool swappingDesirePlayerTwo;

    bool isSwapped = false;

    PlayerInputManager inputManager;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private CinemachineTargetGroup targetGroup;


    private void Awake()
    {
        inputManager = GetComponent<PlayerInputManager>();
        

        
    }
    
    public void SwapPlayers()
    {
        if (swappingDesirePlayerOne && swappingDesirePlayerTwo && players[0].CanSwap && players[1].CanSwap)
        {
            Vector3 tmpPos = players[0].transform.position;
            Vector2 tmpVelocity = players[0].Rb.velocity;
            players[0].transform.position = players[1].transform.position;
            players[0].Rb.velocity = players[1].Rb.velocity;
            players[1].transform.position = tmpPos;
            players[1].Rb.velocity = tmpVelocity;

            Debug.Log("Players swapped!");
            isSwapped = !isSwapped;
            // Swap players in array directly.
            // Handle velocity bla.
        }
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        if (inputManager.playerCount == 1)
        {
            inputManager.playerPrefab = playerPrefab;
            //targetGroup.m_Targets[0].target = playerPrefab.transform;
        }
        else if (inputManager.playerCount == 2)
        {
            //targetGroup.m_Targets[1].target = playerPrefab.transform;

            players = FindObjectsOfType<PlayerController>();
            if (players.Length == 2)
            {
                targetGroup.m_Targets[0].target = players[0].transform;
                targetGroup.m_Targets[1].target = players[1].transform;
                players[0].OnSwappingCall += (swappingDesire) =>
                {
                    swappingDesirePlayerOne = swappingDesire;
                    SwapPlayers();
                };
                players[1].OnSwappingCall += (swappingDesire) =>
                {
                    swappingDesirePlayerTwo = swappingDesire;
                    SwapPlayers();
                };
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
        
}
