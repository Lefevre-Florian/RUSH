using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Cube
{
    public class Cube : MonoBehaviour
    {
        [SerializeField][Range(0.1f, 5f)] private float _Speed = 1f;

        [Header("Raycasting")]
        [SerializeField] private float _RaycastOffsetOutSideCube = 0.4f;
        [SerializeField] private float _RaycastFallHeight = 2f;

        [SerializeField] private int _GroundLayer = 1;

        private float _ElapsedTime = 0f;
        private float _DurationBetweenTick = 1f;

        private Action DoAction = null;

        // Movement & Rotation
        private float _Ratio = 0f;

        private Vector3 _MovementDirection = default;
        private Quaternion _RotationDirection = default;

        private float _RotationOffsetY = 0f;
        private Vector3 _InitialPosition, _TargetedPosition = default;
        private Quaternion _InitialRotation, _TargetedRotation = default;

        // Raycasting & collisions
        private float _RaycastDistance = 0f;
        private Vector3 _Down = default;
        private RaycastHit _Hit = default;

        private void Start()
        {
            float lCubeSide = transform.localScale.x;
            float lCubeDiagonal = Mathf.Sqrt(2) * lCubeSide;
            _RotationOffsetY = lCubeDiagonal / 2 - lCubeSide / 2;

            _RaycastDistance = lCubeSide / 2 + _RaycastOffsetOutSideCube;

            _MovementDirection = transform.forward;
            _RotationDirection = Quaternion.AngleAxis(90f, (-transform.up + transform.position) + (transform.position + transform.right));
            
            SetActionVoid();
        }

        private void Update()
        {
            Tick();
            if (DoAction != null)
                DoAction();
        }

        //Move the clock later in the clock main (and make it a coroutine)
        private void Tick()
        {
            if (_ElapsedTime > _DurationBetweenTick)
            {
                _ElapsedTime -= _DurationBetweenTick;

                InternalCheckCollision();
            }
            _ElapsedTime += Time.deltaTime * _Speed;
        }

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
            _Ratio = _ElapsedTime / _DurationBetweenTick;

            // Without compensating the height of the cube so it may clip in the ground
            //transform.position = Vector3.Lerp(_InitialPosition, _TargetedPosition, _Ratio);
            //transform.rotation = Quaternion.Lerp(_InitialRotation, _TargetedRotation,_Ratio);
            
            // With the height compensation base on the face diagonal so it doesn't clip in the ground
            transform.rotation = Quaternion.Lerp(_InitialRotation, _TargetedRotation, _Ratio);
            transform.position = Vector3.Lerp(_InitialPosition, _TargetedPosition, _Ratio) + (Vector3.up * _RotationOffsetY * Mathf.Sin(Mathf.PI * _Ratio));

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
            transform.position = Vector3.Lerp(_InitialPosition, _TargetedPosition, _ElapsedTime / _DurationBetweenTick);
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

    }
}
