using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public class AudioBGM
    {
        public MusicEvent Music { get; }
        public float VolumeOverride { get; }
        public float FadeOverride { get; }
        private AudioBGM(MusicEvent _music, float _volumeOverride = 1.0f, float _fadeOverride = 1.0f)
        {
            Music = _music;
            VolumeOverride = _volumeOverride;
            FadeOverride = _fadeOverride;
        }

        public static AudioBGM Request(MusicEvent _music)
        {
            return new AudioBGM(_music);
        }
        public static AudioBGM Request(MusicEvent _music, float _fadeOverride, float _volumeOverride = 1.0f)
        {
            return new AudioBGM(_music, _volumeOverride, _fadeOverride);
        }
    }
}
