using System;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Tiles
{
    [RequireComponent(typeof(ColoredTiles))]
    public class Goal : Tile
    {
        [SerializeField] private int _RequiredCubes = 1;

        private Colors _Color = default;

        // Signals
        public event Action OnFullyArrived;

        protected override void Init()
        {
            _Color = GetComponent<ColoredTiles>().Color;

            base.Init();
        }

        protected override void OnCollisionComportement()
        {
            if (_Hit.collider.gameObject.GetComponent<Cube.Cube>().Color != _Color)
                return;

            Destroy(_Hit.collider.gameObject);

            if (--_RequiredCubes == 0)
                OnFullyArrived?.Invoke();
        }

    }
}