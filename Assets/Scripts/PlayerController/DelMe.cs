using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelMe : MonoBehaviour, IButtonTriggerEvent
{
    Rigidbody body;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            CameraShaker.Instance.ShakeCameraAdditive(2, .5f);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            CameraShaker.Instance.ShakeCamera(2, 2f);
        }
    }
    public void OnButtonTriggerAction()
    {
        body.useGravity = true;
    }

    public void ButtonTriggerEvent()
    {
        OnButtonTriggerAction();
    }
}
