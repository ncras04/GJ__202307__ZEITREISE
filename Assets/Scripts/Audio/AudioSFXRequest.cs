using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public class AudioSFXRequest : MonoBehaviour
    {
        public class SoundRequest
        {
            public AudioEvent Sound { get; }
            public bool Is2D { get; }
            public Vector3 Position { get; }
            private SoundRequest(AudioEvent _sound, bool _is2D = true, Vector3 _position = default)
            {
                Sound = _sound;
                Is2D = _is2D;
                Position = _position;
            }

            public static SoundRequest CreateRequest(AudioEvent _sound, Vector3 _position, bool _is2D)
            {
                return new SoundRequest(_sound, _is2D, _position);
            }

            public static SoundRequest CreateRequest(AudioEvent _sound)
            {
                return CreateRequest(_sound, Vector3.zero, false);
            }

            public static SoundRequest CreateRequest(AudioEvent _sound, Vector3 _position)
            {
                return CreateRequest(_sound, _position, false);
            }
        }
    }
}
