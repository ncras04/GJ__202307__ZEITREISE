using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Audio;
using Audio;


public class AudioSFXManager : MonoBehaviour
{
    private string m_tag = "AudioSFXManager";

    [SerializeField]
    private AudioMixerGroup m_soundFXOutput;

    [SerializeField]
    private GameObject m_poolObjectPrefab;

    [SerializeField]
    private int m_maxPoolSize;

    [SerializeField]
    private SoundFXRequestCollection m_requests;

    private IObjectPool<GameObject> m_pool;

    private void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag(m_tag);

        if (objs.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        m_pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, m_maxPoolSize);
    }

    public GameObject CreatePooledItem()
    {
        GameObject go = Instantiate(m_poolObjectPrefab);
        AudioSFXObject obj = go.GetComponent<AudioSFXObject>();
        go.SetActive(true);

        obj.Pool = m_pool;

        return go;
    }

    public void OnTakeFromPool(GameObject _go)
    {
        _go.SetActive(true);
    }

    public void OnReturnedToPool(GameObject _go)
    {
        _go.SetActive(false);
    }

    public void OnDestroyPoolObject(GameObject _go)
    {
        Destroy(_go);
    }

    public void Start()
    {
        m_requests.OnAdd += OnSoundRequest;
    }

    private void OnSoundRequest(AudioSFXRequest _request)
    {
        AudioSFXObject tmp = m_pool.Get().GetComponent<AudioSFXObject>();
        AudioSource tmpSource = tmp.Source;
        AudioEvent tmpEvent = _request.Sound;

        tmp.transform.position = _request.Position;
        tmp.transform.SetParent(_request.Parent);

        tmp.name = $"{tmpEvent.name}-Sound";

        tmpSource.outputAudioMixerGroup = m_soundFXOutput;

        if (tmpEvent.Clips.Length > 1)
        {
            int rnd = Random.Range(0, tmpEvent.Clips.Length - 1);

            if (rnd == tmpEvent.LastPlayedClip)
                tmpSource.clip = rnd == 0 ? tmpEvent.Clips[1] : tmpEvent.Clips[tmpEvent.LastPlayedClip - 1];
        }
        else
            tmpSource.clip = tmpEvent.Clips[0];

        if (tmpEvent.IsPitchAffected)
            tmpSource.pitch = _request.PitchOverride == 0.0f ? Random.Range(0.9f, 1.1f) : _request.PitchOverride;

        tmpSource.volume = tmpEvent.Volume * _request.VolumeOverride;
        tmpSource.spatialBlend = _request.Is2D ? 0.0f : 1.0f;

        tmpSource.Play();

        m_requests.Remove(_request);
    }
}
