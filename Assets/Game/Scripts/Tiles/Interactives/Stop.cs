using System.Collections.Generic;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Tiles
{
    public class Stop : DirectionalTiles
    {
        private const float HEIGHT_OFFSET = 0.5f;

        [SerializeField] private int _WaitDuration = 2;

        [Header("Juiciness")]
        [SerializeField] private GameObject _IcePrisonPrefab = null;
        [SerializeField] private GameObject _IceSparksParticles = null;

        // Logics comportement
        private Queue<Cube.Cube> _CubePaused = new Queue<Cube.Cube>();

        private int _InternalTick = 0;

        // Particles & Rendering
        private GameObject _IcePrison = null;
        private ParticleSystem _IceParticles = null;

        protected override void Init()
        {
            base.Init();
            _Clock.OnReset += Restore;

            _IceParticles = Instantiate(_IceSparksParticles,
                                        transform.position + Vector3.up * HEIGHT_OFFSET,
                                        new Quaternion(),
                                        transform).GetComponent<ParticleSystem>();
        }

        private void Restore()
        {
            _CubePaused.Clear();
            _CubePaused = new Queue<Cube.Cube>();
            _InternalTick = 0;
        }

        protected override void OnCollisionComportement()
        {
            Cube.Cube lCube = _Hit.collider.gameObject.GetComponent<Cube.Cube>();

            if (_CubePaused.Contains(lCube))
                return;

            lCube.SetActionWait(_WaitDuration);
            _CubePaused.Enqueue(lCube);

            _Clock.OnTick += CleanStoppeur;

            _IcePrison = Instantiate(_IcePrisonPrefab, transform);
        }

        private void CleanStoppeur()
        {
            // Dequeue one cube
            if (++_InternalTick % _WaitDuration == 0)
            {
                _CubePaused.Dequeue();

                Destroy(_IcePrison);
                _IcePrison = null;

                _IceParticles.Play();
            }
                
            // Clean the stoppeur if there's no cube remaining in queue
            if (_CubePaused.Count == 0)
            {
                _Clock.OnTick -= CleanStoppeur;
                _InternalTick = 0;
            }
        }

        protected override void Destroy()
        {
            _Clock.OnReset -= Restore;
            _Clock.OnTick -= CleanStoppeur;
            base.Destroy();
        }

    }

}