using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Com.IsartDigital.Rush.Managers;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.UI
{
    public class HUD : MonoBehaviour
    {
        [SerializeField] private Button _PauseButton = null;
        [SerializeField] private Button _GameButton = null;

        private Clock _Clock = null;
        private bool _IsPaused = false;

        private void Start()
        {
            _Clock = Clock.GetInstance();

            TilesPlacer lTilePlacer = TilesPlacer.GetInstance();

            _PauseButton.gameObject.SetActive(false);

            if(_GameButton != null)
                _GameButton.onClick.AddListener(StartGameMode);
        }

        private void StartGameMode()
        {
            _Clock.StartTicking();

            _PauseButton.gameObject.SetActive(true);
            _GameButton.gameObject.SetActive(false);
        }

        public void ManagePauseMode()
        {
            _IsPaused = !_IsPaused;

            if (_IsPaused)
                Time.timeScale = 0f;
            else
                Time.timeScale = 1f;
        }

        private void OnDestroy()
        {
            if (_GameButton != null)
                _GameButton.onClick.RemoveListener(StartGameMode);
            _Clock = null;
        }

    }
}