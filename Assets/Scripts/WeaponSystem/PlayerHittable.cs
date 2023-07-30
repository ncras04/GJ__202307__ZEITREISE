using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponSystem;

public class PlayerHittable : MonoBehaviour, IHittable
{
    [SerializeField] private GlobalInventory _globalInventory;

    public void OnHit(float damage)
    {
        _globalInventory.Health -= damage;
    }
}
