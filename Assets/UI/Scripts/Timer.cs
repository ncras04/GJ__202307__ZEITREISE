using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Countdown;

public class Timer : MonoBehaviour
{
    private TextMeshProUGUI m_timerText = null;

    
    // Start is called before the first frame update
    void Start()
    {
        m_timerText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        m_timerText.text = GameManager.Instance.TimeHighscore.GetDisplayTime();
    }

}
