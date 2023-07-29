using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class HighscoreTextSetter : MonoBehaviour
{
    [SerializeField] private TimeHighscore _highscore;

    [SerializeField] private TMP_Text highscoreText;
    
    void Awake()
    {
        highscoreText = GetComponent<TMP_Text>();
    }
    
    void Update()
    {
        if (!highscoreText || !_highscore)
        {
            return;
        }
        
        highscoreText.text = _highscore.GetDisplayTime();
    }
}