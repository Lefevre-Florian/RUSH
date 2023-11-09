using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Tiles
{
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

        private int _InternalDelay = 0;

        private void Start()
        {
            _Clock = Clock.GetInstance();
            _Clock.OnTick += DelayCubeSpawn;
        }

        private void DelayCubeSpawn()
        {
            _InternalDelay += 1;
            if(_InternalDelay == _SpawnDelay)
            {
                _InternalDelay = 0;

                _Clock.OnTick -= DelayCubeSpawn;
                _Clock.OnTick += CubeSpawner;
            }
        }

        private void CubeSpawner()
        {
            _InternalDelay += 1;
            if(_InternalDelay == _SpawnFrequency && _NumberCubeToSpawn > 0)
            {
                Instantiate(_CubePrefab, transform.position + Vector3.up * 0.5f, transform.rotation, transform.parent) ;

                _NumberCubeToSpawn -= 1;
                _InternalDelay = 0;
            }
        }

        private void OnDestroy()
        {
            _Clock.OnTick -= CubeSpawner;
            _Clock.OnTick -= DelayCubeSpawn;

            _Clock = null;
        }
    }
}
