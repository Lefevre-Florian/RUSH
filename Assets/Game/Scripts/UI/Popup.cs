using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.UI
{
    public class Popup : Screen
    {
        [Header("Buttons")]
        [SerializeField] private Button _ResetBtn = null;
        [SerializeField] private Button _BackBtn = null;

        private HUD _Hud = null;

        protected override void Init()
        {
            _Hud = HUD.GetInstance();

            _ResetBtn.onClick.AddListener(_Hud.ResetGame);
            _BackBtn.onClick.AddListener(Back);
        }

        private void OnDestroy()
        {
            _ResetBtn.onClick.RemoveListener(_Hud.ResetGame);
            _BackBtn.onClick.RemoveListener(Back);
        }
    }
}