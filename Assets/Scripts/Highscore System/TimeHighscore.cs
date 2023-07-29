using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimeHighscore : MonoBehaviour
{
    [SerializeField] private float animationTime;
    [SerializeField] AnimationCurve lerpingCurve = AnimationCurve.Linear(0, 0, 1, 1);

    private float _highscoreTime;

    private float _animatedTime;

    private Coroutine _lerpingTime;


    private Dictionary<float, string> _highScores = new Dictionary<float, string>();

    public Dictionary<float, string> HighScores
    {
        get
        {
            if (!highScoreHasAlreadyBeenLoaded)
            {
                Load();       
            }
            return _highScores;
        }
    }

    private bool highScoreHasAlreadyBeenLoaded = false;

    private void Update()
    {
        _highscoreTime += Time.deltaTime;

        if (_lerpingTime != null)
        {
            _animatedTime += Time.deltaTime;
        }
    }

    public void AddCustomTime(float amount)
    {
        _highscoreTime += amount;

        if (_lerpingTime != null)
        {
            StopCoroutine(_lerpingTime);
            _lerpingTime = null;
        }

        _lerpingTime = StartCoroutine(AnimateTime());
    }

    private IEnumerator AnimateTime()
    {
        var startedValue = _animatedTime;
        var timer = 0.0f;

        while (timer < animationTime)
        {
            timer += Time.deltaTime;
            _animatedTime = Mathf.Lerp(startedValue, _highscoreTime, lerpingCurve.Evaluate(timer / animationTime));
            yield return null;
        }

        _animatedTime = _highscoreTime;
        _lerpingTime = null;
    }

    public void SaveHighscore(string title)
    {
        _highScores.Add(_highscoreTime, title);

        var _highscoreList = _highScores.ToList();

        _highscoreList = _highScores.OrderBy(s => s.Key).ToList();
        
        _highScores.Clear();

        int index = 0;

        foreach (var highScore in _highscoreList)
        {
            _highScores.Add(highScore.Key, highScore.Value);
            PlayerPrefs.SetString("highScore" + index + "title", highScore.Value);
            PlayerPrefs.SetFloat("highScore" + index + "time", highScore.Key);

            index++;
        }
        
        PlayerPrefs.SetInt("amounts", index);
        PlayerPrefs.Save();
        
        highScoreHasAlreadyBeenLoaded = false;
    }

    private void Load()
    {
        if (!PlayerPrefs.HasKey("amount"))
        {
            Debug.LogWarning("No Highscores found!");
            return;
        }
        
        _highScores = new Dictionary<float, string>();
        int amount = PlayerPrefs.GetInt("amounts");

        for (int index = 0; index < amount; index++)
        {
            var title = PlayerPrefs.GetString("highScore" + index + "title");
            var value = PlayerPrefs.GetFloat("highScore" + index + "time");
            _highScores.Add(value, title);
        }

        highScoreHasAlreadyBeenLoaded = true;
    }

    public string GetDisplayTime()
    {
        int minutes = (int) (animationTime / 60);
        int seconds = (int) (animationTime % 60);

        return $"{GetSingleTimeNumber(minutes)}:{GetSingleTimeNumber(seconds)}";
    }

    private string GetSingleTimeNumber(int number)
    {
        if (number < 10)
        {
            return "0" + number.ToString();
        }
        else
        {
            return number.ToString();
        }
    }

    private void OnDestroy()
    {
        StopCoroutine(_lerpingTime);
    }
}