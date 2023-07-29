using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;
using Unity.VisualScripting;

public class AudioSFXTester : MonoBehaviour
{
    [SerializeField]
    private SoundFXRequestCollection m_soundRequests;

    [SerializeField]
    private AudioEvent m_testSound;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            m_soundRequests.Add(AudioSFXRequest.CreateRequest(m_testSound, transform));

        if (Input.GetKeyDown(KeyCode.D))
            m_soundRequests.Add(AudioSFXRequest.CreateRequest(m_testSound));

        if (Input.GetKeyDown(KeyCode.S))
            Destroy(gameObject);
    }
}
