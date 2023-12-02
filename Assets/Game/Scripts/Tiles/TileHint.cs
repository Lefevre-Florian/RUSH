using Com.IsartDigital.Rush.Data;
using Com.IsartDigital.Rush.Managers;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Tiles
{
    public class TileHint : MonoBehaviour
    {
        private const float RAYCAST_OFFSET = 0.2f;

        [SerializeField] private bool _IsHintEnable = true;

        [Header("Tile")]
        [SerializeField] private TileData _Tile = default;

        [Space(5)]
        [SerializeField] private Color _WrongTileColor = Color.red;
        [SerializeField] private Color _RightTileColor = Color.green;

        private GameObject _Renderer = null;
        private Color _OriginalMaterial = default;

        private HintManager _HintManager = null;

        private void Start()
        {
            if (!_IsHintEnable)
                return;

            _Renderer = transform.GetChild(0).gameObject;
            _OriginalMaterial = _Renderer.GetComponent<Renderer>().material.color;

            _HintManager = HintManager.GetInstance();

            _HintManager.OnHintFullyDisplayed += DisplayCompleteHint;
            _HintManager.OnHintDisplayed += DisplayHint;

            _HintManager.OnHintHide += HideHint;

            HideHint();
        }

        private void DisplayHint()
        {
            _Renderer.GetComponent<Renderer>().material.color = _OriginalMaterial;

            _Renderer.SetActive(true);
        }

        private void HideHint() => _Renderer.SetActive(false);

        private void DisplayCompleteHint()
        {
            Color lColor;
            RaycastHit lHit = default;

            if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out lHit, transform.localScale.y + RAYCAST_OFFSET)
                && lHit.collider.gameObject.layer == _Tile.prefab.layer
                && lHit.collider.GetComponent<DirectionalTiles>().Direction == _Tile.direction)
                lColor = _RightTileColor;
            else
                lColor = _WrongTileColor;

            _Renderer.SetActive(true);
            _Renderer.GetComponent<Renderer>().material.color = new Color(lColor.r,
                                                                          lColor.g,
                                                                          lColor.b,
                                                                           _OriginalMaterial.a);
        }

        private void OnDestroy()
        {
            _HintManager.OnHintFullyDisplayed -= DisplayCompleteHint;
            _HintManager.OnHintDisplayed -= DisplayHint;

            _HintManager.OnHintHide -= HideHint;

            _HintManager = null;
        }

    }
}