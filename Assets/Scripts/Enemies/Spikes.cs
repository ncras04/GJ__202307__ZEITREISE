using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponSystem;

public class Spikes : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    private static readonly int Trap = Animator.StringToHash("Trap");

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var component = other.gameObject.GetComponent<DestroyableObject>();
            component.OnHit(component.CurrentHealth);
            
            _animator.SetTrigger(Trap);
        }
    }
}
