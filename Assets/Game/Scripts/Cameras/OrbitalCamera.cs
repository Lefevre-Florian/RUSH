using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Camera
{
    public class OrbitalCamera : MonoBehaviour
    {

        [SerializeField] private float _Speed = 10f;

        [Header("Vertical parameters")]
        [SerializeField] private float _MinYAngle = 0f;
        [SerializeField] private float _MaxYAngle = 80f;

        [Header("Windows / Laptop")]
        [SerializeField] private string _VerticalAxis = "";
        [SerializeField] private string _HorizontalAxis = "";
        [SerializeField] private string _RightClickInput = "";
        [SerializeField] private string _ScrollWheel = "";

        private Vector3 _Center = default;

        private float _Radius = 0f;

        private float _HorizontalAngle = 0f;
        private float _VerticalAngle = 0f;

        private Vector3 _Direction;

        void Start()
        {
            _Center = Vector3.zero;

            transform.LookAt(_Center);
            _Radius = Vector3.Distance(transform.position, _Center);
        }

        void Update()
        {
            if (Input.GetButton(_RightClickInput))
            {

                _VerticalAngle += _Speed * Time.deltaTime * Input.GetAxis(_HorizontalAxis) * Mathf.Deg2Rad;
                _HorizontalAngle += _Speed * Time.deltaTime * Input.GetAxis(_VerticalAxis) * Mathf.Deg2Rad;

                if(_VerticalAngle != 0 || _HorizontalAngle != 0)
                {
                    Debug.Log(_HorizontalAngle);
                    if (_HorizontalAngle > _MaxYAngle * Mathf.Deg2Rad || _HorizontalAngle < _MinYAngle * Mathf.Deg2Rad)
                        return;

                    transform.position = _Center + new Vector3(Mathf.Cos(_HorizontalAngle) * Mathf.Cos(_VerticalAngle),
                                                               Mathf.Sin(_HorizontalAngle),
                                                               Mathf.Cos(_HorizontalAngle) * Mathf.Sin(_VerticalAngle)) * _Radius;
                    transform.LookAt(_Center);
                }
            }
        }
    }
}