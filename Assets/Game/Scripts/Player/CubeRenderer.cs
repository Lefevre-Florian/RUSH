using Com.IsartDigital.Rush.Data;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Cube
{
    public class CubeRenderer : MonoBehaviour
    {
        [Header("Renderers")]
        [SerializeField] private MeshRenderer _BodyRenderer = null;
        [SerializeField] private MeshRenderer _CoreRenderer = null;

        [Header("Accessibility options")]
        [SerializeField] private bool _IsColorblindModeOn = false;
        [SerializeField] private ColorData[] _AccessibilityLibrary = new ColorData[0];

        public void Init(Colors pColorID)
        {
            Color lColor = ColorLibrary.Library[pColorID];
            _BodyRenderer.material.color = new Color(lColor.r,
                                                     lColor.g,
                                                     lColor.b,
                                                     _BodyRenderer.material.color.a);
            _CoreRenderer.material.color = lColor;

            if (_IsColorblindModeOn)
            {
                Texture lIcon;
                int lLength = _AccessibilityLibrary.Length;
                for (int i = 0; i < lLength; i++)
                {
                    if (_AccessibilityLibrary[i].Color == pColorID) 
                        lIcon = _AccessibilityLibrary[i].Texture;
                }
            }
        }

        public void EnableVisibility() => _BodyRenderer.enabled = _CoreRenderer.enabled = true;

        public void DisableVisibility() => _BodyRenderer.enabled = _CoreRenderer.enabled = false;
    }
}