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

    private Rigidbody _rigidbody;

    public event Action OnHittetObject;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        _rigidbody.AddForce(transform.forward * bulletSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
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