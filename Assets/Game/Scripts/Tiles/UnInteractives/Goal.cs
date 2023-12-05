using System;
using System.Collections.Generic;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Tiles
{
    [RequireComponent(typeof(ColoredTiles))]
    public class Goal : Tile
    {
        public static List<Goal> Targets { get; private set; } = new List<Goal>();
        private Colors _Color = default;

        // Signals
        public event Action<Cube.Cube> OnFullyArrived;

        private void Awake() => Targets.Add(this);

        protected override void Init()
        {
            _Color = GetComponent<ColoredTiles>().Color;
            base.Init();
        }

        protected override void OnCollisionComportement()
        {
            if (m_Hit.collider.gameObject.GetComponent<Cube.Cube>().Color != _Color)
                return;

            GameObject lCollider = m_Hit.collider.gameObject;

            OnFullyArrived?.Invoke(lCollider.GetComponent<Cube.Cube>());
            Destroy(lCollider);
        }

        protected override void Destroy()
        {
            Targets.Remove(this);
            base.Destroy();
        }

    }
}