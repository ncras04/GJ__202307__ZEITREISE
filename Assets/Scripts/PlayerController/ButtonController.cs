using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonController : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] float triggerRange = 0.6f;
    [SerializeField] float triggerVelocity = 10f;
    [SerializeField] UnityEvent onButtontTriggerEvent;

    private void Awake()
    {
        triggerVelocity *= triggerVelocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerController player))
        {
            if(Vector2.Dot(player.Rb.velocity * -1, Vector2.up) > triggerRange && player.Rb.velocity.magnitude >= triggerVelocity)
            {
                //Debug.Log("playervelo: " + player.Rb.velocity.magnitude);
                onButtontTriggerEvent?.Invoke();
            }
            //Debug.Log(Vector2.Dot((player.transform.position - transform.position).normalized, transform.up));
        }
    }
}
