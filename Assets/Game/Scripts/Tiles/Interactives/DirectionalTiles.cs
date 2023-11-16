using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Tiles
{
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

        protected Vector3 _DirectionalVector = default;

        protected override void Init()
        {
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

        public void SetDirection(int pDirection)
        {
            _Direction = (Vectors)pDirection;

            UpdateLookDirection();
        }

    }
}