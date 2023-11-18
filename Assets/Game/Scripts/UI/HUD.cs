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

        private Clock _Clock = null;
        private bool _IsPaused = false;

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

            TilesPlacer lTilePlacer = TilesPlacer.GetInstance();

            _ResetButton.onClick.AddListener(ResetGame);
            _GameButton.onClick.AddListener(StartGameMode);
            _BackButton.onClick.AddListener(Back);
            _TimeSlider.onValueChanged.AddListener(OnSliderValueUpdated);

            Restore();
        }

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

        private void OnDestroy()
        {
            _BackButton.onClick.RemoveListener(Back);
            _ResetButton.onClick.RemoveListener(ResetGame);
            _GameButton.onClick.RemoveListener(StartGameMode);
            _Clock = null;

            if (_Instance != null)
                _Instance = null;
        }

    }
}