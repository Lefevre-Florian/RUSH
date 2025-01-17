using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Com.IsartDigital.Rush.Managers;
using TMPro;
using System;
using Com.IsartDigital.Rush.Camera;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.UI
{
    public class HUD : Screen
    {
        #region Singleton

        private static HUD _Instance = null;

        public static HUD GetInstance()
        {
            if (_Instance == null) 
                _Instance = new HUD();
            return _Instance;
        }

        private HUD() : base() { }
        #endregion

        [Serializable]
        private struct TileButton
        {
            public GameObject tile;
            public GameObject button;
        }

        [Header("Buttons")]
        [SerializeField] private CustomToggle _PauseButton = null;
        [SerializeField] private Button _GameButton = null;
        [SerializeField] private Button _ResetButton = null;
        [SerializeField] private Button _BackButton = null;

        [Space(10)]
        [SerializeField] private Button _HintButton = null;
        [SerializeField] private Button _HintFullButton = null;

        [Space(10)]
        [SerializeField] private Slider _TimeSlider = null;

        [Header("Section")]
        [SerializeField] private RectTransform _ReflexionPhaseCanvas = null;
        [SerializeField] private RectTransform _ActionPhaseCanvas = null;

        [Header("Popup & Text")]
        [SerializeField] private GameObject _LoosePopup = null;
        [SerializeField] private GameObject _WinPopup = null;

        [Header("Tiles")]
        [SerializeField] private RectTransform _Container = null;
        [SerializeField] private List<TileButton> _TileDictionary = new List<TileButton>();

        private Clock _Clock = null;
        private bool _IsPaused = false;

        // refs
        private OrbitalCamera _Camera = null;
        private TilesPlacer _TilePlacer = null;
        private HintManager _HintManager = null;

        private List<Button> _TileBtns = new List<Button>();
        private Vector3[] _TileDirection = new Vector3[0];

        private float _CurrentTimeScale = 1f;

        private void Awake()
        {
            if(_Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _Instance = this;
        }

        protected override void Init()
        {
            _CurrentTimeScale = Time.timeScale = 1f;

            _Clock = Clock.GetInstance();

            _TilePlacer = TilesPlacer.GetInstance();
            _Camera = OrbitalCamera.GetInstance();

            _ResetButton.onClick.AddListener(ResetGame);
            _GameButton.onClick.AddListener(StartGameMode);
            _BackButton.onClick.AddListener(Back);

            _PauseButton._Toggled += ManagePause;

            _HintButton.onClick.AddListener(CallHint);
            _HintFullButton.onClick.AddListener(CallFullHint);

            _HintManager = HintManager.GetInstance();

            _TimeSlider.onValueChanged.AddListener(OnSliderValueUpdated);

            // Init tiles buttons
            int lLength = _TilePlacer.Tiles.Length;
            GameObject lBtn, lPrefab = null;
            RectTransform lPrefabRect = null;

            _TileDirection = new Vector3[lLength];

            for (int i = 0; i < lLength; i++)
            {
                lPrefab = _TileDictionary.Find(lTile => lTile.tile == _TilePlacer.Tiles[i].prefab).button;
                lPrefabRect = lPrefab.GetComponent<RectTransform>();

                lBtn = Instantiate(lPrefab, _Container);

                int lIndex = i;
                lBtn.GetComponent<Button>().onClick.AddListener(delegate { SetTileButton(lIndex); });
                lBtn.GetComponentInChildren<Text>().text = _TilePlacer.Tiles[i].quantity.ToString();

                switch (_TilePlacer.Tiles[i].direction)
                {
                    case Vectors.FORWARD:
                        _TileDirection[i] = new Vector3(0f, 0f, Mathf.PI * Mathf.Rad2Deg);
                        break;
                    case Vectors.BACKWARD:
                        _TileDirection[i] = new Vector3(0f, 0f, -Mathf.PI * Mathf.Rad2Deg);
                        break;
                    case Vectors.RIGHT:
                        _TileDirection[i] = new Vector3(0f, 0f, Mathf.PI / 2 * Mathf.Rad2Deg);
                        break;
                    case Vectors.LEFT:
                        _TileDirection[i] = new Vector3(0f, 0f, -Mathf.PI / 2 * Mathf.Rad2Deg);
                        break;
                    default:
                        break;
                }

                lBtn.transform.rotation = Quaternion.Euler(_TileDirection[i]);
                _TileBtns.Add(lBtn.GetComponent<Button>());
            }
            UpdateTileOrientation();

            _Camera.OnMove += UpdateTileOrientation;

            _TilePlacer.OnTilePlaced += UpdateTileStatus;
            _TilePlacer.OnTileRemoved += UpdateTileStatus;

            Restore();
        }

        #region Menu comportement (Self comportement)
        private void Restore()
        {
            _LoosePopup.gameObject.SetActive(false);
            _WinPopup.gameObject.SetActive(false);

            _ReflexionPhaseCanvas.gameObject.SetActive(true);
            _ActionPhaseCanvas.gameObject.SetActive(false);

            _TilePlacer.EnableInput();
        }

        private void StartGameMode()
        {
            _Clock.StartTicking();

            _ReflexionPhaseCanvas.gameObject.SetActive(false);
            _ActionPhaseCanvas.gameObject.SetActive(true);

            _TilePlacer.DisableInput();

            _HintManager.ForceHideHint();
        }

        public void ResetGame()
        {
            Restore();
            _CurrentTimeScale = Time.timeScale = 1f;

            _TimeSlider.value = 1f;

            _GameButton.gameObject.SetActive(true);
            _Clock.ResetTicking();
        }

        private void ManagePause(bool pValue)
        {
            if (pValue)
                Time.timeScale = 0f;
            else
                Time.timeScale = _CurrentTimeScale;
        }

        private void OnSliderValueUpdated(float pValue)
        {
            _CurrentTimeScale = Time.timeScale = pValue;
            //_Clock.UpdateTickMultiplier(pValue);
        }

        private void CallHint() => _HintManager.DisplayHint();

        private void CallFullHint() => _HintManager.DisplayCompleteHint();

        /// <summary>
        /// Display the correct popup depending of the game over state at the end of the play phase
        /// </summary>
        /// <param name="pState">
        /// * true = win
        /// * false = loose
        /// </param>
        public void DisplayGameoverState(bool pState)
        {
            Time.timeScale = 1f;

            if (pState)
                _WinPopup.SetActive(true);
            else
                _LoosePopup.SetActive(true);
        }
        #endregion

        #region Tiles comportement in UI
        private void SetTileButton(int pIndex) => _TilePlacer.SetCurrentTileIndex(pIndex);

        private void UpdateTileStatus(int pIndex) => _TileBtns[pIndex].GetComponentInChildren<Text>().text = _TilePlacer.Tiles[pIndex].quantity.ToString();

        private void UpdateTileOrientation()
        {
            int lLength = _TileBtns.Count;
            for (int i = 0; i < lLength; i++)
            {
                if (_TilePlacer.Tiles[i].direction == Vectors.FORWARD)
                    _TileBtns[i].transform.rotation = Quaternion.Euler(0f, 0f, Mathf.PI * Mathf.Rad2Deg - _TileDirection[i].z + _Camera.transform.eulerAngles.y);
                else
                    _TileBtns[i].transform.rotation = Quaternion.Euler(0f, 0f, Mathf.PI * 2 * Mathf.Rad2Deg - _TileDirection[i].z + _Camera.transform.eulerAngles.y);
            }                    
        }
        #endregion

        private void OnDestroy()
        {
            _HintManager = null;

            _HintFullButton.onClick.RemoveListener(CallFullHint);
            _HintButton.onClick.RemoveListener(CallHint);

            _TilePlacer.OnTilePlaced -= UpdateTileStatus;
            _TilePlacer.OnTileRemoved -= UpdateTileStatus;

            _PauseButton._Toggled -= ManagePause;

            _Camera.OnMove -= UpdateTileOrientation;

            _Camera = null;
            _TilePlacer = null;
            _Clock = null;

            _BackButton.onClick.RemoveListener(Back);
            _ResetButton.onClick.RemoveListener(ResetGame);
            _GameButton.onClick.RemoveListener(StartGameMode);

            if (_Instance != null)
                _Instance = null;
        }

    }
}