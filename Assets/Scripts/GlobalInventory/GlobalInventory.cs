using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Global Inventory", menuName = "Global/Global Inventory", order = 1)]
public class GlobalInventory : ScriptableObject
{
    private const int MaxAmmonition = 15;
    private const int AmmonitionStartValue = 15;
    private const int HealthStartValue = 3;
    
    private int _ammonition;

    public int Ammonition
    {
        get => _ammonition;
        set
        {
            if (_ammonition == MaxAmmonition && value >= _ammonition)
            {
                OnAmmoRestockFailed?.Invoke();
                return;
            }
            
            var ammoRestocked = value > _ammonition;
            
            _ammonition = value;
            
            OnAmmonitionChanged?.Invoke(value);

            if (value <= 0)
            {
                _ammonition = Math.Max(value, 0);
                OnOutOfAmmo?.Invoke();
            }

            if (ammoRestocked)
            {
                _ammonition = Mathf.Min(value, MaxAmmonition);
                OnAmmoRestocked?.Invoke();
            }
        }
    }

    private float _health;

    public float Health
    {
        get => _health;
        set
        {
            _health = value;
            
            OnHealthChanged?.Invoke(value);

            if (value <= 0)
            {
                _health = Math.Max(value, 0);
                OnDeath?.Invoke();
            }
        }
    }

    public event Action<float> OnHealthChanged;
    public event Action OnDeath;
    public event Action<float> OnAmmonitionChanged;
    public event Action OnOutOfAmmo;
    public event Action OnAmmoRestocked;
    public event Action OnAmmoRestockFailed;
    public event Action OnGlobalDataReset;
    

    public void ResetData()
    {
        Health = HealthStartValue;
        Ammonition = AmmonitionStartValue;
        
        OnGlobalDataReset?.Invoke();
    }
}
