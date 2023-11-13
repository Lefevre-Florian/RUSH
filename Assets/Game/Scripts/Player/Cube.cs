using Com.IsartDigital.Rush.Tiles;
using System;
using UnityEngine;
using UnityEngine.ProBuilder;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Cube
{
    public class Cube : MonoBehaviour
    {
        private const int MAX_DIRECTION_COUNT = 4;

        [Header("Raycasting")]
        [SerializeField] private float _RaycastOffsetOutSideCube = 0.4f;
        [SerializeField] private float _RaycastFallHeight = 2f;

        [Header("Collision Layers")]
        [SerializeField] private int _GroundLayer = 6;
        [SerializeField] private int _DirectionLayer = 9;
        [SerializeField] private int _StopperLayer = 10;
        [SerializeField] private int _ConvoyerLayer = 8;
        [SerializeField] private int _TeleporterLayer = 7;
        [SerializeField] private int _SpliterLayer = 12;

        [Header("Timing")]
        [SerializeField] private int _CollisionWallTickWait = 2;

        private Action DoAction = null;

        // Movements & rotations
        private bool _IsStuned = false;
        private Vector3 _MovementDirection = default;

        private Vector3 _InitialPosition, _TargetedPosition = default;
        private Quaternion _InitialRotation, _TargetedRotation = default;

        // Raycasting & collisions
        private float _RaycastDistance = 0f;
        private RaycastHit _Hit = default;

        // References
        private Clock _Clock = null;

        private int _ActionTick = 0;
        private int _InternalTick = 0;

        [HideInInspector] public Colors Color { get; private set; } = default;

        private void Start()
        {
            _RaycastDistance = transform.localScale.x / 2 + _RaycastOffsetOutSideCube;
            _MovementDirection = transform.forward;

            _Clock = Clock.GetInstance();
            _Clock.OnTick += InternalCheckCollision;

            SetActionVoid();
        }

        private void Update()
        {
            if (DoAction != null)
                DoAction();
        }

        public void Init(Colors pSpawnColor)
        {
            Material lMaterial = GetComponent<Renderer>().material;
            lMaterial.color = ColorLibrary.Library[pSpawnColor];

            Color = pSpawnColor;
        }

        #region State Machine
        private void SetActionMove()
        {
            Vector3 lPivot = transform.position + Vector3.down * (transform.localScale.y / 2) + _MovementDirection * (transform.localScale.x / 2);
            Vector3 lAxis = -Vector3.Cross(Vector3.up, transform.position - lPivot).normalized;

            _InitialRotation = transform.rotation;
            _TargetedRotation = Quaternion.AngleAxis(90f, lAxis) * _InitialRotation;

            _InitialPosition = transform.position;
            _TargetedPosition = lPivot + (Quaternion.AngleAxis(90f, lAxis) * (transform.position - lPivot));

            DoAction = DoActionMove;
        }

        private void DoActionMove()
        {            
            transform.position = Vector3.Lerp(_InitialPosition, _TargetedPosition, _Clock.Ratio);
            transform.rotation = Quaternion.Lerp(_InitialRotation, _TargetedRotation, _Clock.Ratio);
        }

        private void SetActionFall()
        {
            _InitialPosition = transform.position;
            _TargetedPosition = transform.position + Vector3.down;

            DoAction = DoActionFall;
        }

        private void DoActionFall() => transform.position = Vector3.Lerp(_InitialPosition, _TargetedPosition, _Clock.Ratio);

        private void SetActionTeleport()
        {
            SetActionWait();
            GetComponent<MeshRenderer>().enabled = false;
            transform.position = _TargetedPosition + Vector3.up * (transform.localScale.y / 2);

            _IsStuned = true;

            DoAction = DoActionTeleport;
        }

        private void DoActionTeleport()
        {
            if (_InternalTick == _ActionTick)
            {
                GetComponent<MeshRenderer>().enabled = true;

                _Clock.OnTick -= InternalClockTick;
                _IsStuned = false;
                SetActionMove();
            }
        }

        private void SetActionWait()
        {
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
                SetActionMove();
            }
        }

        private void SetActionConvoter(Vector3 pDirection)
        {
            _InitialPosition = transform.position;
            _TargetedPosition = _InitialPosition + (pDirection * transform.localScale.x);

            _IsStuned = true;

            SetActionWait();

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
                    SetActionMove();
                }
            }
            else
                transform.position = Vector3.Lerp(_InitialPosition, _TargetedPosition, _Clock.Ratio);
        }

        private void SetActionVoid() => DoAction = null;
        #endregion

        private void InternalClockTick() => _InternalTick += 1;

        private void InternalCheckCollision()
        {
            // Collision check on forward (Cubes & Walls)
            if(Physics.Raycast(transform.position, _MovementDirection, out _Hit, _RaycastDistance))
            {
                GameObject lCollided = _Hit.collider.gameObject;
                if (lCollided.layer == _GroundLayer)
                {
                    for (int i = 0; i < MAX_DIRECTION_COUNT; i++)
                    {
                        if (!Physics.Raycast(transform.position, _MovementDirection, _RaycastDistance)) break;
                        _MovementDirection = Quaternion.AngleAxis(90f, Vector3.up) * _MovementDirection;
                    }

                    _ActionTick = _CollisionWallTickWait;
                    SetActionWait();
                }
                else if (lCollided.layer == gameObject.layer)
                    Debug.Log("Collide with other player so loose the game");
            }

            // Collision check on Ground & Tiles
            if(Physics.Raycast(transform.position, Vector3.down, out _Hit, _RaycastDistance))
            {
                GameObject lCollided = _Hit.collider.gameObject;

                if (lCollided.layer == _GroundLayer)
                {
                    if(!_IsStuned)
                        SetActionMove();
                }
                else if (lCollided.layer == _TeleporterLayer)
                {
                    Teleporter lRef = lCollided.GetComponent<Teleporter>();

                    _TargetedPosition = lRef.OutputPosition;
                    _ActionTick = lRef.TeleportationTick;
                    SetActionTeleport();
                }
                else if (lCollided.layer == _StopperLayer)
                {
                    _ActionTick = lCollided.GetComponent<Stop>().Wait;
                    SetActionWait();
                }
                else if (lCollided.layer == _ConvoyerLayer)
                {
                    _ActionTick = lCollided.GetComponent<Stop>().Wait;
                    SetActionConvoter(lCollided.GetComponent<DirectionalTiles>().GetDirection());
                }
                else if (lCollided.layer == _SpliterLayer)
                {
                    _MovementDirection = lCollided.GetComponent<Spliter>().GetDirection();
                    SetActionMove();
                }
                else if (lCollided.layer == _DirectionLayer)
                {
                    _MovementDirection = lCollided.GetComponent<DirectionalTiles>().GetDirection();
                    SetActionMove();
                }
            }
            else
            {
                // Falling state + check if fall is infinite (in case trigger end of game)
                SetActionFall();
                if (!Physics.Raycast(transform.position, Vector3.down, out _Hit, _RaycastDistance * _RaycastFallHeight))
                {
                    SetActionVoid();
                }
            }
        }

        private void OnDestroy()
        {
            if(_Clock != null)
            {
                // Disconnecting every possible signals
                _Clock.OnTick -= InternalCheckCollision;
                _Clock.OnTick -= InternalClockTick;

                _Clock = null;
            }
        }

    }
}
