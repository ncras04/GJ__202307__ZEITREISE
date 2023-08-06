using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponSystem;

public class SawTrapTrigger : MonoBehaviour
{

    [SerializeField] private float pushBackForce = 80f;


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IHittable target))
        {
            target.OnHit(1);
        }
        if (other.TryGetComponent(out Rigidbody targetRigidbody))
        {
            targetRigidbody.AddForce((other.transform.position - transform.position).normalized * pushBackForce, ForceMode.Impulse);
        }
    }
}
