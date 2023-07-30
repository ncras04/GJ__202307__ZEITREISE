using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    public static CameraShaker Instance;
    [SerializeField] private NoiseSettings shakeNoiseSettings;
    [SerializeField] AnimationCurve shakeCurve;

    private CinemachineVirtualCamera cam;
    private CinemachineBasicMultiChannelPerlin noiseProfile;
    private NoiseSettings baseNoiseSettings;
    private float baseAmplitude;
    private float baseFrequency;

    float shakeTimer;
    float timer = 0f;

    float intensity;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        cam = GetComponent<CinemachineVirtualCamera>();
        noiseProfile = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        baseNoiseSettings = noiseProfile.m_NoiseProfile;
        baseAmplitude = noiseProfile.m_AmplitudeGain;
        baseFrequency = noiseProfile.m_FrequencyGain;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < shakeTimer)
        {
            timer += Time.deltaTime;
            noiseProfile.m_AmplitudeGain = shakeCurve.Evaluate(timer / shakeTimer) * intensity;
            if (timer > shakeTimer)
            {
                noiseProfile.m_NoiseProfile = baseNoiseSettings;
                noiseProfile.m_AmplitudeGain = baseAmplitude;
                noiseProfile.m_FrequencyGain = baseFrequency;
                shakeTimer = 0f;
                timer = 0f;
                this.intensity = 0f;
            }
        }
    }

    public void ShakeCamera(float intensity, float duration)
    {
        shakeTimer = duration;
        timer = 0f;
        this.intensity = intensity;
        noiseProfile.m_NoiseProfile = shakeNoiseSettings;
        noiseProfile.m_AmplitudeGain = intensity;
        noiseProfile.m_FrequencyGain = 2f;
    }
    public void ShakeCameraAdditive(float intensity, float duration)
    {
        shakeTimer += duration;
        this.intensity += intensity;
        noiseProfile.m_NoiseProfile = shakeNoiseSettings;
        noiseProfile.m_AmplitudeGain += intensity;
        noiseProfile.m_FrequencyGain = 2f;
        Debug.Log(noiseProfile.m_AmplitudeGain);
    }
}
