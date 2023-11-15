using Com.IsartDigital.Rush.Cube;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Tiles
{
    [RequireComponent(typeof(ColoredTiles))]
    public class Spawner : MonoBehaviour
    {
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

        private void Start()
        {
            _Clock = Clock.GetInstance();
            _Clock.OnGameStart += StartSpawner;

            _ColorIdentifier = GetComponent<ColoredTiles>().Color;
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
            if(_InternalDelay == _SpawnFrequency && _NumberCubeToSpawn > 0)
            {
                CreateCube();
                _InternalDelay = 0;
            }
        }

        private void CreateCube()
        {
            Instantiate(_CubePrefab, transform.position + Vector3.up * 0.5f, transform.rotation, transform.parent)
                           .GetComponent<Cube.Cube>()
                           .Init(_ColorIdentifier);

            _NumberCubeToSpawn -= 1;
        }

        private void OnDestroy()
        {
            _Clock.OnGameStart -= StartSpawner;

            _Clock.OnTick -= CubeSpawner;
            _Clock.OnTick -= DelayCubeSpawn;

            _Clock = null;
        }
    }
}
