using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreTester : MonoBehaviour
{
    [SerializeField] private TimeHighscore _highscore;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _highscore.AddCustomTime(100);
        }
    }
}
