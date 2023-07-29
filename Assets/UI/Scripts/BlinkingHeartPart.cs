using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkingHeartPart : MonoBehaviour
{
    [Header("Blinking Curve")]
    public AnimationCurve m_blinkingCurve;

    private float m_blinkingValue = 1.0f;
    private bool m_isBlinking = false;
    private float m_blinkingTime = 5.0f;

    private float m_timer = 0.0f;

    public bool IsBlinking
    {
        set { m_isBlinking = value; }
    }

    public float BlinkingTime
    {
        set { m_blinkingTime = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(m_isBlinking)
        {
            m_blinkingValue = m_blinkingCurve.Evaluate(Time.time);
            transform.localScale = Vector3.one * m_blinkingValue;

            m_timer += Time.deltaTime;

            if(m_timer >= m_blinkingTime )
            {
                m_isBlinking=false;
                Destroy(this.gameObject);
            }
        }
    }
}
