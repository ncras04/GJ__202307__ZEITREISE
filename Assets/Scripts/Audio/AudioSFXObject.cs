using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(AudioSource))]
public class AudioSFXObject : MonoBehaviour
{
    public AudioSource Source => m_source;
    public IObjectPool<GameObject> Pool { get => m_pool; set => m_pool = value; }

    private AudioSource m_source;
    private IObjectPool<GameObject> m_pool;

    private void Awake()
    {
        m_source = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (m_source.isPlaying)
            return;

        OnReturnToPool();
    }

    private void OnReturnToPool()
    {
        if(m_pool is not null)
            m_pool.Release(gameObject);
    }
}
