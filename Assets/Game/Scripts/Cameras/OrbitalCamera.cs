using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Camera
{
    public class OrbitalCamera : MonoBehaviour
    {

        [SerializeField] private float _MouseSpeed = 10f;
        [SerializeField] private float _KeyboardSpeed = 10f;

        [Header("Vertical parameters")]
        [SerializeField] private float _MinYAngle = 0f;
        [SerializeField] private float _MaxYAngle = 80f;

        [Header("Zoom parameters")]
        [SerializeField] private float _MinZoom = 5f;
        [SerializeField] private float _MaxZoom = 2f;
        [SerializeField] private float _ZoomForce = 0.5f;

        [Header("PC / Mouse")]
        [SerializeField] private string _VerticalMouseAxis = "";
        [SerializeField] private string _HorizontalMouseAxis = "";
        [SerializeField] private string _RightClickInput = "";
        [SerializeField] private string _ScrollWheel = "";

        [Header("PC / Keyboard")]
        [SerializeField] private string _HorizontalKeyboardAxis = "";
        [SerializeField] private string _VerticalKeyboardAxis = "";

        private Vector3 _Center = default;

        private float _Radius = 0f;

        private float _MaxRadius = 0f;
        private float _MinRadius = 0f;

        private float _HorizontalAngle = 0f;
        private float _VerticalAngle = 0f;

        private Touch _Touch = default;
        private Vector2 _TouchDirection = default;
        private Vector2 _StartTouchPosition = default;

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
            if (Input.touchCount > 0)
            {
                _Touch = Input.GetTouch(0);
                if(_Touch.phase == TouchPhase.Began)
                {
                    _StartTouchPosition = _Touch.position;
                }else if(_Touch.phase == TouchPhase.Moved)
                {
                    _TouchDirection = (_Touch.position - _StartTouchPosition).normalized;

                    _VerticalAngle += _MouseSpeed * Time.deltaTime * _TouchDirection.x * Mathf.Deg2Rad;
                    _HorizontalAngle += _MouseSpeed * Time.deltaTime * _TouchDirection.y * Mathf.Deg2Rad;
                }
            }

            #if UNITY_STANDALONE
            if (Input.GetButton(_RightClickInput))
            {
                // Mouse only
                _VerticalAngle += _MouseSpeed * Time.deltaTime * Input.GetAxis(_HorizontalMouseAxis) * Mathf.Deg2Rad;
                _HorizontalAngle += _MouseSpeed * Time.deltaTime * Input.GetAxis(_VerticalMouseAxis) * Mathf.Deg2Rad;
            }
            else
            {
                // Keyboard only
                _VerticalAngle += _KeyboardSpeed * Time.deltaTime * Input.GetAxis(_HorizontalKeyboardAxis) * Mathf.Deg2Rad;
                _HorizontalAngle += _KeyboardSpeed * Time.deltaTime * Input.GetAxis(_VerticalKeyboardAxis) * Mathf.Deg2Rad;
            }

            // Zoom options
            if (Input.GetAxis(_ScrollWheel) > 0f && _Radius >= _MinRadius)
            {
                _Radius -= _ZoomForce;

                UpdateCameraPositionOnCircle();
            }
            else if (Input.GetAxis(_ScrollWheel) < 0f && _Radius <= _MaxRadius)
            {
                _Radius += _ZoomForce;

                UpdateCameraPositionOnCircle();
            }
            #endif

            if (_VerticalAngle != 0 || _HorizontalAngle != 0)
            {
                _HorizontalAngle = Mathf.Clamp(_HorizontalAngle, _MinYAngle * Mathf.Deg2Rad, _MaxYAngle * Mathf.Deg2Rad);

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