using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Tiles
{
    public abstract class Tile : MonoBehaviour
    {

        private const float RAYCAST_OFFSETING = -0.3f;
        private const float RAYCAST_DISTANCE = 1f;

        [SerializeField] private LayerMask _PlayerLayer = default;

        protected Clock m_Clock = null;
        protected RaycastHit m_Hit = default;

        private void Start() => Init();

        protected virtual void Init()
        {
            m_Clock = Clock.GetInstance();
            m_Clock.OnTick += OnCollisionCheck;
        }

        private void OnCollisionCheck()
        {
            if (Physics.Raycast(transform.position + (Vector3.up * RAYCAST_OFFSETING), 
                                Vector3.up, 
                                out m_Hit, 
                                RAYCAST_DISTANCE, 
                                _PlayerLayer))
                OnCollisionComportement();   
        }

        protected virtual void OnCollisionComportement() { }

        protected virtual void Destroy()
        {
            m_Clock.OnTick -= OnCollisionCheck;
            m_Clock = null;
        }

        private void OnDestroy() => Destroy();

    }
}