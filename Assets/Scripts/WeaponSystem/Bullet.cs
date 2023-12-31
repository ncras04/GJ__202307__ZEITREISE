using System;
using System.Security.Cryptography;
using UnityEngine;
using WeaponSystem;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float damage;

    [SerializeField] private ParticleSystem interactionParticle;

    [SerializeField] private string ignoredTag;

    public string IgnoredTag
    {
        get => ignoredTag;
        set => ignoredTag = value;
    }

    private Rigidbody _rigidbody;

    public event Action OnHittetObject;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _rigidbody.AddForce(transform.forward * bulletSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!string.IsNullOrEmpty(ignoredTag) && other.CompareTag(ignoredTag))
        {
            return;
        }
        
        Destroy(gameObject);
        var myTransform = transform;

        if (interactionParticle != null)
        {
            Instantiate(interactionParticle, myTransform.position, myTransform.rotation);
        }
        
        OnHittetObject?.Invoke();

        var hittetObject = other.GetComponent<IHittable>();

        if (hittetObject != null)
        {
            hittetObject.OnHit(damage);
        }
    }
}