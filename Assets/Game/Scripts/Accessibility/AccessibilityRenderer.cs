using Com.IsartDigital.Rush.Data;
using Com.IsartDigital.Rush.Cube;
using UnityEngine;
using Com.IsartDigital.Rush.Camera;
using UnityEngine.UI;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Accessibility
{
    public abstract class AccessibilityRenderer : MonoBehaviour
    {
        [Header("Options & Parameters")]
        [SerializeField] private bool m_IsColorblindModeOn = false;
        [SerializeField] private ColorData[] m_AccessibilityLibrary = new ColorData[0];

        [SerializeField] protected bool m_IsStatic = true;

        [Header("Renderer")]
        [SerializeField] protected GameObject m_Prefab = null;
        [SerializeField] protected float m_Height = 1f;

        protected OrbitalCamera m_Camera = null;
        protected Transform m_Icon = null;


        protected Sprite LoadTexture(Colors pColorID)
        {
            if (!m_IsColorblindModeOn)
                return null;

            Sprite lIcon = null;

            int lLength = m_AccessibilityLibrary.Length;
            for (int i = 0; i < lLength; i++)
            {
                if (m_AccessibilityLibrary[i].Color == pColorID)
                {
                    lIcon = m_AccessibilityLibrary[i].Texture;
                    break;
                }
            }

            return lIcon;
        }

        protected virtual void Destructor()
        {
            m_Icon = null;

            if (!m_IsStatic && m_Camera != null)
            {
                m_Camera.OnMove -= LookAt;
                m_Camera = null;
            }
        }

        protected void LookAt()
        {
            m_Icon.LookAt(new Vector3(m_Camera.transform.position.x,
                                      m_Icon.position.y,
                                      m_Camera.transform.position.z));
        }

        private void OnDestroy() => Destructor();
    }
}