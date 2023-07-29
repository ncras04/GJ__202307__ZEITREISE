using Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceSaver : MonoBehaviour
{
    private void OnDisable()
    {
        Transform tmp = GetComponentInChildren<AudioSource>().transform;
        AudioSFXObject obj = tmp.GetComponent<AudioSFXObject>();

        obj.Source.Stop();
        tmp.SetParent(null);
    }

}
