using System;
using UnityEngine;

namespace WeaponSystem
{
    public class DestroyableObject : MonoBehaviour, IHittable
    {
        [SerializeField] private float maxHealth;
        
        [Header("Optional")]
        [SerializeField] private GameObject destroyedVersion;
        [SerializeField] private GameObject _deathParticle;

        private float _currentHealth;

        public float CurrentHealth => _currentHealth;

        public bool IsDeath => _currentHealth <= 0;

        public event Action OnDeath; 
        public event Action<float> OnGettingHit;
        
        //needs expanded with sounds that will be fired

        private void Awake()
        {
            _currentHealth = maxHealth;
        }

        public void OnHit(float damage)
        {
            if (IsDeath)
            {
                return;
            }
            
            _currentHealth -= damage;

            _currentHealth = Mathf.Max(_currentHealth, 0);

            if (IsDeath)
            {
                var myTransform = transform;
                
                if (destroyedVersion)
                {
                    Instantiate(destroyedVersion, myTransform.position, myTransform.rotation);
                }

                if (_deathParticle)
                {
                    Instantiate(_deathParticle, myTransform.position, myTransform.rotation);
                }
                
                Destroy(gameObject);
                OnDeath?.Invoke();
            }
            else
            {
                OnGettingHit?.Invoke(damage);
            }
        }
    }
}