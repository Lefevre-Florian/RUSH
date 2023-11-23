using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Cube
{
    public class CubeRenderer : MonoBehaviour
    {
        [Header("Renderers")]
        [SerializeField] private MeshRenderer _BodyRenderer = null;
        [SerializeField] private MeshRenderer _CoreRenderer = null;

        public void Init(Color pColor)
        {
            _BodyRenderer.material.color = new Color(pColor.r,
                                                     pColor.g,
                                                     pColor.b,
                                                     _BodyRenderer.material.color.a);
            _CoreRenderer.material.color = pColor;
        }

        public void EnableVisibility() => _BodyRenderer.enabled = _CoreRenderer.enabled = true;

        public void DisableVisibility() => _BodyRenderer.enabled = _CoreRenderer.enabled = false;
    }
}