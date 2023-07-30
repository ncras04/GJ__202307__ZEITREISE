using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponSystem;

public class PlayerHittable : MonoBehaviour, IHittable
{
    [SerializeField] private GlobalInventory _globalInventory;
    [SerializeField] private GameObject replacedElement;

    public void OnHit(float damage)
    {
        if (_globalInventory.IsDeath)
        {
            return;
        }
        
        _globalInventory.Health -= damage;
        CameraShaker.Instance.ShakeCamera(3, .5f);

        if (_globalInventory.IsDeath)
        {
            Instantiate(replacedElement, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
