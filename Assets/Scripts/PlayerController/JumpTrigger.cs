using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTrigger : MonoBehaviour
{

    PlayerController controller;
    private void Awake()
    {
        controller = GetComponentInParent<PlayerController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        controller.ResetJumps();
    }

    private void OnTriggerExit(Collider other)
    {
        controller.LeftGround();
    }
    // eventually coyote to false after timer.
}
