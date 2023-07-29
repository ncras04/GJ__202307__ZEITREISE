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

    private bool m_isGlowingRed = false;

    private Image m_image = null;

    public bool IsGlowingRed
    {
        set { m_isGlowingRed = value;}
    }

    // Start is called before the first frame update
    void Start()
    {
        m_image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if(m_isGlowingRed)
        {
            m_glowingValue = m_animationCurve.Evaluate(Time.time);
            m_image.color = Color.Lerp(m_mainColor, m_redColor, m_glowingValue);
        }
    }

    public void ResetColor()
    {
        m_image.color = m_mainColor;
    }
}
