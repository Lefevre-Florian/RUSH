using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.UI
{
    public class HUD : MonoBehaviour
    {
        private bool _IsPaused = false;

        public void ManagePauseMode()
        {
            _IsPaused = !_IsPaused;

            if (_IsPaused)
                Time.timeScale = 0f;
            else
                Time.timeScale = 1f;
        }

        public void StartGamePhase()
        {
            Clock lClock = Clock.GetInstance();
        }
    }
}