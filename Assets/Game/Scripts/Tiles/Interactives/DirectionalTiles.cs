using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Tiles
{
    public class DirectionalTiles : MonoBehaviour
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

        private void Start()
        {
            _DirectionalVector = _Directions[_Direction];

            transform.LookAt(transform.position + _DirectionalVector);
        }

        public virtual Vector3 GetDirection()
        {
            return _DirectionalVector;
        }

    }
}