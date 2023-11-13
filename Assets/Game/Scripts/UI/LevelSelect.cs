using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.UI
{
    public class LevelSelect : MonoBehaviour
    {
        [Header("Level")]
        [SerializeField] private Transform _LevelContainer = null;
        [SerializeField] private GameObject[] _LevelRenderers = new GameObject[0];
        [SerializeField] private float _RendererScaleRatio = 5;

        [Header("Navigation")]
        [SerializeField] private Button _NextBtn = null;
        [SerializeField] private Button _PreviousBtn = null;
        [SerializeField] private TextMeshProUGUI _LevelNameLabel = null;

        private int _CurrentIndex = 0;

        private void Start()
        {
            int lLength = _LevelRenderers.Length;
            for (int i = 0; i < lLength; i++)
            {
                _LevelRenderers[i] = Instantiate(_LevelRenderers[i],
                                                 _LevelContainer.position,
                                                 new Quaternion(),
                                                 _LevelContainer);
                _LevelRenderers[i].transform.localScale /= _RendererScaleRatio;
                _LevelRenderers[i].SetActive(false);
            }

            _LevelRenderers[_CurrentIndex].SetActive(true);
            UpdateNavigationState();

            _NextBtn.onClick.AddListener(NextLevel);
            _PreviousBtn.onClick.AddListener(PreviousLevel);
        }

        public void NextLevel()
        {
            if (_CurrentIndex + 1 < _LevelRenderers.Length)
            {
                _LevelRenderers[_CurrentIndex].SetActive(false);
                _LevelRenderers[++_CurrentIndex].SetActive(true);
            }       

            UpdateNavigationState();
        }

        public void PreviousLevel()
        {
            if (_CurrentIndex - 1 >= 0)
            {
                _LevelRenderers[_CurrentIndex].SetActive(false);
                _LevelRenderers[--_CurrentIndex].SetActive(true);
            }

            UpdateNavigationState();
        }

        private void UpdateNavigationState()
        {
            _LevelNameLabel.text = _LevelRenderers[_CurrentIndex].name;

            if (_CurrentIndex == 0)
                SetButtonState(_PreviousBtn, false);
            else if (_CurrentIndex == _LevelRenderers.Length - 1)
                SetButtonState(_NextBtn, false);
            else
            {
                SetButtonState(_NextBtn, true);
                SetButtonState(_PreviousBtn, true); 
            }
        }

        private void SetButtonState(Button pButton, bool pState) => pButton.enabled = pState;

        private void OnDestroy()
        {
            _PreviousBtn.onClick.RemoveListener(PreviousLevel);
            _NextBtn.onClick.RemoveListener(NextLevel);
        }

    }
}