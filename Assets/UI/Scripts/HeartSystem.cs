using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartSystem : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> m_hearts = new List<GameObject>();
    [SerializeField]
    private float m_life = 3.0f;

    private GlobalInventory m_inventory = null;

    private void Start()
    {
        for (int i = 0; i < (int)m_life * 2 - 1; i += 2)
        {
            if (gameObject.transform.GetChild(i).gameObject != null)
            {
                m_hearts.Add(gameObject.transform.GetChild(i).gameObject);
                m_hearts.Add(gameObject.transform.GetChild(i + 1).gameObject);
            }
            else
            {
                break;
            }
        }

        //Instantiate(m_inventory);
        
        //m_life = m_inventory.Health;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void Damage()
    {

        for(int i = m_hearts.Count - 1;i >= 0;i--)
        {
            if (m_hearts[i].gameObject != null)
            {
                Destroy(m_hearts[i].gameObject);
                break;
            }
        }
    }
}
