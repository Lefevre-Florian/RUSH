using System;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Tiles
{
    [RequireComponent(typeof(ColoredTiles))]
    public class Goal : MonoBehaviour
    {
        private const float RAYCAST_DISTANCE = 1f;

        [SerializeField] private int _RequiredCubes = 1;

        private Clock _Clock = null;
        private RaycastHit _Hit = default;

        private Colors _Color = default;

        // Signals
        public event Action OnFullyArrived;
        
        void Start()
        {
            _Color = GetComponent<ColoredTiles>().Color;

            _Clock = Clock.GetInstance();
            _Clock.OnTick += OnCollisionCheck;
        }

        private void OnCollisionCheck()
        {
            Debug.DrawRay(transform.position, Vector3.up, Color.red, 0.5f);
            if(Physics.Raycast(transform.position, Vector3.up, out _Hit, RAYCAST_DISTANCE))
            {
                
                if (_Hit.collider.gameObject.GetComponent<Cube.Cube>().Color != _Color)
                    return;

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