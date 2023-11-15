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

        private List<Cube.Cube> _Cubes = new List<Cube.Cube>();

        private int _CountBeforeEnd = 0;

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

            _Clock.OnReset += Init;

            Init();
        }

        private void Init()
        {
            _CountBeforeEnd = Goal.Targets.Count;
            foreach (Goal lTarget in Goal.Targets)
                lTarget.OnFullyArrived += UpdateEndGameScore;

            foreach (Spawner lSpawner in Spawner.Spawners)
                lSpawner.OnCubeSpawned += AddCubeToPlayingParty;

            _Cubes = new List<Cube.Cube>();
        }

        private void AddCubeToPlayingParty(Cube.Cube pCube)
        {
            pCube.OnDied += GameOver;
            _Cubes.Add(pCube);
        }
        
        // Executed if loose condition = true
        private void GameOver()
        {
            CleanGame();
            _HUD.DisplayGameoverState(false);
        }

        // Executed if win condition = true
        private void UpdateEndGameScore()
        {
            if (--_CountBeforeEnd == 0)
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
                foreach (Cube.Cube lCube in _Cubes)
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
            _Clock.OnReset -= Init;

            _Clock = null;
            _HUD = null;

            if (_Instance != null)
                _Instance = null;
        }

    }
}