using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Bullet : MonoBehaviour
{
    [Header("Animation Curve")]
    [SerializeField]
    private AnimationCurve m_animationCurve;

    [Header("Values")]
    [SerializeField]
    private float m_glowingValue = 1.0f;
    
    private Color m_mainColor = Color.white;
    private Color m_redColor = Color.red;
    private Color m_whiteColor = new Color(0.5f, 0.5f, 0.5f);

    //[SerializeField]
    private bool m_isGlowingRed = false;
    //[SerializeField]
    private bool m_isGlowingWhite = false;

    private Image m_image = null;

    public bool IsGlowingRed
    {
        set { m_isGlowingRed = value;}
    }

    public bool IsGlowingWhite
    {
        set { m_isGlowingWhite = value;}
    }

    // Start is called before the first frame update
    void Start()
    {
        m_image = GetComponent<Image>();
        m_image.color = m_mainColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isGlowingRed )
        { 
            m_glowingValue = m_animationCurve.Evaluate(Time.time);
            m_image.color = Color.Lerp(m_mainColor, m_redColor, m_glowingValue);
        }
        else if (m_isGlowingWhite )
        {
            m_glowingValue = m_animationCurve.Evaluate(Time.time);
            m_image.color = Color.Lerp(m_mainColor, m_whiteColor, m_glowingValue);
        }
    }

    public void ResetColor()
    {
        m_image.color = m_mainColor;
    }
}
