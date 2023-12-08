using System;
using System.Collections;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Managers
{
    public class HintManager : MonoBehaviour
    {
        #region Singleton
        private static HintManager _Instance = null;

        public static HintManager GetInstance()
        {
            if (_Instance == null)
                _Instance = new HintManager();
            return _Instance;
        }

        private HintManager() : base() { }
        #endregion

        [SerializeField][Min(.5f)] private float _HintShownDuration = 1f;

        // Signals
        public event Action OnHintDisplayed;
        public event Action OnHintFullyDisplayed;
        public event Action OnHintHide;

        private Coroutine _Timer = null;

        private void Awake()
        {
            if(_Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _Instance = this;
        }

        public void DisplayHint()
        {
            OnHintDisplayed?.Invoke();

            StartTimer();
        }

        public void DisplayCompleteHint()
        {
            OnHintFullyDisplayed?.Invoke();

            StartTimer();
        }

        private void StartTimer()
        {
            if (_Timer != null)
                StopCoroutine(_Timer);

            _Timer = StartCoroutine(DisplayedTimer());
        }

        private IEnumerator DisplayedTimer()
        {
            yield return new WaitForSeconds(_HintShownDuration);

            OnHintHide?.Invoke();

            if (_Timer != null)
                StopCoroutine(_Timer);
        }

        public void ForceHideHint()
        {
            OnHintHide?.Invoke();

            if (_Timer != null)
                StopCoroutine(_Timer);
        }

        private void OnDestroy()
        {
            if (_Timer != null)
                StopCoroutine(_Timer);

            if (_Instance != null)
                _Instance = null;
        }
    }
}