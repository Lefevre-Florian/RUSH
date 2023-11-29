using UnityEngine;

namespace Com.IsartDigital.Rush.Accessibility
{
    [RequireComponent(typeof(Cube.Cube))]
    public class CubeAccessibilityRenderer : AccessibilityRenderer
    {
        private void Start()
        {
            Colors lID = GetComponent<Cube.Cube>().Color;

            LoadTexture(lID);
        }
    }
}