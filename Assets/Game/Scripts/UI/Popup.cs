using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.UI
{
    [RequireComponent(typeof(Animator))]
    public class Popup : Screen
    {
        [Header("Buttons")]
        [SerializeField] private Button _ResetBtn = null;
        [SerializeField] private Button _BackBtn = null;

        [Header("Juiciness")]
        [SerializeField] private string _Trigger = "";
        [SerializeField] private Animator _Animator = null;

        private HUD _Hud = null;

        protected override void Init()
        {
            _Hud = HUD.GetInstance();

            _ResetBtn.onClick.AddListener(_Hud.ResetGame);
            _BackBtn.onClick.AddListener(Back);
        }

        private void OnEnable()
        {
            if (_Animator != null)
                _Animator.SetTrigger(_Trigger);      
        }

        private void OnDestroy()
        {
            _ResetBtn.onClick.RemoveListener(_Hud.ResetGame);
            _BackBtn.onClick.RemoveListener(Back);
        }
    }
}