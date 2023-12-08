using Com.IsartDigital.Rush.Data;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Managers
{
    public class LevelManager : MonoBehaviour
    {
        #region Singleton
        [SerializeField] private Animator _TransitionScreen = null;
        [SerializeField] private string _FadeInTrigger = "";
        [SerializeField] private string _FadeOutTrigger = "";

        private static LevelManager _Instance = null;

        public static LevelManager GetInstance()
        {
            if (_Instance == null)
                _Instance = new LevelManager();
            return _Instance;
        }

        private LevelManager() : base() { }

        #endregion

        [Header("Levels")]
        [SerializeField] private int _LevelID = 0;
        [SerializeField] private Level[] _Levels = null;

        [Space(10)]
        [SerializeField] private GameObject _GameScene = null;

        [Header("Scene Management")]
        [SerializeField] private Transform _GameContainer = null;

        public GameObject[] Levels
        {
            get 
            {
                int lLength = _Levels.Length;
                GameObject[] lModel = new GameObject[lLength];
                for (int i = 0; i < lLength; i++)
                    lModel[i] = _Levels[i].Model;
                return lModel; 
            }
            private set {}
        }

        private void Awake()
        {
            if(_Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _Instance = this;
        }

        public void LoadLevel(int pLevelID)
        {
            _LevelID = pLevelID;
            if(_Levels != null && _LevelID < _Levels.Length)
            {
                TriggerFadeOutScreen();
                Transform lLevel = SwitchScene(_GameScene);
                if(lLevel != null)
                {
                    Instantiate(_Levels[_LevelID].LevelPrefab,  
                                FlowManager.GetInstance().LevelContainer);
                    TilesPlacer.GetInstance().SetTiles(_Levels[_LevelID]);
                }
            }
        }

        public Transform SwitchScene(GameObject pPrefab)
        {
            if (_GameContainer != null && _GameContainer.childCount != 0)
            {
                int lLength = _GameContainer.childCount;
                for (int i = 0; i < lLength; i++)
                    Destroy(_GameContainer.GetChild(i).gameObject);

                return Instantiate(pPrefab, _GameContainer).transform;
            }
            return null;
        }

        public void TriggerFadeInScreen() => _TransitionScreen.SetTrigger(_FadeInTrigger);

        public void TriggerFadeOutScreen() => _TransitionScreen.SetTrigger(_FadeOutTrigger);

        private void OnDestroy()
        {
            if (_Instance != null)
                _Instance = null;
        }
    }
}
