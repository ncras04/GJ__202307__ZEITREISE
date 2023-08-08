using System;
using Audio;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInput))]
public class Weapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private GameObject _particleSystem;
    [SerializeField] private GlobalInventory _globalInventory;
    private InputActionAsset _inputActionAsset;

    [SerializeField] private Transform muzzleSpawn;
    
    [SerializeField] private float fireRate;
    
    
    [SerializeField] private SoundFXRequestCollection sfx;
    [SerializeField] private AudioEvent shootSound;

    private float _shotTimer;

    private bool CanFire => _shotTimer > 1 / fireRate;

    public event Action OnShot;

    private void Awake()
    {
        Setup();
        
        SubscribeInputs();
    }
    
    void Update()
    {
        if (!CanFire)
        {
            _shotTimer += Time.deltaTime;
        }
    }

    private void Shoot()
    {
        if (!CanFire || _globalInventory.Ammonition == 0)
        {
            return;
        }
        
        _shotTimer = 0;

        if (_bulletPrefab)
        {
            var bullet = Instantiate<Bullet>(_bulletPrefab, muzzleSpawn.position, muzzleSpawn.rotation);
        }
        else
        {
            Debug.LogError("No Buller Prefab was set, you idiot");
        }
        
        if (_particleSystem)
        {
            var particle = Instantiate<GameObject>(_particleSystem, muzzleSpawn.position, muzzleSpawn.rotation, muzzleSpawn);
        }
        else
        {
            Debug.LogError("No Particle Prefab was set, you idiot");
        }


        CameraShaker.Instance.ShakeCamera(1.6f, 0.4f);
        sfx.Add(AudioSFX.Request(shootSound));
        
        _globalInventory.Ammonition--;
        OnShot?.Invoke();
    }

    private void Setup()
    {
        _shotTimer = 0;

        _inputActionAsset = GetComponent<PlayerInput>().actions;
    }

    private void SubscribeInputs()
    {
        if (!_inputActionAsset)
        {
            Debug.LogWarning("No Input Asset set");
            return;
        }
        //Check if player Future!
        
        
        //if true -> Subscribe
        _inputActionAsset.FindAction("Interact").started += context => Shoot();

    }
}