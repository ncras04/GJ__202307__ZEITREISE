using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private PlayerController[] players;
    bool swappingDesirePlayerOne;
    bool swappingDesirePlayerTwo;


    private void Awake()
    {
        players = FindObjectsOfType<PlayerController>();

        if(players.Length == 2 )
        {
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
    
    public void SwapPlayers()
    {
        if (swappingDesirePlayerOne && swappingDesirePlayerTwo && players[0].CanSwap && players[1].CanSwap)
        {
            Vector3 tmpPos = players[0].transform.position;
            players[0].transform.position = players[1].transform.position;
            players[1].transform.position = tmpPos;

            Debug.Log("Players swapped!");
            // Handle velocity bla.
        }
    }
}
