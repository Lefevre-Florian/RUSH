using Com.IsartDigital.Rush.Data;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Managers
{
    public class LevelManager : MonoBehaviour
    {
        #region Singleton
        private static LevelManager _Instance = null;

        public static LevelManager GetInstance()
        {
            if (_Instance == null)
                _Instance = new LevelManager();
            return _Instance;
        }

        private LevelManager() : base() { }

        #endregion

        [SerializeField] private int _LevelID = 0;
        [SerializeField] private Level[] _Levels = null;

        [SerializeField] private GameObject _GameScene = null;

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
                Transform lLevel = Instantiate(_GameScene, Vector3.zero, new Quaternion(), transform.parent).transform;
                Instantiate(_Levels[_LevelID].LevelPrefab, lLevel.transform.position, new Quaternion(), lLevel);
                Debug.Log(TilesPlacer.GetInstance());
                TilesPlacer.GetInstance().SetTiles(_Levels[_LevelID]);
            }
        }

        private void OnDestroy()
        {
            if (_Instance != null)
                _Instance = null;
        }
    }
}
