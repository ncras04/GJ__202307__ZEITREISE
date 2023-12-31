using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInput))]
public class Weapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private ParticleSystem _particleSystem;
    private InputActionAsset _inputActionAsset;

    [SerializeField] private Transform muzzleSpawn;
    
    [SerializeField] private float fireRate;

    private float _shotTimer;

    private bool CanFire => _shotTimer > 1 / fireRate;

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
        if (!CanFire)
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
        
        if (_bulletPrefab)
        {
            var particle = Instantiate<ParticleSystem>(_particleSystem, muzzleSpawn.position, muzzleSpawn.rotation);
        }
        else
        {
            Debug.LogError("No Particle Prefab was set, you idiot");
        }
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