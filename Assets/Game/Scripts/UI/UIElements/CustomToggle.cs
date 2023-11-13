using UnityEngine;
using UnityEngine.UI;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.UI
{
    public class CustomToggle : MonoBehaviour
    {
        [SerializeField] private Sprite _PlayImg = null;

        private Image _Image = null;
        private Sprite _PauseImg = null;

        private bool _IsToggle = false;

        public delegate void Toggled(bool pToggleState);
        public Toggled _Toggled;

        void Start()
        {
            _Image = GetComponent<Image>();
            _PauseImg = _Image.sprite;
        }

        public void Toggle()
        {
            if (_IsToggle)
                _Image.sprite = _PauseImg;
            else
                _Image.sprite = _PlayImg;

            _IsToggle = !_IsToggle;
            _Toggled?.Invoke(_IsToggle);
        }
    }
}
