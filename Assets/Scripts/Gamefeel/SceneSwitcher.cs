using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSwitcher : MonoBehaviour
{
    [SerializeField] private float transitionTime = 0.4f;
    [SerializeField] private Image fadingImage;
    [SerializeField] private AnimationCurve fadingCurve;
    [SerializeField] private string targetScene;

    private int count;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            count++;
        }

        if (count == 2)
        {
            StartCoroutine(FadeOut());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            count--;
        }
    }

    private IEnumerator FadeOut()
    {
        if (fadingImage == null)
        {
            yield break; 
        }
        
        fadingImage.gameObject.SetActive(true);

        float timer = 0;

        while (timer < transitionTime)
        {
            var color = fadingImage.color;
            color.a = Mathf.Lerp(0, 1, fadingCurve.Evaluate(timer / transitionTime));

            fadingImage.color = color;
            
            yield return null;
            timer += Time.deltaTime;
        }

        SceneManager.LoadScene(targetScene);
    }
}