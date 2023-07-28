using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private float fireRate;

    private float _shotTimer;

    private bool CanFire => _shotTimer > 1 / fireRate;

    private void Awake()
    {
        Setup();
        
        SubscribeInputs();
    }


    void Update()
    {
        if (!CanFire)
        {
            _shotTimer += Time.deltaTime;
        }

        if (CanFire)
        {
            Shoot();
        }
        
        //Needs to replace with new Input System
    }

    private void Shoot()
    {
        
    }

    private void Setup()
    {
        _shotTimer = 0;
    }

    private void SubscribeInputs()
    {
        //Check if player Future!
        
        //if true -> Subscribe
        
        
    }
}