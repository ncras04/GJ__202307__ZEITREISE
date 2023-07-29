using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandTrigger : MonoBehaviour
{
    PlayerController controller;
    private void Awake()
    {
        controller = GetComponentInParent<PlayerController>();
    }

    // Remember all other cant swap cases.
    private void OnTriggerEnter(Collider other)
    {
        controller.CanSwap = false;
    }

    private void OnTriggerExit(Collider other)
    {
        controller.CanSwap = true;
    }
}
