using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HeartSystem : MonoBehaviour
{
    [Header("Scriptable Objects")] public GlobalInventory m_globalInventory;

    [Header("Hearts")] [SerializeField] private GameObject m_lifeHearts = null;
    [SerializeField] private GameObject m_loosingHearts = null;
    [SerializeField] private float m_blinkingTime = 5.0f;

    [Header("UI Panels")] [SerializeField] private GameObject m_failedPanel = null;

    private List<GameObject> m_lifehearts = new List<GameObject>();
    private List<BlinkingHeartPart> m_loosehearts = new List<BlinkingHeartPart>();

    private void Start()
    {
        if (m_failedPanel)
        {
            m_failedPanel.SetActive(false);
        }

        for (int i = 0; i < (int) m_globalInventory.Health * 2 - 1; i += 2)
        {
            if (m_lifeHearts.transform.GetChild(i).gameObject != null)
            {
                m_lifehearts.Add(m_lifeHearts.transform.GetChild(i).gameObject);
                m_loosehearts.Add(m_loosingHearts.transform.GetChild(i).GetComponent<BlinkingHeartPart>());
                m_lifehearts.Add(m_lifeHearts.transform.GetChild(i + 1).gameObject);
                m_loosehearts.Add(m_loosingHearts.transform.GetChild(i + 1).GetComponent<BlinkingHeartPart>());
            }
            else
            {
                break;
            }
        }

        m_globalInventory.OnGetDamage += Damage;
        m_globalInventory.OnDeath += OnDeath;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void Damage(float _damageAmount)
    {
        Debug.Log(_damageAmount.ToString());

        for (int i = m_lifehearts.Count - 1; i >= 0; i--)
        {
            if (m_lifehearts[i].gameObject != null)
            {
                Destroy(m_lifehearts[i].gameObject);

                m_loosehearts[i].BlinkingTime = m_blinkingTime;
                m_loosehearts[i].IsBlinking = true;

                if (_damageAmount > 0.5 && i > 0)
                {
                    for (int j = 1; j < _damageAmount * 2; j++)
                    {
                        Destroy(m_lifehearts[i - j].gameObject);

                        m_loosehearts[i - j].BlinkingTime = m_blinkingTime;
                        m_loosehearts[i - j].IsBlinking = true;
                    }
                }

                break;
            }
        }
    }

    private void OnDeath()
    {
        //Maybe Red blinking Hearts

        //Failed Screen

        if (m_failedPanel)
        {
            m_failedPanel.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Du hurensohn du musst das UI noch zuweisen oder erstellen. WTF");
        }
    }
}