using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Data
{
    [CreateAssetMenu(menuName = "RUSH/NewLevel",fileName = "DefaultLevel",order = 0)]
    public class Level : ScriptableObject
    {
        [SerializeField] private TileData[] _Tiles = new TileData[0];
        [SerializeField] private GameObject _Level = null;

        public TileData[] Tile { get { return _Tiles; } private set { _Tiles = value; } }

        public GameObject LevelPrefab { get { return _Level; } private set { _Level = value; } }
    }
}
