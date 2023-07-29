using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighscoreTextSetter : MonoBehaviour
{
    [SerializeField] private TimeHighscore _highscore;

    [SerializeField] private TMP_Text highscoreText;
    
    void Update()
    {
        if (!highscoreText || !_highscore)
        {
            return;
        }
        
        highscoreText.text = _highscore.GetDisplayTime();
    }
}