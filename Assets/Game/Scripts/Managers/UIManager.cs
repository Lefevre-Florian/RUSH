using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Managers
{
    public class UIManager : MonoBehaviour
    {

        #region Singleton
        private static UIManager _Instance = null;

        public static UIManager GetInstance()
        {
            if (_Instance == null)
                _Instance = new UIManager();
            return _Instance;
        }

        private UIManager() : base() {}
        #endregion

        [SerializeField] private Transform _Container = null;

        private void Awake()
        {
            if(_Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _Instance = this;
        }

        public void LoadUI(GameObject pInterface) => Instantiate(pInterface, _Container);

        private void OnDestroy()
        {
            if (_Instance != null)
                _Instance = null;
        }
    }
}