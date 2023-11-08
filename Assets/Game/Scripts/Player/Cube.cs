using Com.IsartDigital.Rush.Tiles;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Cube
{
    public class Cube : MonoBehaviour
    {
        [Header("Raycasting")]
        [SerializeField] private float _RaycastOffsetOutSideCube = 0.4f;
        [SerializeField] private float _RaycastFallHeight = 2f;

        [Header("Collision Layers")]
        [SerializeField] private int _GroundLayer = 1;
        [SerializeField] private int _TeleporterLayer = 1;

        private Action DoAction = null;

        // Movements & rotations
        private Vector3 _MovementDirection = default;
        private Quaternion _RotationDirection = default;

        private float _RotationOffsetY = 0f;
        private Vector3 _InitialPosition, _TargetedPosition = default;
        private Quaternion _InitialRotation, _TargetedRotation = default;

        // Raycasting & collisions
        private float _RaycastDistance = 0f;
        private Vector3 _Down = default;
        private RaycastHit _Hit = default;

        // References
        private Clock _Clock = null;

        private int _ActionTick = 0;
        private int _InternalTick = 0;
        private bool _CheckCollision = true;

        private void Start()
        {
            float lCubeSide = transform.localScale.x;
            float lCubeDiagonal = Mathf.Sqrt(2) * lCubeSide;
            _RotationOffsetY = lCubeDiagonal / 2 - lCubeSide / 2;

            _RaycastDistance = lCubeSide / 2 + _RaycastOffsetOutSideCube;

            _MovementDirection = transform.forward;
            _RotationDirection = Quaternion.AngleAxis(90f, transform.right);

            _Clock = Clock.GetInstance();
            _Clock.OnTick += InternalCheckCollision;

            SetActionVoid();
        }

        private void Update()
        {
            if (DoAction != null)
                DoAction();
        }

        private void SetActionMove()
        {
            //(transform.localPosition + (_MovementDirection * transform.localScale.x) + Vector3.down * 0.5f);

            _InitialRotation = transform.rotation;
            _TargetedRotation = _RotationDirection * _InitialRotation;

            _InitialPosition = transform.position;
            _TargetedPosition = _InitialPosition + (_MovementDirection * transform.localScale.x);

            //Quaternion.AngleAxis(lAngle * (i + 1), transform.right) * transform.up * lRadius + transform.position

            DoAction = DoActionMove;
        }

        private void DoActionMove()
        {
            // Without compensating the height of the cube so it may clip in the ground
            //transform.position = Vector3.Lerp(_InitialPosition, _TargetedPosition, _Ratio);
            //transform.rotation = Quaternion.Lerp(_InitialRotation, _TargetedRotation,_Ratio);
            
            // With the height compensation base on the face diagonal so it doesn't clip in the ground
            transform.rotation = Quaternion.Lerp(_InitialRotation, _TargetedRotation, _Clock.Ratio);
            transform.position = Vector3.Lerp(_InitialPosition, _TargetedPosition, _Clock.Ratio) + (Vector3.up * _RotationOffsetY * Mathf.Sin(Mathf.PI * _Clock.Ratio));

            // Check for the best solution so a Quaternion rotation around the forward base vertice
            //transform.position = Vector3.Lerp(_InitialPosition, _TargetedPosition, _Clock.Ratio);
            //transform.rotation = Quaternion.Lerp(_InitialRotation, _TargetedRotation, _Clock.Ratio);
        }

        private void SetActionFall()
        {
            _InitialPosition = transform.position;
            _TargetedPosition = transform.position + Vector3.down;

            DoAction = DoActionFall;
        }

        private void DoActionFall() => transform.position = Vector3.Lerp(_InitialPosition, _TargetedPosition, _Clock.Ratio);

        private void SetActionTeleport(Teleporter pTeleporter)
        {
            _InternalTick = 0;
            GetComponent<MeshRenderer>().enabled = false;

            _ActionTick = pTeleporter.TeleportationTick;
            transform.position = pTeleporter.OutputPosition + Vector3.up * (transform.localScale.y / 2);

            _Clock.OnTick += InternalClockTick;
            _Clock.OnTick -= InternalCheckCollision;

            DoAction = DoActionTeleport;
        }

        private void DoActionTeleport()
        {
            if (_InternalTick == _ActionTick)
            {
                GetComponent<MeshRenderer>().enabled = true;

                _Clock.OnTick -= InternalClockTick;
                _Clock.OnTick += InternalCheckCollision;

                SetActionMove();
            }
        }

        private void SetActionVoid() => DoAction = null;

        private void InternalClockTick() => _InternalTick += 1;

        private void InternalCheckCollision()
        {
            _Down = Vector3.down;

            if(Physics.Raycast(transform.position, _Down, out _Hit, _RaycastDistance))
            {
                Debug.DrawRay(transform.position, _Down * _RaycastDistance, Color.red);
                GameObject lCollided = _Hit.collider.gameObject;

                if (lCollided.layer == _GroundLayer)
                    SetActionMove();
                else if (lCollided.layer == _TeleporterLayer)
                    SetActionTeleport(lCollided.GetComponent<Teleporter>());
            }
            else
            {
                SetActionFall();
                /*if (!Physics.Raycast(transform.position, _Down, out _Hit, _RaycastDistance * _RaycastFallHeight))
                    SetActionVoid();*/
            }
        }

        private void OnDestroy()
        {
            if(_Clock != null)
            {
                _Clock.OnTick -= InternalCheckCollision;
                _Clock = null;
            }
        }

    }
}
