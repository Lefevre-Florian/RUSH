using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Juiciness
{
    [RequireComponent(typeof(ParticleSystem))]
    public class StainParticles : MonoBehaviour
    {
        [SerializeField] private float _Duration = 1f;

        private Clock _Clock = null;
        private float _InternalTimer = 0f;

        private void Start()
        {
            _Clock = Clock.GetInstance();
            _Clock.OnTick += CleanParticle;
        }

        private void CleanParticle()
        {
            if(++_InternalTimer >= _Duration)
                Destroy(gameObject);
        }

        private void OnDestroy()
        {
            _Clock.OnTick -= CleanParticle;
            _Clock = null;
        }
    }
}