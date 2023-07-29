using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponSystem;

public class Spikes : MonoBehaviour
{
    public float Damage = 1000;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            var component = other.gameObject.GetComponent<DestroyableObject>();
            component.OnHit(component.CurrentHealth);
            //Debug.Log(component.CurrentHealth);
        }
    }
}
