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

        [SerializeField] private int _GroundLayer = 1;

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

        //Move the clock later in the clock main (and make it a coroutine)

        private void SetActionMove()
        {
            _InitialRotation = transform.rotation;
            _TargetedRotation = _RotationDirection * _InitialRotation;

            _InitialPosition = transform.position;
            _TargetedPosition = _InitialPosition + (_MovementDirection * transform.localScale.x);

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
        }

        private void SetActionFall()
        {
            _InitialPosition = transform.position;
            _TargetedPosition = transform.position + Vector3.down;

            DoAction = DoActionFall;
        }

        private void DoActionFall()
        {
            transform.position = Vector3.Lerp(_InitialPosition, _TargetedPosition, _Clock.Ratio);
        }

        private void SetActionVoid() => DoAction = null;

        private void InternalCheckCollision()
        {
            _Down = Vector3.down;

            if(Physics.Raycast(transform.position, _Down, out _Hit, _RaycastDistance))
            {
                Debug.DrawRay(transform.position, _Down * _RaycastDistance, Color.red);
                GameObject lCollided = _Hit.collider.gameObject;

                if (lCollided.layer == _GroundLayer)
                    SetActionMove();
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