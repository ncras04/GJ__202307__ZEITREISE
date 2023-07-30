using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FinishPanel : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_time = null;
    [SerializeField]
    private TMP_InputField m_name = null;

    [SerializeField]
    private UI_Highscore m_highscorePanel = null;

    private void Start()
    {
        m_time.text = TimeHighscore._highscoreTime.ToString();
    }

    public void AddNewHighscore()
    {
        //GameManager.Instance.TimeHighscore.SaveHighscore(m_name.text);
        m_highscorePanel.transform.parent.gameObject.SetActive(true);
        m_highscorePanel.AddNewHighscore(m_name.text, TimeHighscore._highscoreTime);
        //m_highscorePanel.AddNewHighscore();
        gameObject.SetActive(false);
    }

    public void RandomName()
    {
        string name = "Player" + Random.Range(1, 100);

         m_name.text = name;
    }

    public void FinishedGame()
    {
        //m_time.text = GameManager.Instance.TimeHighscore.GetDisplayTime();
    }
}
