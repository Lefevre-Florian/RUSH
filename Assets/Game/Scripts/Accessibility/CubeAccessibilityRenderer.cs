using Com.IsartDigital.Rush.Camera;
using UnityEngine;
using UnityEngine.UI;

namespace Com.IsartDigital.Rush.Accessibility
{
    [RequireComponent(typeof(Cube.Cube))]
    public class CubeAccessibilityRenderer : AccessibilityRenderer
    {

        private void Start()
        {
            Colors lID = GetComponent<Cube.Cube>().Color;
            Sprite lTexture = LoadTexture(lID);

            if (lTexture != null)
            {
                Transform lIcon = Instantiate(m_Prefab,
                                              transform.position + Vector3.up * m_Height,
                                              m_Prefab.transform.rotation,
                                              transform).transform;
                lIcon.GetComponentInChildren<Image>().sprite = lTexture;

                if (m_IsStatic)
                    lIcon.gameObject.isStatic = true;
                else
                {
                    m_Icon = lIcon;

                    m_Camera = OrbitalCamera.GetInstance();
                    m_Camera.OnMove += LookAt;
                }
            }
        }

        private void LateUpdate()
        {
            if(m_Icon != null)
            {
                m_Icon.position = transform.position + Vector3.up * m_Height;
            }
        }
    }
}