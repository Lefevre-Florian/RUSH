using Com.IsartDigital.Rush.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.UI
{
    public class LevelSelect : Screen
    {
        [Header("Level")]
        [SerializeField] private Transform _LevelContainer = null;
        [SerializeField] private float _RendererScaleRatio = 5;

        [Header("Navigation")]
        [SerializeField] private Button _NextBtn = null;
        [SerializeField] private Button _PreviousBtn = null;
        [SerializeField] private Button _StartBtn = null;
        [SerializeField] private TextMeshProUGUI _LevelNameLabel = null;

        [Header("Input")]
        [SerializeField] private string _BackInput = "";

        [Header("Juiciness")]
        [SerializeField] private float _RendererRotationSpeed = 25f; 

        private GameObject[] _LevelRenderers = new GameObject[0];
        private int _CurrentIndex = 0;

        private Transform _LevelRenderer = null;

        private void Update()
        {
            if (Input.GetButtonDown(_BackInput))
                Back();

            if(_LevelRenderer != null)
            {
                _LevelRenderer.Rotate(Vector3.up, _RendererRotationSpeed * Time.deltaTime);
            }
        }

        protected override void Init()
        {
            _LevelRenderers = LevelManager.GetInstance().Levels;

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
            _LevelRenderer = _LevelRenderers[_CurrentIndex].transform;

            UpdateNavigationState();

            _StartBtn.onClick.AddListener(StartLevel);
            _NextBtn.onClick.AddListener(NextLevel);
            _PreviousBtn.onClick.AddListener(PreviousLevel);
        }

        private void NextLevel()
        {
            if (_CurrentIndex + 1 < _LevelRenderers.Length)
            {
                _LevelRenderers[_CurrentIndex].SetActive(false);
                _LevelRenderers[++_CurrentIndex].SetActive(true);
            }       

            UpdateNavigationState();
        }

        private void PreviousLevel()
        {
            if (_CurrentIndex - 1 >= 0)
            {
                _LevelRenderers[_CurrentIndex].SetActive(false);
                _LevelRenderers[--_CurrentIndex].SetActive(true);
            }

            UpdateNavigationState();
        }

        private void StartLevel()
        {
            LevelManager.GetInstance().LoadLevel(_CurrentIndex); 
            Close();
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

            Transform lTemp = _LevelRenderer;
            _LevelRenderer = _LevelRenderers[_CurrentIndex].transform;
            _LevelRenderer.rotation = lTemp.rotation;
        }

        private void SetButtonState(Button pButton, bool pState) => pButton.enabled = pState;

        private void OnDestroy()
        {
            _StartBtn.onClick.RemoveListener(StartLevel);
            _PreviousBtn.onClick.RemoveListener(PreviousLevel);
            _NextBtn.onClick.RemoveListener(NextLevel);
        }

    }
}