using System.Collections.Generic;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Tiles
{
    /// <summary>
    /// Parent class for every interactive classes (Stop / Turnstile / Convoyer)
    /// </summary>
    public class DirectionalTiles : Tile
    {
        private static Dictionary<Vectors, Vector3> _Directions = new Dictionary<Vectors, Vector3>
        {
            {Vectors.FORWARD, Vector3.forward},
            {Vectors.BACKWARD, Vector3.back},
            {Vectors.RIGHT, Vector3.right},
            {Vectors.LEFT, Vector3.left}
        };

        [SerializeField] private Vectors _Direction = default;

        [Header("Spawn")]
        [SerializeField] private GameObject _SpawnParticlePrefab = default;
        [SerializeField] private float _SpwanParticleOffset = 0.01f;

        protected Vector3 _DirectionalVector = default;

        public Vectors Direction
        {
            get { return _Direction; }
            private set { _Direction = value; }
        }

        protected override void Init()
        {
            Instantiate(_SpawnParticlePrefab, transform.position + Vector3.up * _SpwanParticleOffset, transform.rotation, transform.parent);

            UpdateLookDirection();
            base.Init();
        }

        private void UpdateLookDirection()
        {
            _DirectionalVector = _Directions[_Direction];
            transform.LookAt(transform.position + _DirectionalVector);
        }

        protected override void OnCollisionComportement()
        {
            Cube.Cube lCube = _Hit.collider.gameObject.GetComponent<Cube.Cube>();
            lCube.SetDirectionMove(GetDirection());
        }

        protected virtual Vector3 GetDirection()
        {
            return _DirectionalVector;
        }

        public void SetDirection(Vectors pDirection)
        {
            _Direction = pDirection;

            UpdateLookDirection();
        }

    }
}