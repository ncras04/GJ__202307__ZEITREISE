using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(transform.parent.parent != null && (other.CompareTag("Player") || other.CompareTag("Crate")))
            other.transform.parent = transform.parent.parent;

        if(other.TryGetComponent(out PlayerController controller))
        {
            controller.Rb.interpolation = RigidbodyInterpolation.None;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Crate"))
            other.transform.parent = null;

        if (other.TryGetComponent(out PlayerController controller))
        {
            controller.Rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }
}
