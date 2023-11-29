using Com.IsartDigital.Rush.Camera;
using Com.IsartDigital.Rush.Tiles;
using UnityEngine;
using UnityEngine.UI;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Accessibility
{
    [RequireComponent(typeof(ColoredTiles))]
    public class TileAccessibilityRenderer : AccessibilityRenderer
    {
        [SerializeField] private bool _IsStatic = true;

        private OrbitalCamera _Camera = null;
        private Transform _Icon = null;

        private void Start()
        {
            Colors lID = GetComponent<ColoredTiles>().Color;
            Sprite lTexture = LoadTexture(lID);

            if(lTexture != null)
            {
                Transform lIcon = Instantiate(m_Prefab,
                                              transform.position + Vector3.up * m_Height,
                                              m_Prefab.transform.rotation,
                                              transform).transform;
                lIcon.GetComponentInChildren<Image>().sprite = lTexture;

                if (_IsStatic)
                    lIcon.gameObject.isStatic = true;
                else
                {
                    _Icon = lIcon;

                    _Camera = OrbitalCamera.GetInstance();
                    _Camera.OnMove += LookAt;
                }
            }
        }

        private void LookAt()
        {
            _Icon.LookAt(new Vector3(_Camera.transform.position.x,
                                     _Icon.position.y,
                                     _Camera.transform.position.z));
        }

        private void OnDestroy()
        {
            _Icon = null;

            if (!_IsStatic)
            {
                _Camera.OnMove -= LookAt;
                _Camera = null;
            }
        }
    }
}