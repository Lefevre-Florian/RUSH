using UnityEngine;
using UnityEngine.UI;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.UI
{
    public class Titlecard : MonoBehaviour
    {

        [SerializeField] private Button _ExitButton = default;
        [SerializeField] private Button _PlayButton = default;

        [SerializeField] private GameObject _LevelToLoad = default;

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
            Instantiate(_LevelToLoad,
                        Vector3.zero,
                        new Quaternion(),
                        transform.parent);

            Destroy(gameObject);
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
