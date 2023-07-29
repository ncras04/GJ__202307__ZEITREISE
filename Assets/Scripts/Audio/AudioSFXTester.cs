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

    [ContextMenu("Test Sound!")]
    void TestSound()
    {
        m_soundRequests.Add(AudioSFXRequest.CreateRequest(m_testSound));
    }
}
