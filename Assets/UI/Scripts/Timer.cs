using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Countdown;

public class Timer : MonoBehaviour
{
    private float m_timer = 0.0f;
    private TextMeshProUGUI m_timerText = null;

    // Start is called before the first frame update
    void Start()
    {
        m_timerText = GetComponent<TextMeshProUGUI>();

        m_timer = 0.0f;   
    }

    // Update is called once per frame
    void Update()
    {
        m_timer += Time.deltaTime;

        DisplayTime();
    }

    private void DisplayTime()
    {
        var minutes = Mathf.FloorToInt(m_timer / 60);
        var seconds = Mathf.FloorToInt(m_timer % 60);
        var milleseconds = m_timer % 1 * 1000;

        m_timerText.text = string.Format("{0:00}:{1:00},{2:00}", minutes, seconds, milleseconds);
    }
}
