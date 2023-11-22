using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Tiles
{
    public class Spliter : DirectionalTiles
    {
        private int _Switchers = 0;

        protected override void Init()
        {
            base.Init();
            _Clock.OnReset += Restore;
        }

        private void Restore() => _Switchers = 0;

        protected override Vector3 GetDirection()
        {
            return (_Switchers++ % 2 == 0) ? transform.right : -transform.right;
        }

        protected override void Destroy()
        {
            _Clock.OnReset -= Restore;
            base.Destroy();
        }
    }
}