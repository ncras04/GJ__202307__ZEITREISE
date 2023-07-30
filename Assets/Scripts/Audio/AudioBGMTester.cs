using Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioBGMester : MonoBehaviour
{
    [SerializeField]
    private BGMRequestCollection BGM;

    [SerializeField]
    private MusicEvent m_testMusic1;

    public void Start()
    {
            BGM.Add
                (AudioBGM.Request(m_testMusic1));
    }
}