using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponSystem;

public class Diamond : MonoBehaviour, IHittable
{
    [SerializeField] private GameObject brokenObject;
    [SerializeField] private GameObject shatterEffect;
    [SerializeField] private GameObject trailEffect;
    [SerializeField] private AnimationCurve fadeoutCurve;
    private MeshRenderer rend;
    private Collider col;

    private void Awake()
    {
        brokenObject.SetActive(false);
        col = GetComponent<Collider>();
        rend = GetComponent<MeshRenderer>();
    }

    private void Break()
    {
        rend.enabled = false;
        col.enabled = false;
        Instantiate(shatterEffect, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        brokenObject.SetActive(true);
    }
    public void Explode(float explosionStrength, Vector3 explosionPosition, float radius, float upwardsModifier)
    {
        Break();
        var rbs = brokenObject.GetComponentsInChildren<Rigidbody>();
        foreach (var rb in rbs)
        {
            if(Random.value > 0.4f)
            {
                Instantiate(trailEffect,rb.transform.position,rb.transform.rotation).transform.parent = rb.transform;
            }
            rb.AddExplosionForce(explosionStrength, explosionPosition, radius, upwardsModifier, ForceMode.Impulse);
            //Destroy(rb.gameObject, 3f);
            StartCoroutine(FadeOutFracture(5f, rb.gameObject.GetComponent<Renderer>()));
        }
    }

    IEnumerator FadeOutFracture(float fadeoutTime, Renderer rend)
    {
        float timer = 0f;
        Color startColor = rend.material.color;
        Color endColor = startColor;
        endColor.a = 0f;
        while (timer < fadeoutTime)
        {
            timer += Time.deltaTime;

            rend.material.color = Color.Lerp(startColor, endColor,fadeoutCurve.Evaluate( timer / fadeoutTime));
            yield return null;

        }
        Destroy(rend.gameObject, 0f);
    }

    public void OnHit(float damage)
    {
        Explode(10, transform.position, 0.6f, 2f);
    }
}
