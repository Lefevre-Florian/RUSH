using System;
using System.Collections.Generic;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Tiles
{
    [RequireComponent(typeof(ColoredTiles))]
    public class Spawner : MonoBehaviour
    {
        public static List<Spawner> Spawners { get; private set; } = new List<Spawner>();

        // In tick rate sync
        [Header("Spawn")]
        [SerializeField] private int _NumberCubeToSpawn = 1;
        [SerializeField] private int _SpawnDelay = 1;
        [SerializeField] private int _SpawnFrequency = 1;

        [Space(10)]
        [SerializeField] private GameObject _CubePrefab = default;

        private Clock _Clock = null;

        private Colors _ColorIdentifier = default;
        private int _InternalDelay = 0;

        private int _InternalNumberCubeToSpawn = 0;

        // Signals
        public event Action<Cube.Cube> OnCubeSpawned;

        private void Awake() => Spawners.Add(this);

        private void Start()
        {
            _Clock = Clock.GetInstance();
            _Clock.OnReset += Init;
            
            Init();

            _ColorIdentifier = GetComponent<ColoredTiles>().Color;
        }

        private void Init()
        {
            _InternalNumberCubeToSpawn = _NumberCubeToSpawn;
            _InternalDelay = 0;

            _Clock.OnTick -= DelayCubeSpawn;
            _Clock.OnTick -= CubeSpawner;

            _Clock.OnGameStart += StartSpawner;
        }

        private void StartSpawner()
        {
            _Clock.OnGameStart -= StartSpawner;
            _Clock.OnTick += DelayCubeSpawn;
        }

        private void DelayCubeSpawn()
        {
            _InternalDelay += 1;
            if(_InternalDelay == _SpawnDelay)
            {
                _InternalDelay = 0;

                CreateCube();

                _Clock.OnTick -= DelayCubeSpawn;
                _Clock.OnTick += CubeSpawner;
            }
        }

        private void CubeSpawner()
        {
            _InternalDelay += 1;
            if(_InternalDelay == _SpawnFrequency && _InternalNumberCubeToSpawn > 0)
            {
                CreateCube();
                _InternalDelay = 0;
            }
        }

        private void CreateCube()
        {
            Cube.Cube lCube = Instantiate(_CubePrefab, transform.position + Vector3.up * 0.5f, transform.rotation, transform.parent)
                                         .GetComponent<Cube.Cube>();
            lCube.Init(_ColorIdentifier);

            OnCubeSpawned?.Invoke(lCube);
            _InternalNumberCubeToSpawn -= 1;
        }

        private void OnDestroy()
        {
            Spawners.Remove(this);

            _Clock.OnGameStart -= StartSpawner;

            _Clock.OnReset -= Init;

            _Clock.OnTick -= CubeSpawner;
            _Clock.OnTick -= DelayCubeSpawn;

            _Clock = null;
        }
    }
}
