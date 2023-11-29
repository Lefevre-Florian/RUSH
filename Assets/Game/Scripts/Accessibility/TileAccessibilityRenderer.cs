using Com.IsartDigital.Rush.Tiles;
using UnityEngine;
using UnityEngine.UI;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Accessibility
{
    [RequireComponent(typeof(ColoredTiles))]
    public class TileAccessibilityRenderer : AccessibilityRenderer
    {
        private void Start()
        {
            Colors lID = GetComponent<ColoredTiles>().Color;
            Sprite lTexture = LoadTexture(lID);

            if(lTexture != null)
            {
                Transform lIcon = Instantiate(m_Prefab,
                                              transform.position + Vector3.up * m_Height,
                                              new Quaternion(),
                                              transform).transform;
                lIcon.GetComponentInChildren<Image>().sprite = lTexture;
            }
        }
    }
}