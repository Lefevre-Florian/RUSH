using System;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Camera
{
    public class OrbitalCamera : MonoBehaviour
    {

        #region Singleton
        private static OrbitalCamera _Instance = null;

        public static OrbitalCamera GetInstance()
        {
            if (_Instance == null)
                _Instance = new OrbitalCamera();
            return _Instance;
        }

        private OrbitalCamera() : base() { }
        #endregion

        [Header("Support speed")]
        [SerializeField] private float _MouseSpeed = 10f;
        [SerializeField] private float _KeyboardSpeed = 10f;
        [SerializeField] private float _PhoneSpeed = 10f;

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

        #if UNITY_STANDALONE
        private float _MaxRadius = 0f;
        private float _MinRadius = 0f;
        #endif
        
        private float _HorizontalAngle = 0f;
        private float _VerticalAngle = 90f;

        #if UNITY_ANDROID
        private Touch _Touch = default;
        private Vector2 _TouchDirection = default;
        private Vector2 _StartTouchPosition = default;
        #endif

        // Signals
        public event Action OnMove;

        private void Awake()
        {
            if(_Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _Instance = this;
        }

        void Start()
        {
            _Center = Vector3.zero;

            transform.LookAt(_Center);
            _Radius = Vector3.Distance(transform.position, _Center);

            #if UNITY_STANDALONE
            _MaxRadius = _Radius + _MaxZoom;
            _MinRadius = _Radius - _MinZoom;
            #endif

            _HorizontalAngle = Mathf.Atan2(Vector3.up.y, Vector3.up.x);
            _VerticalAngle = Mathf.Acos(Vector3.up.z / _Radius);
            UpdateCameraPositionOnCircle();
        }

        void Update()
        {
            #if UNITY_ANDROID
            if (Input.touchCount > 0)
            {
                _Touch = Input.GetTouch(0);
                if(_Touch.phase == TouchPhase.Began)
                {
                    _StartTouchPosition = _Touch.position;
                }else if(_Touch.phase == TouchPhase.Moved)
                {
                    _TouchDirection = (_Touch.position - _StartTouchPosition).normalized;

                    _VerticalAngle += _PhoneSpeed * Time.deltaTime * _TouchDirection.x * Mathf.Deg2Rad;
                    _HorizontalAngle += _PhoneSpeed * Time.deltaTime * _TouchDirection.y * Mathf.Deg2Rad;
                }
            }
            #endif

            #if UNITY_STANDALONE
            if (Input.GetButton(_RightClickInput))
            {
                // Mouse only
                _VerticalAngle += _MouseSpeed * Time.deltaTime * Input.GetAxis(_HorizontalMouseAxis) * Mathf.Deg2Rad;
                _HorizontalAngle += _MouseSpeed * Time.deltaTime * Input.GetAxis(_VerticalMouseAxis) * Mathf.Deg2Rad;

                OnMove?.Invoke();
                UpdateCameraPositionOnCircle();
            }
            else if(Input.GetAxis(_HorizontalKeyboardAxis) != 0f || Input.GetAxis(_VerticalKeyboardAxis) != 0)
            {
                // Keyboard only
                _VerticalAngle += _KeyboardSpeed * Time.deltaTime * Input.GetAxis(_HorizontalKeyboardAxis) * Mathf.Deg2Rad;
                _HorizontalAngle += _KeyboardSpeed * Time.deltaTime * Input.GetAxis(_VerticalKeyboardAxis) * Mathf.Deg2Rad;

                OnMove?.Invoke();
                UpdateCameraPositionOnCircle();
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
        }

        private void UpdateCameraPositionOnCircle()
        {
            _HorizontalAngle = Mathf.Clamp(_HorizontalAngle, _MinYAngle * Mathf.Deg2Rad, _MaxYAngle * Mathf.Deg2Rad);

            transform.position = _Center + new Vector3(Mathf.Cos(_HorizontalAngle) * Mathf.Cos(_VerticalAngle),
                                                       Mathf.Sin(_HorizontalAngle),
                                                       Mathf.Cos(_HorizontalAngle) * Mathf.Sin(_VerticalAngle)) * _Radius;
            transform.LookAt(_Center);
        }

        public void CenterCameraOnPositionOnCircle(Vector3 pLookAt)
        {
            float lRadius = (_Center - pLookAt).magnitude;
            _VerticalAngle = Mathf.Acos(pLookAt.z / lRadius);
            _HorizontalAngle = Mathf.Atan2(pLookAt.y , pLookAt.x);

            UpdateCameraPositionOnCircle();
            transform.LookAt(pLookAt);
        }

        private void OnDestroy()
        {
            if (_Instance != null)
                _Instance = null;
        }

    }
}