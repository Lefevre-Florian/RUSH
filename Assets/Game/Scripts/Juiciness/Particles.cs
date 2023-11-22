using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Juiciness
{
    [RequireComponent(typeof(ParticleSystem))]
    public class Particles : MonoBehaviour
    {
        private void Start() => Init();

        protected virtual void Init()
        {
            ParticleSystem.MainModule lParticles = GetComponent<ParticleSystem>().main;
            lParticles.stopAction = ParticleSystemStopAction.Callback;
        }

        private void Update() => Process();

        protected virtual void Process() { }

        private void OnParticleSystemStopped() => Destroy(gameObject);
    }
}