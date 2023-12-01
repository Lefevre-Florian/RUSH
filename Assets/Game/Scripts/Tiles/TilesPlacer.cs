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

        private const float RAYCAST_OFFSET = 0.2f;
        private const float TILE_SIZE = 0.5f;

        [Header("Tiles & Fabric")]
        [SerializeField] private Level _TileDatas = null;
        [SerializeField] private int _GroundLayer = 6;
        [SerializeField] private LayerMask _GroundMask = default;

        [Header("Windows / PC")]
        [SerializeField] private string _InputAccept = "";
        [SerializeField] private string _InputDelete = "";

        [SerializeField] private string _InputMouseVertical = "";
        [SerializeField] private string _InputMouseHorizontal = "";

        [SerializeField] private Transform _Container = null;

        [Header("Juiciness")]
        [SerializeField] private GameObject _TileBlueprint = null;
        [SerializeField] private GameObject _CloudParticles = null;
        [SerializeField][Range(0f,2f)] private float _CloudHeight = 2f;

        [Header("Animation triggers")]
        [SerializeField] private string _TriggerDelete = "";
        [SerializeField] private string _TriggerCast = "";

        private RaycastHit _Hit = default;
        private UnityEngine.Camera _MainCamera = null;

        private Transform _TargetedGameobject = null;

        private int _CurrentIndex = 0;
        private int[] _TilesLayers = new int[0];
        private TileData[] _TileFabric = new TileData[0];

        // Renderer
        private Transform _Preview = null;
        private Renderer _PreviewRenderer = null;
        private Animator _PreviewAnimator = null;

        private bool _InputTriggerable = true;
        private bool _IsDisplayable = true;

        private float _RaycastDistance = TILE_SIZE + RAYCAST_OFFSET;

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
            
            if(_TileDatas == null)
            {
                _InputTriggerable = false;
                return;
            }
            SetTiles(_TileDatas);

        }

        private void Start()
        {
            _MainCamera = UnityEngine.Camera.main;
        }

        private void Update()
        {
            if (!_InputTriggerable)
                return;

            if ((Input.GetAxis(_InputMouseVertical) != 0f || Input.GetAxis(_InputMouseHorizontal) != 0f))
            {
                if(_Preview == null)
                {
                    _Preview = Instantiate(_TileBlueprint, transform.parent).transform;
                    _PreviewRenderer = _Preview.GetComponentInChildren<Renderer>();
                    _Preview.gameObject.SetActive(false);

                    _PreviewAnimator = Instantiate(_CloudParticles, 
                                                   _Preview.localPosition + Vector3.up * _CloudHeight , 
                                                   new Quaternion(), 
                                                   _Preview).GetComponentInChildren<Animator>();

                    UpdatePreview();
                }

                if (Physics.Raycast(_MainCamera.ScreenPointToRay(Input.mousePosition), out _Hit, float.MaxValue)
                    && (!Physics.Raycast(_Hit.collider.gameObject.transform.position, Vector3.up, _RaycastDistance, _GroundMask)))
                {
                    //Cast on the upper block
                    if (!_Preview.gameObject.activeSelf)
                        _Preview.gameObject.SetActive(true);

                    _Preview.transform.position = ((_Hit.collider.gameObject.layer == _GroundLayer) ? _Hit.collider.gameObject.transform.position : _Hit.collider.gameObject.transform.position - Vector3.up) + Vector3.up * TILE_SIZE;
                }
                else
                {
                    _Preview.gameObject.SetActive(false);
                }
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
                        if(_Preview != null) _PreviewAnimator.SetTrigger(_TriggerDelete);

                        int lLength = _TileFabric.Length;
                        int lIndex = -1;
                        for (int i = 0; i < lLength; i++)
                        {
                            if (_TileFabric[i].prefab.gameObject.layer == _TargetedGameobject.gameObject.layer 
                                && _TileFabric[i].direction == _TargetedGameobject.GetComponent<DirectionalTiles>().Direction)
                            {
                                lIndex = i;
                                break;
                            }
                        }

                        if(lIndex != -1)
                        {
                            _TileFabric[lIndex].quantity += 1;
                            OnTileRemoved?.Invoke(lIndex);

                            _CurrentIndex = lIndex;
                            UpdatePreview();
                        }
                        
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
                        if (_Preview != null) _PreviewAnimator.SetTrigger(_TriggerCast);

                        _TileFabric[_CurrentIndex].quantity -= 1;

                        DirectionalTiles lTile;
                        lTile = Instantiate(_TileFabric[_CurrentIndex].prefab,
                                            _TargetedGameobject.position + Vector3.up * (_TargetedGameobject.localScale.y / 2),
                                            new Quaternion(),
                                            _Container).GetComponent<DirectionalTiles>();
                        lTile.SetDirection(_TileFabric[_CurrentIndex].direction);

                        OnTilePlaced?.Invoke(_CurrentIndex);
                        if (_TileFabric[_CurrentIndex].quantity == 0) ChangeTileType();
                    }
                }
                    
            }
        }

        public void EnableInput() => _InputTriggerable = true;

        public void DisableInput() => _InputTriggerable = false;
        
        public void SetCurrentTileIndex(int pIndex)
        {
            if (pIndex >= 0 && pIndex < _TileFabric.Length)
            {
                _CurrentIndex = pIndex;
                UpdatePreview();
            }
        }

        private void ChangeTileType()
        {
            _CurrentIndex = (_CurrentIndex + 1 >= _TileFabric.Length) ? 0 : _CurrentIndex + 1;
            UpdatePreview();

            OnTileChanged?.Invoke(_CurrentIndex);
        }

        private void UpdatePreview()
        {
            if (_Preview == null)
                return;

            if (CheckFabricFullness())
            {
                _PreviewRenderer.material.mainTexture = _TileFabric[_CurrentIndex].Material.mainTexture;

                Vector3 lLookDirection = Vector3.zero;
                switch (_TileFabric[_CurrentIndex].direction)
                {
                    case Vectors.FORWARD:
                        lLookDirection = Vector3.forward;
                        break;
                    case Vectors.BACKWARD:
                        lLookDirection = Vector3.back;
                        break;
                    case Vectors.RIGHT:
                        lLookDirection = Vector3.right;
                        break;
                    case Vectors.LEFT:
                        lLookDirection = Vector3.left;
                        break;
                    default:
                        break;
                }

                _PreviewRenderer.transform.LookAt(_PreviewRenderer.transform.position + lLookDirection);
            }
            else
            {
                _PreviewRenderer.material.mainTexture = null;
            }
        }

        private bool CheckFabricFullness()
        {
            foreach (TileData lTile in _TileFabric)
            {
                if (lTile.quantity != 0)
                    return true;
            }
            return false;
        }

        private void OnDestroy()
        {
            if (_Instance != null)
                _Instance = null;
        }
    }
}
