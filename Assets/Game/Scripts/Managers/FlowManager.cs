using Com.IsartDigital.Rush.Tiles;
using Com.IsartDigital.Rush.UI;
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
        private const float WARNING_OFFSET = 2.5f;

        [SerializeField] private Transform _LevelContainer = null;
        [SerializeField] private GameObject _WarningSignPrefab = null;

        private Dictionary<Cube.Cube, bool> _Cubes = new Dictionary<Cube.Cube, bool>();
        private GameObject _WarningSign = null;

        public  Transform LevelContainer { get { return _LevelContainer; } private set { _LevelContainer = value; } }

        // Refs
        private Clock _Clock = null;
        private HUD _HUD = null;

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
            _Clock = Clock.GetInstance();
            _HUD = HUD.GetInstance();

            _Clock.OnReset += Restore;

            Init();
        }

        private void Init()
        {
            if (_WarningSign != null)
                Destroy(_WarningSign);

            foreach (Goal lTarget in Goal.Targets)
                lTarget.OnFullyArrived += UpdateEndGameScore;

            foreach (Spawner lSpawner in Spawner.Spawners)
                lSpawner.OnCubeSpawned += AddCubeToPlayingParty;

            _Cubes = new Dictionary<Cube.Cube, bool>();
        }

        private void Restore()
        {
            CleanGame();
            Init();
        }

        private void AddCubeToPlayingParty(Cube.Cube pCube)
        {
            pCube.OnDied += GameOver;
            _Cubes.Add(pCube, false);
        }
        
        // Executed if loose condition = true
        private void GameOver(Vector3 pPosition)
        {
            _WarningSign = Instantiate(_WarningSignPrefab, 
                                       pPosition + Vector3.up * WARNING_OFFSET, 
                                       new Quaternion(), 
                                       LevelContainer);

            CleanGame();
            Camera.OrbitalCamera.GetInstance().CenterCameraOnPositionOnCircle(pPosition);
            _HUD.DisplayGameoverState(false);
        }

        // Executed if win condition = true
        private void UpdateEndGameScore(Cube.Cube pCube)
        {
            _Cubes[pCube] = true;

            bool lAllCubesWin = false;
            foreach (bool lState in _Cubes.Values)
            {
                if (!lState)
                {
                    lAllCubesWin = false;
                    break;
                }
                lAllCubesWin = true;
            }

            if (lAllCubesWin)
            {
                CleanGame();
                _HUD.DisplayGameoverState(true);
            }
        }

        private void CleanGame()
        {
            _Clock.StopTicking();

            foreach (Spawner lSpawner in Spawner.Spawners)
                lSpawner.OnCubeSpawned -= AddCubeToPlayingParty;
            
            if(_Cubes != null)
            {
                foreach (Cube.Cube lCube in _Cubes.Keys)
                    lCube.OnDied -= GameOver;
                _Cubes.Clear();
                _Cubes = null;
            }
            
            foreach (Goal lTarget in Goal.Targets)
                lTarget.OnFullyArrived -= UpdateEndGameScore;
        }

        private void OnDestroy()
        {
            CleanGame();
            _Clock.OnReset -= Restore;

            _Clock = null;
            _HUD = null;

            if (_Instance != null)
                _Instance = null;
        }

    }
}