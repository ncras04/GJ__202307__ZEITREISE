using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;

[RequireComponent(typeof(Light))]
public class TorchFlickering : MonoBehaviour
{
    [SerializeField] private Vector2 _rangeOfLightIntensity;
    [SerializeField] private float _lerpTime;

    private Light _light;

    private Coroutine _currentCorutine;

    private void Awake()
    {
        _light = GetComponent<Light>();
        _currentCorutine = StartCoroutine(FlickeringLerp());
    }

    IEnumerator FlickeringLerp()
    {
        float timer = 0;
        float currentLightIntensity = _light.intensity;
        float targetIntensity = UnityEngine.Random.Range(_rangeOfLightIntensity.x, _rangeOfLightIntensity.y);

        while (timer < _lerpTime)
        {
            _light.intensity = Mathf.Lerp(currentLightIntensity, targetIntensity, timer / _lerpTime);
            timer += Time.deltaTime;
            yield return null;
        }

        _currentCorutine = StartCoroutine(FlickeringLerp());
    }

    private void OnDestroy()
    {
        StopCoroutine(_currentCorutine);
    }
}
