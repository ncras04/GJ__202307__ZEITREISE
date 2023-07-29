using System;
using UnityEngine;

namespace Countdown
{
    public class TimeManager : MonoBehaviour
    {
        public event Action OnTimerStarted;
        public event Action OnTimerEnds;

        private float _timeToReach;
        private float _timer;
        private bool _isRunning;
        private bool _isPaused;
        private bool _initialized;

        public float Timer => _timer;

        public void StartTimer()
        {
            _isRunning = true;
            _isPaused = false;
            OnTimerStarted?.Invoke();
        }

        public void SetTimer(float timer)
        {
            if (timer <= 0)
            {
                return;
            }
            
            _initialized = true;
            _timeToReach = timer;
        }

        private void Update()
        {
            if (!_isRunning || _isPaused || !_initialized)
            {
                return;
            }
            
            _timer += Time.deltaTime;

            if (_timer > _timeToReach)
            {
                Reset();
                OnTimerEnds?.Invoke();
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _isPaused = false;
        }

        public void Reset()
        {
            _timer = 0;
            Stop();
        }

        public void Pause()
        {
            _isPaused = true;
        }

        public void Continue()
        {
            _isPaused = false;
        }
    }
}