using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Tiles
{
    public abstract class Tile : MonoBehaviour
    {

        private const float RAYCAST_OFFSETING = -0.3f;
        private const float RAYCAST_DISTANCE = 1f;

        [SerializeField] private LayerMask _PlayerLayer = default;

        protected Clock _Clock = null;
        protected RaycastHit _Hit = default;

        private void Start() => Init();

        protected virtual void Init()
        {
            _Clock = Clock.GetInstance();
            _Clock.OnTick += OnCollisionCheck;
        }

        private void OnCollisionCheck()
        {
            if (Physics.Raycast(transform.position + (Vector3.up * RAYCAST_OFFSETING), Vector3.up, out _Hit, RAYCAST_DISTANCE, _PlayerLayer))
                OnCollisionComportement();   
        }

        protected virtual void OnCollisionComportement() { }

        protected virtual void Destroy()
        {
            _Clock.OnTick -= OnCollisionCheck;
            _Clock = null;
        }

        private void OnDestroy() => Destroy();

    }
}