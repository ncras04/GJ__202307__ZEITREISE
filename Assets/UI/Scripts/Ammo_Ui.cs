using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ammo_Ui : MonoBehaviour
{
    public GlobalInventory m_inventory;

    private TextMeshProUGUI m_ammoText = null;

    // Start is called before the first frame update
    void Start()
    {
        m_ammoText = GetComponent<TextMeshProUGUI>();

        m_ammoText.text = m_inventory.Ammonition.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
