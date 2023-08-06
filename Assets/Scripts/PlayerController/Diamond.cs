using DG.Tweening;
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

        if (TryGetComponent(out RotateObject rotater))
        {
            rotater.CurrentTween.Kill();
        }
        if (TryGetComponent(out FloatingObject floater))
        {
            floater.CurrentTween.Kill();
        }
    }
    public void Explode(float explosionStrength, Vector3 explosionPosition, float radius, float upwardsModifier)
    {
        Break();
        var rbs = brokenObject.GetComponentsInChildren<Rigidbody>();
        foreach (var rb in rbs)
        {
            if(Random.value > 0.4f && trailEffect != null)
            {
                Instantiate(trailEffect,rb.transform.position,rb.transform.rotation).transform.parent = rb.transform;
            }
            rb.AddExplosionForce(explosionStrength, explosionPosition, radius, upwardsModifier, ForceMode.Impulse);
            //Destroy(rb.gameObject, 3f);
            StartCoroutine(FadeOutFracture(10f, rb.gameObject.GetComponent<MeshRenderer>()));
        }
        Destroy(gameObject, 11f);
    }

    IEnumerator FadeOutFracture(float fadeoutTime, MeshRenderer rend)
    {
        float timer = 0f;
        Color startColor = rend.material.color;
        Vector3 tmpScale = rend.transform.localScale;

        while (timer < fadeoutTime)
        {
            timer += Time.deltaTime;

            // need consider emiission...
            startColor.a = 1 - fadeoutCurve.Evaluate(timer / fadeoutTime);
            rend.material.color = startColor;

            tmpScale.x = 1 - fadeoutCurve.Evaluate(timer / fadeoutTime);
            tmpScale.y = tmpScale.x;
            tmpScale.z = tmpScale.x;
            rend.transform.localScale = tmpScale;
            yield return null;

        }
        //Destroy(rend.gameObject, 0f);
    }

    public void OnHit(float damage)
    {
        Explode(2.5f, transform.position, 0.6f, 2f);
    }
}
