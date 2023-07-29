using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] float triggerRange = 0.6f;
    [SerializeField] float triggerVelocity = 10f;
    private void Awake()
    {
        triggerVelocity *= triggerVelocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerController player))
        {
            if(Vector2.Dot((player.transform.position - transform.position).normalized, transform.up) > triggerRange && player.Rb.velocity.sqrMagnitude > triggerVelocity)
            {
                Debug.Log("playervelo: " + player.Rb.velocity.magnitude);
            }
            //if(Vector2.Dot(transform.up, (transform.position - player.transform.position).normalized))

            Debug.Log(Vector2.Dot((player.transform.position - transform.position).normalized, transform.up));
        }
    }
}
