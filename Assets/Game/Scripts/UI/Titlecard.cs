using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.UI
{
    public class Titlecard : Screen
    {

        [SerializeField] private Button _ExitButton = default;
        [SerializeField] private Button _PlayButton = default;

        [Header("Juiciness")]
        [SerializeField] private Animator _CameraAnimator = default;
        [SerializeField] private string _CameraTransitionAnim = "";

        private Coroutine _Timer = null;

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
            _PlayButton.onClick.RemoveListener(Play);
            _CameraAnimator.Play(_CameraTransitionAnim);

            if (_Timer == null)
                _Timer = StartCoroutine(Timer()); 
        }

        private IEnumerator Timer()
        {
            yield return new WaitForSeconds(_CameraAnimator.GetCurrentAnimatorStateInfo(0).length);

            if (_Timer != null)
                StopCoroutine(_Timer);

            Close();
            Next();
        }

        private void OnDestroy()
        {
            if (_Timer != null)
                StopCoroutine(_Timer);

            _PlayButton.onClick.RemoveListener(Play);

            #if UNITY_STANDALONE
            if (_ExitButton != null)
                _ExitButton.onClick.RemoveListener(Exit);
            #endif
        }
    }
}
