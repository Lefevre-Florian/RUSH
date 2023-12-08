using System;
using System.Collections;
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

        private Coroutine _InternalTimer = null;
        private Coroutine _ParticleTimer = null;

        private float _TimeMultiplier = 1f;
        public float ElapsedTime { get; private set; } = 0f;
        public float Ratio { get {return ElapsedTime / (DURATION_BETWEEN_TICK / _TimeMultiplier); } private set { } }

        private int _CurrentTick = 0;

        // Signals
        public event Action OnTick;
        public event Action OnGameStart;
        public event Action OnReset;

        public event Action OnParticleTicking;

        private void Awake()
        {
            if (_Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _Instance = this;
        }

        private void Start() => StartParticleEmission();

        private void Update()
        {
            ElapsedTime += Time.deltaTime * _TimeMultiplier;
        }

        public void ResetTicking()
        {
            StopTicking();
            _TimeMultiplier = 1f;

            OnReset?.Invoke();
            StartParticleEmission();
        }

        public void StartTicking()
        {
            StopParticleEmission();
            if (_InternalTimer != null)
                StopCoroutine(_InternalTimer);
            _InternalTimer = StartCoroutine(Tick());

            OnGameStart?.Invoke();
            ElapsedTime = 0f;
        }

        private void StartParticleEmission()
        {
            if (_InternalTimer != null)
                StopCoroutine(_InternalTimer);
            _InternalTimer = StartCoroutine(ParticleTick());
        }

        private void StopParticleEmission()
        {
            if (_InternalTimer != null)
                StopCoroutine(_InternalTimer);
            _InternalTimer = null;
        }

        public void StopTicking()
        {
            if (_InternalTimer != null)
                StopCoroutine(_InternalTimer);
            _InternalTimer = null;

            ElapsedTime = 0f;
        }

        private IEnumerator Tick()
        {
            while (isActiveAndEnabled)
            {
                yield return new WaitForSeconds(DURATION_BETWEEN_TICK / _TimeMultiplier);
                OnTick?.Invoke();

                ElapsedTime = 0f;
            }

            StopCoroutine(_InternalTimer);
            yield return null;
        }

        private IEnumerator ParticleTick()
        {
            while (isActiveAndEnabled)
            {
                yield return new WaitForSeconds(DURATION_BETWEEN_TICK);
                OnParticleTicking?.Invoke();
            }
            StopCoroutine(_InternalTimer);
            yield return null;
        }

        public void UpdateTickMultiplier(float pMultiplier)
        {
            if (pMultiplier > MIN_MULTIPLIER || pMultiplier < MAX_MULTIPLIER)
                _TimeMultiplier = pMultiplier;
        }

        private void OnDestroy()
        {
            StopParticleEmission();
            StopTicking();

            if (_Instance != null)
                _Instance = null;
        }
    }
}
