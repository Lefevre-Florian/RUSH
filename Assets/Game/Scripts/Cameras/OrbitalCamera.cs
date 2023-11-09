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

        [Header("Zoom parameters")]
        [SerializeField] private float _MinZoom = 5f;
        [SerializeField] private float _MaxZoom = 2f;
        [SerializeField] private float _ZoomForce = 0.5f;

        [Header("Windows / Laptop")]
        [SerializeField] private string _VerticalAxis = "";
        [SerializeField] private string _HorizontalAxis = "";
        [SerializeField] private string _RightClickInput = "";
        [SerializeField] private string _ScrollWheel = "";

        private Vector3 _Center = default;

        private float _Radius = 0f;

        private float _MaxRadius = 0f;
        private float _MinRadius = 0f;

        private float _HorizontalAngle = 0f;
        private float _VerticalAngle = 0f;

        void Start()
        {
            _Center = Vector3.zero;

            transform.LookAt(_Center);
            _Radius = Vector3.Distance(transform.position, _Center);

            _MaxRadius = _Radius + _MaxZoom;
            _MinRadius = _Radius - _MinZoom;
        }

        void Update()
        {
            if (Input.GetButton(_RightClickInput))
            {
                _VerticalAngle += _Speed * Time.deltaTime * Input.GetAxis(_HorizontalAxis) * Mathf.Deg2Rad;
                _HorizontalAngle += _Speed * Time.deltaTime * Input.GetAxis(_VerticalAxis) * Mathf.Deg2Rad;

                if(_VerticalAngle != 0 || _HorizontalAngle != 0)
                {
                    _HorizontalAngle = Mathf.Clamp(_HorizontalAngle, _MinYAngle * Mathf.Deg2Rad, _MaxYAngle * Mathf.Deg2Rad);

                    UpdateCameraPositionOnCircle();
                }
            }

            if(Input.GetAxis(_ScrollWheel) > 0f && _Radius >= _MinRadius)
            {
                _Radius -= _ZoomForce;

                UpdateCameraPositionOnCircle();
            }
            else if(Input.GetAxis(_ScrollWheel) < 0f && _Radius <= _MaxRadius)
            {
                _Radius += _ZoomForce;

                UpdateCameraPositionOnCircle();
            }

        }

        private void UpdateCameraPositionOnCircle()
        {
            transform.position = _Center + new Vector3(Mathf.Cos(_HorizontalAngle) * Mathf.Cos(_VerticalAngle),
                                                               Mathf.Sin(_HorizontalAngle),
                                                               Mathf.Cos(_HorizontalAngle) * Mathf.Sin(_VerticalAngle)) * _Radius;
            transform.LookAt(_Center);
        }
    }
}