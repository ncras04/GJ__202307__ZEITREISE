using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using WeaponSystem;

public class PlayerHittable : MonoBehaviour, IHittable
{
    [SerializeField] private GlobalInventory _globalInventory;
    [SerializeField] private GameObject replacedElement;

    [SerializeField] private SoundFXRequestCollection sfx;
    [SerializeField] private AudioEvent damageSound;
    
    FollowOrbController followOrbController;

    private void Awake()
    {
        followOrbController = FindObjectOfType<FollowOrbController>();
    }

    public void OnHit(float damage)
    {
        if (_globalInventory.IsDeath)
        {
            return;
        }
        
        _globalInventory.Health -= damage;
        CameraShaker.Instance.ShakeCamera(3, .5f);
        sfx.Add(AudioSFX.Request(damageSound));

        if (_globalInventory.IsDeath)
        {
            var tmp = Instantiate(replacedElement, transform.position, transform.rotation);
            if (followOrbController != null)
                followOrbController.ExecuteNavigationCommand(tmp.transform, true, 2f, 0, 5, 3f);
            Destroy(gameObject);
        }
    }
}
