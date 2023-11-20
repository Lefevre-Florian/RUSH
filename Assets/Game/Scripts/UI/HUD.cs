using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Com.IsartDigital.Rush.Managers;
using TMPro;
using System;

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
        [SerializeField] private Button _PauseButton = null;
        [SerializeField] private Button _GameButton = null;
        [SerializeField] private Button _ResetButton = null;
        [SerializeField] private Button _BackButton = null;

        [Space(10)]
        [SerializeField] private Slider _TimeSlider = null;

        [Header("Popup & Text")]
        [SerializeField] private TextMeshProUGUI _MsgLabel = null;
        [SerializeField][TextArea] private string _LooseText = "";
        [SerializeField] private string[] _WinTexts = new string[0];

        [Header("Tiles")]
        [SerializeField] private RectTransform _Container = null;
        [SerializeField] private float _ContainerSpacingHeight = 50f;
        [SerializeField] private List<TileButton> _TileDictionary = new List<TileButton>();

        private Clock _Clock = null;
        private bool _IsPaused = false;

        private TilesPlacer _TilePlacer = null;

        private List<Button> _TileBtns = new List<Button>();

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
            _Clock = Clock.GetInstance();

            _TilePlacer = TilesPlacer.GetInstance();

            _ResetButton.onClick.AddListener(ResetGame);
            _GameButton.onClick.AddListener(StartGameMode);
            _BackButton.onClick.AddListener(Back);
            _TimeSlider.onValueChanged.AddListener(OnSliderValueUpdated);

            // Init tiles buttons
            int lLength = _TilePlacer.Tiles.Length;
            GameObject lBtn, lPrefab = null;
            RectTransform lPrefabRect = null;

            for (int i = 0; i < lLength; i++)
            {
                lPrefab = _TileDictionary.Find(lTile => lTile.tile == _TilePlacer.Tiles[i].prefab).button;
                lPrefabRect = lPrefab.GetComponent<RectTransform>();

                lBtn = Instantiate(lPrefab, 
                                   new Vector2(_Container.transform.position.x - (_Container.rect.width / 2 - (lPrefabRect.rect.width/2)),
                                               _Container.rect.height - ((i+1) * lPrefabRect.rect.size.y + _ContainerSpacingHeight * i)
                                                ),
                                   new Quaternion(),
                                   _Container);

                int lIndex = i;
                lBtn.GetComponent<Button>().onClick.AddListener(delegate { SetTileButton(lIndex); });
                lBtn.GetComponentInChildren<Text>().text = _TilePlacer.Tiles[i].quantity.ToString();

                _TileBtns.Add(lBtn.GetComponent<Button>());
            }

            _TilePlacer.OnTilePlaced += UpdateTileStatus;
            _TilePlacer.OnTileRemoved += UpdateTileStatus;

            Restore();
        }

        #region Menu comportement (Self comportement)
        private void Restore()
        {
            _PauseButton.gameObject.SetActive(false);
            _ResetButton.gameObject.SetActive(false);

            _MsgLabel.gameObject.SetActive(false);
        }

        private void StartGameMode()
        {
            _Clock.StartTicking();

            _PauseButton.gameObject.SetActive(true);
            _GameButton.gameObject.SetActive(false);
            _ResetButton.gameObject.SetActive(true);
        }

        private void ResetGame()
        {
            Restore();
            Time.timeScale = 1f;

            _GameButton.gameObject.SetActive(true);
            _Clock.ResetTicking();
        }

        private void OnSliderValueUpdated(float pValue) => _Clock.UpdateTickMultiplier(pValue);

        public void ManagePauseMode()
        {
            _IsPaused = !_IsPaused;

            if (_IsPaused)
                Time.timeScale = 0f;
            else
                Time.timeScale = 1f;
        }

        /// <summary>
        /// Display the correct popup depending of the game over state at the end of the play phase
        /// </summary>
        /// <param name="pState">
        /// * true = win
        /// * false = loose
        /// </param>
        public void DisplayGameoverState(bool pState)
        {
            Time.timeScale = 0f;

            _MsgLabel.gameObject.SetActive(true);
            if (pState)
                _MsgLabel.text = _WinTexts[UnityEngine.Random.Range(0, _WinTexts.Length - 1)];
            else
                _MsgLabel.text = _LooseText;
        }
        #endregion

        #region Tiles comportement in UI
        private void SetTileButton(int pIndex) => _TilePlacer.SetCurrentTileIndex(pIndex);

        private void UpdateTileStatus(int pIndex) => _TileBtns[pIndex].GetComponentInChildren<Text>().text = _TilePlacer.Tiles[pIndex].quantity.ToString();

        private void UpdateTileOrientation()
        {

        }
        #endregion

        private void OnDestroy()
        {
            _TilePlacer.OnTilePlaced -= UpdateTileStatus;
            _TilePlacer.OnTileRemoved -= UpdateTileStatus;

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