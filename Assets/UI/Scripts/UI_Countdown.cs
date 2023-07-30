using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using TMPro;
using UnityEngine;

public class UI_Countdown : MonoBehaviour
{
    [Header("Elements")] [SerializeField] private GameObject waitingForPlayer;

    [Header("Animation Curve")] [SerializeField]
    private AnimationCurve m_animationCurve;

    [Header("Countdown")] [SerializeField] private float m_timeForCountdown = 3.0f;

    [Header("Values")] [SerializeField] private float m_graphValue = 1.0f;
    [SerializeField] private float m_minFontSize = 150.0f;
    [SerializeField] private float m_maxFontSize = 300.0f;

    [SerializeField] private TextMeshProUGUI m_CountdownText = null;

    private bool m_startAnimation = true;

    private const string preTextWatingPlayers = "Waiting for Players...";

    // Start is called before the first frame update
    void Start()
    {
        m_CountdownText.text = string.Format("{0:0}", m_timeForCountdown);

        GameManager.Instance.TimeManager.OnTimerStarted += OnTimerStarted;
        GameManager.Instance.TimeManager.OnTimerEnds += OnTimerEnds;

        GameManager.Instance.OnGameStateChanged += state =>
        {
            switch (state)
            {
                case GameState.WaitingForPlayers:
                    m_CountdownText.gameObject.SetActive(false);
                    waitingForPlayer.SetActive(true);
                    waitingForPlayer.GetComponent<TMP_Text>().text = $"{preTextWatingPlayers} \n 0 of 2";
                    break;
                case GameState.CountDown:
                    m_CountdownText.gameObject.SetActive(true);
                    waitingForPlayer.SetActive(false);
                    break;
            }
        };

        GameManager.Instance.PlayerManager.OnNextPlayerJoined += amount =>
            waitingForPlayer.GetComponent<TMP_Text>().text = $"{preTextWatingPlayers} \n {amount} of 2";
    }

    private void OnTimerStarted()
    {
        m_startAnimation = true;
    }

    private void OnTimerEnds()
    {
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        m_timeForCountdown = (3 - GameManager.Instance.TimeManager.Timer);
        //m_CountdownText.text = string.Format("{0:0}", GameManager.Instance.TimeManager.Timer.ToString());

        //m_timeForCountdown -= Time.deltaTime;
        var seconds = m_timeForCountdown % 60;
        m_CountdownText.text = string.Format("{0:0}", seconds);

        if (m_startAnimation)
        {
            m_graphValue = m_animationCurve.Evaluate(Time.time);
            m_CountdownText.fontSize = Mathf.Lerp(m_minFontSize, m_maxFontSize, m_graphValue);
        }
    }
}