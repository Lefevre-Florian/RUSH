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
        [SerializeField] private GameObject[] _Levels = null;

        [SerializeField] private GameObject _GameScene = null;

        public GameObject[] Levels
        {
            get { return _Levels; }
            private set { _Levels = value; }
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
                Instantiate(_Levels[_LevelID], lLevel.transform.position, new Quaternion(), lLevel);
            }
        }

        private void OnDestroy()
        {
            if (_Instance != null)
                _Instance = null;
        }
    }
}
