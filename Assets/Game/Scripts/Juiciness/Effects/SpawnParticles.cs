using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Juiciness
{
    [RequireComponent(typeof(ParticleSystem))]
    public class SpawnParticles : MonoBehaviour
    {
        private enum State
        {
            DELAY = 0,
            LOOP = 1
        }

        private float _Delay = 0f;
        private float _Frequency = 0f;

        private int _NbCube = 0;
        private int _ActualFrequency = 0;

        private float _WaitDuration = 0f;
        private float _Counter = 0f;

        private bool _IsSetup = false;

        private ParticleSystem _Module = null;
        private State _Phase = State.DELAY;

        private Clock _Clock = null;

        private void OnEnable()
        {
            if (_IsSetup)
                InternalInit();
        }

        private void OnDisable()
        {
            if (_IsSetup)
            {
                _Clock.OnParticleTicking -= Loop;
                _Clock = null;
            }
        }

        private void InternalInit()
        {
            _Phase = State.DELAY;

            _WaitDuration = _Delay;
            _Counter = 0f;

            _ActualFrequency = _NbCube;

            _Clock = Clock.GetInstance();
            _Clock.OnParticleTicking += Loop;
        }

        public void Init(float pDelay, float pFreq, int pNbCube, Color pColor)
        {
            _Delay = pDelay;
            _Frequency = pFreq;

            _ActualFrequency = _NbCube = pNbCube;

            _Module = GetComponent<ParticleSystem>();

            ParticleSystemRenderer lMain = GetComponent<ParticleSystemRenderer>();
            lMain.material.color = pColor;

            _IsSetup = true;
            InternalInit();
        }


        private void Loop()
        {
            if (++_Counter == _WaitDuration)
            {
                _Module.Play();
                _Counter = 0f;

                if (_Phase == State.DELAY)
                {
                    _Phase = State.LOOP;
                    _WaitDuration = _Frequency;
                }
            }

            if (_Phase == State.LOOP &&
               --_ActualFrequency == _NbCube)
            {
                _ActualFrequency = _NbCube;
                _WaitDuration = _Delay;

                _Phase = State.DELAY;
            }
        }
    }
}