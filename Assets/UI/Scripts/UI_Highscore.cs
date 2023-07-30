using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Highscore : MonoBehaviour
{
    [SerializeField]
    private List<HighscorePlace> m_places = new List<HighscorePlace>();
    [SerializeField]
    private float m_timeToReturn = 5.0f;
    [SerializeField]
    private GameObject m_panel = null;

    private Dictionary<float, string> m_highScores = new Dictionary<float, string>();
    private List<float> m_dicValues = new List<float>();

    private bool m_finishedLevel = false;

    private void Awake()
    {

        
    }

    // Start is called before the first frame update
    void Start()
    {
        m_panel.SetActive(false);
        //GameManager.Instance.TimeHighscore.SaveHighscore("Player1");

        GetHighscoreValues();
        DisplayHighscore();

        //GameManager.Instance.TimeHighscore.OnAddNewHighscore += AddNewHighscore;
    }

    private void AddNewHighscore()
    {
        GetHighscoreValues();
        DisplayHighscore ();

        m_panel.SetActive (true);

        m_finishedLevel = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_finishedLevel)
        {
            m_timeToReturn -=Time.deltaTime;

            if(m_timeToReturn <= 0)
            {
                m_finishedLevel=false;
                SceneManager.LoadScene(0);
            }
        }
    }

    private void GetHighscoreValues()
    {
        m_highScores.Clear();
        m_dicValues.Clear();

        LoadPlayerPrefs();
        //m_highScores = GameManager.Instance.TimeHighscore.HighScores;
        m_dicValues = m_highScores.Keys.ToList() ;

        m_dicValues.Sort();
    }

    private void DisplayHighscore()
    {
        int i = 0;
        foreach (var key in m_dicValues)
        {
            if (i <= m_places.Count - 1)
            {
                m_places[i].ChangePlace(m_highScores[key], key);
                i++;
            }
        }

        if (i <= m_places.Count - 1)
        {
            for (int j = i; j < m_places.Count; j++)
            { 
                m_places[j].ChangePlace("---", 0);
            }
        }
    }

    private void LoadPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey("amount"))
        {
            Debug.LogWarning("No Highscores found!");
            return;
        }

        m_highScores = new Dictionary<float, string>();
        int amount = PlayerPrefs.GetInt("amounts");

        for (int index = 0; index < amount; index++)
        {
            var title = PlayerPrefs.GetString("highScore" + index + "title");
            var value = PlayerPrefs.GetFloat("highScore" + index + "time");
            m_highScores.Add(value, title);
        }
    }
}