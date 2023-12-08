// Author : Lefevre Florian
using UnityEngine;

namespace Com.IsartDigital.Rush.Tiles
{
    public class ConvoyerTile : DirectionalTiles
    {        
        [Header("Juiciness")]
        [SerializeField] private GameObject _PushingHands = null;
        [SerializeField][Range(0.1f, 0.25f)] private float _HandsOffset = 0.15f;

        private Transform _HandsTransform = null;

        private Vector3 _InitialPosition = default;
        private Vector3 _FinalPosition = default;

        private bool _IsMovingSmth = false;

        protected override void Init()
        {
            base.Init();

            _HandsTransform = Instantiate(_PushingHands,
                                        transform.position + transform.right / 2f,
                                        new Quaternion(),
                                        transform).transform;
            _HandsTransform.gameObject.SetActive(false);
        }

        protected override void OnCollisionComportement()
        {
            Cube.Cube lCube = m_Hit.collider.gameObject.GetComponent<Cube.Cube>();
            lCube.SetActionSlideMove(GetDirection());

            _HandsTransform.gameObject.SetActive(true);
            _IsMovingSmth = true;

            m_Clock.OnTick += ResetAnimation;

            _InitialPosition = lCube.transform.position + (transform.right / 4f) - (m_DirectionalVector / (2f - _HandsOffset));
            _FinalPosition = lCube.transform.position + (transform.right / 4f) + (m_DirectionalVector / 2f);
        }

        private void ResetAnimation()
        {
            _HandsTransform.gameObject.SetActive(false);
            _IsMovingSmth = false;

            m_Clock.OnTick -= ResetAnimation;
        }

        private void Update()
        {
            if (_IsMovingSmth)
                _HandsTransform.position = Vector3.Lerp(_InitialPosition, _FinalPosition, m_Clock.Ratio);
        }

        protected override void Destroy()
        {
            m_Clock.OnTick -= ResetAnimation;

            base.Destroy();
        }

    }
}
