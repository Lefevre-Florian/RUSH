using Com.IsartDigital.Rush.Tiles;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Managers
{
    public class FlowManager : MonoBehaviour
    {
        #region Singleton
        private static FlowManager _Instance = null;

        private FlowManager() : base() { }

        public static FlowManager GetInstance()
        {
            if (_Instance == null)
                _Instance = new FlowManager();
            return _Instance;
        }
        #endregion

        [SerializeField] private Transform _TargetsContainer = null;

        private int _CountBeforeEnd = 0;
        private Goal[] _Targets = null;

        private void Awake()
        {
            if(_Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _Instance = this;
        }

        private void Start()
        {
            int lLength = _TargetsContainer.childCount;
            _Targets = new Goal[lLength];
            _CountBeforeEnd = lLength;

            Goal lTarget;
            for (int i = 0; i < lLength; i++)
            {
                lTarget = _TargetsContainer.GetChild(i).GetComponent<Goal>();
                lTarget.OnFullyArrived += UpdateEndGameScore;
                _Targets[i] = lTarget;
            }
        }

        private void UpdateEndGameScore()
        {
            if (--_CountBeforeEnd == 0)
                Debug.Log("Fin de partie");
        }

        private void OnDestroy()
        {
            foreach (Goal lTarget in _Targets)
                lTarget.OnFullyArrived -= UpdateEndGameScore;
            _Targets = null;

            if (_Instance != null)
                _Instance = null;
        }

    }
}