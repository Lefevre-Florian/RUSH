using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.Rush
{
    public class Clock : MonoBehaviour
    {
        #region Singleton
        private static Clock _Instance = null; 

        private Clock() : base() { }

        public static Clock GetInstance()
        {
            if (_Instance == null)
                _Instance = new Clock();

            return _Instance;
        }

        #endregion

        private const float DURATION_BETWEEN_TICK = 1f;

        private const float MAX_MULTIPLIER = 5f;
        private const float MIN_MULTIPLIER = .1f;

        [SerializeField][Range(MIN_MULTIPLIER, MAX_MULTIPLIER)] private float _TickMultiplier = 1f;

        private Coroutine _InternalTimer = null;

        private float _UpdatedDurationTick = DURATION_BETWEEN_TICK;
        public float ElapsedTime { get; private set; } = 0f;
        public float Ratio { get {return ElapsedTime / _UpdatedDurationTick; } private set { } }

        // Signals
        public event Action OnTick;

        private void Awake()
        {
            if (_Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _Instance = this;
        }
        
        private void Start()
        {
            _InternalTimer = StartCoroutine(Tick());
        }

        private void Update()
        {
            ElapsedTime += Time.deltaTime * _TickMultiplier;
        }

        private IEnumerator Tick()
        {
            while (isActiveAndEnabled)
            {
                yield return new WaitForSeconds(_UpdatedDurationTick);
                OnTick?.Invoke();

                ElapsedTime -= _UpdatedDurationTick;
            }

            StopCoroutine(_InternalTimer);
            yield return null;
        }

        public void UpdateTickMultiplier(float pMultiplier)
        {
            if (pMultiplier > MIN_MULTIPLIER || pMultiplier < MAX_MULTIPLIER)
            {
                ElapsedTime = 0f;

                _TickMultiplier = pMultiplier;
                _UpdatedDurationTick = DURATION_BETWEEN_TICK / _TickMultiplier;

                StopCoroutine(_InternalTimer);
                _InternalTimer = StartCoroutine(Tick());
            } 
        }
        private void OnValidate()
        {
            if(_InternalTimer != null)
                UpdateTickMultiplier(_TickMultiplier);
        }

        private void OnDestroy()
        {
            if (_InternalTimer != null)
                StopCoroutine(Tick());

            if (_Instance != null)
                _Instance = null;
        }
    }
}
