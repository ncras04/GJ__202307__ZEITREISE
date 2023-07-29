using UnityEngine;
using UnityEngine.UIElements;

namespace Audio
{
    public class AudioSFX
    {
        public AudioEvent Sound { get; }
        public bool Is2D { get; }
        public Vector3 Position { get; }
        public Transform Parent { get; }
        public float PitchOverride { get; }
        public float VolumeOverride { get; }
        private AudioSFX(AudioEvent _sound, bool _is2D = true, Vector3 _position = default, Transform _parent = null, float _pitchOverride = 0.0f, float _volumeOverride = 1.0f)
        {
            Sound = _sound;
            Is2D = _is2D;
            Position = _position;
            Parent = _parent;
            PitchOverride = _pitchOverride;
            VolumeOverride = _volumeOverride;
        }

        public static AudioSFX Request(AudioEvent _sound, bool _is2D, Vector3 _position, Transform _parent)
        {
            return new AudioSFX(_sound, _is2D, _position, _parent);
        }

        public static AudioSFX Request(AudioEvent _sound)
        {
            return new AudioSFX(_sound);
        }

        public static AudioSFX Request(AudioEvent _sound, float _volumeOverride)
        {
            return new AudioSFX(_sound, true, Vector3.zero, null, 0.0f, _volumeOverride);
        }

        public static AudioSFX Request(AudioEvent _sound, Transform _parent, float _pitchOverride, float _volumeOverride)
        {
            return new AudioSFX(_sound, false, _parent.position, _parent, _pitchOverride, _volumeOverride);
        }

        public static AudioSFX Request(AudioEvent _sound, float _pitchOverride, float _volumeOverride)
        {
            return new AudioSFX(_sound, true, Vector3.zero, null, _pitchOverride, _volumeOverride);
        }

        public static AudioSFX Request(AudioEvent _sound, Transform _parent, float _volumeOverride)
        {
            return new AudioSFX(_sound, false, _parent.position, _parent, 0.0f, _volumeOverride);
        }

        public static AudioSFX Request(AudioEvent _sound, Transform _parent)
        {
            return new AudioSFX(_sound, false, _parent.position, _parent);
        }

        public static AudioSFX Request(AudioEvent _sound, Vector3 _position)
        {
            return new AudioSFX(_sound, false, _position, null);
        }

        public static AudioSFX Request(AudioEvent _sound, Vector3 _position, Transform _parent)
        {
            return new AudioSFX(_sound, false, _position, _parent);
        }
    }
}
