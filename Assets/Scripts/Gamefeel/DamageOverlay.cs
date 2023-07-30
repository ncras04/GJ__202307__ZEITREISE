using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DamageOverlay : MonoBehaviour
{
    [SerializeField] private GlobalInventory _globalInventory;
    [SerializeField] private float _targetAlpha = 7;
    [SerializeField] private float _displayTime;
    [SerializeField] private AnimationCurve _displayCurve;
    private Image _image;
    
    void Awake()
    {
        _image = GetComponent<Image>();
        _globalInventory.OnGetDamage += GlobalInventoryOnOnGetDamage;
        _globalInventory.OnDeath += () => GlobalInventoryOnOnGetDamage(-1);
    }

    private void GlobalInventoryOnOnGetDamage(float damage)
    {
        StartCoroutine(ShortOverlayDisplay(damage));
    }

    private IEnumerator ShortOverlayDisplay(float damage)
    {
        float timer = 0;
        Color color;
        
        while (timer < _displayTime)
        {
            timer += Time.deltaTime;

            color = _image.color;
            color.a = (_displayCurve.Evaluate(timer / _displayTime) * _targetAlpha) / 255;
            _image.color = color;
            yield return null;
        }

        if (Math.Abs(damage - (-1)) < 0.001f && GameManager.Instance.GameState == GameState.Successful)
        {
            color = _image.color;
            color.a = _targetAlpha / 255;
            _image.color = color;
        }
        else
        {
            color = _image.color;
            color.a = 0;
            _image.color = color;
        }
        
        
    }
}
