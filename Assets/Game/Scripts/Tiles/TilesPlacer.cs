using Com.IsartDigital.Rush.Data;
using Com.IsartDigital.Rush.Tiles;
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

        [Header("Tiles & Fabric")]
        [SerializeField] private Level _TileDatas = null;
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
        private TileData[] _TileFabric = new TileData[0];

        private GameObject _Preview = null;

        private bool _InputTriggerable = true;
        private bool _IsDisplayable = true;

        #if UNITY_ANDROID
        private Touch _Touch = default;
        #endif

        public TileData[] Tiles { get { return _TileFabric; } private set { _TileFabric = value; } }

        // Signals
        public event Action<int> OnTilePlaced;
        public event Action<int> OnTileRemoved;
        public event Action<int> OnTileChanged;

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

            if(_TileDatas == null)
            {
                _InputTriggerable = false;
                return;
            }

            SetTiles(_TileDatas);
        }

        private void Update()
        {
            if (!_InputTriggerable)
                return;

            if (_IsDisplayable && _Preview != null)
            {
                if (Physics.Raycast(_MainCamera.ScreenPointToRay(Input.mousePosition), out _Hit, float.MaxValue))
                    _Preview.transform.position = _Hit.collider.gameObject.transform.position;
            }

            // Input that place or destroy the tile
            #if UNITY_STANDALONE
            if (Input.GetButtonDown(_InputAccept))
                InsertTile(_MainCamera.ScreenPointToRay(Input.mousePosition));
            if (Input.GetButtonDown(_InputDelete))
                DeleteTile(_MainCamera.ScreenPointToRay(Input.mousePosition));
            #endif

            #if UNITY_ANDROID
            if(Input.touchCount > 0)
            {
                _Touch = Input.GetTouch(0);
                if(_Touch.phase == TouchPhase.Began)
                {
                    DeleteTile(_MainCamera.ScreenPointToRay(_Touch.position));
                    InsertTile(_MainCamera.ScreenPointToRay(_Touch.position));
                }
            }
            #endif
        }

        public void SetTiles(Level pData)
        {
            _TileDatas = pData;
            int lLength = _TileDatas.Tile.Length;

            _TileFabric = new TileData[lLength];
            _TilesLayers = new int[lLength];

            for (int i = 0; i < lLength; i++)
            {
                _TileFabric[i] = _TileDatas.Tile[i];
                _TilesLayers[i] = _TileFabric[i].prefab.layer;
            }

            _Preview = _TileFabric[0].prefab;
            _InputTriggerable = true;
        }

        private void DeleteTile(Ray pRay)
        {
            if (Physics.Raycast(pRay, out _Hit, float.MaxValue))
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
                        _TileFabric[lIndex].quantity += 1;
                        OnTileRemoved?.Invoke(_TileFabric[_CurrentIndex].quantity);

                        CheckFabricFullness();

                        Destroy(_TargetedGameobject.gameObject);
                    }
                }

            }
        }

        private void InsertTile(Ray pRay)
        {
            if (Physics.Raycast(pRay, out _Hit, float.MaxValue))
            {
                if (_Hit.collider.gameObject.layer == _GroundLayer)
                {
                    _TargetedGameobject = _Hit.collider.gameObject.transform;
                    if (_TargetedGameobject != null && _TileFabric[_CurrentIndex].quantity != 0)
                    {
                        _TileFabric[_CurrentIndex].quantity -= 1;
                        
                        DirectionalTiles lTile;
                        lTile = Instantiate(_TileFabric[_CurrentIndex].prefab,
                                            _TargetedGameobject.position + Vector3.up * (_TargetedGameobject.localScale.y / 2),
                                            new Quaternion(),
                                            _Container).GetComponent<DirectionalTiles>();
                        lTile.SetDirection(_TileFabric[_CurrentIndex].direction);

                        CheckFabricFullness();

                        OnTilePlaced?.Invoke(_TileFabric[_CurrentIndex].quantity);
                        if (_TileFabric[_CurrentIndex].quantity == 0) ChangeTileType();
                    }
                }
                    
            }
        }

        public void EnableInput() => _InputTriggerable = true;

        public void DisableInput() => _InputTriggerable = false;

        private void ChangeTileType()
        {
            _CurrentIndex = (_CurrentIndex + 1 >= _TileFabric.Length) ? 0 : _CurrentIndex + 1;

            OnTileChanged?.Invoke(_CurrentIndex);
        }

        private void CheckFabricFullness()
        {
            foreach (TileData lTile in _TileFabric)
            {
                if (lTile.quantity != 0)
                {
                    _IsDisplayable = true;
                    break;
                }
                _IsDisplayable = false;
            }
        }

        private void OnDestroy()
        {
            if (_Instance != null)
                _Instance = null;
        }
    }
}
