using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.UI
{
    public class HUD : MonoBehaviour
    {
        [SerializeField] private Button _GameButton = null;

        private Clock _Clock = null;
        private bool _IsPaused = false;

        private void Start()
        {
            _Clock = Clock.GetInstance();

            if(_GameButton != null)
                _GameButton.onClick.AddListener(_Clock.StartTicking);
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
                _GameButton.onClick.RemoveListener(_Clock.StartTicking);
            _Clock = null;
        }

    }
}