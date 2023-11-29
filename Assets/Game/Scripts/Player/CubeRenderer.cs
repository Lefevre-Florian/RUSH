using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Cube
{
    [RequireComponent(typeof(Cube))]
    public class CubeRenderer : MonoBehaviour
    {
        [Header("Renderers")]
        [SerializeField] private MeshRenderer _BodyRenderer = null;
        [SerializeField] private MeshRenderer _CoreRenderer = null;

        private void Start()
        {
            Color lColor = ColorLibrary.Library[GetComponent<Cube>().Color];

            _BodyRenderer.material.color = new Color(lColor.r,
                                                     lColor.g,
                                                     lColor.b,
                                                     _BodyRenderer.material.color.a);
            _CoreRenderer.material.color = lColor;
        }

        public void EnableVisibility() => _BodyRenderer.enabled = _CoreRenderer.enabled = true;

        public void DisableVisibility() => _BodyRenderer.enabled = _CoreRenderer.enabled = false;
    }
}