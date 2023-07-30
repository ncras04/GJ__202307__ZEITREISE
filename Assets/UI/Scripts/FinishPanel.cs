using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FinishPanel : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField m_name = null;
    [SerializeField]
    private TextMeshProUGUI m_time = null;

    private void Start()
    {
        m_time.text = GameManager.Instance.TimeHighscore.GetDisplayTime();
    }

    public void AddNewHighscore()
    {
        GameManager.Instance.TimeHighscore.SaveHighscore(m_name.text);
    }

    public void RandomName()
    {
        string name = "Player" + Random.Range(1, 100);

         m_name.text = name;
    }
}
