using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Tiles
{
    public class Goal : MonoBehaviour
    {
        private const float DISTANCE_CHECK = 1f;

        [SerializeField] private int _RequiredCubes = 1;
        [SerializeField] private int _CubeLayer = 3;

        private Clock _Clock = null;
        private RaycastHit _Hit = default;

        // Signals
        public event Action OnFullyArrived;
        
        void Start()
        {
            _Clock = Clock.GetInstance();

            _Clock.OnTick += OnCollisionCheck;
        }

        private void OnCollisionCheck()
        {
            Debug.DrawRay(transform.position, Vector3.up, Color.blue, 1f);
            if(Physics.Raycast(transform.position, Vector3.up, out _Hit, 1f))
            {
                Destroy(_Hit.collider.gameObject);

                if (--_RequiredCubes == 0)
                    OnFullyArrived?.Invoke();
            }
        }

        private void OnDestroy()
        {
            if(_Clock != null)
                _Clock.OnTick -= OnCollisionCheck;
            _Clock = null;
        }
    }
}