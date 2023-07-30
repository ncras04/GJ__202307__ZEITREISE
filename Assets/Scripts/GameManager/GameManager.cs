using System;
using System.Collections;
using Countdown;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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

    private GameState _gameState;

    public GameState GameState
    {
        set
        {
            _gameState = value;
            OnGameStateChanged?.Invoke(value);
        }
        get => _gameState;
    }

    [field: SerializeField] public PlayerManager PlayerManager { set; get; }
    [field: SerializeField] public TimeHighscore TimeHighscore { set; get; }
    [field: SerializeField] public TimeManager StartTimer { set; get; }

    [SerializeField] private GlobalInventory _globalInventory;
    
    [SerializeField] private float restartSceneDelay;

    [Header("Settings"), SerializeField] private float countDownTime = 3;
    
    public event Action<GameState> OnGameStateChanged;
    public event Action OnGameStarted;

    public void Restart(string reason)
    {
        TimeHighscore.Stop();
        GameState = GameState.Successful;
        PlayerManager.inputManager.DisableJoining();
        DisablePlayerInputs();

        StartCoroutine(ReloadSceneDelayed());
    }

    IEnumerator ReloadSceneDelayed()
    {
        yield return new WaitForSecondsRealtime(restartSceneDelay);
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
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
        
        _globalInventory.ResetData();
        _globalInventory.OnDeath += () => Restart("Player Death");

        PlayerManager.OnNextPlayerJoined += OnPlayerJoined;
    }

    private void OnPlayerJoined(int playerAmount)
    {
        DisablePlayerInputs();
        
        if (playerAmount < 2)
        {
            return;
        }

        StartTimer.SetTimer(countDownTime);
        StartTimer.OnTimerEnds += TimeManagerOnOnTimerEnds;
        StartTimer.StartTimer();
        
        GameState = GameState.CountDown;
    }

    private void DisablePlayerInputs()
    {
        foreach (var player in FindObjectsOfType<PlayerController>())
        {
            player.GetComponent<PlayerInput>().DeactivateInput();
        }
    }

    private void TimeManagerOnOnTimerEnds()
    {
        if(TimeHighscore != null) TimeHighscore.StartTimer();

        PlayerManager.GetTopPlayer(true).GetComponent<PlayerInput>().ActivateInput();
        PlayerManager.GetTopPlayer(false).GetComponent<PlayerInput>().ActivateInput();

        GameState = GameState.Playing;
        
        OnGameStarted?.Invoke();
    }
}