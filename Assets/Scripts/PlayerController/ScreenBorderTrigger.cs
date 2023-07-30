using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenBorderTrigger : MonoBehaviour
{
    [SerializeField] bool isLeftBorder = true;
    [SerializeField] CanvasGroup screenBorder;
    [SerializeField] float tagetAlpha = 0.5f;
    [SerializeField] float fadeSpeed = 1f;
    [SerializeField] GlobalInventory inventory;
    Coroutine currentRoutine;
    IEnumerator FadeScreenBorder(float to, float duration)
    {
        if(screenBorder == null) yield break;
        float timer = 0f;
        float startAlpha = screenBorder.alpha;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            screenBorder.alpha = Mathf.Lerp(startAlpha, to, timer/duration);
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (currentRoutine != null) StopCoroutine(currentRoutine);
            if(isLeftBorder)
                currentRoutine = StartCoroutine(FadeScreenBorder(tagetAlpha, fadeSpeed));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(isLeftBorder && transform.position.x < other.transform.position.x)
            {
                if(currentRoutine!= null)StopCoroutine(currentRoutine);
                    currentRoutine = StartCoroutine(FadeScreenBorder(0f, fadeSpeed));

            }else if(!isLeftBorder && transform.position.x > other.transform.position.x)
            {
                if (currentRoutine != null) StopCoroutine(currentRoutine);
                currentRoutine = StartCoroutine(FadeScreenBorder(0f, fadeSpeed));
            }
            else
            {
                inventory.Health -= inventory.Health;
            }
            
        }
    }
}
