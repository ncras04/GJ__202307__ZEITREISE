using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject m_highscorePanel = null;
    [SerializeField]
    private GameObject m_CreditPanel = null;

    public void StartGame()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void Highscore()
    {
        if (m_highscorePanel.activeSelf)
            m_highscorePanel.SetActive(false);
        else
        {
            CloseAllPanels();
            m_highscorePanel.SetActive(true);
        }
    }

    public void Credits()
    {

        if (m_CreditPanel.activeSelf)
            m_CreditPanel.SetActive(false);
        else
        {
            CloseAllPanels();
            m_CreditPanel.SetActive(true);
        }
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }

    private void CloseAllPanels()
    {
        m_highscorePanel.SetActive(false);
        m_CreditPanel.SetActive(false);
    }
}
