using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.IsartDigital.Rush.UI
{
    public class Titlecard : MonoBehaviour
    {

        [SerializeField] private Button _ExitButton = default;

        private void Start()
        {
            #if UNITY_ANDROID
            Destroy(_ExitButton);
            #endif
            #if UNITY_STANDALONE
            if (_ExitButton == null)
                return;

            _ExitButton.onClick.AddListener(Exit);
            #endif
        }

        private void Exit()
        {
            #if UNITY_STANDALONE
            Application.Quit();
            #endif

            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }

        private void OnDestroy()
        {
            #if UNITY_STANDALONE
            if (_ExitButton != null)
                _ExitButton.onClick.RemoveListener(Exit);
            #endif
        }
    }
}
