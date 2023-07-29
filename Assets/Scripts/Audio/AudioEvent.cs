using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public enum EMixerOutput
    {
        MAIN,
        BGM,
        SFX,
    }

    [CreateAssetMenu(fileName = "New AudioEvent", menuName = "Data/Audio/AudioEvent", order = 0)]
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
