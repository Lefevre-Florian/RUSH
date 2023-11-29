using Com.IsartDigital.Rush.Data;
using Com.IsartDigital.Rush.Cube;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Accessibility
{
    public abstract class AccessibilityRenderer : MonoBehaviour
    {
        [Header("Options & Parameters")]
        [SerializeField] private bool m_IsColorblindModeOn = false;
        [SerializeField] private ColorData[] m_AccessibilityLibrary = new ColorData[0];

        [Header("Renderer")]
        [SerializeField] protected GameObject m_Prefab = null;
        [SerializeField] protected float m_Height = 1f;

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
    }
}