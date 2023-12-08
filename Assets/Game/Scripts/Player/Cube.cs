using Com.IsartDigital.Rush.Accessibility;
using System;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Cube
{
    public class Cube : MonoBehaviour
    {
        private const float ROTATION_ANGLE = 90f;

        private const int MAX_DIRECTION_COUNT = 4;
        private const int MAX_FALLING_HEIGHT = 3;

        [Header("Raycasting")]
        [SerializeField] private float _RaycastOffsetOutSideCube = 0.4f;
        [SerializeField] private float _RaycastFallHeight = 2f;

        [Header("Collision Layers")]
        [SerializeField] private int _GroundLayer = 6;
        [SerializeField] private LayerMask _Ground = default;

        [Header("Timing")]
        [SerializeField] private int _CollisionWallTickWait = 2;

        [Header("Animation")]
        [SerializeField] private string _LandingAnimation = "";
        [SerializeField] private string _WallAnimation = "";
        [SerializeField] private string _DeathAnimation = "";

        [Space(5)]
        [SerializeField] private CubeRenderer _Renderer = null;

        private Action DoAction = null;

        // Movements & rotations
        private bool _IsStuned = false;
        private Vector3 _MovementDirection = default;

        private Vector3 _InitialPosition, _TargetedPosition = default;
        private Quaternion _InitialRotation, _TargetedRotation = default;

        private int _FallCounter = 0;

        // Raycasting & collisions
        private float _RaycastDistance = 0f;
        private RaycastHit _Hit = default;

        // Animations
        private Animator _Animator = null;

        // References
        private Clock _Clock = null;

        // Components
        public CubeRenderer Renderer { get { return _Renderer; } private set { _Renderer = value; } }

        private int _ActionTick = 0;
        private int _InternalTick = 0;

        [HideInInspector] public Colors Color { get; private set; } = default;

        // Signals
        public event Action<Vector3> OnDied;

        private void Start()
        {
            _RaycastDistance = transform.localScale.x / 2 + _RaycastOffsetOutSideCube;
            _MovementDirection = transform.forward;

            _Clock = Clock.GetInstance();
            _Clock.OnTick += InternalCheckCollision;
            _Clock.OnReset += Clear;

            _Animator = GetComponentInChildren<Animator>();

            SetActionVoid();
        }

        private void Update()
        {
            if (Physics.Raycast(transform.position, _MovementDirection, out _Hit, transform.localScale.x / 2 - _RaycastOffsetOutSideCube) 
                && _Hit.transform.gameObject.layer == gameObject.layer)
                OnDied?.Invoke(transform.position);
                
            if (DoAction != null)
                DoAction();
        }

        public void Init(Colors pSpawnColor) => Color = pSpawnColor;

        #region State Machine
        public void SetActionDeath()
        {
            SetActionVoid();
        }
        
        public void SetActionMove()
        {
            _IsStuned = false;
            Vector3 lPivot = transform.position + Vector3.down * (transform.localScale.y / 2) + _MovementDirection * (transform.localScale.x / 2);
            Vector3 lAxis = -Vector3.Cross(Vector3.up, transform.position - lPivot).normalized;

            _InitialRotation = transform.rotation;
            _TargetedRotation = Quaternion.AngleAxis(ROTATION_ANGLE, lAxis) * _InitialRotation;

            _InitialPosition = transform.position;
            _TargetedPosition = lPivot + (Quaternion.AngleAxis(ROTATION_ANGLE, lAxis) * (transform.position - lPivot));

            DoAction = DoActionMove;
        }

        private void DoActionMove()
        {            
            transform.rotation = Quaternion.Lerp(_InitialRotation, _TargetedRotation, _Clock.Ratio);
            transform.position = Vector3.Lerp(_InitialPosition, _TargetedPosition, _Clock.Ratio);
        }

        public void SetDirectionMove(Vector3 pDirection)
        {
            _MovementDirection = pDirection;
            SetActionMove();
        }

        private void SetActionFall()
        {
            _InitialPosition = transform.position;
            _TargetedPosition = transform.position + Vector3.down;

            DoAction = DoActionFall;
        }

        private void DoActionFall() => transform.position = Vector3.Lerp(_InitialPosition, _TargetedPosition, _Clock.Ratio);

        public void SetActionWait(int pWaitDuration = 1)
        {
            _Clock.OnTick -= InternalClockTick;

            _ActionTick = pWaitDuration;
            _InternalTick = 0;

            _Clock.OnTick += InternalClockTick;
            _IsStuned = true;

            DoAction = DoActionWait;
        }

        private void DoActionWait()
        {
            if(_InternalTick == _ActionTick)
            {
                _Clock.OnTick -= InternalClockTick;
                _IsStuned = false;
                InternalCheckCollision();
            }
        }

        public void SetActionSlideMove(Vector3 pDirection)
        {
            if (Physics.Raycast(transform.position, pDirection, _RaycastDistance, _Ground))
            {
                SetActionVoid();
                return;
            }

            _Clock.OnTick -= InternalClockTick;

            _InitialPosition = transform.position;
            _TargetedPosition = _InitialPosition + (pDirection * transform.localScale.x);

            _IsStuned = true;

            _Clock.OnTick += InternalClockTick;
            
            _InternalTick = 0;
            _ActionTick = 2;

            DoAction = DoActionConvoyer;
        }

        private void DoActionConvoyer()
        {
            if (transform.position == _TargetedPosition)
            {
                if(_InternalTick == _ActionTick)
                {
                    _Clock.OnTick -= InternalClockTick;
                    _IsStuned = false;
                }
            }
            else
                transform.position = Vector3.Lerp(_InitialPosition, _TargetedPosition, _Clock.Ratio);
        }

        private void SetActionVoid() => DoAction = null;
        #endregion

        private void InternalClockTick()
        {
            _InternalTick += 1;
        }

        private void InternalCheckCollision()
        {
            // Collision check on forward (Cubes & Walls)
            if(Physics.Raycast(transform.position, _MovementDirection, out _Hit, _RaycastDistance))
            {
                GameObject lCollided = _Hit.collider.gameObject;
                if (lCollided.layer == _GroundLayer && !_IsStuned)
                {
                    _Animator.SetTrigger(_WallAnimation);

                    for (int i = 0; i < MAX_DIRECTION_COUNT; i++)
                    {
                        if (!Physics.Raycast(transform.position, _MovementDirection, _RaycastDistance, _Ground)) break;
                        _MovementDirection = Quaternion.AngleAxis(90f, Vector3.up) * _MovementDirection;
                    }

                    SetActionWait(_CollisionWallTickWait);
                }
            }

            // Collision check on Ground
            if(Physics.Raycast(transform.position, Vector3.down, out _Hit, _RaycastDistance, _Ground))
            {
                _FallCounter = 0;

                if (DoAction == DoActionFall)
                    _Animator.SetTrigger(_LandingAnimation);

                if (!_IsStuned)
                    SetActionMove();
            }
            else
            {
                // Falling state + check if fall is infinite (in case trigger end of game)
                SetActionFall();
                if (++_FallCounter > MAX_FALLING_HEIGHT
                    && !Physics.Raycast(transform.position, Vector3.down, out _Hit, _RaycastDistance * _RaycastFallHeight))
                {
                    SetActionVoid();
                    OnDied?.Invoke(transform.position);
                }
            }
        }

        private void Clear() => Destroy(gameObject);

        private void OnDestroy()
        {
            if(_Clock != null)
            {
                // Disconnecting every possible signals
                _Clock.OnTick -= InternalCheckCollision;
                _Clock.OnTick -= InternalClockTick;

                _Clock.OnReset -= Clear;

                _Clock = null;
            }
        }

    }
}
