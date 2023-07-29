using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public class AudioSFXRequest
    {
        public AudioEvent Sound { get; }
        public bool Is2D { get; }
        public Vector3 Position { get; }
        public Transform Parent { get; }
        private AudioSFXRequest(AudioEvent _sound, bool _is2D = true, Vector3 _position = default, Transform _parent = null)
        {
            Sound = _sound;
            Is2D = _is2D;
            Position = _position;
            Parent = _parent;
        }

        public static AudioSFXRequest CreateRequest(AudioEvent _sound, bool _is2D, Vector3 _position, Transform _parent)
        {
            return new AudioSFXRequest(_sound, _is2D, _position, _parent);
        }

        public static AudioSFXRequest CreateRequest(AudioEvent _sound)
        {
            return CreateRequest(_sound, true, Vector3.zero, null);
        }

        public static AudioSFXRequest CreateRequest(AudioEvent _sound, Transform _parent)
        {
            return CreateRequest(_sound, false, _parent.position, _parent);
        }

        public static AudioSFXRequest CreateRequest(AudioEvent _sound, Vector3 _position)
        {
            return CreateRequest(_sound, false, _position, null);
        }

        public static AudioSFXRequest CreateRequest(AudioEvent _sound, Vector3 _position, Transform _parent)
        {
            return CreateRequest(_sound, false, _position, _parent);
        }
    }
}
