using System.Collections.Generic;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Tiles
{
    public class Stop : DirectionalTiles
    {
        [SerializeField] private int _WaitDuration = 2;

        private Queue<Cube.Cube> _CubePaused = new Queue<Cube.Cube>();

        private int _InternalTick = 0;

        protected override void Init()
        {
            base.Init();
            _Clock.OnReset += Restore;
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
        }

        private void CleanStoppeur()
        {
            if (++_InternalTick % _WaitDuration == 0)
                _CubePaused.Dequeue();

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