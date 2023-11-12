using System;
using System.Linq;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Managers
{
    public class TilesPlacer : MonoBehaviour
    {
        #region Singleton
        private static TilesPlacer _Instance = null;

        private TilesPlacer() : base() { }

        public static TilesPlacer GetInstance()
        {
            if (_Instance == null)
                _Instance = new TilesPlacer();
            return _Instance;
        }
        #endregion

        [Serializable]
        private struct Tiles
        {
            public GameObject _Type;
            public int _Quantity;
        }

        [Header("Tiles & Fabric")]
        [SerializeField] private Tiles[] _TileFabric = new Tiles[0];
        [SerializeField] private int _GroundLayer = 6;

        [Header("Windows / PC")]
        [SerializeField] private string _InputAccept = "";
        [SerializeField] private string _InputDelete = "";
        [SerializeField] private string _InputScroll = "";

        [SerializeField] private Transform _Container = null;

        private RaycastHit _Hit = default;
        private UnityEngine.Camera _MainCamera = null;

        private Transform _TargetedGameobject = null;

        private int _CurrentIndex = 0;
        private int[] _TilesLayers = new int[0];

        private void Awake()
        {
            if(_Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _Instance = this;
        }

        private void Start()
        {
            _MainCamera = UnityEngine.Camera.main;

            int lLength = _TileFabric.Length;
            _TilesLayers = new int[lLength];
            for (int i = 0; i < lLength; i++)
                _TilesLayers[i] = _TileFabric[i]._Type.layer;
        }

        private void Update()
        {
            if (Input.GetButtonDown(_InputAccept))
                InsertTile();
            if (Input.GetButtonDown(_InputDelete))
                DeleteTile();
        }

        private void DeleteTile()
        {
            if (Physics.Raycast(_MainCamera.ScreenPointToRay(Input.mousePosition), out _Hit, float.MaxValue))
            {
                if (_TilesLayers.Contains(_Hit.collider.gameObject.layer))
                {
                    _TargetedGameobject = _Hit.collider.gameObject.transform;
                    if (_TargetedGameobject != null)
                    {
                        int lLength = _TilesLayers.Length;
                        int lIndex = 0;
                        for (int i = 0; i < lLength; i++)
                        {
                            if (_TilesLayers[i] == _TargetedGameobject.gameObject.layer)
                            {
                                lIndex = i;
                                break;
                            }
                        }
                        _TileFabric[lIndex]._Quantity += 1;
                        Destroy(_TargetedGameobject.gameObject);
                    }
                }

            }
        }

        private void InsertTile()
        {
            if (Physics.Raycast(_MainCamera.ScreenPointToRay(Input.mousePosition), out _Hit, float.MaxValue))
            {
                if (_Hit.collider.gameObject.layer == _GroundLayer)
                {
                    _TargetedGameobject = _Hit.collider.gameObject.transform;
                    if (_TargetedGameobject != null && _TileFabric[_CurrentIndex]._Quantity != 0)
                    {
                        _TileFabric[_CurrentIndex]._Quantity -= 1;
                        Instantiate(_TileFabric[_CurrentIndex]._Type,
                                    _TargetedGameobject.position + Vector3.up,
                                    new Quaternion(),
                                    _Container);

                        if (_TileFabric[_CurrentIndex]._Quantity == 0) ChangeTileType();
                    }
                }
                    
            }
        }

        private void ChangeTileType()
        {
            _CurrentIndex = (_CurrentIndex + 1 >= _TileFabric.Length) ? 0 : _CurrentIndex + 1;
            Debug.Log(_CurrentIndex);
        }

        private void OnDestroy()
        {
            if (_Instance != null)
                _Instance = null;
        }
    }
}
