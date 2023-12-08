using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Tiles
{
    public class ColoredTiles : MonoBehaviour
    {
        [SerializeField] private Colors _Color = default;

        public Colors Color { get { return _Color; } private set { _Color = value; } }

        public Color RealColor { get { return ColorLibrary.Library[_Color]; } private set {; } }

        private void Start()
        {
            Renderer lMaterial = null;
            int lLength = transform.childCount;
            for (int i = 0; i < lLength; i++)
            {
                if(transform.GetChild(i).TryGetComponent<Renderer>(out lMaterial))
                    lMaterial.material.color = ColorLibrary.Library[_Color];
            }
        }
    }
}