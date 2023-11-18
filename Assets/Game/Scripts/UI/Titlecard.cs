using UnityEngine;
using UnityEngine.UI;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.UI
{
    public class Titlecard : Screen
    {

        [SerializeField] private Button _ExitButton = default;
        [SerializeField] private Button _PlayButton = default;

        protected override void Init()
        {
            #if UNITY_ANDROID
            Destroy(_ExitButton);
            #endif
            #if UNITY_STANDALONE
            if (_ExitButton == null)
                return;

            _ExitButton.onClick.AddListener(Exit);
            #endif

            _PlayButton.onClick.AddListener(Play);
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

        private void Play()
        {
            Close();
            Next();
        }

        private void OnDestroy()
        {
            _PlayButton.onClick.RemoveListener(Play);

            #if UNITY_STANDALONE
            if (_ExitButton != null)
                _ExitButton.onClick.RemoveListener(Exit);
            #endif
        }
    }
}
