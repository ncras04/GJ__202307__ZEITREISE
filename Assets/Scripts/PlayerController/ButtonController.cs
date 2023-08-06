using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonController : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] float triggerRange = 0.6f;
    [SerializeField] float triggerVelocity = 10f;
    [SerializeField] bool onlyTriggerOnce;
    [SerializeField] UnityEvent onButtontTriggerEvent;
    [SerializeField] TriggerActionTimeline triggeredActionTimeline = TriggerActionTimeline.SameTimeline;
    [SerializeField] Renderer buttonRenderer;
    bool triggeredAction = false;

    private void Awake()
    {
        triggerVelocity *= triggerVelocity;
        if (buttonRenderer != null && triggeredActionTimeline == TriggerActionTimeline.InFuture)
        {
            buttonRenderer.material.color = Color.green;
        }else if (buttonRenderer != null && triggeredActionTimeline == TriggerActionTimeline.InPast)
        {
            buttonRenderer.material.color = Color.red;
        }else if(buttonRenderer != null && triggeredActionTimeline == TriggerActionTimeline.SameTimeline)
        {
            buttonRenderer.material.color = Color.white;
        }
    }

    private void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerController player))
        {
            if(Vector2.Dot(player.Rb.velocity * -1, Vector2.up) > triggerRange && player.Rb.velocity.sqrMagnitude >= triggerVelocity)
            {
                if(onlyTriggerOnce && !triggeredAction || !onlyTriggerOnce)
                {
                    onButtontTriggerEvent?.Invoke();
                    triggeredAction = true;
                } 
            }
        }
    }

    enum TriggerActionTimeline { SameTimeline, InFuture, InPast };
}
