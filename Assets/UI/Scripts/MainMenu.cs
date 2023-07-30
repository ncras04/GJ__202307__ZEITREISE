using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject m_CreditPanel = null;

    public void StartGame()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void Credits()
    {
        if (m_CreditPanel.activeSelf) 
            m_CreditPanel.SetActive(false);
        else
            m_CreditPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}
