using UnityEngine;
using Audio;

public class AudioSFXTester : MonoBehaviour
{
    [SerializeField]
    private SoundFXRequestCollection m_soundRequests;

    [SerializeField]
    private AudioEvent m_testSound;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            m_soundRequests.Add
                (AudioSFX.Request(m_testSound, transform));

        if (Input.GetKeyDown(KeyCode.D))
            m_soundRequests.Add
                (AudioSFX.Request(m_testSound));

        if (Input.GetKeyDown(KeyCode.S))
            Destroy(gameObject);
    }
}
