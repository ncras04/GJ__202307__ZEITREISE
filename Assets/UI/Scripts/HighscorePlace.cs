using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighscorePlace : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_name = null;
    [SerializeField]
    private TextMeshProUGUI m_time = null;
    
    public void ChangePlace(string _name, float _time)
    {
        m_name.text = _name;
        m_time.text = _time.ToString();
    }
}