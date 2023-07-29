using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Ammo_Ui : MonoBehaviour
{
    [Header ("Scriptable Objects")]
    public GlobalInventory m_inventory;

    private TextMeshProUGUI m_ammoText = null;
    private UI_Bullet m_bullet = null;
    
    private bool m_isOutOfAmmo = false;
    private bool m_AmmoIsReached = false;

    private int m_ammo = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_ammoText = GetComponent<TextMeshProUGUI>();
        m_bullet = GetComponent<UI_Bullet>();

        m_ammoText.text = m_inventory.Ammonition.ToString();
        m_ammo = m_inventory.Ammonition;

        m_inventory.OnAmmonitionChanged += OnAmmonitionChanged;
        m_inventory.OnOutOfAmmo += OnOutOfAmmo;
        m_inventory.OnAmmoRestocked += OnAmmoRestocked;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnAmmonitionChanged(int _value)
    {
        m_ammoText.text = _value.ToString();

        if(m_isOutOfAmmo || m_AmmoIsReached)
        {
            m_bullet.ResetColor();
            m_isOutOfAmmo =false;
            m_AmmoIsReached=false;
        }
    }

    private void OnAmmoRestocked()
    {
        
    }

    private void OnOutOfAmmo()
    {
        m_bullet.IsGlowingRed = true;
        m_isOutOfAmmo = true;
    }

    public void AddAmmo()
    {
        m_ammo++;
        OnAmmonitionChanged(m_ammo);
    }

    public void SubAmmo()
    {
        m_ammo--;
        OnAmmonitionChanged(m_ammo);

        if (m_ammo == 0)
            OnOutOfAmmo();
    }
}
