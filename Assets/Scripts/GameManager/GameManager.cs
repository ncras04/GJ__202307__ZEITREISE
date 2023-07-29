using System;
using Countdown;
using UnityEngine;
using UnityEngine.InputSystem;

public enum GameState
{
    WaitingForPlayers,
    CountDown,
    Playing,
    Successful
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState GameState { set; get; } = GameState.WaitingForPlayers;

    [field: SerializeField] public PlayerManager PlayerManager { set; get; }

    [field: SerializeField] public TimeHighscore TimeHighscore { set; get; }
    [field: SerializeField] public TimeManager TimeManager { set; get; }

    [Header("Settings"), SerializeField] private float countDownTime = 3;
    
    

    public event Action OnGameStarted;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        PlayerManager.OnNextPlayerJoined += OnPlayerJoined;
    }

    private void OnPlayerJoined(int playerAmount)
    {
        foreach (var player in FindObjectsOfType<PlayerController>())
        {
            Debug.Log("Penis", player);
            player.GetComponent<PlayerInput>().DeactivateInput();
        }
        
        if (playerAmount < 2)
        {
            return;
        }

        
        TimeManager.SetTimer(countDownTime);
        TimeManager.OnTimerEnds += TimeManagerOnOnTimerEnds;
        TimeManager.StartTimer();
        
        GameState = GameState.CountDown;
    }

    private void TimeManagerOnOnTimerEnds()
    {
        if(TimeHighscore != null) TimeHighscore.StartTimer();
        Debug.Log("Test");

        PlayerManager.GetTopPlayer(true).GetComponent<PlayerInput>().ActivateInput();
        PlayerManager.GetTopPlayer(false).GetComponent<PlayerInput>().ActivateInput();

        GameState = GameState.Playing;
        
        OnGameStarted?.Invoke();
    }
}