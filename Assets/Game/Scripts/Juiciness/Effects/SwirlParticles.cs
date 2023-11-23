using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Juiciness
{
    [RequireComponent(typeof(ParticleSystem))]
    public class SwirlParticles : Particles
    {
        [SerializeField] private float _SwirlSpeed = 0f;
        [SerializeField] private AnimationCurve _TweenCurve = null;

        private float _ElapsedTime = 0f;

        protected override void Init()
        {
            base.Init();
            transform.localScale = Vector3.one * _TweenCurve.Evaluate(0f);
        }

        protected override void Process()
        {
            transform.rotation = Quaternion.AngleAxis(_SwirlSpeed * Time.deltaTime, Vector3.up) * transform.rotation;

            _ElapsedTime += Time.deltaTime;
            transform.localScale = Vector3.one * _TweenCurve.Evaluate(_ElapsedTime);
        }
    }
}