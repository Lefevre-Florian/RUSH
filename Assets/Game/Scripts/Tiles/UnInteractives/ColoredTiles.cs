using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Tiles
{
    public class ColoredTiles : MonoBehaviour
    {
        [SerializeField] private Colors _Color = default;

        public Colors Color { get { return _Color; } private set { _Color = value; } }

        private void Start()
        {
            Material lMaterial = null;
            int lLength = transform.childCount;
            for (int i = 0; i < lLength; i++)
            {
                lMaterial = transform.GetChild(i).GetComponent<Renderer>().material;
                lMaterial.color = ColorLibrary.Library[_Color];
            }
        }
    }
}