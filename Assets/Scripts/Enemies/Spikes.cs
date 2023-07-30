using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponSystem;

public class Spikes : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private GlobalInventory _inventory;
    private static readonly int Trap = Animator.StringToHash("Trap");

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var component = other.gameObject.GetComponent<PlayerHittable>();
            component.OnHit(_inventory.Health);
            
            _animator.SetTrigger(Trap);
        }
    }
}
