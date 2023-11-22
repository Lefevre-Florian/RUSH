using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Juiciness
{
    [RequireComponent(typeof(ParticleSystem))]
    public class Particles : MonoBehaviour
    {

        [SerializeField] private float _SwirlSpeed = 0f;

        private void Start()
        {
            ParticleSystem.MainModule lParticles = GetComponent<ParticleSystem>().main;
            lParticles.stopAction = ParticleSystemStopAction.Callback;
        }

        private void Update() => transform.rotation = Quaternion.AngleAxis(_SwirlSpeed * Time.deltaTime, Vector3.up) * transform.rotation;

        private void OnParticleSystemStopped()
        {
            Destroy(gameObject);
        }
    }
}