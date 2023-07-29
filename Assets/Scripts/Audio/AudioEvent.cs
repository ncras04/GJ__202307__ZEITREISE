using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

namespace Audio
{
    public enum EMixerOutput
    {
        MAIN,
        BGM,
        SFX,
    }

    public class AudioEvent : ScriptableObject
    {
        public float Volume => m_volume;
        public AudioClip[] Clips => m_clips;
        public EMixerOutput MixerOutput => m_mixerOutput;

        [SerializeField]
        private float m_volume;
        [SerializeField]
        private AudioClip[] m_clips;
        [SerializeField]
        private EMixerOutput m_mixerOutput;
    }
}
