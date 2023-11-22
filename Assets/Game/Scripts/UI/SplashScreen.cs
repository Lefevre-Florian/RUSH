using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.UI
{
    public class SplashScreen : Screen
    {
        [SerializeField] private float _Duration;
        [SerializeField] private AnimationCurve _TweenCurve = null;

        [Space(10)]
        [SerializeField] private Image _Logo = default;

        private Coroutine _Timer = null;
        private float _ElapsedTime = 0f;

        protected override void Init()
        {
            _Timer = StartCoroutine(Timer());
        }

        private void Update()
        {
            _ElapsedTime += Time.deltaTime;

            _Logo.color = new Color(_Logo.color.r,
                                    _Logo.color.g,
                                    _Logo.color.b,
                                    _TweenCurve.Evaluate(_ElapsedTime / _Duration));
        }

        private IEnumerator Timer()
        {
            yield return new WaitForSeconds(_Duration);
            StopCoroutine(_Timer);

            Next();
            Close();
        }

        private void OnDestroy()
        {
            if (_Timer != null)
                StopCoroutine(_Timer);
        }
    }
}