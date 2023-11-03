using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Managers
{
    public class LevelManager : MonoBehaviour
    {
        #region Singleton
        private static LevelManager _Instance = null;

        private static LevelManager GetInstance()
        {
            if (_Instance == null)
                _Instance = new LevelManager();
            return _Instance;
        }

        private LevelManager() : base() { }

        #endregion

        [SerializeField] private int _LevelID = 0;
        [SerializeField] private GameObject[] _Levels = null;


        private void Awake()
        {
            if(_Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _Instance = this;
        }

        private void Start() => LoadLevel(_LevelID);

        public void LoadLevel(int pLevelID)
        {
            _LevelID = pLevelID;
            if(_Levels != null && _LevelID < _Levels.Length)
            {
                Instantiate(_Levels[_LevelID], Vector3.zero, new Quaternion(), transform.parent);
            }
        }

        private void OnDestroy()
        {
            if (_Instance != null)
                _Instance = null;
        }
    }
}
