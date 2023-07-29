using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;

public class AudioSFXTester : MonoBehaviour
{
    [SerializeField]
    private SoundFXRequestCollection m_soundRequests;

    [SerializeField]
    private AudioEvent m_testSound;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            m_soundRequests.Add(AudioSFXRequest.CreateRequest(m_testSound));
    }
}
